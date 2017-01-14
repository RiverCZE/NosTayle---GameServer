using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.NosTale.Entities.Players;
using NosTayleGameServer.NosTale.Entities.Players.Inventorys;
using NosTayleGameServer.NosTale.Items.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.UpgradeSystem
{
    static class SpUpgradeManager
    {
        public static Dictionary<int, SpUpgrade> upgradeList = new Dictionary<int, SpUpgrade>();

        static SpUpgradeManager()
        {
            upgradeList.Add(0, new SpUpgrade(1));
            upgradeList.Add(1, new SpUpgrade(2));
            upgradeList.Add(2, new SpUpgrade(3));
            upgradeList.Add(3, new SpUpgrade(4));
            upgradeList.Add(4, new SpUpgrade(5));
            upgradeList.Add(5, new SpUpgrade(6));
            upgradeList.Add(6, new SpUpgrade(7));
            upgradeList.Add(7, new SpUpgrade(8));
            upgradeList.Add(8, new SpUpgrade(9));
            upgradeList.Add(9, new SpUpgrade(10));
            upgradeList.Add(10, new SpUpgrade(11));
            upgradeList.Add(11, new SpUpgrade(12));
            upgradeList.Add(12, new SpUpgrade(13));
            upgradeList.Add(13, new SpUpgrade(14));
            upgradeList.Add(14, new SpUpgrade(15));
        }

        public static SpUpgrade GetUpgrade(int upgrade)
        {
            if (upgradeList.ContainsKey(upgrade))
                return upgradeList[upgrade];
            return null;
        }

        public static bool SpLevelOk(Specialist sp)
        {
            if (sp.upgrade < 5 && sp.level >= 21)
                return true;
            else if (sp.upgrade < 10 && sp.level >= 41)
                return true;
            else if (sp.upgrade < 15 && sp.level >= 51)
                return true;
            return false;
        }

        public static void UpgradeSp(Player user, Specialist sp, InventorySlot spSlot, int slot, int rouleauType)
        {
            if (DateTime.Now.Subtract(user.upItemTime).TotalSeconds < 5)
                return;
            user.upItemTime = DateTime.Now;
            if (sp.upgrade < 0 || sp.upgrade > 14)
            {
                user.SendPacket(GlobalMessage.MakeShopEnd(1));
                return;
            }
            int fMoonId = 1030;
            int penId = 2282;
            int speItemId = (sp.card.id > 4099 ? SpUpgradeManager.GetUpgrade(sp.upgrade).specialItemId2 : SpUpgradeManager.GetUpgrade(sp.upgrade).specialItemId);
            user.inventory.isBlocked = true;
            try
            {
                int messageType = 0;
                Dictionary<InventorySlot, int> fMoons = user.inventory.GetSlotByItemId(1, fMoonId, SpUpgradeManager.GetUpgrade(sp.upgrade).fMoonCount);
                Dictionary<InventorySlot, int> pens = user.inventory.GetSlotByItemId(2, penId, SpUpgradeManager.GetUpgrade(sp.upgrade).penCount);
                Dictionary<InventorySlot, int> speItems = user.inventory.GetSlotByItemId(2, speItemId, SpUpgradeManager.GetUpgrade(sp.upgrade).specialCount);
                switch (rouleauType)
                {
                    case 9:
                        {
                            if (!SpLevelOk(sp))
                            {
                                user.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(user.languagePack, "error.upsp.level")));
                                user.SendPacket(GlobalMessage.MakeShopEnd(1));
                                user.inventory.isBlocked = false;
                                return;
                            }
                            if (fMoons != null && pens != null && speItems != null && user.gold >= SpUpgradeManager.GetUpgrade(sp.upgrade).gold)
                            {
                                user.gold -= SpUpgradeManager.GetUpgrade(sp.upgrade).gold;
                                user.SendGold();
                                ServerPacket packet = new ServerPacket(Outgoing.invSlot);
                                packet.AppendInt(1);
                                foreach (InventorySlot invSlot in fMoons.Keys)
                                {
                                    int result = invSlot.DeleteSlotCount(fMoons[invSlot]);
                                    if (result <= 0)
                                    {
                                        int nSlot = invSlot.idSlot;
                                        user.inventory.itemsList[1][nSlot] = null;
                                        packet.AppendString(nSlot + ".-1.0.0");
                                    }
                                    else
                                        packet.AppendString(invSlot.idSlot + "." + fMoonId + "." + invSlot.amount + ".0");
                                }
                                user.SendPacket(packet);
                                packet = new ServerPacket(Outgoing.invSlot);
                                packet.AppendInt(2);
                                foreach (InventorySlot invSlot in pens.Keys)
                                {
                                    int result = invSlot.DeleteSlotCount(pens[invSlot]);
                                    if (result <= 0)
                                    {
                                        int nSlot = invSlot.idSlot;
                                        user.inventory.itemsList[2][nSlot] = null;
                                        packet.AppendString(nSlot + ".-1.0.0");
                                    }
                                    else
                                        packet.AppendString(invSlot.idSlot + "." + penId + "." + Convert.ToString(invSlot.amount) + ".0");
                                }
                                foreach (InventorySlot invSlot in speItems.Keys)
                                {
                                    int result = invSlot.DeleteSlotCount(speItems[invSlot]);
                                    if (result <= 0)
                                    {
                                        int nSlot = invSlot.idSlot;
                                        user.inventory.itemsList[2][nSlot] = null;
                                        packet.AppendString(nSlot + ".-1.0.0");
                                    }
                                    else
                                        packet.AppendString(invSlot.idSlot + "." + speItemId + "." + Convert.ToString(invSlot.amount) + ".0");
                                }
                                user.SendPacket(packet);
                                int randomNumber = UpgradeController.upRandom.Next(1, 101);
                                int pWin = SpUpgradeManager.GetUpgrade(sp.upgrade).pWin;
                                int pFail = SpUpgradeManager.GetUpgrade(sp.upgrade).pFail;
                                if (randomNumber <= pWin)
                                    messageType = 1;
                                else if (randomNumber <= pFail + pWin)
                                    messageType = 3;
                                else
                                    messageType = 4;
                            }
                            else
                                user.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(user.languagePack, "error.up.needobject")));
                            user.SendPacket(GlobalMessage.MakeShopEnd(1));
                        }
                        break;
                    case 25:
                        {
                            if (!SpLevelOk(sp))
                            {
                                user.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(user.languagePack, "error.upsp.level")));
                                user.SendPacket(GlobalMessage.MakeShopEnd(1));
                                user.inventory.isBlocked = false;
                                return;
                            }
                            if (sp.upgrade >= 10)
                            {
                                user.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(user.languagePack, "error.upsp.red")));
                                user.SendPacket(GlobalMessage.MakeShopEnd(1));
                                user.inventory.isBlocked = false;
                                return;
                            }
                            if (fMoons != null && pens != null && speItems != null && user.gold >= SpUpgradeManager.GetUpgrade(sp.upgrade).gold)
                            {
                                ServerPacket resultRouleau = user.inventory.DeleteOneItemByWid(user, 1, 25);
                                if (resultRouleau.ToString() != null)
                                {
                                    user.gold -= SpUpgradeManager.GetUpgrade(sp.upgrade).gold;
                                    user.SendGold();
                                    ServerPacket packet = new ServerPacket(Outgoing.invSlot);
                                    packet.AppendInt(1);
                                    foreach (InventorySlot invSlot in fMoons.Keys)
                                    {
                                        int result = invSlot.DeleteSlotCount(fMoons[invSlot]);
                                        if (result <= 0)
                                        {
                                            int nSlot = invSlot.idSlot;
                                            user.inventory.itemsList[1][nSlot] = null;
                                            packet.AppendString(nSlot + ".-1.0.0");
                                        }
                                        else
                                            packet.AppendString(invSlot.idSlot + "." + fMoonId + "." + invSlot.amount + ".0");
                                    }
                                    user.SendPacket(packet);
                                    packet = new ServerPacket(Outgoing.invSlot);
                                    packet.AppendInt(2);
                                    foreach (InventorySlot invSlot in pens.Keys)
                                    {
                                        int result = invSlot.DeleteSlotCount(pens[invSlot]);
                                        if (result <= 0)
                                        {
                                            int nSlot = invSlot.idSlot;
                                            user.inventory.itemsList[2][nSlot] = null;
                                            packet.AppendString(nSlot + ".-1.0.0");
                                        }
                                        else
                                        {
                                            packet.AppendString(invSlot.idSlot + "." + penId + "." + invSlot.amount + ".0");
                                        }
                                    }
                                    user.SendPacket(packet);
                                    int randomNumber = UpgradeController.upRandom.Next(1, 101);
                                    int pWin = SpUpgradeManager.GetUpgrade(sp.upgrade).pWin;
                                    int pFail = SpUpgradeManager.GetUpgrade(sp.upgrade).pFail;
                                    if (randomNumber <= pWin)
                                    {
                                        messageType = 2;
                                        foreach (InventorySlot invSlot in speItems.Keys)
                                        {
                                            int result = invSlot.DeleteSlotCount(speItems[invSlot]);
                                            if (result <= 0)
                                            {
                                                int nSlot = invSlot.idSlot;
                                                user.inventory.itemsList[2][nSlot] = null;
                                                packet.AppendString(nSlot + ".-1.0.0");
                                            }
                                            else
                                                packet.AppendString(invSlot.idSlot + "." + speItemId + "." + invSlot.amount + ".0");
                                        }
                                        user.SendPacket(packet);
                                    }
                                    else if (randomNumber <= pFail + pWin)
                                        messageType = 5;
                                    else
                                        messageType = 6;
                                    user.SendPacket(resultRouleau);
                                }
                                else
                                    return;
                            }
                            else
                                user.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(user.languagePack, "error.up.needobject")));
                            user.SendPacket(GlobalMessage.MakeShopEnd(1));
                        }
                        break;
                    case 26:
                        {
                            if (!SpLevelOk(sp))
                            {
                                user.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(user.languagePack, "error.upsp.level")));
                                user.SendPacket(GlobalMessage.MakeShopEnd(1));
                                user.inventory.isBlocked = false;
                                return;
                            }
                            if (sp.upgrade < 10)
                            {
                                user.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(user.languagePack, "error.upsp.blue")));
                                user.SendPacket(GlobalMessage.MakeShopEnd(1));
                                user.inventory.isBlocked = false;
                                return;
                            }
                            if (fMoons != null && pens != null && speItems != null && user.gold >= SpUpgradeManager.GetUpgrade(sp.upgrade).gold)
                            {
                                ServerPacket resultRouleau = user.inventory.DeleteOneItemByWid(user, 1, 26);
                                if (resultRouleau != null)
                                {
                                    user.gold -= SpUpgradeManager.GetUpgrade(sp.upgrade).gold;
                                    user.SendGold();
                                    ServerPacket packet = new ServerPacket(Outgoing.invSlot);
                                    packet.AppendInt(1);
                                    foreach (InventorySlot invSlot in fMoons.Keys)
                                    {
                                        int result = invSlot.DeleteSlotCount(fMoons[invSlot]);
                                        if (result <= 0)
                                        {
                                            int nSlot = invSlot.idSlot;
                                            user.inventory.itemsList[1][nSlot] = null;
                                            packet.AppendString(nSlot + ".-1.0.0");
                                        }
                                        else
                                            packet.AppendString(invSlot.idSlot + "." + fMoonId + "." + invSlot.amount + ".0");
                                    }
                                    user.SendPacket(packet);
                                    packet = new ServerPacket(Outgoing.invSlot);
                                    packet.AppendInt(2);
                                    foreach (InventorySlot invSlot in pens.Keys)
                                    {
                                        int result = invSlot.DeleteSlotCount(pens[invSlot]);
                                        if (result <= 0)
                                        {
                                            int nSlot = invSlot.idSlot;
                                            user.inventory.itemsList[2][nSlot] = null;
                                            packet.AppendString(nSlot + ".-1.0.0");
                                        }
                                        else
                                            packet.AppendString(invSlot.idSlot + "." + penId + "." + invSlot.amount + ".0");
                                    }
                                    user.SendPacket(packet);
                                    int randomNumber = UpgradeController.upRandom.Next(1, 101);
                                    int pWin = SpUpgradeManager.GetUpgrade(sp.upgrade).pWin;
                                    int pFail = SpUpgradeManager.GetUpgrade(sp.upgrade).pFail;
                                    if (randomNumber <= pWin)
                                    {
                                        messageType = 2;
                                        foreach (InventorySlot invSlot in speItems.Keys)
                                        {
                                            int result = invSlot.DeleteSlotCount(speItems[invSlot]);
                                            if (result <= 0)
                                            {
                                                int nSlot = invSlot.idSlot;
                                                user.inventory.itemsList[2][nSlot] = null;
                                                packet.AppendString(nSlot + ".-1.0.0");
                                            }
                                            else
                                                packet.AppendString(invSlot.idSlot + "." + speItemId + "." + invSlot.amount + ".0");
                                        }
                                        user.SendPacket(packet);
                                    }
                                    else if (randomNumber <= pFail + pWin)
                                        messageType = 5;
                                    else
                                        messageType = 6;
                                    user.SendPacket(resultRouleau);
                                }
                                else
                                    return;
                            }
                            else
                                user.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(user.languagePack, "error.up.needobject")));
                            user.SendPacket(GlobalMessage.MakeShopEnd(1));
                        }
                        break;
                    case 35:
                        if (sp.card.id == 907)
                        {
                            ServerPacket result = user.inventory.DeleteOneItemByWid(user, 1, 35);
                            if (result != null)
                            {
                                int randomNumber = UpgradeController.upRandom.Next(1, 101);
                                int pWin = SpUpgradeManager.GetUpgrade(sp.upgrade).pWin;
                                int pFail = SpUpgradeManager.GetUpgrade(sp.upgrade).pFail + SpUpgradeManager.GetUpgrade(sp.upgrade).pDestroy;
                                if (randomNumber <= pWin)
                                    messageType = 2;
                                else if (randomNumber <= pFail + pWin)
                                    messageType = 5;
                                user.SendPacket(result);
                            }
                        }
                        break;
                    case 38:
                        if (sp.card.id == 900)
                        {
                            ServerPacket result = user.inventory.DeleteOneItemByWid(user, 1, 38);
                            if (result != null)
                            {
                                int randomNumber = UpgradeController.upRandom.Next(1, 101);
                                int pWin = SpUpgradeManager.GetUpgrade(sp.upgrade).pWin;
                                int pFail = SpUpgradeManager.GetUpgrade(sp.upgrade).pFail + SpUpgradeManager.GetUpgrade(sp.upgrade).pDestroy;
                                if (randomNumber <= pWin)
                                    messageType = 2;
                                else if (randomNumber <= pFail + pWin)
                                    messageType = 5;
                                user.SendPacket(result);
                            }
                        }
                        break;
                    case 42:
                        if (sp.card.id == 4099)
                        {
                            ServerPacket result = user.inventory.DeleteOneItemByWid(user, 1, 42);
                            if (result != null)
                            {
                                int randomNumber = UpgradeController.upRandom.Next(1, 101);
                                int pWin = SpUpgradeManager.GetUpgrade(sp.upgrade).pWin;
                                int pFail = SpUpgradeManager.GetUpgrade(sp.upgrade).pFail + SpUpgradeManager.GetUpgrade(sp.upgrade).pDestroy;
                                if (randomNumber <= pWin)
                                    messageType = 2;
                                else if (randomNumber <= pFail + pWin)
                                    messageType = 5;
                                user.SendPacket(result);
                            }
                        }
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error up system ID: {0} for SP", rouleauType);
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                }
                if (user.easySpUp)
                    messageType = 1;
                if (messageType != 0)
                {
                    switch (messageType)
                    {
                        case 1:
                            spSlot.Updated = true;
                            sp.upgrade++;
                            sp.totalPoints = sp.CalculePoints();
                            user.SendPacket(Inventory.MakeEquipPacket(0, slot, sp.card.id, 0, sp.upgrade));
                            user.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(user.languagePack, "message.upitem.win")));
                            user.map.SendEffect(1, user.id, 3005);
                            if (sp.upgrade > 7 && user.family != null)
                                user.family.AddToHis(new Familys.FamilyHistorics.HistoricItem(String.Format("7|{0}|{1}|{2}", user.name, sp.card.id, sp.upgrade), GameServer.Timestamp()));
                            break;
                        case 2:
                                spSlot.Updated = true;
                                sp.upgrade++;
                                sp.totalPoints = sp.CalculePoints();
                                user.SendPacket(Inventory.MakeEquipPacket(0, slot, sp.card.id, 0, sp.upgrade));
                                user.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(user.languagePack, "message.upitem.win")));
                                user.SendPacket(GlobalMessage.MakeMessage(0, 0, 11, GameServer.GetLanguage(user.languagePack, "message.upitem.usedp")));
                                user.map.SendEffect(1, user.id, 3004);
                                user.map.SendEffect(1, user.id, 3005);
                                if (sp.upgrade > 7 && user.family != null)
                                    user.family.AddToHis(new Familys.FamilyHistorics.HistoricItem(String.Format("7|{0}|{1}|{2}", user.name, sp.card.id, sp.upgrade), GameServer.Timestamp()));
                            break;
                        case 3:
                            user.SendPacket(GlobalMessage.MakeAlert(0,GameServer.GetLanguage(user.languagePack, "message.upitem.fail")));
                            break;
                        case 4:
                            user.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(user.languagePack, "message.upsp.destroy")));
                            sp.isDead = true;
                            user.SendPacket(Inventory.MakeEquipPacket(0, slot, sp.card.id, -2, sp.upgrade));
                            break;
                        case 5:
                            user.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(user.languagePack, "message.upsp.fail2")));
                            user.SendPacket(GlobalMessage.MakeMessage(0, 0, 11, GameServer.GetLanguage(user.languagePack, "message.upitem.usedp")));
                            user.map.SendEffect(1, user.id, 3004);
                            break;
                        case 6:
                            user.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(user.languagePack, "message.upsp.destroy2")));
                            user.SendPacket(GlobalMessage.MakeMessage(0, 0, 11, GameServer.GetLanguage(user.languagePack, "message.upitem.usedp")));
                            user.map.SendEffect(1, user.id, 3004);
                            break;
                    }
                }
                user.SendPacket(GlobalMessage.MakeShopEnd(2));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            user.inventory.isBlocked = false;
        }
    }
}
