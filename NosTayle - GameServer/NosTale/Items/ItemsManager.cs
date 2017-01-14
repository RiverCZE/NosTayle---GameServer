using NosTayleGameServer.Communication;
using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.Core;
using NosTayleGameServer.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Items
{
    class ItemsManager
    {
        internal Dictionary<int, ItemBase> itemList = new Dictionary<int, ItemBase>();
        internal static Random random = new Random();

        public ItemsManager()
        {
            LoadItems();
        }

        public void LoadItems()
        {
            itemList.Clear();
            DataTable dataTable = null;
            WriteConsole.WriteStructure("GAME", "Load items list...");
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                dataTable = dbClient.ReadDataTable("SELECT * FROM items;");
            }
            foreach (DataRow itemRow in dataTable.Rows)
            {
                try
                {
                    itemList.Add((int)itemRow["vnum"], new ItemBase((int)itemRow["vnum"], itemRow["name"].ToString(), (int)itemRow["inventory"], (int)itemRow["eqSlot"], itemRow["type"].ToString(), ServerMath.StringToBool(itemRow["coloredItem"].ToString()), (int)itemRow["displayNum"], (int)itemRow["element"], (int)itemRow["percentMin"], (int)itemRow["percentMax"], (int)itemRow["damageMin"], (int)itemRow["damageMax"], (int)itemRow["corpDef"], (int)itemRow["distDef"], (int)itemRow["magicDef"], (int)itemRow["dodge"], (int)itemRow["hitRate"], (int)itemRow["criticChance"], (int)itemRow["criticDamage"], (int)itemRow["fireResist"], (int)itemRow["waterResist"], (int)itemRow["ligthResist"], (int)itemRow["darknessResist"], (int)itemRow["speed"], (int)itemRow["price"], (int)itemRow["levelReq"], (int)itemRow["joblevelReq"], (int)itemRow["icoReputReq"], itemRow["class"].ToString(), (int)itemRow["effect"], ServerMath.StringToBool(itemRow["deleteOnUse"].ToString()), itemRow["extraData"].ToString()));
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine((int)itemRow["id"] + " is buggy");
                    Console.WriteLine(ex.ToString());
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            Console.WriteLine("{0} items loads !", itemList.Count);
        }

        public void GetItemInfo(Session session, int itemId)
        {
            if (this.itemList.ContainsKey(itemId))
            {
                ItemBase iBase = this.itemList[itemId];
                ServerPacket packet = new ServerPacket(Outgoing.equipInfo);
                switch (iBase.eqSlot)
                {
                    case 0:
                        if (iBase.sword || iBase.adventer)
                        {
                            packet.AppendInt(0);
                            packet.AppendInt(iBase.id);
                            packet.AppendInt(0);
                            packet.AppendInt(0);
                            packet.AppendInt(0);
                            packet.AppendInt(iBase.levelReq);
                            packet.AppendInt(iBase.damageMin);
                            packet.AppendInt(iBase.damageMax);
                            packet.AppendInt(iBase.hitRate);
                            packet.AppendInt(iBase.criticChance);
                            packet.AppendInt(iBase.criticDamage);
                            packet.AppendInt(0);
                            packet.AppendInt(100);
                            packet.AppendInt(iBase.price);
                            packet.AppendInt(-1);
                        }
                        else if (iBase.archer)
                        {
                            packet.AppendInt(1);
                            packet.AppendInt(iBase.id);
                            packet.AppendInt(0);
                            packet.AppendInt(0);
                            packet.AppendInt(0);
                            packet.AppendInt(iBase.levelReq);
                            packet.AppendInt(iBase.damageMin);
                            packet.AppendInt(iBase.damageMax);
                            packet.AppendInt(iBase.hitRate);
                            packet.AppendInt(iBase.criticChance);
                            packet.AppendInt(iBase.criticDamage);
                            packet.AppendInt(0);
                            packet.AppendInt(100);
                            packet.AppendInt(iBase.price);
                            packet.AppendInt(-1);
                        }
                        else if (iBase.mage)
                        {
                            packet.AppendInt(5);
                            packet.AppendInt(iBase.id);
                            packet.AppendInt(0);
                            packet.AppendInt(0);
                            packet.AppendInt(0);
                            packet.AppendInt(iBase.levelReq);
                            packet.AppendInt(iBase.damageMin);
                            packet.AppendInt(iBase.damageMax);
                            packet.AppendInt(iBase.hitRate);
                            packet.AppendInt(iBase.criticChance);
                            packet.AppendInt(iBase.criticDamage);
                            packet.AppendInt(0);
                            packet.AppendInt(100);
                            packet.AppendInt(iBase.price);
                            packet.AppendInt(-1);
                        }
                        break;
                    case 1:
                        {
                            packet.AppendInt(2);
                            packet.AppendInt(iBase.id);
                            packet.AppendInt(0);
                            packet.AppendInt(0);
                            packet.AppendInt(0);
                            packet.AppendInt(iBase.levelReq);
                            packet.AppendInt(iBase.corpDef);
                            packet.AppendInt(iBase.distDef);
                            packet.AppendInt(iBase.magicDef);
                            packet.AppendInt(iBase.dodge);
                            packet.AppendInt(iBase.price);
                            packet.AppendString("-1 0 0 0");
                        }
                        break;
                    case 2:
                        {
                            packet.AppendInt(3);
                            packet.AppendInt(iBase.id);
                            packet.AppendInt(iBase.levelReq);
                            packet.AppendInt(iBase.corpDef);
                            packet.AppendInt(iBase.distDef);
                            packet.AppendInt(iBase.magicDef);
                            packet.AppendInt(iBase.dodge);
                            packet.AppendInt(0);
                            packet.AppendInt(0);
                            packet.AppendInt(0);
                            packet.AppendInt(0);
                            packet.AppendInt(iBase.price);
                            packet.AppendInt(0);
                            packet.AppendInt(0);
                            packet.AppendInt(-1);
                        }
                        break;
                    case 3:
                        {
                            packet.AppendInt(3);
                            packet.AppendInt(iBase.id);
                            packet.AppendInt(iBase.levelReq);
                            packet.AppendInt(iBase.corpDef);
                            packet.AppendInt(iBase.distDef);
                            packet.AppendInt(iBase.magicDef);
                            packet.AppendInt(iBase.dodge);
                            packet.AppendInt(iBase.fireResist);
                            packet.AppendInt(iBase.waterResist);
                            packet.AppendInt(iBase.ligthResist);
                            packet.AppendInt(iBase.darknessResist);
                            packet.AppendInt(iBase.price);
                            packet.AppendInt(0);
                            packet.AppendInt(0);
                            packet.AppendInt(-1);
                        }
                        break;
                    case 4:
                        {
                            packet.AppendInt(3);
                            packet.AppendInt(iBase.id);
                            packet.AppendInt(iBase.levelReq);
                            packet.AppendInt(iBase.corpDef);
                            packet.AppendInt(iBase.distDef);
                            packet.AppendInt(iBase.magicDef);
                            packet.AppendInt(iBase.dodge);
                            packet.AppendInt(iBase.fireResist);
                            packet.AppendInt(iBase.waterResist);
                            packet.AppendInt(iBase.ligthResist);
                            packet.AppendInt(iBase.darknessResist);
                            packet.AppendInt(iBase.price);
                            packet.AppendInt(0);
                            packet.AppendInt(0);
                            packet.AppendInt(-1);
                        }
                        break;
                    case 5:
                        if (iBase.sword || iBase.adventer)
                        {
                            packet.AppendInt(1);
                            packet.AppendInt(iBase.id);
                            packet.AppendInt(0);
                            packet.AppendInt(0);
                            packet.AppendInt(0);
                            packet.AppendInt(iBase.levelReq);
                            packet.AppendInt(iBase.damageMin);
                            packet.AppendInt(iBase.damageMax);
                            packet.AppendInt(iBase.hitRate);
                            packet.AppendInt(iBase.criticChance);
                            packet.AppendInt(iBase.criticDamage);
                            packet.AppendInt(0);
                            packet.AppendInt(100);
                            packet.AppendInt(iBase.price);
                            packet.AppendInt(-1);
                        }
                        else if (iBase.archer)
                        {
                            packet.AppendInt(0);
                            packet.AppendInt(iBase.id);
                            packet.AppendInt(0);
                            packet.AppendInt(0);
                            packet.AppendInt(0);
                            packet.AppendInt(iBase.levelReq);
                            packet.AppendInt(iBase.damageMin);
                            packet.AppendInt(iBase.damageMax);
                            packet.AppendInt(iBase.hitRate);
                            packet.AppendInt(iBase.criticChance);
                            packet.AppendInt(iBase.criticDamage);
                            packet.AppendInt(0);
                            packet.AppendInt(100);
                            packet.AppendInt(iBase.price);
                            packet.AppendInt(-1);
                        }
                        break;
                    case 9:
                        {
                            packet.AppendInt(3);
                            packet.AppendInt(iBase.id);
                            packet.AppendInt(iBase.levelReq);
                            packet.AppendInt(iBase.corpDef);
                            packet.AppendInt(iBase.distDef);
                            packet.AppendInt(iBase.magicDef);
                            packet.AppendInt(iBase.dodge);
                            packet.AppendInt(0);
                            packet.AppendInt(0);
                            packet.AppendInt(0);
                            packet.AppendInt(0);
                            packet.AppendInt(iBase.price);
                            packet.AppendInt(0);
                            packet.AppendInt(0);
                            packet.AppendInt(-1);
                        }
                        break;
                    case 10:
                        {
                            packet.AppendInt(4);
                            packet.AppendInt(iBase.id);
                            packet.AppendInt(iBase.element);
                            packet.AppendInt(iBase.percentMin);
                        }
                        break;
                    case 12:
                        /*Specialist sp = new Specialist(-1, -1, iBase.id, false, 0, 0, 1);
                        user.send(sp.getInfos(0));*/
                        return;
                    case 13:
                        {
                            packet.AppendInt(2);
                            packet.AppendInt(iBase.id);
                            packet.AppendInt(0);
                            packet.AppendInt(0);
                            packet.AppendInt(0);
                            packet.AppendInt(iBase.levelReq);
                            packet.AppendInt(iBase.corpDef);
                            packet.AppendInt(iBase.distDef);
                            packet.AppendInt(iBase.magicDef);
                            packet.AppendInt(iBase.dodge);
                            packet.AppendInt(iBase.price);
                            packet.AppendString("-1 2 -1 0");
                        }
                        break;
                    case 14:
                        {
                            packet.AppendInt(3);
                            packet.AppendInt(iBase.id);
                            packet.AppendInt(iBase.levelReq);
                            packet.AppendInt(iBase.corpDef);
                            packet.AppendInt(iBase.distDef);
                            packet.AppendInt(iBase.magicDef);
                            packet.AppendInt(iBase.dodge);
                            packet.AppendInt(iBase.fireResist);
                            packet.AppendInt(iBase.waterResist);
                            packet.AppendInt(iBase.ligthResist);
                            packet.AppendInt(iBase.darknessResist);
                            packet.AppendInt(iBase.price);
                            packet.AppendInt(0);
                            packet.AppendInt(2);
                            packet.AppendInt(-1);
                        }
                        break;
                }
                session.GetSock().Send(packet.GetBytes());
            }
        }
    }
}
