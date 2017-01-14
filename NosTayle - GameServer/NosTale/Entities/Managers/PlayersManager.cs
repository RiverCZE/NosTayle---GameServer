using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.NosTale.Entities.Players;
using NosTayleGameServer.NosTale.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Entities.Managers
{
    public class PlayersManager
    {
        private List<Player> players;
        internal delegate void startCycle();
        internal startCycle cycleTask;
        internal Object lockedItem = new Object();

        internal int playersCount
        {
            get
            {
                return this.players.Count();
            }
        }

        public PlayersManager()
        {
            this.players = new List<Player>();
            this.cycleTask = new startCycle(OnCycle);
        }

        #region Gestion des joueurs
        public void AddUser(int id, Player player)
        {
            lock (this.lockedItem)
            {
                Player exist = this.GetPlayerById(id);
                if (exist != null)
                    this.players.Remove(exist);
                this.players.Add(player);
            }
        }

        public void RemoveUser(Player player)
        {
            lock (this.lockedItem)
            {
                this.players.Remove(player);
            }
        }
        #endregion

        #region Méthodes d'envoie de données
        public void SendUserList(Player player)
        {
            lock (this.lockedItem)
            {
                for (int i = 0; i < this.players.Count; i++)
                {
                    Player otherPlayer = this.players[i];
                    if (otherPlayer != null && otherPlayer != player)
                        player.SendPlayerIn(otherPlayer);
                }
            }

        }

        public void SendToAll(Player sender, ServerPacket packet, bool player_included)
        {
            byte[] rPacket = packet.GetBytes();
            lock (this.lockedItem)
            {
                foreach (Player player in this.players)
                {
                    if (player == sender && !player_included)
                        continue;
                    player.SendPacket(rPacket);
                }
            }
        }

        public void SendToAll(ServerPacket packet)
        {
            byte[] rPacket = packet.GetBytes();
            lock (this.lockedItem)
            {
                foreach (Player player in this.players)
                    player.SendPacket(rPacket);
            }
        }

        public void SendToAll(ServerPacket packet, List<Player> notReceivers)
        {
            byte[] rPacket = packet.GetBytes();
            int length = rPacket.Length;
            lock (this.lockedItem)
            {
                foreach (Player player in this.players)
                {
                    if (notReceivers.Contains(player))
                        continue;
                    player.SendPacket(rPacket);
                }
            }
        }

        public void SendToAll(string packet, string message, params object[] args)
        {
            Dictionary<string, byte[]> langsPacket = new Dictionary<string, byte[]>();
            foreach (string key in GameServer.GetLangsManager().langsServer.Keys)
            {
                langsPacket.Add(key, new ServerPacket(String.Format(packet, String.Format(GameServer.GetLanguage(key, message), args))).GetBytes());
            }
            lock (this.lockedItem)
            {
                foreach (Player player in this.players)
                    player.SendPacket(langsPacket[player.languagePack]);
            }
        }
        #endregion

        #region Sortie des utilisateurs (CI, Mini-Pays, Raids, TS, etc...)
        public void ExitAllUsers(bool isMiniland, bool specialMap, int ownerId)
        {
            List<Player> newPlayerList = new List<Player>();
            lock (this.lockedItem)
            {
                foreach (Player user in this.players)
                {
                    if (user.id == ownerId)
                    {
                        newPlayerList.Add(user);
                        continue;
                    }
                    ServerPacket packet = new ServerPacket(Outgoing.mapIni);
                    packet.AppendInt(0);
                    packet.AppendInt(user.map.mapId);
                    packet.AppendInt(0);
                    user.SendPacket(packet);
                    user.SendPacket(new ServerPacket(Outgoing.mapOut));
                    if (specialMap && GameServer.GetMapData(user.saveMap) != null)
                    {
                        if (user.GetSession().GetChannel().generalMaps.ContainsKey(user.saveMap))
                        {
                            user.map = user.GetSession().GetChannel().generalMaps[user.saveMap];
                            user.mapId = user.saveMap;
                        }
                        else
                        {
                            user.GetSession().GetChannel().generalMaps.Add(user.saveMap, new Map(user.saveMap, user.GetSession().GetChannel()));
                            user.map = user.GetSession().GetChannel().generalMaps[user.saveMap];
                            user.mapId = user.saveMap;
                        }
                        user.x = user.saveX;
                        user.y = user.saveY;
                        user.map.AddUser(user, false);
                        if (isMiniland)
                            user.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(user.languagePack, "message.miniland.ownerf")));
                    }
                }
            }
            this.players = newPlayerList;
        }
        #endregion

        #region Recherche d'utilisateurs
        public bool PlayerInListById(int id) //Savoir si un joueur est présent par son id
        {
            if (this.players.Find(x => x.GetId() == id) != null)
                return true;
            return false;
        }

        public bool PlayerInListByName(string name) //Savoir si un joueur est présent par son pseudo
        {
            if (this.players.Find(x => x.GetName() == name) != null)
                return true;
            return false;
        }

        public List<Player> GetPlayersList() //Récupérer une copie de la liste des joueurs
        {
            lock (this.lockedItem)
                return new List<Player>(this.players);
        }

        public Player GetPlayerById(int id) //Récupérer un joueur par son id
        {
            return this.players.Find(x => x.GetId() == id);
        }

        public Player GetPlayerByName(string name) //Récupérer un joueur par son nom
        {
            return this.players.Find(x => x.GetName() == name);
        }
        #endregion

        public void OnCycle()
        {
            List<Player> pList = this.GetPlayersList();
            foreach (Player player in pList)
                player.OnCycle();
        }
    }
}
