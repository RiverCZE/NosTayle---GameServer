using NosTayleGameServer.NosTale.Entities.Players;
using NosTayleGameServer.NosTale.Entities.Players.Inventorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.UpgradeSystem
{
    static class UpgradeController
    {
        public static Random upRandom = new Random();

        public static void GetUp(Player user, string[] data)
        {
            try
            {
                switch (data.Length)
                {
                    case 3:
                        switch (data[0])
                        {
                            case "7":
                                if (data[1] == "0")
                                {
                                    if (user.inventory.itemsList[0][Convert.ToInt32(data[2])] != null)
                                    {
                                        InventoryItem item = user.inventory.itemsList[0][Convert.ToInt32(data[2])].invItem;
                                        if (item.isItem)
                                            ItemUpgradeManager.RareUpgrade(user, item, Convert.ToInt32(data[2]));
                                    }
                                }
                                break;
                        }
                        break;
                    case 4:
                        if (user.inventory.itemsList[0][Convert.ToInt32(data[2])] != null)
                        {
                            InventoryItem item = user.inventory.itemsList[0][Convert.ToInt32(data[2])].invItem;
                            if (item.isSp)
                            {
                                if (item.specialist.upgrade < 15)
                                    SpUpgradeManager.UpgradeSp(user, item.specialist, user.inventory.itemsList[0][Convert.ToInt32(data[2])], Convert.ToInt32(data[2]), Convert.ToInt32(data[0]));
                            }
                            else if (item.isItem)
                            {
                                switch (Convert.ToInt32(data[0]))
                                {
                                    case 1:
                                        if (item.item.upgrade < 10)
                                            ItemUpgradeManager.UpgradeItem(user, item.item, user.inventory.itemsList[0][Convert.ToInt32(data[2])], Convert.ToInt32(data[2]), Convert.ToInt32(data[0]));
                                        break;
                                    default:
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("Error up system ID: {0} for ITEM", data[0]);
                                        Console.ForegroundColor = ConsoleColor.White;
                                        break;
                                }
                            }
                        }
                        break;
                }
            }
            catch
            {
            }
        }
    }
}
