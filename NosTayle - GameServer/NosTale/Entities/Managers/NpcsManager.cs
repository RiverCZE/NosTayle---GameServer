using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.Core;
using NosTayleGameServer.NosTale.Entities.Bases;
using NosTayleGameServer.NosTale.Entities.Npcs;
using NosTayleGameServer.NosTale.Maps;
using NosTayleGameServer.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Entities.Managers
{
    class NpcsManager
    {
        internal int mapId;
        internal List<Npc> entitieList = new List<Npc>();
        private static Random randomNpc = new Random();
        public static int Random(int min, int max) { lock (NpcsManager.randomNpc) { return NpcsManager.randomNpc.Next(min, max); } }


        internal int npcCount
        {
            get
            {
                return this.entitieList.Count();
            }
        }

        public NpcsManager(int mapId)
        {
            this.mapId = mapId;
        }

        public NpcsManager(int mapId, List<Npc> npcList, bool cycleMap, Map map)
        {
            this.mapId = mapId;
            lock (entitieList)
            {
                foreach (Npc npc in npcList)
                {
                    if (npc.mapId == mapId && !this.entitieList.Exists(x => x.id == npc.id))
                    {
                        lock (this.entitieList)
                            this.entitieList.Add(new Npc(npc, map));
                    }
                }
            }
        }

        public void Initialize(int type)
        {
            entitieList = new List<Npc>();
            DataTable dataTable = null;
            WriteConsole.WriteStructure("GAME", String.Format("Load {0} list...", type == 2 ? "NPC's" : "MONSTERS"));
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                dataTable = dbClient.ReadDataTable("SELECT * FROM entities WHERE type=" + type + ";");
            }
            int i = 0;
            foreach (DataRow npcRow in dataTable.Rows)
            {
                i++;
                EntitieBase eBase = EntitieBaseManager.GetEntitieBase((int)npcRow["vnum"]);
                lock (this.entitieList)
                    entitieList.Add(new Npc((int)npcRow["id"], (int)npcRow["vnum"], GameCrypto.GetRealString(npcRow["name"].ToString()), (int)npcRow["type"], (int)npcRow["level"], (int)npcRow["attackUpgrade"], (int)npcRow["defenseUpgrade"], (int)npcRow["map"], (int)npcRow["x"], (int)npcRow["y"], (int)npcRow["direction"], eBase.hp, eBase.mp, eBase.element_type, eBase.element, eBase.fire_res, eBase.water_res, eBase.light_res, eBase.dark_res, eBase.attack_min, eBase.attack_max, eBase.hitrate, eBase.corpDef, eBase.distDef, eBase.magicDef, eBase.dodge, (int)npcRow["respawnTime"], ServerMath.StringToBool(npcRow["strictRespawn"].ToString()), ServerMath.StringToBool(npcRow["strictBase"].ToString()), ServerMath.StringToBool(npcRow["botMove"].ToString()), ServerMath.StringToBool(npcRow["shop"].ToString()), (int)npcRow["shopType"], (int)npcRow["menuType"], GameCrypto.GetRealString(npcRow["shopName"].ToString()), (int)npcRow["shopId"], npcRow["shopList"].ToString().Split(';'), (int)npcRow["dialogId"], npcRow["nRunList"].ToString().Split(';'), "default"));
            }
            Console.WriteLine("{0} {1} loads !", i,  type == 2 ? "npc's" : "monsters");
        }

        public void OnCycle(Map map)
        {
            try
            {
                List<Npc> outNpc = new List<Npc>();
                for (int i = 0; i < this.entitieList.Count; i++)
                {
                    Npc npc = this.entitieList[i];
                    npc.OnCycle(map);
                    if (npc.sVerif())
                        outNpc.Add(npc);
                }
                lock (this.entitieList)
                    foreach (Npc npc in outNpc)
                        this.entitieList.Remove(npc);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public bool NpcInListById(int id)
        {
            if (this.entitieList.Exists(x => x.id == id))
                return true;
            return false;
        }

        public Npc GetNpcById(int id)
        {
            return this.entitieList.Find(x => x.id == id);
        }

        public List<Npc> GetNpcs()
        {
            return this.entitieList;
        }
    }
}
