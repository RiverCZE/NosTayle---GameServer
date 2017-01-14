using NosTayleGameServer.Core;
using NosTayleGameServer.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Shops
{
    public class ShopsManager
    {
        internal Dictionary<int, Shop> shopList = new Dictionary<int, Shop>();
        internal Dictionary<int, int> shopItemsAll = new Dictionary<int, int>();

        public void Initizialize()
        {
            shopList = new Dictionary<int, Shop>();
            DataTable dataTable = null;
            WriteConsole.WriteStructure("GAME", "Load Shops list...");
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                dataTable = dbClient.ReadDataTable("SELECT * FROM shops;");
            }
            foreach (DataRow shopRow in dataTable.Rows)
            {
                shopList.Add((int)shopRow["id"], new Shop((int)shopRow["id"], (int)shopRow["type"], shopRow["name"].ToString()));
                shopList[(int)shopRow["id"]].Initialize();
            }
            Console.WriteLine("{0} shops loads !", this.shopList.Count);
        }
    }
}
