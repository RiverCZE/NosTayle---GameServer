using MySql.Data.MySqlClient.Memcached;
using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.NosTale.Channels;
using NosTayleGameServer.NosTale.Entities.Managers;
using NosTayleGameServer.NosTale.Entities.Players;
using NosTayleGameServer.NosTale.Entities.Players.Inventorys;
using NosTayleGameServer.NosTale.Items;
using NosTayleGameServer.NosTale.Maps.Portals;
using NosTayleGameServer.NosTale.MiniGames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Maps.SpecialMaps
{
    public class Miniland : Map
    {
        internal Player owner;
        internal string motto;
        internal int dayVisites;
        internal int visites;
        internal int statue;
        internal string lastUpdateVisites;
        internal Dictionary<int, Item> miniLandItems = new Dictionary<int, Item>();

        public Miniland(Player owner, int statue, string motto, int dayVisites, int visites, string lastUpdateVisites)
        {
            this.mapId = 20001;
            this.channel = owner.GetSession().GetChannel();
            this.owner = owner;
            this.statue = statue;
            if (NOnlyChar(' ', motto))
                this.motto = motto;
            else
                this.motto = "Bienvenue.";
            this.dayVisites = dayVisites;
            this.visites = visites;
            this.lastUpdateVisites = lastUpdateVisites;
            this.npcsManager = new NpcsManager(-1);
            this.monstersManager = new NpcsManager(-1);
            this.portalsManager = new PortalsManager();
            this.portalsManager.AddPortal(1, new Portal(1, -1, "special_exit", 20001, 4998, 3, 8, 0, 0, 0));
            this.specialMap = true;
            this.StartCycleTask();
        }

        public bool NOnlyChar(char c, string str)
        {
            if (str.Length == 0)
                return false;
            for (int i = 0; i < str.Length; i++)
                if (str[i] != c)
                    return true;
            return false;
        }

        public void LoadItems()
        {
            foreach (InventorySlot invSlot in owner.inventory.itemsList[3])
                if (invSlot != null && invSlot.invItem.item.x > 0 && invSlot.invItem.item.y > 0)
                    this.miniLandItems.Add(invSlot.idSlot, invSlot.invItem.item);
        }

        public bool nOnlyChar(char c, string str)
        {
            if (str.Length == 0)
                return false;
            for (int i = 0; i < str.Length; i++)
                if (str[i] != c)
                    return true;
            return false;
        }

        public Item AlreadyExist(string type, int iBId)
        {
            foreach (Item item in miniLandItems.Values)
                if (item.itemBase.type == type && (item.itemBase.type == "ml_game" && item.itemBase.id == iBId))
                    return item;
            return null;
        }

        public bool PointOk(int zone, string key)
        {
            switch (zone)
            {
                case 1:
                    if (GameServer.mlZone1.points.ContainsKey(key))
                        return true;
                    break;
                case 2:
                    if (GameServer.mlZone2.points.ContainsKey(key))
                        return true;
                    break;
                case 3:
                    if (GameServer.mlZone3.points.ContainsKey(key))
                        return true;
                    break;
            }
            return false;
        }

        public bool NoNpcNoUserNoItem(int zone, int x, int y)
        {
            if (this.owner.x == x && this.owner.y == y)
                return false;
                /*lock (user.channel.miniLandList[user.player.id].npcsManager.entitieList)
                    foreach (Npc npc in user.channel.miniLandList[user.player.id].npcsManager.getNpcs())
                        if (npc.x == x && npc.y == y)
                            return false;*/
            foreach (Item item in miniLandItems.Values)
            {
                if (item.itemBase.mlZone == zone)
                {
                    int height = item.itemBase.height - 1;
                    if (height < 0)
                        height = 0;
                    int width = item.itemBase.width - 1;
                    if (width < 0)
                        width = 0;
                    for (int i = item.x; i <= item.x + width; i++)
                        for (int i2 = item.y; i2 >= item.y - height; i2--)
                            if (i == x && i2 == y)
                                return false;
                }
            }
            return true;
        }

        #region Modifications
        public void EditMoto(string motto)
        {
            if (this.motto == motto)
                return;
            if (nOnlyChar(' ', motto))
                this.motto = motto.Substring(0, motto.Length < 50 ? motto.Length : 50);
            else
                this.motto = "Bienvenue.";
            this.owner.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(this.owner.languagePack, "message.update.mlmotto")));
        }
        public void EditStatue(int statue)
        {
            this.statue = statue;
            string message = "";
            switch (statue)
            {
                case 0:
                    message = GameServer.GetLanguage(this.owner.languagePack, "message.miniland.edit.open");
                    break;
                case 1:
                    message = GameServer.GetLanguage(this.owner.languagePack, "message.miniland.edit.private");
                    break;
                case 2:
                    message = GameServer.GetLanguage(this.owner.languagePack, "message.miniland.edit.locked");
                    break;
            }
            this.owner.SendPacket(GlobalMessage.MakeAlert(4, message));
            if (statue > 0 && statue <= 2)
                this.playersManagers.ExitAllUsers(true, true, this.owner.id);
        }
        public void AddItem(int slot, int x, int y)
        {
            if (this.statue == 2)
            {
                if (this.owner.inventory.itemsList[3][slot] != null && this.owner.inventory.itemsList[3][slot].invItem.isItem)
                {
                    Item item = this.owner.inventory.itemsList[3][slot].invItem.item;
                    if (AlreadyExist(item.itemBase.type, item.itemBase.id) != null && item.itemBase.type != "ml_default")
                    {
                        this.owner.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(this.owner.languagePack, "message.miniland.itemtype.already")));
                        return;
                    }
                    if (item.x <= 0 && item.y <= 0 && item.itemBase.type.StartsWith("ml_") && item.itemBase.type != "ml_panel" && item.itemBase.type != "ml_bell")
                    {
                        int height = item.itemBase.height - 1;
                        if (height < 0)
                        {
                            height = 0;
                        }
                        int width = item.itemBase.width - 1;
                        if (width < 0)
                        {
                            width = 0;
                        }
                        for (int i = x; i <= x + width; i++)
                        {
                            for (int i2 = y; i2 >= y - height; i2--)
                            {
                                if (!PointOk(item.itemBase.mlZone, i + ";" + i2))
                                    return;
                                if (!NoNpcNoUserNoItem(item.itemBase.mlZone, i, i2))
                                    return;
                            }
                        }
                        item.x = x;
                        item.y = y;
                        miniLandItems.Add(slot, item);
                        ServerPacket packet = new ServerPacket(Outgoing.mlEff);
                        packet.AppendInt(item.itemBase.displayNum);
                        packet.AppendString(item.x + "" + item.y);
                        packet.AppendInt(item.x);
                        packet.AppendInt(item.y);
                        packet.AppendInt(0);
                        this.owner.SendPacket(packet);
                        this.owner.SendPoints();
                        packet = new ServerPacket(Outgoing.mlObj);
                        packet.AppendInt(1);
                        packet.AppendInt(slot);
                        packet.AppendInt(item.x);
                        packet.AppendInt(item.y);
                        packet.AppendInt(item.itemBase.width);
                        packet.AppendInt(item.itemBase.height);
                        packet.AppendInt(item.durabilites);
                        packet.AppendInt(8);
                        packet.AppendInt(0);
                        packet.AppendInt(0);
                        this.owner.SendPacket(packet);
                        this.owner.inventory.itemsList[3][slot].Updated = true;
                    }
                    else
                        this.owner.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(this.owner.languagePack, "message.miniland.item.alreadyplaced")));
                }
            }
            else
                this.owner.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(this.owner.languagePack, "message.miniland.mustlock")));
        }
        public void RemoveItem(int slot)
        {
            if (this.statue == 2)
            {
                if (this.owner.inventory.itemsList[3][slot] != null && this.owner.inventory.itemsList[3][slot].invItem.isItem)
                {
                    Item item = this.owner.inventory.itemsList[3][slot].invItem.item;
                    if (item.x > 0 && item.y > 0 && item.itemBase.type.StartsWith("ml_"))
                    {
                        ServerPacket packet = new ServerPacket("eff_g");
                        packet.AppendInt(item.itemBase.displayNum);
                        packet.AppendString(item.x.ToString() + item.y.ToString());
                        packet.AppendInt(item.x);
                        packet.AppendInt(item.y);
                        packet.AppendInt(1);
                        this.owner.SendPacket(packet);
                        this.owner.SendPoints();
                        packet = new ServerPacket("mlobj");
                        packet.AppendInt(0);
                        packet.AppendInt(slot);
                        packet.AppendInt(item.x);
                        packet.AppendInt(item.y);
                        packet.AppendInt(item.itemBase.width);
                        packet.AppendInt(item.itemBase.height);
                        packet.AppendInt(item.durabilites);
                        packet.AppendInt(8);
                        packet.AppendInt(0);
                        packet.AppendInt(0);
                        this.owner.SendPacket(packet);
                        item.x = 0;
                        item.y = 0;
                        miniLandItems.Remove(slot);
                        //SAVESLOT
                        this.owner.inventory.itemsList[3][slot].Updated = true;
                    }
                }
            }
            else
                this.owner.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(this.owner.languagePack, "message.miniland.mustlock2")));
        }
        #endregion

        #region Envoie de packets
        public void SendItem(Player player)
        {
            List<ServerPacket> messages_items = new List<ServerPacket>();
            ServerPacket message_base = new ServerPacket(Outgoing.mltObj);
            foreach (int key in miniLandItems.Keys)
            {
                Item item = miniLandItems[key];
                message_base.AppendString(item.itemBase.id + "." + key + "." + item.x + "." + item.y);
                ServerPacket packet = new ServerPacket(Outgoing.mlEff);
                packet.AppendInt(item.itemBase.displayNum);
                packet.AppendString(item.x + "" + item.y);
                packet.AppendInt(item.x);
                packet.AppendInt(item.y);
                packet.AppendInt(0);
                messages_items.Add(packet);
            }
            player.SendPacket(message_base);
            foreach (ServerPacket message in messages_items)
                player.SendPacket(message);
        }
        override public void SendOtherMapInfos(Player receiver)
        {
            if (this.mapId == 20001)
            {
                if (this.owner != receiver)
                {
                    this.AddVisite();
                    receiver.SendPacket(GlobalMessage.MakeAlert(0, this.motto));
                    ServerPacket packet = new ServerPacket(Outgoing.mlvisitesenter);
                    packet.AppendInt(3800);
                    packet.AppendString(this.owner.name);
                    packet.AppendInt(this.dayVisites);
                    packet.AppendInt(this.visites);
                    packet.AppendInt(this.GetMaxPlayer());
                    packet.AppendString(this.motto);
                    receiver.SendPacket(packet);
                }
                this.SendItem(receiver);
                if (this.owner == receiver)
                    this.SendUserMLInfos();
                receiver.SendPacket(this.MakeVisitesMessages(receiver.languagePack));
            }
        }
        public void SendUserMLInfos()
        {
            if (this.lastUpdateVisites != GameServer.GetActualDate())
            {
                this.lastUpdateVisites = GameServer.GetActualDate();
                this.dayVisites = 0;
            }
            ServerPacket packet = new ServerPacket(Outgoing.miniLandInfo);
            packet.AppendInt(3800);
            packet.AppendInt(this.owner.gamePoints);
            packet.AppendInt(100);
            packet.AppendInt(this.dayVisites);
            packet.AppendInt(this.visites);
            packet.AppendInt(10);
            packet.AppendInt(this.statue);
            packet.AppendString("Mélodie^Mini-Pays^'Printemps'");
            packet.AppendString(this.motto.Replace(' ', '^'));
            this.owner.SendPacket(packet);
        }
        public void EnterMiniLand(Player user)
        {
            if (user == this.owner)
            {
                user.map.ChangeMapRequest(user, this, 5, 8);
                return;
            }
            if (this.owner != null)
            {
                if (this.playersManagers.playersCount >= this.GetMaxPlayer())
                {
                    user.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(user.languagePack, "message.minilandFull")));
                    return;
                }
                switch (this.statue)
                {
                    case 0:
                        user.map.ChangeMapRequest(user, this, 5, 8);
                        break;
                    case 1:
                        if (owner.friendList.GetFriend(user.id) != null)
                        {
                            user.map.ChangeMapRequest(user, this, 5, 8);
                            return;
                        }
                        user.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(user.languagePack, "message.miniland.private")));
                        break;
                    case 2:
                        user.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(user.languagePack, "message.miniland.locked")));
                        break;
                }
            }
        }
        #endregion

        #region Actions sur les objets
        public void UseObj(Player receiver, int slot)
        {
            if (this.miniLandItems[slot] != null)
            {
                Item item = this.miniLandItems[slot];
                switch (item.itemBase.type)
                {
                    case "ml_game":
                        {
                            if (GameServer.GetMiniGameManager().mgList.ContainsKey(item.itemBase.gameId))
                            {
                                MiniGame minigame = GameServer.GetMiniGameManager().mgList[item.itemBase.gameId];
                                ServerPacket packet = new ServerPacket("mlo_info");
                                packet.AppendBool(owner == receiver);
                                packet.AppendInt(item.itemBase.id);
                                packet.AppendInt(slot);
                                packet.AppendInt(receiver.gamePoints);
                                packet.AppendBool(item.durabilites < 1000);
                                packet.AppendInt(0);
                                packet.AppendInt(minigame.b0Min);
                                packet.AppendInt(minigame.b0Max);
                                packet.AppendInt(minigame.b1Min);
                                packet.AppendInt(minigame.b1Max);
                                packet.AppendInt(minigame.b2Min);
                                packet.AppendInt(minigame.b2Max);
                                packet.AppendInt(minigame.b3Min);
                                packet.AppendInt(minigame.b3Max);
                                packet.AppendInt(minigame.b4Min);
                                packet.AppendInt(minigame.b4Max);
                                packet.AppendInt(minigame.b5Min);
                                packet.AppendInt(minigame.b5Max);
                                receiver.SendPacket(packet);
                            }
                        }
                        break;
                    case "ml_warehouse":
                        if (this.owner == receiver)
                        {
                            ServerPacket packet = new ServerPacket("stash_all");
                            packet.AppendInt(item.itemBase.wSlot);
                            receiver.SendPacket(packet);
                        }
                        break;
                }
            }
        }
        public void PlayGame(Player user, int slot, int iBaseId)
        {
            if (this.miniLandItems[slot] != null)
            {
                if (this.miniLandItems[slot].itemBase.id == iBaseId && this.miniLandItems[slot].itemBase.type == "ml_game")
                {
                    Item item = this.miniLandItems[slot];
                    if (GameServer.GetMiniGameManager().mgList.ContainsKey(item.itemBase.gameId))
                    {
                        MiniGame minigame = GameServer.GetMiniGameManager().mgList[item.itemBase.gameId];
                        ServerPacket packet = new ServerPacket("mlo_st");
                        packet.AppendInt(minigame.gameId);
                        user.SendPacket(packet);
                    }
                }
            }
        }
        public void GameAwardLook(Player user, int slot, int iBaseId, int score)
        {
            if (this.miniLandItems[slot] != null)
            {
                if (this.miniLandItems[slot].itemBase.id == iBaseId && this.miniLandItems[slot].itemBase.type == "ml_game")
                {
                    Item item = this.miniLandItems[slot];
                    if (GameServer.GetMiniGameManager().mgList.ContainsKey(item.itemBase.gameId))
                    {
                        MiniGame minigame = GameServer.GetMiniGameManager().mgList[item.itemBase.gameId];
                        minigame.LookAwards(user, score);
                    }
                }
            }
        }
        public void GameTakeAward(Player user, int slot, int iBaseId, int box)
        {
            if (this.miniLandItems[slot] != null)
            {
                if (this.miniLandItems[slot].itemBase.id == iBaseId && this.miniLandItems[slot].itemBase.type == "ml_game")
                {
                    Item item = this.miniLandItems[slot];
                    if (GameServer.GetMiniGameManager().mgList.ContainsKey(item.itemBase.gameId))
                    {

                        MiniGame minigame = GameServer.GetMiniGameManager().mgList[item.itemBase.gameId];
                        minigame.TakeBox(user, this.miniLandItems[slot].itemBase.gameLevel, box);
                    }
                }
            }
        }
        public bool GameAction(Player user, string[] packet)
        {
            /*
                       * mg 7 slot vnum = Récolte
                       * mg 5 slot vnum = Durabilité
                       * mg 1 slot vnum = Jouer
                       * mg 3 slot vnum score score = Récompense
                       * mg 2 slot vnum = Quitte
                       * mg 4 slot vnum box = Prendre boîte
            */
            switch (packet[0])
            {
                case "1":
                    if (packet.Length == 3)
                        this.PlayGame(user, Convert.ToInt32(packet[1]), Convert.ToInt32(packet[2]));
                    break;
                case "3":
                    if (packet.Length == 5)
                        this.GameAwardLook(user, Convert.ToInt32(packet[1]), Convert.ToInt32(packet[2]), Convert.ToInt32(packet[3]));
                    break;
                case "4":
                    if (packet.Length == 4)
                        this.GameTakeAward(user, Convert.ToInt32(packet[1]), Convert.ToInt32(packet[2]), Convert.ToInt32(packet[3]));
                    break;
                default:
                    return false;
            }
            return true;
        }
        #endregion
        public int GetMaxPlayer() { return 10; }
        public ServerPacket MakeVisitesMessages(string languagePack)
        {
            return GlobalMessage.MakeMessage(0, 0, 10, String.Format(GameServer.GetLanguage(languagePack, "message.minilanvisites"), this.visites, this.dayVisites));
        }

        public void AddVisite()
        {
            if (this.lastUpdateVisites != GameServer.GetActualDate())
            {
                this.lastUpdateVisites = GameServer.GetActualDate();
                this.dayVisites = 0;
            }
            owner.miniLand.visites++;
            owner.miniLand.dayVisites++;
            this.SendUserMLInfos();
        }
    }
}
