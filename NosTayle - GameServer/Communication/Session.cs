using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.Core;
using NosTayleGameServer.Net;
using NosTayleGameServer.NosTale;
using NosTayleGameServer.NosTale.Accounts;
using NosTayleGameServer.NosTale.Channels;
using NosTayleGameServer.NosTale.Entities.Players;
using NosTayleGameServer.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication
{
    public class Session
    {
        private Account account;
        private Player player;
        private Channel channel;
        private SocketConnection sock;
        private bool charsLoads;
        private int decryptSession;
        private DateTime connectedAt;


        public Session(Channel channel, ref SocketConnection sock)
        {
            this.channel = channel;
            this.sock = sock;
            this.sock.Initialise(new SocketConnection.ReceiveDelegate(this.ReceivePackets), this);
            this.connectedAt = DateTime.Now;
        }

        public void SetPlayer(Player player) { this.player = player; }
        public Player GetPlayer() { return this.player; }

        public void ReceivePackets(ref byte[] bytes, int length)
        {
            if (this.charsLoads && this.decryptSession != 0) //Utilisateur en jeu !
            {
                string[] packets = GameCrypto.DecryptGame(this.decryptSession, bytes, length).Split((char)0xFF);
                for (int i = 0; i < packets.Length; i++)
                {
                    string pHeader = packets[i].Split(' ')[0];
                    if (packets[i].Length >= pHeader.Length + 1)
                    {
                        string packetData = packets[i].Substring(pHeader.Length + 1);
                        if (packetData.Split(' ')[0] != "0")
                            StaticMessageHandler.HandlePacket(this, new SessionMessage(packets[i].Substring(pHeader.Length + 1)));
                    }
                }
            }
            else if (this.decryptSession == 0) //On récupére pour la première fois le ticket de connexion
            {
                if (bytes[0] == 153 || bytes[0] == 154)
                {
                    string dS = GameCrypto.DecryptSessionPacket(bytes);
                    if (dS[dS.Length - 1] == ' ')
                        dS = dS.Substring(0, dS.Length - 1);
                    if (dS.Split(' ').Length == 2)
                    {
                        try
                        {
                            Convert.ToInt32(dS.Split(' ')[0]);
                            this.decryptSession = Convert.ToInt32(dS.Split(' ')[1]);
                        }
                        catch
                        {
                            this.sock.CloseConnection();
                            return;
                        }
                    }
                    else
                    {
                        this.sock.CloseConnection();
                        return;
                    }
                }
                else
                {
                    this.sock.CloseConnection();
                    return;
                }
            }
            else if (this.decryptSession != 0 && !this.charsLoads) //Envoie de la liste des personnages sur le compte.
            {
                string packet = GameCrypto.DecryptGame(this.decryptSession, bytes, length);
                if (packet.Split((char)0xFF).Length != 2)
                {
                    this.sock.CloseConnection();
                    return;
                }
                try
                {
                    if (packet.Split((char)0xFF)[0].Split(' ').Length != 2 || packet.Split((char)0xFF)[1].Split(' ').Length != 2)
                    {
                        this.sock.CloseConnection();
                        return;
                    }
                    if (!this.sock.isDisconnected())
                    {
                        this.charsLoads = true;
                        this.account = new Account(this.sock.Address);
                        this.account.name = packet.Split((char)0xFF)[0].Split(' ')[1];
                        this.account.password = MD5.MD5Hash(packet.Split((char)0xFF)[1].Split(' ')[1]);
                        DataTable dataTable = null;
                        using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("user", this.account.name);
                            dbClient.AddParamWithValue("password", this.account.password);
                            dbClient.AddParamWithValue("session", this.decryptSession);
                            dataTable = dbClient.ReadDataTable("SELECT * FROM accounts WHERE user = @user AND password = @password AND session = @session AND online='0';");
                        }
                        if (dataTable.Rows.Count == 1)
                        {
                            DataRow dataRow = dataTable.Rows[0];
                            this.account.id = (int)dataRow["id"];
                            this.account.online = true;
                            this.account.isGm = ServerMath.StringToBool(dataRow["isGm"].ToString());
                            this.account.languagePack = GameServer.LanguageExist(dataRow["languagePack"].ToString()) ? dataRow["languagePack"].ToString() : "fr";
                            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
                                dbClient.ExecuteQuery("UPDATE accounts SET online = '1' WHERE id = " + this.account.id + " LIMIT 1");
                            this.account.LoadChars(this);
                        }
                        else
                        {
                            this.account = null;
                            this.sock.CloseConnection();
                            return;
                        }
                    }
                }
                catch { this.sock.CloseConnection(); }
            }
        }

        public Account GetAccount() { return this.account; }

        public Channel GetChannel() { return this.channel; }

        public SocketConnection GetSock() { return this.sock; }

        public DateTime ConnectedAt() { return this.connectedAt; }

        public void SendPacket(ServerPacket packet) { this.sock.Send(packet.GetBytes()); }
        public void SendPacket(byte[] packet) { this.sock.Send(packet); }
    }
}
