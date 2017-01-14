using NosTayleGameServer.Communication;
using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.Core;
using NosTayleGameServer.Core.Langs;
using NosTayleGameServer.Net;
using NosTayleGameServer.NosTale.Channels;
using NosTayleGameServer.NosTale.Entities.Bases;
using NosTayleGameServer.NosTale.Entities.Managers;
using NosTayleGameServer.NosTale.Entities.Players;
using NosTayleGameServer.NosTale.Familys;
using NosTayleGameServer.NosTale.Items;
using NosTayleGameServer.NosTale.Levels;
using NosTayleGameServer.NosTale.Maps;
using NosTayleGameServer.NosTale.Maps.MiniLandZone;
using NosTayleGameServer.NosTale.Maps.Portals;
using NosTayleGameServer.NosTale.MiniGames;
using NosTayleGameServer.NosTale.Missions.Quests;
using NosTayleGameServer.NosTale.Shops;
using NosTayleGameServer.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale
{
    static class GameServer
    {
        private static ConfigurationData configuration = new ConfigurationData("config.conf"); //Load configuration file
        private static string clientVersion;
        private static List<Channel> channels;
        private static GmServerListener adminToolsServer;
        public static int serverId
        {
            get { return Convert.ToInt32(GameServer.GetConfigurationData("server.id")); }
        }
        public static DateTime startedTime;

        //IDS UNIQUES
        private static int lastItemId;
        private static int lastSpId;
        private static int lastFairyId;
        private static int lastNpcId;
        private static int lastPortalId;
        private static int lastFamilyId;

        //MINILANDS
        public static MinilandZone mlZone1;
        public static MinilandZone mlZone2;
        public static MinilandZone mlZone3;

        //MANAGERS
        private static DatabaseManager databaseManager;
        private static ItemsManager itemsManager;
        private static PlayersManager playersOnServer;
        private static Dictionary<int, MapData> mapsData;
        private static Dictionary<int, PortalsManager> portals = new Dictionary<int, PortalsManager>();
        private static LangsManager langsManager;
        private static NpcsManager npcsManager = new NpcsManager(-1);
        private static NpcsManager monstersManager = new NpcsManager(-1);
        public static List<int> villageMapId = new List<int>() { 1, 9 };
        public static LevelsManager levelsManager;
        public static List<Family> familys;
        private static ShopsManager shopsManager;
        private static MiniGameManager miniGameManager;
        private static QuestManager questsManager;

        //TASKING
        internal static Task familyCycleTask;

        public static void Initialize() //START GAME SERVER
        {
            Console.OutputEncoding = Encoding.GetEncoding(1258);
            Console.InputEncoding = Encoding.GetEncoding(1258);
            Console.Title = "NosReturn - GameServer";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("NosReturn Game Server 1.0 bêta by NosReturn");
            Console.ForegroundColor = ConsoleColor.White;
            GameServer.clientVersion = GameServer.GetConfigurationData("client.version");
            Console.WriteLine("    -For version: " + GameServer.clientVersion);
            DateTime now = DateTime.Now;
            //ALL LOAD SERVER SYSTEMS
            DatabaseServer databaseServer = new DatabaseServer(GameServer.GetConfigurationData("db.hostname"), uint.Parse(GameServer.GetConfigurationData("db.port")), GameServer.GetConfigurationData("db.username"), GameServer.GetConfigurationData("db.password"));
            Database database = new Database(GameServer.GetConfigurationData("db.name"), uint.Parse(GameServer.GetConfigurationData("db.pool.minsize")), uint.Parse(GameServer.GetConfigurationData("db.pool.maxsize")));
            GameServer.databaseManager = new DatabaseManager(databaseServer, database);
            WriteConsole.WriteStructure("MySQL", "Connection to database...");
            using (DatabaseClient dbClient = GameServer.databaseManager.GetClient())
            {
                Console.WriteLine("Ok!");
                StaticMessageHandler.Initialize();
                GameServer.itemsManager = new ItemsManager();
                GameServer.LoadPortals();
                GameServer.playersOnServer = new PlayersManager();
                GameServer.langsManager = new LangsManager();
                EntitieBaseManager.Initialize();
                GameServer.npcsManager.Initialize(2);
                GameServer.monstersManager.Initialize(3);
                GameServer.shopsManager = new ShopsManager();
                GameServer.shopsManager.Initizialize();
                GameServer.questsManager = new QuestManager();
                GameServer.miniGameManager = new MiniGameManager();
                GameServer.miniGameManager.Initialize();
                GameServer.mlZone1 = new MinilandZone(1);
                GameServer.mlZone2 = new MinilandZone(2);
                GameServer.mlZone3 = new MinilandZone(3);
                GameServer.LoadMapsData();
                try
                {
                    GameServer.lastItemId = (int)dbClient.ReadDataRow("SELECT ID FROM items_server" + GameServer.serverId + " ORDER BY ID DESC LIMIT 1")[0];
                }
                catch { }
                try
                {
                    GameServer.lastSpId = (int)dbClient.ReadDataRow("SELECT SPID FROM sps_server" + GameServer.serverId + " ORDER BY SPID DESC LIMIT 1")[0];
                }
                catch { }
                try
                {
                    GameServer.lastFairyId = (int)dbClient.ReadDataRow("SELECT FAIRYID FROM fairies_server" + GameServer.serverId + " ORDER BY FAIRYID DESC LIMIT 1")[0];
                }
                catch { }
                try
                {
                    GameServer.lastNpcId = (int)dbClient.ReadDataRow("SELECT ID FROM entities ORDER BY ID DESC LIMIT 1")[0];
                }
                catch { }
                try
                {
                    GameServer.lastFamilyId = (int)dbClient.ReadDataRow("SELECT ID FROM familys_server" + GameServer.serverId + " ORDER BY ID DESC LIMIT 1")[0];
                }
                catch { }
                GameServer.lastItemId++;
                GameServer.lastSpId++;
                GameServer.lastFairyId++;
                GameServer.lastNpcId++;
                GameServer.lastFamilyId++;
                GameServer.levelsManager = new LevelsManager(99, 80);
                //Chargement des canaux
                GameServer.channels = new List<Channel>();
                for (int i = 1; i <= Convert.ToInt32(GameServer.GetConfigurationData("channel.count")); i++)
                    channels.Add(new Channel(i, GameServer.GetConfigurationData("channel." + i + ".ip"), GameServer.GetConfigurationData("channel." + i + ".port"), GameServer.lastPortalId, GameServer.lastNpcId));
                foreach (Channel channel in GameServer.channels)
                    channel.GetListener().StartListening();
                GameServer.adminToolsServer = new GmServerListener(GameServer.GetConfigurationData("admintools.ip"), Convert.ToInt32(GameServer.GetConfigurationData("admintools.port")));

                WriteConsole.WriteStructure("MySQL", "Database update...");
                dbClient.ExecuteQuery("UPDATE accounts SET online = '0';");
                Console.WriteLine("Ok!");
            }
            GameServer.familys = new List<Family>();
            GameServer.StartFamilyTask();
            TimeSpan timeSpan = DateTime.Now - now;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[SERVER] ");
            Console.ForegroundColor = ConsoleColor.White;
            GameServer.startedTime = DateTime.Now;
            Console.WriteLine("=> Server Ready {0} s, {1} ms !", timeSpan.Seconds, timeSpan.Milliseconds);
        }

        public static string GetConfigurationData(string dataName) { return GameServer.configuration.data[dataName]; }
        public static DatabaseManager GetDatabaseManager() { return GameServer.databaseManager; }
        public static ItemsManager GetItemsManager() { return GameServer.itemsManager; }
        public static PlayersManager GetPlayersManager() { return GameServer.playersOnServer; }
        public static GmServerListener GetAdminToolsServer() { return GameServer.adminToolsServer; }
        public static Dictionary<int, PortalsManager> GetPortals() { return GameServer.portals; }
        public static LangsManager GetLangsManager() { return GameServer.langsManager; }
        public static string GetLanguage(string pack, string key) { return GameServer.langsManager.GetText(pack, key); }
        public static string GetItemName(string pack, string key) { return GameServer.langsManager.GetItemName(pack, key); }
        public static bool LanguageExist(string pack) { if (GameServer.langsManager.langsServer.ContainsKey(pack)) return true; return false; }
        public static string GetActualDate() { return DateTime.Now.ToString("dd/MM/yyyy"); }
        public static NpcsManager GetNpcsManager() { return GameServer.npcsManager; }
        public static NpcsManager GetMonstersManager() { return GameServer.monstersManager; }
        public static LevelsManager GetLevelsManager() { return GameServer.levelsManager; }
        public static ShopsManager GetShopManager() { return GameServer.shopsManager; }
        public static MiniGameManager GetMiniGameManager() { return GameServer.miniGameManager; }
        public static QuestManager GetQuestsManager() { return GameServer.questsManager; }

        public static void LoadPortals()
        {
            GameServer.portals.Clear();
            DataTable dataTable = null;
            WriteConsole.WriteStructure("GAME", "Load portals...");
            using (DatabaseClient dbClient = GameServer.databaseManager.GetClient())
            {
                dataTable = dbClient.ReadDataTable("SELECT * FROM portals;");
            }
            int i = 0;
            foreach (DataRow portailRow in dataTable.Rows)
            {
                i++;
                if ((int)portailRow["id"] > lastPortalId)
                    lastPortalId = (int)portailRow["id"];
                if (!GameServer.portals.ContainsKey((int)portailRow["mapId"]))
                    GameServer.portals.Add((int)portailRow["mapId"], new PortalsManager());
                GameServer.portals[(int)portailRow["mapId"]].AddPortal((int)portailRow["id"], new Portal((int)portailRow["id"], (int)portailRow["type"], portailRow["portalDType"].ToString(), (int)portailRow["mapId"], (int)portailRow["title"], (int)portailRow["xPos"], (int)portailRow["yPos"], (int)portailRow["mapDirection"], (int)portailRow["xMapDirection"], (int)portailRow["yMapDirection"]));
            }
            Console.WriteLine("{0} portals loads !", i);
        }

        #region Informations sur les bases de maps
        public static void LoadMapsData()
        {
            WriteConsole.WriteStructure("GAME", "Load map zones...");
            GameServer.mapsData = new Dictionary<int, MapData>();
            var files = from file in Directory.EnumerateFiles(@".\\mapszones", "*", SearchOption.AllDirectories)
                        select new
                        {
                            File = file,
                        };

            foreach (var f in files)
            {
                MapData mapData = new MapData();
                mapData.Initialize(f.File);
                mapsData.Add(Convert.ToInt32(f.File.Substring(".\\mapszones\\".Length + 1)), mapData);
            }
            Console.WriteLine("{0} map zones loads !", files.Count());
        }

        public static MapData GetMapData(int mapId)
        {
            if (GameServer.mapsData.ContainsKey(mapId))
                return GameServer.mapsData[mapId];
            else
                return null;
        }
        #endregion

        public static void UpdateLanguage()
        {
            LangsManager newLg = new LangsManager();
            GameServer.langsManager = newLg;
        }

        #region Nouveaux ids pour objets, sp etc....
        internal static Object itemId = new Object();
        public static int NewItemId() { lock (GameServer.itemId) { return lastItemId++; } }
        internal static Object spId = new Object();
        public static int NewSpId() { lock (GameServer.spId) { return lastSpId++; } }
        internal static Object fairyId = new Object();
        public static int NewFairyId() { lock (GameServer.fairyId) { return lastFairyId++; } }
        internal static Object familyId = new Object();
        public static int NewFamilyId() { lock (GameServer.familyId) { return lastFamilyId++; } }
        #endregion

        #region Envoie d'informations aux utilisateurs
        public static bool SendWhisper(Player sender, string namereceiver, string message)
        {
            Player receiver = GameServer.playersOnServer.GetPlayerByName(namereceiver);
            if (receiver != null)
            {
                string end = "";
                if (receiver.GetSession().GetChannel() != sender.GetSession().GetChannel())
                    end = String.Format(GameServer.GetLanguage(receiver.languagePack, "message.end.channel"), sender.GetSession().GetChannel().id);
                sender.SendPacket(GlobalMessage.MakeSpeak(sender.type, sender.id, (receiver.isGm && receiver.gmActivate ? 8 : 5), (receiver.isGm && receiver.gmActivate ? receiver.name : sender.name), message));
                receiver.SendPacket(GlobalMessage.MakeSpeak(sender.type, sender.id, (sender.isGm && sender.gmActivate ? 15 : 5), sender.name, message + end));
                if (sender.map == receiver.map && sender.isGm && sender.gmActivate)
                    receiver.SendPacket(GlobalMessage.MakeSpeak(1, sender.id, 1, sender.name, message));
                if (end != "")
                    sender.SendPacket(GlobalMessage.MakeMessage(sender.id, sender.type, 11, String.Format(GameServer.GetLanguage(sender.languagePack, "message.sendwhisper.channel"), namereceiver, receiver.GetSession().GetChannel().id)));
                return true;
            }
            return false;
        }
        #endregion

        #region Méthodes pour les familles
        public static void StartFamilyTask()
        {
            GameServer.familyCycleTask = new Task(FamilyCycle);
            familyCycleTask.Start();
        }

        public static void FamilyCycle()
        {
            while (true)
            {
                try
                {
                    lock (GameServer.familys)
                    {
                        foreach (Family f in GameServer.familys)
                            if (!f.isCycling)
                                f.onCycle();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"familytaskerror.txt", true))
                    {
                        file.WriteLine(ex.ToString());
                    }
                }
                System.Threading.Thread.Sleep(25);
            }
        }

        public static Family GetFamily(int id)
        {
            Family family = GameServer.familys.Find(x => x.GetId() == id);
            if (family != null)
                return family;
            family = Family.LoadFamily(id);
            if (family != null)
                GameServer.familys.Add(family);
            return GameServer.familys.Find(x => x.GetId() == id);
        }

        public static void DeleteFamily(Family family)
        {
            lock (GameServer.familys)
                GameServer.familys.Remove(family);
        }
        #endregion

        public static int Timestamp()
        {
            return (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        public static void Destroy()
        {
            try
            {
                foreach (Channel c in GameServer.channels)
                {
                    c.CloseChannel();
                }
            }
            catch { }
            try
            {
                Console.WriteLine("Disconnect all players.");
                List<Player> players = GameServer.GetPlayersManager().GetPlayersList();
                foreach (Player p in players)
                {
                    p.GetSession().GetSock().CloseConnection();
                }
            }
            catch { }
            try
            {
                Console.WriteLine("Destroying database manager.");
                MySql.Data.MySqlClient.MySqlConnection.ClearAllPools();
                GameServer.databaseManager = null;
            }
            catch { }
        }
    }
}
