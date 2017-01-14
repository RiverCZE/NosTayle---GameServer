using NosTayleGameServer.Core;
using NosTayleGameServer.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.MiniGames
{
    class MiniGameManager
    {
        internal Dictionary<int, MiniGame> mgList;

        public MiniGameManager() { }

        public void Initialize()
        {
            WriteConsole.WriteStructure("GAME", "Load Mini-Games...");
            mgList = new Dictionary<int, MiniGame>();
            DataTable dataTable = null;
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                dataTable = dbClient.ReadDataTable("SELECT * FROM minigames;");
            }
            foreach (DataRow mgRow in dataTable.Rows)
            {
                this.mgList.Add((int)mgRow["gameId"], new MiniGame((int)mgRow["gameId"], mgRow["b0"].ToString(), mgRow["b1"].ToString(), mgRow["b2"].ToString(), mgRow["b3"].ToString(), mgRow["b4"].ToString(), mgRow["b5"].ToString(), mgRow["awards"].ToString()));
            }
            Console.WriteLine("{0} loads !", this.mgList.Count);
        }
    }
}
