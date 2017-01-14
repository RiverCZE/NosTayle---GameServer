using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.NosTale.Entities.Players;
using NosTayleGameServer.NosTale.Entities.Players.Inventorys;
using NosTayleGameServer.NosTale.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.UpgradeSystem
{
    static class ItemUpgradeManager
    {

        internal static Dictionary<int, int[]> raresProba;
        internal static List<int> degatsUp = new List<int>() { 0, 10, 15, 22, 32, 43, 54, 65, 90, 120, 200 };
        public static Dictionary<int, ItemUpgrade> upgradeList = new Dictionary<int, ItemUpgrade>();

        static ItemUpgradeManager()
        {
            raresProba = new Dictionary<int, int[]>();
            raresProba.Add(-2, new int[8] { 5, 20, 16, 14, 13, 12, 11, 9 });
            raresProba.Add(-1, new int[8] { 20, 18, 14, 12, 11, 10, 9, 6 });
            raresProba.Add(0, new int[8] { 40, 13, 9, 9, 9, 8, 7, 5 });
            raresProba.Add(1, new int[8] { 45, 13, 9, 8, 8, 7, 6, 4 });
            raresProba.Add(2, new int[8] { 50, 12, 8, 8, 7, 6, 5, 4 });
            raresProba.Add(3, new int[8] { 55, 10, 8, 7, 7, 6, 4, 3 });
            raresProba.Add(4, new int[8] { 60, 9, 7, 6, 6, 5, 4, 3 });
            raresProba.Add(5, new int[8] { 65, 8, 7, 6, 5, 4, 3, 2 });
            raresProba.Add(6, new int[8] { 70, 8, 7, 5, 4, 3, 2, 1 });
            raresProba.Add(7, new int[8] { 70, 8, 7, 5, 4, 3, 2, 1 });

            upgradeList.Add(0, new ItemUpgrade(1));
            upgradeList.Add(1, new ItemUpgrade(2));
            upgradeList.Add(2, new ItemUpgrade(3));
            upgradeList.Add(3, new ItemUpgrade(4));
            upgradeList.Add(4, new ItemUpgrade(5));
            upgradeList.Add(5, new ItemUpgrade(6));
            upgradeList.Add(6, new ItemUpgrade(7));
            upgradeList.Add(7, new ItemUpgrade(8));
            upgradeList.Add(8, new ItemUpgrade(9));
            upgradeList.Add(9, new ItemUpgrade(10));
        }

        public static ItemUpgrade GetUpgrade(int upgrade)
        {
            if (upgradeList.ContainsKey(upgrade))
                return upgradeList[upgrade];
            return null;
        }

        public static void RareUpgrade(Player user, InventoryItem invItem, int slot)
        {
            Item item = invItem.item;
            if (item.itemBase.eqSlot != 0 && item.itemBase.eqSlot != 1 && item.itemBase.eqSlot != 5)
            {
                ServerPacket packet = new ServerPacket(Outgoing.shopEnd);
                packet.AppendInt(1);
                user.SendPacket(packet);
                return;
            }
            user.inventory.isBlocked = true;
            Dictionary<InventorySlot, int> matos = user.inventory.GetSlotByItemId(1, 1014, 5);
            if (matos != null && user.gold >= 500)
            {
                user.gold -= 500;
                user.SendGold();
                ServerPacket packet = new ServerPacket(Outgoing.invSlot);
                packet.AppendInt(1);
                foreach (InventorySlot invSlot in matos.Keys)
                {
                    int result = invSlot.DeleteSlotCount(matos[invSlot]);
                    if (result <= 0)
                    {
                        int nSlot = invSlot.idSlot;
                        user.inventory.itemsList[1][nSlot] = null;
                        packet.AppendString(nSlot + ".-1.0.0");
                    }
                    else
                    {
                        packet.AppendString(invSlot.idSlot + ".1014." + invSlot.amount + ".0");
                    }
                }
                user.SendPacket(packet);
                int random = UpgradeController.upRandom.Next(1, 101);
                bool destroy = false;
                int newRare = 0;
                if (random <= raresProba[item.rare][0])
                    destroy = true;
                else if (random <= raresProba[item.rare][0] + raresProba[item.rare][1])
                    newRare = 1;
                else if (random <= raresProba[item.rare][0] + raresProba[item.rare][1] + raresProba[item.rare][2])
                    newRare = 2;
                else if (random <= raresProba[item.rare][0] + raresProba[item.rare][1] + raresProba[item.rare][2] + raresProba[item.rare][3])
                    newRare = 3;
                else if (random <= raresProba[item.rare][0] + raresProba[item.rare][1] + raresProba[item.rare][2] + raresProba[item.rare][3] + raresProba[item.rare][4])
                    newRare = 4;
                else if (random <= raresProba[item.rare][0] + raresProba[item.rare][1] + raresProba[item.rare][2] + raresProba[item.rare][3] + raresProba[item.rare][4] + raresProba[item.rare][5])
                    newRare = 5;
                else if (random <= raresProba[item.rare][0] + raresProba[item.rare][1] + raresProba[item.rare][2] + raresProba[item.rare][3] + raresProba[item.rare][4] + raresProba[item.rare][5] + raresProba[item.rare][6])
                    newRare = 6;
                else if (random <= raresProba[item.rare][0] + raresProba[item.rare][1] + raresProba[item.rare][2] + raresProba[item.rare][3] + raresProba[item.rare][4] + raresProba[item.rare][5] + raresProba[item.rare][6] + raresProba[item.rare][7])
                    newRare = 7;
                if (destroy)
                {
                    user.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(user.languagePack, "message.bet.destroy")));
                    user.SendPacket(GlobalMessage.MakeMessage(user.id, 0, 11, GameServer.GetLanguage(user.languagePack, "message.bet.destroy")));
                    user.inventory.RemoveItem(user, 0, slot);
                }
                else
                {
                    int raresP = (item.rare < 6 ? item.rare : (item.rare == 6 ? 7 : 10));
                    int itemTotalPoints = item.itemBase.damageMax + item.itemBase.hitRate + item.itemBase.corpDef + item.itemBase.distDef + item.itemBase.magicDef + item.itemBase.dodge;
                    if (item.itemBase.eqSlot == 0 || item.itemBase.eqSlot == 5)
                    {
                        try
                        {
                            int attaques_points = 2 * (itemTotalPoints / 3);
                            int hit_rate_points = itemTotalPoints / 3;
                            int newDMin = new Random().Next(item.itemBase.damageMin, item.itemBase.damageMin + (attaques_points / 2) + 1) + (raresP * (1 + item.itemBase.levelReq / 5) / 3) * 2;
                            int newDMax = new Random().Next(item.itemBase.damageMax, item.itemBase.damageMax + (attaques_points / 2) + 1) + (raresP * (1 + item.itemBase.levelReq / 5) / 3) * 2;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    item.rare = newRare;
                    user.inventory.itemsList[0][slot].Updated = true;
                    packet = new ServerPacket(Outgoing.invSlot);
                    packet.AppendInt(0);
                    packet.AppendString(slot + "." + item.itemBase.id + "." + item.rare + ".0.0");
                    user.SendPacket(packet);
                    user.SendPacket(GlobalMessage.MakeAlert(0, String.Format(GameServer.GetLanguage(user.languagePack, "message.bet.win"), newRare)));
                    user.SendPacket(GlobalMessage.MakeMessage(user.id, 0, 12, String.Format(GameServer.GetLanguage(user.languagePack, "message.bet.win"), newRare)));
                    user.map.SendEffect(1, user.id, 3006);
                }
            }
            else
                user.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(user.languagePack, "message.bet.noitem")));
            user.inventory.isBlocked = false;
            user.SendPacket(GlobalMessage.MakeShopEnd(1));
        }

        public static void UpgradeItem(Player user, Item item, InventorySlot itemSlot, int slot, int rouleauType)
        {
            if (item.upgrade < 0 || item.upgrade > 9)
            {
                ServerPacket packet = new ServerPacket(Outgoing.shopEnd);
                packet.AppendInt(1);
                user.SendPacket(packet);
                return;
            }
            int cellaId = 1014;
            user.inventory.isBlocked = true;
            try
            {
                int messageType = 0;
                switch (rouleauType)
                {
                    case 1:
                        {
                            if (item.nivFixed)
                            {
                                user.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(user.languagePack, "error.upgrade.fixed")));
                                user.SendPacket(GlobalMessage.MakeShopEnd(1));
                                user.inventory.isBlocked = false;
                                return;
                            }
                            Dictionary<InventorySlot, int> cellas = user.inventory.GetSlotByItemId(1, cellaId, ItemUpgradeManager.GetUpgrade(item.upgrade).cellaCount);
                            Dictionary<InventorySlot, int> gemmes = user.inventory.GetSlotByItemId(1, ItemUpgradeManager.GetUpgrade(item.upgrade).specialItemId, ItemUpgradeManager.GetUpgrade(item.upgrade).specialCount);
                            if (cellas != null && gemmes != null && user.gold >= ItemUpgradeManager.GetUpgrade(item.upgrade).gold)
                            {
                                user.gold -= SpUpgradeManager.GetUpgrade(item.upgrade).gold;
                                user.SendGold();
                                ServerPacket packet = new ServerPacket(Outgoing.invSlot);
                                packet.AppendInt(1);
                                foreach (InventorySlot invSlot in cellas.Keys)
                                {
                                    int result = invSlot.DeleteSlotCount(cellas[invSlot]);
                                    if (result <= 0)
                                    {
                                        int nSlot = invSlot.idSlot;
                                        user.inventory.itemsList[1][nSlot] = null;
                                        packet.AppendString(nSlot + ".-1.0.0");
                                    }
                                    else
                                        packet.AppendString(invSlot.idSlot + "." + cellaId + "." + invSlot.amount + ".0");

                                }
                                user.SendPacket(packet);
                                packet = new ServerPacket(Outgoing.invSlot);
                                packet.AppendInt(1);
                                foreach (InventorySlot invSlot in gemmes.Keys)
                                {
                                    int result = invSlot.DeleteSlotCount(gemmes[invSlot]);
                                    if (result <= 0)
                                    {
                                        int nSlot = invSlot.idSlot;
                                        user.inventory.itemsList[2][nSlot] = null;
                                        packet.AppendString(nSlot + ".-1.0.0");
                                    }
                                    else
                                        packet.AppendString(invSlot.idSlot + "." + ItemUpgradeManager.GetUpgrade(item.upgrade).specialItemId + "." + invSlot.amount + ".0");
                                }
                                user.SendPacket(packet);
                                int randomNumber = UpgradeController.upRandom.Next(1, 101);
                                int pWin = ItemUpgradeManager.GetUpgrade(item.upgrade).pWin;
                                int pFail = ItemUpgradeManager.GetUpgrade(item.upgrade).pDestroy;
                                int pFix = ItemUpgradeManager.GetUpgrade(item.upgrade).pFix;
                                if (randomNumber <= pWin)
                                    messageType = 1;
                                else if (randomNumber <= pFail + pWin + pFix)
                                    messageType = 3;
                                else
                                    messageType = 4;
                            }
                            else
                                user.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(user.languagePack, "error.up.needobject")));
                            user.SendPacket(GlobalMessage.MakeShopEnd(1));
                        }
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error up system ID: {0} for ITEM", rouleauType);
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                }
                if (messageType != 0)
                {
                    switch (messageType)
                    {
                        case 1:
                            itemSlot.Updated = true;
                            item.upgrade++;
                            user.SendPacket(Inventory.MakeEquipPacket(0, slot, item.itemBase.id, 0, item.upgrade));
                            user.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(user.languagePack, "message.upitem.win")));
                            user.map.SendEffect(1, user.id, 3005);
                            break;
                        case 2:
                            itemSlot.Updated = true;
                            item.upgrade++;
                            user.SendPacket(Inventory.MakeEquipPacket(0, slot, item.itemBase.id, 0, item.upgrade));
                            user.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(user.languagePack, "message.upitem.win")));
                            user.SendPacket(GlobalMessage.MakeMessage(0, 0, 11, GameServer.GetLanguage(user.languagePack, "message.upitem.usedp")));
                            user.map.SendEffect(1, user.id, 3004);
                            user.map.SendEffect(1, user.id, 3005);
                            break;
                        case 3:
                            user.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(user.languagePack, "message.upitem.fail")));
                            break;
                    }
                }
                user.SendPacket(GlobalMessage.MakeShopEnd(2));
            }
            catch
            {
            }
            user.inventory.isBlocked = false;
        }
    }
}
