using NosTayleGameServer.NosTale;
using NosTayleGameServer.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.AdminTools
{
    class AdminSession
    {
        private Socket _socket;
        private int adminId, adminRank;
        private string adminName;
        public bool connectedAtAdmin;
        private byte[] receiveData;
        private bool disconnected;
        public DateTime connectedAt;

        public AdminSession(Socket socket)
        {
            this._socket = socket;
            this.receiveData = new byte[1024];
            this._socket.BeginReceive(this.receiveData, 0, this.receiveData.Length, SocketFlags.None, new AsyncCallback(this.ReceiveData), this._socket);
            this.connectedAt = DateTime.Now;
        }

        public void Disconnect()
        {
            if (!this.disconnected)
            {
                this.disconnected = true;
                this._socket.Shutdown(SocketShutdown.Both);
                this._socket.Close();
                this._socket.Dispose();
                GameServer.GetAdminToolsServer().admSessions.Remove(this);
            }
        }

        public void ReceiveData(IAsyncResult AR)
        {
            try
            {
                int count = 0;
                try
                {
                    count = this._socket.EndReceive(AR);
                }
                catch
                {
                }
                string receiveString = Encoding.GetEncoding(1258).GetString(this.receiveData, 0, count);
                if (count > 0 && receiveString.Length > 0)
                {
                    //Reception d'une commande
                    this.ReceiveMessage(receiveString.Split(' ')[0], receiveString.Substring(receiveString.Split(' ')[0].Length + 1));
                    this._socket.BeginReceive(this.receiveData, 0, this.receiveData.Length, SocketFlags.None, new AsyncCallback(this.ReceiveData), this._socket);
                }
                else
                {
                    this.Disconnect();
                    return;
                }
            }
            catch
            {
                this.Disconnect();
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                message += (char)0;
                this._socket.BeginSend(Encoding.GetEncoding(1258).GetBytes(message), 0, message.Length, SocketFlags.None, new AsyncCallback(this.OnSend), this._socket);
            }
            catch
            {
            }
        }

        private void OnSend(IAsyncResult ar)
        {
            if (!this.disconnected)
            {
                try
                {
                    this._socket.EndSend(ar);
                }
                catch
                {
                }
            }
        }

        private void ReceiveMessage(string header, string message)
        {
            switch (header)
            {
                case "login":
                    if (message.Split(' ').Length == 2)
                    {
                        DataTable dataTable;
                        this.adminName = message.Split(' ')[0];
                        using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("user", this.adminName);
                            dbClient.AddParamWithValue("password", message.Split(' ')[1]);
                            dataTable = dbClient.ReadDataTable("SELECT * FROM accounts WHERE user = @user AND password = @password AND isGm = '1';");
                        }
                        if (dataTable.Rows.Count == 1)
                        {
                            DataRow dataRow = dataTable.Rows[0];
                            this.adminId = (int)dataRow["id"];
                            this.connectedAtAdmin = true;
                            this.SendMessage(String.Format("connectok {0} {1} {2}", GameServer.GetPlayersManager().playersCount, GameServer.GetAdminToolsServer().admSessions.FindAll(x => x.connectedAtAdmin == true).Count, 10));
                        }
                        else
                        {
                            this.SendMessage("Nom d'administrateur ou mot de passe incorrecte.");
                            this.Disconnect();
                            return;
                        }
                    }
                    else
                    {
                        this.SendMessage("Nom d'administrateur ou mot de passe incorrecte.");
                        this.Disconnect();
                    }
                    break;
                default:
                    this.Disconnect();
                    break;
            }
        }
    }
}
