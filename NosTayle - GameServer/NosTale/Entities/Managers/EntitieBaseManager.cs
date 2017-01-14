using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.Core;
using NosTayleGameServer.NosTale.Entities.Bases;
using NosTayleGameServer.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Entities.Managers
{
    public static class EntitieBaseManager
    {
        private static Dictionary<int, EntitieBase> entitieList = new Dictionary<int, EntitieBase>();

        public static void Initialize()
        {
            EntitieBaseManager.entitieList = new Dictionary<int, EntitieBase>();
            DataTable dataTable = null;
            WriteConsole.WriteStructure("GAME", "Load entities bases...");
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                dataTable = dbClient.ReadDataTable("SELECT * FROM entitie_base;");
            }
            int i = 0;
            foreach (DataRow entitieRow in dataTable.Rows)
            {
                i++;
                EntitieBaseManager.entitieList.Add((int)entitieRow["id"], new EntitieBase((int)entitieRow["id"], GameCrypto.GetRealString(entitieRow["name"].ToString()), (int)entitieRow["levelBase"], (int)entitieRow["type"], (int)entitieRow["agressive"], ServerMath.StringToBool(entitieRow["canCapture"].ToString()), ServerMath.StringToBool(entitieRow["canMoved"].ToString()), ServerMath.StringToBool(entitieRow["canBeDeplaced"].ToString()), (int)entitieRow["elementType"], (int)entitieRow["element"], (int)entitieRow["hp"], (int)entitieRow["mp"], (int)entitieRow["attackType"], (int)entitieRow["attackMin"], (int)entitieRow["attackMax"], (int)entitieRow["attackCells"], (int)entitieRow["hitrate"], (int)entitieRow["criticRate"], (int)entitieRow["criticDegats"], (int)entitieRow["corpDef"], (int)entitieRow["distDef"], (int)entitieRow["magicDef"], (int)entitieRow["dodge"], (int)entitieRow["fireRes"], (int)entitieRow["waterRes"], (int)entitieRow["lightRes"], (int)entitieRow["darkRes"], (int)entitieRow["exp"], (int)entitieRow["expJob"], entitieRow["effect"].ToString(), entitieRow["drops"].ToString()));
            }
            Console.WriteLine("{0} entities base loads !", i);
        }

        public static EntitieBase GetEntitieBase(int id)
        {
            if (EntitieBaseManager.entitieList.ContainsKey(id))
                return EntitieBaseManager.entitieList[id];
            else
                return new EntitieBase(id, "-", 1, 1, 0, false, false, false, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, "", "");
        }
    }
}
