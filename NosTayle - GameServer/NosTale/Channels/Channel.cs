using NosTayleGameServer.Communication;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.Net;
using NosTayleGameServer.NosTale.Entities.Managers;
using NosTayleGameServer.NosTale.Entities.Players;
using NosTayleGameServer.NosTale.Maps;
using NosTayleGameServer.NosTale.Maps.SpecialMaps;
using NosTayleGameServer.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Channels
{
    public class Channel
    {
        internal int id;
        private string ip;
        private string port;
        private bool channelStarted;

        private List<Session> NoPlayersSessions;
        private List<Session> sNewSessions;
        private PlayersManager playersOnChannel;
        internal Dictionary<int, Map> generalMaps;
        internal Dictionary<string, Arena> arenas = new Dictionary<string, Arena>();
        internal int lastNpcId;
        internal int lastPortalId;

        private SocketListener sListener;
        private Task cycleTask;
        private bool inCycle;

        public Channel(int idChannel, string ipChannel, string portChannel, int lastPortalId, int lastNpcId)
        {
            this.id = idChannel;
            this.ip = ipChannel;
            this.port = portChannel;
            this.lastNpcId = lastNpcId;
            this.lastPortalId = lastPortalId;
            this.NoPlayersSessions = new List<Session>();
            this.sNewSessions = new List<Session>();
            this.playersOnChannel = new PlayersManager();
            this.generalMaps = new Dictionary<int, Map>();
            this.sListener = new SocketListener(ipChannel, portChannel, this);
            arenas.Add("individual", new Arena(2006, "individual"));
            arenas.Add("family", new Arena(2106, "family"));
            this.cycleTask = new Task(this.OnCycle);
            this.cycleTask.Start();
            this.channelStarted = true;
            NosTayleGameServer.Core.WriteConsole.WriteStructure("GAME", "=>Channel " + id + " started on " + ip + ":" + port, true);
        }

        public void CloseChannel()
        {
            if (this.channelStarted)
            {
                this.channelStarted = false;
                this.GetListener().StopListening();
                Console.WriteLine("Channel {0} stopped.", this.id);
            }
        }

        public SocketListener GetListener() { return this.sListener; }

        public PlayersManager GetPlayersManager() { return this.playersOnChannel; }

        public int NewNpcId() { return this.lastNpcId++; }

        #region Connexions et deconnexions
        internal void ReceiveNewConnection(SocketInformation socketInformation_0)
        {
            SocketConnection socketConnection = new SocketConnection(socketInformation_0);
            if (!this.channelStarted)
            {
                socketConnection.CloseConnection();
                return;
            }
            if (this.playersOnChannel.playersCount >= 500)
            {
                socketConnection.Send(GlobalMessage.MakeInfo("Serveur plein/Server full").GetBytes());
                socketConnection.CloseConnection();
                return;
            }
            Session newSession = new Session(this, ref socketConnection);
            Console.WriteLine("[CHANNEL {0}] New connection from {1}", this.id, socketConnection.Address);
            this.sNewSessions.Add(newSession);
        }

        internal void DisconnectUser(Session session)
        {
            try
            {
                if (session.GetPlayer() != null)
                {
                    GameServer.GetPlayersManager().RemoveUser(session.GetPlayer());
                    this.playersOnChannel.RemoveUser(session.GetPlayer());
                }
                using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
                {
                    if (session.GetAccount() != null)
                        dbClient.AddParamWithValue("lp", session.GetAccount().languagePack);
                    if (session.GetPlayer() != null)
                    {
                        Player player = session.GetPlayer();
                        if (player.map != null)
                        {
                            player.map.RemoveUser(player);
                            if (player.pet != null)
                                player.map.RemoveNpc(player.pet);
                            SaveEquipment(player);
                            player.inventory.SaveInventory(player);
                            player.SaveData();
                        }
                        player.miniLand.ExitAllUser(true, player.id);
                        if (player.group != null)
                            player.group.RemoveUser(player);
                        if (player.mlPanel != null && player.mlPanel.map != null)
                            player.mlPanel.map.RemoveNpc(player.mlPanel);
                        player.friendList.SendDisconection();
                    }
                    if (session.GetAccount() != null)
                    {
                        dbClient.AddParamWithValue("banned", Convert.ToInt32(session.GetAccount().banned).ToString());
                        dbClient.ExecuteQuery("UPDATE accounts SET online = '0', languagePack = @lp, block = @banned WHERE id = " + session.GetAccount().id + " LIMIT 1");
                    }
                }
                if (this.channelStarted)
                    Console.WriteLine("[CHANNEL {0}] Connection dropped from {1}", this.id, session.GetSock().Address);
                GC.SuppressFinalize(session);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.ForegroundColor = ConsoleColor.White;
                using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
                {
                    if (session.GetAccount() != null)
                    {
                        dbClient.AddParamWithValue("banned", Convert.ToInt32(session.GetAccount().banned).ToString());
                        dbClient.AddParamWithValue("lp", session.GetAccount().languagePack);
                        dbClient.ExecuteQuery("UPDATE accounts SET online = '0', languagePack = @lp, block = @banned WHERE id = " + session.GetAccount().id + " LIMIT 1");
                    }
                }
            }
        }

        public void SaveEquipment(Player user)
        {
            if (user.weapon.id != -1)
                user.weapon.Save(user.id, user.GetSession().GetAccount().id, 0, 1, -1);
            if (user.weapon2.id != -1)
                user.weapon2.Save(user.id, user.GetSession().GetAccount().id, 0, 1, -1);
            if (user.armor.id != -1)
                user.armor.Save(user.id, user.GetSession().GetAccount().id, 0, 1, -1);
            if (user.chap.id != -1)
                user.chap.Save(user.id, user.GetSession().GetAccount().id, 0, 1, -1);
            if (user.sp.spId != -1)
                user.sp.SaveSP(user.id, user.GetSession().GetAccount().id, 0, 1, -1, 1);
            if (user.mask.id != -1)
                user.mask.Save(user.id, user.GetSession().GetAccount().id, 0, 1, -1);
            if (user.fairy.fairyId != -1)
                user.fairy.Save(user.id, user.GetSession().GetAccount().id, 0, 1, -1);
            if (user.glove.id != -1)
                user.glove.Save(user.id, user.GetSession().GetAccount().id, 0, 1, -1);
            if (user.boot.id != -1)
                user.boot.Save(user.id, user.GetSession().GetAccount().id, 0, 1, -1);
            if (user.costumeBody.id != -1)
                user.costumeBody.Save(user.id, user.GetSession().GetAccount().id, 0, 1, -1);
            if (user.costumeHead.id != -1)
                user.costumeBody.Save(user.id, user.GetSession().GetAccount().id, 0, 1, -1);
            GC.Collect();
        }

        internal void AddPlayerOnline(Player player)
        {
            this.NoPlayersSessions.Remove(player.GetSession());
            this.sNewSessions.Remove(player.GetSession());
            GameServer.GetPlayersManager().AddUser(player.GetId(), player);
            this.playersOnChannel.AddUser(player.GetId(), player);
        }
        #endregion

        #region Envoie d'informations
        public void SendSpeaker(Player player, string message)
        {
            if (player.isMuted)
                return;
            ServerPacket result = player.inventory.DeleteOneItemByType(player, 2, "speaker");
            if (result != null)
            {
                this.playersOnChannel.SendToAll(GlobalMessage.MakeMessage(player.id, player.type, 13, "<Total> [" + player.name + "]:" + message));
                player.SendPacket(result);
            }
        }
        #endregion

        #region Méthodes du cycle
        private void OnCycle()
        {
            while (true)
            {
                if (!this.inCycle)
                {
                    this.inCycle = true;
                    this.SecureSessions();
                    this.inCycle = false;
                }
                Thread.Sleep(10000);
            }
        }

        private void SecureSessions()
        {
            List<Session> sVerif = this.sNewSessions;
            this.sNewSessions = new List<Session>();
            foreach (Session session in sVerif)
            {
                if ((Int32)(DateTime.Now.Subtract(session.ConnectedAt())).TotalSeconds >= 10 && session.GetAccount() == null)
                    session.GetSock().CloseConnection();
                else if (session.GetAccount() == null)
                    this.sNewSessions.Add(session);
            }

        }
        #endregion
    }
}
