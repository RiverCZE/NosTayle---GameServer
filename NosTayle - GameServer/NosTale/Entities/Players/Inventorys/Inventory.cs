using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.Core;
using NosTayleGameServer.NosTale.Items;
using NosTayleGameServer.NosTale.Items.Others;
using NosTayleGameServer.NosTale.Maps.SpecialMaps;
using NosTayleGameServer.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Entities.Players.Inventorys
{
    public class Inventory
    {
        public Dictionary<int, InventorySlot[]> itemsList = new Dictionary<int, InventorySlot[]>();
        public bool inventoryCopied = false;
        public bool isBlocked = false;

        public Inventory() { }

        public Inventory(int userId, Inventory invCopy)
        {
            invCopy.inventoryCopied = true;
            this.itemsList = new Dictionary<int, InventorySlot[]>(invCopy.itemsList);
        }

        public int FuturIndex(InventorySlot[] inventory)
        {
            return Array.IndexOf(inventory, null);
        }

        public void LoadItems(Player player)
        {
            itemsList.Clear();
            DataTable dataTable = null;
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("charId", player.id);
                dataTable = dbClient.ReadDataTable("SELECT * FROM items_server" + GameServer.serverId + " WHERE charId = @charId AND equiped = '0' AND inWareHouse = '0';");
                InventorySlot[] inv0 = new InventorySlot[60];
                InventorySlot[] inv1 = new InventorySlot[60];
                InventorySlot[] inv2 = new InventorySlot[60];
                InventorySlot[] inv3 = new InventorySlot[50];
                InventorySlot[] inv6 = new InventorySlot[45];
                InventorySlot[] inv7 = new InventorySlot[45];
                foreach (DataRow itemRow in dataTable.Rows)
                {
                    Item item = new Item((int)itemRow["id"], player.id);
                    if (item.itemBase.inventory == 0 && item.invPos > 59 || item.itemBase.inventory == 1 && item.invPos > 59 || item.itemBase.inventory == 2 && item.invPos > 59 || item.itemBase.inventory == 3 && item.invPos > 49 || item.itemBase.inventory == 6 && item.invPos > 44 || item.itemBase.inventory == 7 && item.invPos > 44)
                        continue;
                    InventoryItem inventoryItem = new InventoryItem(item, "item");
                    switch (item.itemBase.inventory)
                    {
                        case 0:
                            if (item.itemBase.eqSlot == 13 && item.inEquip == 0 || item.itemBase.eqSlot == 14 && item.inEquip == 0)
                            {
                                bool placeInventory = false;
                                for (int i = 0; i < 10; i++)
                                {
                                    if (inv7[i] == null)
                                    {
                                        inv7[i] = new InventorySlot(i, (int)itemRow["amount"]);
                                        inv7[i].invItem = inventoryItem;
                                        placeInventory = true;
                                        break;
                                    }
                                }
                                if (!placeInventory)
                                {
                                    for (int i = 0; i < Convert.ToInt32(48); i++)
                                    {
                                        if (inv0[i] == null)
                                        {
                                            inventoryItem.item.invPos = i;
                                            inv0[inventoryItem.item.invPos] = new InventorySlot(inventoryItem.item.invPos, (int)itemRow["amount"]);
                                            inv0[inventoryItem.item.invPos].invItem = inventoryItem;
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (inv0[item.invPos] == null && item.invPos != -1)
                            {
                                inv0[item.invPos] = new InventorySlot(item.invPos, (int)itemRow["amount"]);
                                inv0[item.invPos].invItem = inventoryItem;
                            }
                            else
                            {
                                int nextIndex = this.FuturIndex(inv0);
                                if (nextIndex > -1)
                                {
                                    item.invPos = nextIndex;
                                    inv0[nextIndex] = new InventorySlot(item.invPos, (int)itemRow["amount"]);
                                    inv0[item.invPos].invItem = inventoryItem;
                                }
                            }
                            break;
                        case 1:
                            if (inv1[item.invPos] == null)
                            {
                                inv1[item.invPos] = new InventorySlot(item.invPos, (int)itemRow["amount"]);
                                inv1[item.invPos].invItem = inventoryItem;
                            }
                            break;
                        case 2:
                            if (inv2[item.invPos] == null)
                            {
                                inv2[item.invPos] = new InventorySlot(item.invPos, (int)itemRow["amount"]);
                                inv2[item.invPos].invItem = inventoryItem;
                            }
                            break;
                        case 3:
                            if (inv3[item.invPos] == null && item.invPos != -1)
                            {
                                inv3[item.invPos] = new InventorySlot(item.invPos, (int)itemRow["amount"]);
                                inv3[item.invPos].invItem = inventoryItem;
                            }
                            else
                            {
                                int newPos = this.FuturIndex(inv3);
                                if (newPos > -1)
                                {
                                    item.invPos = newPos;
                                    inv3[item.invPos] = new InventorySlot(item.invPos, (int)itemRow["amount"]);
                                    inv3[item.invPos].invItem = inventoryItem;
                                }
                            }
                            break;
                        /*case 7:
                            if (!inv7.ContainsKey(item.invPos))
                            {
                                inv7.Add(item.invPos, new InventorySlot(item.invPos));
                                inv7[item.invPos].itemList.Add(inventoryItem);
                            }
                            else
                            {
                                inv7[item.invPos].itemList.Add(inventoryItem);
                            }
                            break;*/
                    }
                }
                dataTable = dbClient.ReadDataTable("SELECT * FROM sps_server" + GameServer.serverId + " WHERE charId = @charId AND equiped = '0' AND inWareHouse = '0';");
                foreach (DataRow spRow in dataTable.Rows)
                {
                    InventoryItem inventoryItem = new InventoryItem(new Specialist((int)spRow["spId"]), "specialist");
                    if (inventoryItem.specialist.inEquip)
                    {
                        if (inv0[inventoryItem.specialist.invPos] == null && inventoryItem.specialist.invPos != -1)
                        {
                            inv0[inventoryItem.specialist.invPos] = new InventorySlot(inventoryItem.specialist.invPos, 1);
                            inv0[inventoryItem.specialist.invPos].invItem = inventoryItem;
                        }
                        else
                        {
                            int newPos = this.FuturIndex(inv0);
                            if (newPos > -1)
                            {
                                inventoryItem.specialist.invPos = newPos;
                                inv0[inventoryItem.specialist.invPos] = new InventorySlot(inventoryItem.specialist.invPos, 1);
                                inv0[inventoryItem.specialist.invPos].invItem = inventoryItem;
                            }
                        }
                    }
                    else
                    {
                        bool placeSpInventory = false;
                        int newPos = this.FuturIndex(inv6);
                        if (newPos > -1)
                        {
                            inv6[newPos] = new InventorySlot(newPos, 1);
                            inv6[newPos].invItem = inventoryItem;
                            placeSpInventory = true;
                        }
                        if (!placeSpInventory)
                        {
                            newPos = this.FuturIndex(inv0);
                            if (newPos > -1)
                            {
                                inventoryItem.specialist.invPos = newPos;
                                inv0[newPos] = new InventorySlot(inventoryItem.specialist.invPos, 1);
                                inv0[inventoryItem.specialist.invPos].invItem = inventoryItem;
                            }
                        }
                    }
                }
                dataTable = dbClient.ReadDataTable("SELECT * FROM fairies_server" + GameServer.serverId + " WHERE charId = @charId AND equiped = '0' AND inWareHouse = '0';");
                foreach (DataRow fairyRow in dataTable.Rows)
                {
                    InventoryItem inventoryItem = new InventoryItem(new Fairy((int)fairyRow["fairyId"]), "fairy");
                    if (inv0[inventoryItem.fairy.invPos] == null && inventoryItem.fairy.invPos != -1)
                    {
                        inv0[inventoryItem.fairy.invPos] = new InventorySlot(inventoryItem.fairy.invPos, 1);
                        inv0[inventoryItem.fairy.invPos].invItem = inventoryItem;
                    }
                    else
                    {
                        int newPos = this.FuturIndex(inv6);
                        if (newPos > -1)
                        {
                            inventoryItem.fairy.invPos = newPos;
                            inv0[inventoryItem.fairy.invPos] = new InventorySlot(inventoryItem.fairy.invPos, 1);
                            inv0[inventoryItem.fairy.invPos].invItem = inventoryItem;
                        }
                    }
                }
                itemsList.Add(0, inv0);
                itemsList.Add(1, inv1);
                itemsList.Add(2, inv2);
                itemsList.Add(3, inv3);
                itemsList.Add(6, inv6);
                itemsList.Add(7, inv7);
            }
        }

        public ServerPacket BuildInventory(int level)
        {
            ServerPacket packet = new ServerPacket(Outgoing.inv);
            packet.AppendInt(level);
            if (level == 0)
            {
                foreach (InventorySlot inventorySlot in itemsList[0])
                {
                    if (inventorySlot != null)
                    {
                        InventoryItem itemInventory = inventorySlot.invItem;
                        if (itemInventory.isSp)
                        {
                            packet.AppendString(inventorySlot.idSlot + "." + itemInventory.itemBase.id + ".0." + itemInventory.specialist.upgrade + ".0 ");
                        }
                        else if (itemInventory.isFairy)
                        {
                            packet.AppendString(inventorySlot.idSlot + "." + itemInventory.itemBase.id + ".0.0.0 ");
                        }
                        else if (itemInventory.item.color > 0)
                        {
                            packet.AppendString(inventorySlot.idSlot + "." + itemInventory.itemBase.id + "." + itemInventory.item.color + "." + itemInventory.item.color + ".0 ");
                        }
                        else
                        {
                            packet.AppendString(inventorySlot.idSlot + "." + itemInventory.item.itemBase.id + "." + itemInventory.item.rare + "." + itemInventory.item.upgrade + ".0 ");
                        }
                    }
                }
            }
            else if (level == 3)
            {
                //MiniLand
                foreach (InventorySlot inventorySlot in itemsList[level])
                {
                    if (inventorySlot != null)
                    {
                        InventoryItem itemInventory = inventorySlot.invItem;
                        packet.AppendString(inventorySlot.idSlot + "." + itemInventory.itemBase.id + "." + inventorySlot.amount + ".1 ");
                    }
                }
            }
            else if (level == 6)
            {
                //Sps
                foreach (InventorySlot inventorySlot in itemsList[6])
                {
                    if (inventorySlot != null)
                    {
                        InventoryItem itemInventory = inventorySlot.invItem;
                        if (itemInventory.itemBase.isSp)
                        {
                            packet.AppendString(inventorySlot.idSlot + "." + itemInventory.itemBase.id + "." + (itemInventory.specialist.isDead ? -2 : 0) + "." + itemInventory.specialist.upgrade + " ");
                        }
                    }
                }
            }
            else
            {
                //autres
                foreach (InventorySlot inventorySlot in itemsList[level])
                {
                    if (inventorySlot != null)
                    {
                        InventoryItem itemInventory = inventorySlot.invItem;
                        packet.AppendString(inventorySlot.idSlot + "." + itemInventory.itemBase.id + "." + inventorySlot.amount + ".1 ");
                    }
                }
            }
            return packet;
        }

        public ServerPacket BuildMlObj()
        {
            ServerPacket packet = new ServerPacket(Outgoing.minilandObjList);
            foreach (InventorySlot inventorySlot in itemsList[3])
            {
                if (inventorySlot != null)
                {
                    int used = 0;
                    if (inventorySlot.invItem.item.x > 0 && inventorySlot.invItem.item.y > 0)
                        used = 1;
                    Item item = inventorySlot.invItem.item;
                    packet.AppendString(inventorySlot.idSlot + "." + used + "." + item.x + "." + item.y + "." + item.itemBase.width + "." + item.itemBase.height + "." + item.durabilites + ".100.0.0");
                }
            }
            return packet;
        }

        public void BuildAllInventory(Player player)
        {
            player.SendGold();
            player.SendPacket(BuildInventory(0)); //EQUIP INVENTAIRE
            player.SendPacket(BuildInventory(1)); //PRINCIPAL INVENTAIRE
            player.SendPacket(BuildInventory(2)); //ETC INVENTAIRE
            player.SendPacket(BuildInventory(3));
            player.SendPacket(BuildMlObj());
            player.SendPacket(BuildInventory(6)); //INVENTAIRE SP
            player.SendPacket(BuildInventory(7)); //COSTUME INVENTAIRE
        }

        public void SaveInventory(Player player)
        {
            foreach (InventorySlot[] inventory in itemsList.Values)
                foreach (InventorySlot inventorySlot in inventory)
                    if (inventorySlot != null)
                        if (inventorySlot.Updated)
                        {
                            inventorySlot.SaveSlot(player.id, player.GetSession().GetAccount().id);
                            inventorySlot.Updated = false;
                        }
        }

        public void SendInvSlot(Player player)
        {
            ServerPacket packet = new ServerPacket(Outgoing.invSlotCount);
            packet.AppendInt(0);
            packet.AppendString("48 48 48");
            player.SendPacket(packet);
        }

        #region Utilisation des objets
        public void WearItem(Player player, int slot, int inventory)
        {
            if (inventory == 0 && slot >= 0 && this.itemsList[0].Length > slot && this.itemsList[0][slot] != null && itemsList[0][slot].invItem.gId != -1)
            {
                InventoryItem wearItem = itemsList[0][slot].invItem;
                if (!wearItem.UserClassCanUse(player.userClass) || player.level < wearItem.itemBase.levelReq || wearItem.itemBase.jobLevelReq > player.jobLevel || wearItem.itemBase.icoReputReq > ServerMath.ConvertReputToIco(player.reputation))
                {
                    player.SendPacket(GlobalMessage.MakeMessage(0, 0, 10, GameServer.GetLanguage(player.languagePack, "error.char.cantuseitem")));
                    return;
                }
                if (player.GetEquipeBySlot(wearItem.itemBase.eqSlot).id == -1)
                {
                    switch (wearItem.itemBase.eqSlot)
                    {
                        case 0:
                            player.weapon = wearItem.item;
                            break;
                        case 1:
                            player.armor = wearItem.item;
                            break;
                        case 2:
                            player.chap = wearItem.item;
                            break;
                        case 3:
                            player.glove = wearItem.item;
                            break;
                        case 4:
                            player.boot = wearItem.item;
                            break;
                        case 5:
                            player.weapon2 = wearItem.item;
                            break;
                        case 9:
                            player.mask = wearItem.item;
                            break;
                        case 10:
                            if (player.spInUsing && player.sp.card.element != wearItem.fairy.itemBase.element && wearItem.fairy.itemBase.element != 0 && player.sp.card.element != 0)
                            {
                                player.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(player.languagePack, "error.sp.notsameelement")));
                                return;
                            }
                            if (player.fairy.fairyId == -1)
                            {
                                player.fairy = wearItem.fairy;
                                player.map.SendMap(player.FairyPacket());

                            }
                            else
                            {
                                InventoryItem fairyCopy = new InventoryItem(player.fairy, "fairy");
                                player.fairy = wearItem.fairy;
                                player.fairy.invPos = -1;
                                itemsList[0][slot] = new InventorySlot(slot, 1);
                                itemsList[0][slot].invItem = fairyCopy;
                                itemsList[0][slot].SaveSlot(player.id, player.GetSession().GetAccount().id);
                                player.SendPacket(MakeEquipPacket(0, slot, itemsList[0][slot].invItem.fairy.itemBase.id, 0, 0));
                                player.SendStatPoints();
                                player.SendEquipment();
                                player.map.SendMap(player.FairyPacket());
                                return;
                            }
                            break;
                        case 12:
                            if (player.sp.spId == -1)
                                player.sp = wearItem.specialist;
                            else
                            {
                                if (!player.spInUsing)
                                {
                                    InventoryItem spCopy = new InventoryItem(player.sp, "specialist");
                                    player.sp = wearItem.specialist;
                                    player.sp.invPos = -1;
                                    itemsList[0][slot] = new InventorySlot(slot, 1);
                                    itemsList[0][slot].invItem = spCopy;
                                    itemsList[0][slot].invItem.specialist.inEquip = true;
                                    itemsList[0][slot].SaveSlot(player.id, player.GetSession().GetAccount().id);
                                    player.SendPacket(MakeEquipPacket(0, slot, itemsList[0][slot].invItem.specialist.card.id, (itemsList[0][slot].invItem.specialist.isDead ? -2 : 0), itemsList[0][slot].invItem.specialist.upgrade));
                                    player.SendStatPoints();
                                    player.SendEquipment();
                                    return;
                                }
                                else
                                {
                                    player.SendPacket(GlobalMessage.MakeMessage(0, 0, 10, GameServer.GetLanguage(player.languagePack, "error.sp.cantwear")));
                                    return;
                                }
                            }
                            break;
                        case 13:
                            player.costumeBody = wearItem.item;
                            break;
                        case 14:
                            player.costumeHead = wearItem.item;
                            break;
                    }
                    itemsList[0][slot] = null;
                    player.SendPacket(MakeEquipPacket(0, slot, -1, 0, 0));
                }
                else
                {
                    Item ItemCopy = null;
                    switch (wearItem.itemBase.eqSlot)
                    {
                        case 0:
                            ItemCopy = new Item(player.weapon, slot, false);
                            player.weapon = wearItem.item;
                            player.weapon.invPos = -1;
                            break;
                        case 1:
                            ItemCopy = new Item(player.armor, slot, false);
                            player.armor = wearItem.item;
                            player.armor.invPos = -1;
                            break;
                        case 2:
                            ItemCopy = new Item(player.chap, slot, false);
                            player.chap = wearItem.item;
                            player.chap.invPos = -1;
                            break;
                        case 3:
                            ItemCopy = new Item(player.glove, slot, false);
                            player.glove = wearItem.item;
                            player.glove.invPos = -1;
                            break;
                        case 4:
                            ItemCopy = new Item(player.boot, slot, false);
                            player.boot = wearItem.item;
                            player.boot.invPos = -1;
                            break;
                        case 5:
                            ItemCopy = new Item(player.weapon2, slot, false);
                            player.weapon2 = wearItem.item;
                            player.weapon2.invPos = -1;
                            break;
                        case 9:
                            ItemCopy = new Item(player.mask, slot, false);
                            player.mask = wearItem.item;
                            player.mask.invPos = -1;
                            break;
                        case 13:
                            ItemCopy = new Item(player.costumeBody, slot, false);
                            player.costumeBody = wearItem.item;
                            player.costumeBody.invPos = -1;
                            break;
                        case 14:
                            ItemCopy = new Item(player.costumeHead, slot, false);
                            player.costumeHead = wearItem.item;
                            player.costumeHead.invPos = -1;
                            break;
                    }
                    itemsList[0][slot] = new InventorySlot(slot, 1);
                    itemsList[0][slot].invItem = new InventoryItem(ItemCopy, "item");
                    itemsList[0][slot].SaveSlot(player.id, player.GetSession().GetAccount().id);
                    player.SendPacket(MakeEquipPacket(0, slot, ItemCopy.itemBase.id, ItemCopy.rare, (ItemCopy.color != 0 ? ItemCopy.color : ItemCopy.upgrade)));
                }
                player.SendStatPoints();
                player.SendEquipment();
                player.map.SendMap(player.GetEqPacket());
            }
        }
        public void UnWearItem(Player player, int slot, int unknow)
        {
            if (slot < 15 && slot >= 0 && (player.GetEquipeBySlot(slot).id != -1 || slot == 12 && player.sp.spId != -1 || slot == 10 && player.fairy.fairyId != -1))
            {
                int newIndex = this.FuturIndex(this.itemsList[0]);
                if (newIndex > -1)
                {
                    itemsList[0][newIndex] = new InventorySlot(newIndex, 1);
                    if (slot == 12)
                        itemsList[0][newIndex].invItem = new InventoryItem(new Specialist(player.sp), "specialist");
                    else if (slot == 10)
                        itemsList[0][newIndex].invItem = new InventoryItem(new Fairy(player.fairy), "fairy");
                    else
                        itemsList[0][newIndex].invItem = new InventoryItem(new Item(player.GetEquipeBySlot(slot), newIndex, false), "item");
                    switch (slot)
                    {
                        case 0:
                            player.weapon = new Item(-1, player.id);
                            break;
                        case 1:
                            player.armor = new Item(-1, player.id);
                            break;
                        case 2:
                            player.chap = new Item(-1, player.id);
                            break;
                        case 3:
                            player.glove = new Item(-1, player.id);
                            break;
                        case 4:
                            player.boot = new Item(-1, player.id);
                            break;
                        case 5:
                            player.weapon2 = new Item(-1, player.id);
                            break;
                        case 9:
                            player.mask = new Item(-1, player.id);
                            break;
                        case 10:
                            player.fairy = new Fairy(-1);
                            player.map.SendMap(player.FairyPacket());
                            break;
                        case 12:
                            if (player.spInUsing)
                                player.PutSp();
                            itemsList[0][newIndex].invItem.specialist.inEquip = true;
                            player.sp = new Specialist(-1);
                            break;
                        case 13:
                            itemsList[0][newIndex].invItem.item.inEquip = 1;
                            player.costumeBody = new Item(-1, player.id);
                            break;
                        case 14:
                            itemsList[0][newIndex].invItem.item.inEquip = 1;
                            player.costumeHead = new Item(-1, player.id);
                            break;
                    }
                    if (itemsList[0][newIndex].invItem.isSp)
                        player.SendPacket(MakeEquipPacket(0, newIndex, itemsList[0][newIndex].invItem.itemBase.id, 0, itemsList[0][newIndex].invItem.specialist.upgrade));
                    else if (itemsList[0][newIndex].invItem.isFairy)
                        player.SendPacket(MakeEquipPacket(0, newIndex, itemsList[0][newIndex].invItem.itemBase.id, 0, 0));
                    else
                        player.SendPacket(MakeEquipPacket(0, newIndex, itemsList[0][newIndex].invItem.itemBase.id, itemsList[0][newIndex].invItem.item.rare, (itemsList[0][newIndex].invItem.item.color != 0 ? itemsList[0][newIndex].invItem.item.color : itemsList[0][newIndex].invItem.item.upgrade)));
                    player.SendStatPoints();
                    player.SendEquipment();
                    player.map.SendMap(player.GetEqPacket());
                    itemsList[0][newIndex].SaveSlot(player.id, player.GetSession().GetAccount().id);
                    return;
                }
            }
        }
        public void UseItem(Player user, int inventory, int slot)
        {
            if (itemsList[inventory][slot] != null)
            {
                if (itemsList[inventory][slot].invItem.itemBase.isSp)
                {
                    if (user.sp.spId == -1)
                    {
                        if (!user.spInUsing)
                        {
                            InventoryItem item = itemsList[inventory][slot].invItem;
                            if (item.itemBase.jobLevelReq > user.jobLevel || item.itemBase.icoReputReq > ServerMath.ConvertReputToIco(user.reputation))
                            {
                                user.SendPacket(GlobalMessage.MakeMessage(0, 0, 10, GameServer.GetLanguage(user.languagePack, "error.char.cantuseitem")));
                                return;
                            }
                            user.sp = item.specialist;
                            itemsList[6][slot] = null;
                            user.SendPacket(BuildInventory(6));
                            user.SendStatPoints();
                            user.SendEquipment();
                        }
                        else
                            user.SendPacket(GlobalMessage.MakeMessage(0, 0, 10, GameServer.GetLanguage(user.languagePack, "error.sp.cantwear")));
                        return;
                    }
                    else
                    {
                        if (!user.spInUsing)
                        {
                            InventoryItem item = itemsList[inventory][slot].invItem;
                            if (item.itemBase.jobLevelReq > user.jobLevel || item.itemBase.icoReputReq > ServerMath.ConvertReputToIco(user.reputation))
                            {
                                user.SendPacket(GlobalMessage.MakeMessage(0, 0, 10, GameServer.GetLanguage(user.languagePack, "error.char.cantuseitem")));
                                return;
                            }
                            InventoryItem ItemCopy = new InventoryItem(user.sp, "specialist");
                            user.sp = item.specialist;
                            user.sp.invPos = -1;
                            itemsList[6][slot] = new InventorySlot(slot, 1);
                            itemsList[6][slot].invItem = ItemCopy;
                            itemsList[6][slot].invItem.specialist.inEquip = false;
                            itemsList[6][slot].SaveSlot(user.id, user.GetSession().GetAccount().id);
                            user.SendPacket(this.BuildInventory(6));
                            user.SendStatPoints();
                            user.SendEquipment();
                        }
                        else
                            user.SendPacket(GlobalMessage.MakeMessage(0, 0, 10, GameServer.GetLanguage(user.languagePack, "error.sp.cantwear")));
                        return;
                    }
                }
                else if (itemsList[inventory][slot].invItem.itemBase.eqSlot == 13 || itemsList[inventory][slot].invItem.itemBase.eqSlot == 14)
                {
                    InventoryItem item = itemsList[inventory][slot].invItem;
                    switch (itemsList[inventory][slot].invItem.itemBase.eqSlot)
                    {
                        case 13:
                            if (user.costumeBody.id == -1)
                            {
                                user.costumeBody = item.item;
                                itemsList[inventory][slot] = null;
                            }
                            else
                            {
                                Item ItemCopy = new Item(user.costumeBody, slot, false);
                                user.costumeBody = item.item;
                                user.costumeBody.invPos = -1;
                                itemsList[inventory][slot] = new InventorySlot(slot, 1);
                                itemsList[inventory][slot].invItem = new InventoryItem(ItemCopy, "item");
                                itemsList[inventory][slot].SaveSlot(user.id, user.GetSession().GetAccount().id);
                            }
                            break;
                        case 14:
                            if (user.costumeHead.id == -1)
                            {
                                user.costumeHead = item.item;
                                itemsList[inventory][slot] = null;
                            }
                            else
                            {
                                Item ItemCopy = new Item(user.costumeHead, slot, false);
                                user.costumeHead = item.item;
                                user.costumeHead.invPos = -1;
                                itemsList[inventory][slot] = new InventorySlot(slot, 1);
                                itemsList[inventory][slot].invItem = new InventoryItem(ItemCopy, "item");
                                itemsList[inventory][slot].SaveSlot(user.id, user.GetSession().GetAccount().id);
                            }
                            break;
                    }
                    user.SendPacket(this.BuildInventory(7));
                    user.SendStatPoints();
                    user.SendEquipment();
                }
                else if (itemsList[inventory][slot].invItem.item.UseItem(user, inventory, slot))
                {
                    if (itemsList[inventory][slot].invItem.itemBase.deleteOnUse)
                    {
                        itemsList[inventory][slot].amount--;
                        if (itemsList[inventory][slot].amount > 0)
                        {
                            itemsList[inventory][slot].Updated = true;
                            user.SendPacket(MakeInvSlotBase(inventory, slot, itemsList[inventory][slot].invItem.itemBase.id, itemsList[inventory][slot].amount));
                        }
                        else
                        {
                            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
                            {
                                dbClient.ExecuteQuery("DELETE FROM items_server" + GameServer.serverId + " WHERE id =  '" + itemsList[inventory][slot].invItem.item.id + "' AND charId = '" + user.id + "';");
                            }
                            itemsList[inventory][slot] = null;
                            user.SendPacket(MakeInvSlotBase(inventory, slot, -1, 0));
                        }
                    }
                }
                else
                    user.SendPacket(MakeInvSlotBase(inventory, slot, itemsList[inventory][slot].invItem.itemBase.id, itemsList[inventory][slot].amount));
            }
            else
                return;
        }
        #endregion

        #region Suppression d'objet
        public ServerPacket DeleteOneItemByType(Player player, int inventory, string type)
        {
            ServerPacket packet = null;
            if (!itemsList.ContainsKey(inventory))
                return packet;
            InventorySlot slot = itemsList[inventory].FirstOrDefault(x => x != null && x.invItem.item.itemBase.type == type);
            int pos = Array.IndexOf(itemsList[inventory], slot);
            if (slot != null)
            {
                if (slot.invItem.itemBase.deleteOnUse)
                {
                    slot.amount--;
                    if (slot.amount > 0)
                    {
                        slot.SaveSlot(player.id, player.GetSession().GetAccount().id);
                        packet = new ServerPacket(Outgoing.invSlot);
                        packet.AppendInt(inventory);
                        packet.AppendString(pos + "." + slot.invItem.itemBase.id + "." + slot.amount + ".0");
                    }
                    else
                    {
                        using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
                        {
                            dbClient.ExecuteQuery("DELETE FROM items_server" + GameServer.serverId + " WHERE id =  '" + slot.invItem.item.id + "';");
                        }
                        itemsList[inventory][pos] = null;
                        packet = new ServerPacket(Outgoing.invSlot);
                        packet.AppendInt(inventory);
                        packet.AppendString(pos + ".-1.0.0");
                    }
                }
                else
                {
                    packet = new ServerPacket(Outgoing.invSlot);
                    packet.AppendInt(inventory);
                    packet.AppendString(pos + "." + slot.invItem.itemBase.id + "." + slot.amount + ".0");
                }
                return packet;
            }
            return packet;
        }

        public ServerPacket DeleteOneItemByTypeAndSlot(Player user, int inventory, int slot, string type)
        {
            ServerPacket packet = null;
            if (!itemsList.ContainsKey(inventory))
                return packet;
            InventorySlot invSlot = itemsList[inventory][slot];
            if (invSlot != null)
            {
                if (invSlot.invItem.itemBase.deleteOnUse)
                {
                    invSlot.amount--;
                    if (invSlot.amount > 0)
                    {
                        invSlot.SaveSlot(user.id, user.GetSession().GetAccount().id);
                        packet = new ServerPacket(Outgoing.invSlot);
                        packet.AppendInt(inventory);
                        packet.AppendString(slot + "." + invSlot.invItem.itemBase.id + "." + invSlot.amount + ".0");
                    }
                    else
                    {
                        using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
                        {
                            dbClient.ExecuteQuery("DELETE FROM items_server" + GameServer.serverId + " WHERE id =  '" + invSlot.invItem.item.id + "';");
                        }
                        itemsList[inventory][slot] = null;
                        packet = new ServerPacket(Outgoing.invSlot);
                        packet.AppendInt(inventory);
                        packet.AppendString(slot + ".-1.0.0");
                    }
                }
                else
                {
                    packet = new ServerPacket(Outgoing.invSlot);
                    packet.AppendInt(inventory);
                    packet.AppendString(slot + "." + invSlot.invItem.itemBase.id + "." + invSlot.amount + ".0");
                }
                return packet;
            }
            return packet;
        }

        public void RemoveItem(Player user, int inventory, int slot)
        {
            if (!this.itemsList.ContainsKey(inventory) || itemsList[inventory].Length <= slot || itemsList[inventory][slot] == null)
                return;
            if (itemsList[inventory][slot].invItem.isItem && inventory == 3)
                if (itemsList[inventory][slot].invItem.item.x > 0 && itemsList[inventory][slot].invItem.item.y > 0)
                    return;
            user.SendPacket(MakeInvSlotBase(inventory, slot, -1, 0));
            InventorySlot invSlot = itemsList[inventory][slot];
            itemsList[inventory][slot] = null;
            invSlot.SaveSlot(user.id, user.GetSession().GetAccount().id);
            invSlot.DeleteSlot();
        }

        public void DropItem(Player user, int inventory, int slot, int qty)
        {
            if (!this.itemsList.ContainsKey(inventory) || itemsList[inventory].Length <= slot || itemsList[inventory][slot] == null || itemsList[inventory][slot].amount < qty)
                return;
            itemsList[inventory][slot].amount -= qty;
            Random drop = new Random();
            DropItem dItem = new DropItem(0, user.x + drop.Next(-1, 2), user.y + drop.Next(-1, 2), DateTime.Now, false, itemsList[inventory][slot].invItem.isSp, itemsList[inventory][slot].invItem.isFairy, itemsList[inventory][slot].invItem.isItem, false, qty, itemsList[inventory][slot].invItem.item, itemsList[inventory][slot].invItem.specialist, itemsList[inventory][slot].invItem.fairy, null, null);
            if (itemsList[inventory][slot].amount > 0)
            {
                itemsList[inventory][slot].Updated = true;
                user.SendPacket(MakeInvSlotBase(inventory, slot, itemsList[inventory][slot].invItem.itemBase.id, itemsList[inventory][slot].amount));
            }
            else
            {
                itemsList[inventory][slot].DeleteSlot();
                itemsList[inventory][slot] = null;
                user.SendPacket(MakeInvSlotBase(inventory, slot, -1, 0));
            }
            user.map.AddDropItem(dItem);
        }

        public ServerPacket DeleteOneItemByWid(Player player, int inventory, int wId)
        {
            ServerPacket packet = null;
            if (!itemsList.ContainsKey(inventory))
                return packet;
            InventorySlot slot = itemsList[inventory].FirstOrDefault(x => x != null && x.invItem.item.itemBase.wId == wId);
            int pos = Array.IndexOf(itemsList[inventory], slot);
            if (slot != null)
            {
                if (slot.invItem.itemBase.deleteOnUse)
                {
                    slot.amount--;
                    if (slot.amount > 0)
                    {
                        slot.SaveSlot(player.id, player.GetSession().GetAccount().id);
                        packet = new ServerPacket(Outgoing.invSlot);
                        packet.AppendInt(inventory);
                        packet.AppendString(pos + "." + slot.invItem.itemBase.id + "." + slot.amount + ".0");
                    }
                    else
                    {
                        using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
                        {
                            dbClient.ExecuteQuery("DELETE FROM items_server" + GameServer.serverId + " WHERE id =  '" + slot.invItem.item.id + "';");
                        }
                        itemsList[inventory][pos] = null;
                        packet = new ServerPacket(Outgoing.invSlot);
                        packet.AppendInt(inventory);
                        packet.AppendString(pos + ".-1.0.0");
                    }
                }
                else
                {
                    packet = new ServerPacket(Outgoing.invSlot);
                    packet.AppendInt(inventory);
                    packet.AppendString(pos + "." + slot.invItem.itemBase.id + "." + slot.amount + ".0");
                }
                return packet;
            }
            return packet;
        }
        #endregion

        #region Ajout d'objet
        public int AddItem(Player player, int inventory, object item, string type, int amount, bool sendInventory)
        {
            if (!itemsList.ContainsKey(inventory))
            {
                return -1;
            }

            bool placeInventory = false;
            int allowed_slot = 48;
            if (type == "item")
            {
                Item realItem = (Item)item;
                if (inventory == 3 && this.AlreadyHasItem(inventory, realItem.itemBase.id) && realItem.itemBase.type != "ml_default")
                {
                    return -2;
                }
            }
            int newIndex = this.FuturIndex(this.itemsList[inventory]);
            if (newIndex < allowed_slot && newIndex > -1)
            {
                itemsList[inventory][newIndex] = new InventorySlot(newIndex, amount);
                itemsList[inventory][newIndex].invItem = new InventoryItem(item, type);
                itemsList[inventory][newIndex].SaveSlot(player.id, player.GetSession().GetAccount().id);
                placeInventory = true;
            }
            if (!placeInventory)
                return -1;
            if (sendInventory)
            {
                player.SendPacket(MakeEquipPacket(inventory, newIndex, itemsList[inventory][newIndex].invItem.itemBase.id, itemsList[inventory][newIndex].invItem.GetUpgrade(1), itemsList[inventory][newIndex].invItem.GetUpgrade(2)));
                if (inventory == 3)
                    player.SendPacket(this.BuildMlObj());
            }
            return newIndex;
        }
        public bool AddBasicItem(Player player, int inventory, int itemBaseId, int amount)
        {
            int maxItemInSlot = 99 - amount;
            InventorySlot newSlot = this.itemsList[inventory].FirstOrDefault(x => x != null && x.invItem.itemBase.id == itemBaseId && x.amount <= maxItemInSlot);
            int newIndex = this.FuturIndex(this.itemsList[inventory]);
            if (newSlot != null)
            {
                newSlot.amount += amount;
                player.SendPacket(MakeInvSlotBase(inventory, newSlot.idSlot, itemBaseId, itemsList[inventory][newSlot.idSlot].amount));
                itemsList[inventory][newSlot.idSlot].SaveSlot(player.id, player.GetSession().GetAccount().id);
                return true;
            }
            else if (newIndex > -1 && newIndex < 48)
            {
                newSlot = new InventorySlot(newIndex, amount);
                Item newItem = new Item(GameServer.NewItemId(), player.id, itemBaseId, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, true);
                newSlot.invItem = new InventoryItem(new Item(newItem, newIndex, false), "item");
                itemsList[inventory][newIndex] = newSlot;
                itemsList[inventory][newIndex].SaveSlot(player.id, player.GetSession().GetAccount().id);
                player.SendPacket(MakeInvSlotBase(inventory, newIndex, itemBaseId, amount));
                return true;
            }
            return false;
        }
        #endregion

        #region Déplacement d'objets
        //A REFAIRE
        public void MoveItem(Player user, int inventorytype, int slot, int amount, int slot_target)
        {
            if (slot < 0 || slot > 48 || slot_target < 0 || slot_target > 48 || inventorytype < 0 || inventorytype > 2 || itemsList[inventorytype][slot] == null || itemsList[inventorytype][slot].amount < amount)
                return;
            InventoryItem itemInventory = itemsList[inventorytype][slot].invItem;

            if (itemsList[inventorytype][slot_target] != null && inventorytype != 0)
            {
                InventoryItem item_target = itemsList[inventorytype][slot_target].invItem;
                if (itemInventory.itemBase.id == item_target.itemBase.id)
                {
                    if (itemsList[inventorytype][slot_target].amount + amount <= 99)
                    {
                        itemsList[inventorytype][slot_target].amount += amount;
                        itemsList[inventorytype][slot].amount -= amount;
                        itemsList[inventorytype][slot_target].SaveSlot(user.id, user.GetSession().GetAccount().id);
                        if (itemsList[inventorytype][slot].amount == 0)
                        {
                            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
                            {
                                dbClient.ExecuteQuery("DELETE FROM items_server" + GameServer.serverId + " WHERE id =  '" + itemsList[inventorytype][slot].invItem.item.id + "' AND charId = '" + user.id + "';");
                            }
                            itemsList[inventorytype][slot] = null;
                        }
                        else
                            itemsList[inventorytype][slot].SaveSlot(user.id, user.GetSession().GetAccount().id);
                    }
                    else
                    {
                        int amountReal = 99 - itemsList[inventorytype][slot_target].amount;
                        itemsList[inventorytype][slot_target].amount += amountReal;
                        itemsList[inventorytype][slot].amount -= amountReal;
                        itemsList[inventorytype][slot_target].SaveSlot(user.id, user.GetSession().GetAccount().id);
                        itemsList[inventorytype][slot].SaveSlot(user.id, user.GetSession().GetAccount().id);
                    }
                }
                else
                {
                    InventoryItem item = itemsList[inventorytype][slot_target].invItem;
                    int iAmount = itemsList[inventorytype][slot_target].amount;
                    InventoryItem item2 = itemsList[inventorytype][slot].invItem;
                    int iAmount2 = itemsList[inventorytype][slot].amount;
                    itemsList[inventorytype][slot] = null;
                    itemsList[inventorytype][slot_target] = null;
                    InventorySlot newIS = new InventorySlot(slot, iAmount);
                    InventorySlot newIS2 = new InventorySlot(slot_target, iAmount2);
                    newIS.invItem = item;
                    newIS2.invItem = item2;
                    itemsList[inventorytype][slot] = newIS;
                    itemsList[inventorytype][slot].SaveSlot(user.id, user.GetSession().GetAccount().id);
                    itemsList[inventorytype][slot_target] = newIS2;
                    itemsList[inventorytype][slot_target].SaveSlot(user.id, user.GetSession().GetAccount().id);
                }
            }
            else
            {
                if (itemsList[inventorytype][slot_target] != null && inventorytype == 0 && itemsList[inventorytype][slot].amount < amount)
                    return;
                if (amount < itemsList[inventorytype][slot].amount && itemsList[inventorytype][slot].invItem.itemBase.inventory != 0)
                {
                    Item moveItem = new Item(itemsList[inventorytype][slot].invItem.item, slot_target, false);
                    moveItem.invPos = slot_target;
                    itemsList[inventorytype][slot_target] = new InventorySlot(slot_target, amount);
                    itemsList[inventorytype][slot_target].invItem = new InventoryItem(new Item(itemsList[inventorytype][slot].invItem.item, slot_target, false), "item");
                    itemsList[inventorytype][slot].amount -= amount;
                    itemsList[inventorytype][slot_target].SaveSlot(user.id, user.GetSession().GetAccount().id);
                    itemsList[inventorytype][slot].SaveSlot(user.id, user.GetSession().GetAccount().id);
                    itemsList[inventorytype][slot].invItem.item.id = GameServer.NewItemId();
                    itemsList[inventorytype][slot].invItem.item.mustInsert = true;
                }
                else
                {
                    InventoryItem moveItem = new InventoryItem(itemsList[inventorytype][slot].invItem);
                    if (itemsList[inventorytype][slot_target] != null && inventorytype == 0)
                    {
                        if (moveItem.isSp)
                        {
                            moveItem.specialist.inEquip = true;
                            moveItem.specialist.invPos = slot_target;
                        }
                        else if (moveItem.isFairy)
                        {
                            moveItem.fairy.invPos = slot_target;
                        }
                        else
                        {
                            moveItem.item.invPos = slot_target;
                        }
                        InventoryItem moveItem2 = new InventoryItem(itemsList[inventorytype][slot_target].invItem);
                        if (moveItem2.isSp)
                        {
                            moveItem2.specialist.inEquip = true;
                            moveItem2.specialist.invPos = slot;
                        }
                        else if (moveItem2.isFairy)
                        {
                            moveItem2.fairy.invPos = slot;
                        }
                        else
                        {
                            moveItem2.item.invPos = slot;
                        }
                        itemsList[inventorytype][slot_target].invItem = moveItem;
                        itemsList[inventorytype][slot].invItem = moveItem2;
                        itemsList[inventorytype][slot_target].SaveSlot(user.id, user.GetSession().GetAccount().id);
                        itemsList[inventorytype][slot].SaveSlot(user.id, user.GetSession().GetAccount().id);
                    }
                    else
                    {
                        if (moveItem.itemBase.isSp)
                        {
                            moveItem.specialist.inEquip = true;
                            moveItem.specialist.invPos = slot_target;
                        }
                        else if (moveItem.isFairy)
                        {
                            moveItem.fairy.invPos = slot_target;
                        }
                        else
                        {
                            moveItem.item.invPos = slot_target;
                        }
                        itemsList[inventorytype][slot_target] = new InventorySlot(slot_target, amount);
                        itemsList[inventorytype][slot_target].invItem = new InventoryItem(itemsList[inventorytype][slot].invItem);
                        itemsList[inventorytype][slot] = null;
                        itemsList[inventorytype][slot_target].SaveSlot(user.id, user.GetSession().GetAccount().id);
                    }
                }
            }
            user.SendPacket(this.BuildInventory(inventorytype));
        }
        public void MoveItemSpecial(Player user, int inventorytype, int slot, int inventory_target, int slot_target)
        {
            if (itemsList[inventorytype][slot] == null)
                return;
            if (inventory_target == 6 && !itemsList[inventorytype][slot].invItem.itemBase.isSp)
                return;
            if (inventory_target == 7 && !itemsList[inventorytype][slot].invItem.isItem && itemsList[inventorytype][slot].invItem.itemBase.eqSlot != 13 && itemsList[inventorytype][slot].invItem.itemBase.eqSlot != 14)
                return;

            if (itemsList[inventory_target][slot_target] != null)
                return;
            else
            {
                InventoryItem inventoryItem = itemsList[inventorytype][slot].invItem;
                if (inventoryItem.isSp)
                {
                    if (inventory_target == 0)
                    {
                        inventoryItem.specialist.inEquip = true;
                    }
                    else
                    {
                        inventoryItem.specialist.inEquip = false;
                    }
                }
                if (inventoryItem.isItem)
                {
                    if (inventory_target == 0)
                    {
                        inventoryItem.item.inEquip = 1;
                    }
                    else
                    {
                        inventoryItem.item.inEquip = 0;
                    }
                }
                int amount = itemsList[inventorytype][slot].amount;
                itemsList[inventorytype][slot].amount--;
                if (itemsList[inventorytype][slot].invItem.itemBase.eqSlot != -1 || itemsList[inventorytype][slot].amount < 1)
                    itemsList[inventorytype][slot] = null;
                itemsList[inventory_target][slot_target] = new InventorySlot(slot_target, amount);
                itemsList[inventory_target][slot_target].invItem = inventoryItem;
                itemsList[inventory_target][slot_target].SaveSlot(user.id, user.GetSession().GetAccount().id);
                user.SendPacket(BuildInventory(inventorytype));
                user.SendPacket(BuildInventory(inventory_target));
            }
        }
        #endregion

        #region Recherche d'objet
        public InventorySlot SlotByBaseId(InventorySlot[] slots, int baseId)
        {
            return slots.FirstOrDefault(x => x != null && x.invItem.item.itemBase.id == baseId);
        }

        public bool AlreadyHasItem(int inventory, int base_id)
        {
            if (!itemsList.ContainsKey(inventory))
                return false;
            InventorySlot slot = this.SlotByBaseId(itemsList[inventory], base_id);
            if (slot != null)
                return true;
            return false;
        }

        public void GetItemInfo(Player user, int type, int slot)
        {
            int inventory = 0;
            if (type == 10)
                inventory = 6;
            else if (type == 11)
                inventory = 7;
            if (slot > -1 && slot < itemsList[inventory].Length && itemsList[inventory][slot] != null)
            {
                if (itemsList[inventory][slot].invItem.itemBase.eqSlot != -1)
                {
                    InventoryItem invItem = itemsList[inventory][slot].invItem;
                    Item item = new Item(-1, -1);
                    Specialist sp = new Specialist(-1);
                    Fairy fairy = new Fairy(-1);
                    if (itemsList[inventory][slot].invItem.isSp)
                    {
                        sp = itemsList[inventory][slot].invItem.specialist;
                    }
                    else if (itemsList[inventory][slot].invItem.isFairy)
                    {
                        fairy = itemsList[inventory][slot].invItem.fairy;
                    }
                    else
                    {
                        item = itemsList[inventory][slot].invItem.item;
                    }
                    if (invItem.isSp)
                        user.SendPacket(invItem.specialist.GetInfos(0));
                    else if (invItem.isFairy)
                        invItem.fairy.GetInfo(user);
                    else
                        invItem.item.GetEquipInfo(user);
                }
            }
        }

        public Dictionary<InventorySlot, int> GetSlotByItemId(int inventory, int itemId, int count)
        {
            int countFind = 0;
            Dictionary<InventorySlot, int> slotList = new Dictionary<InventorySlot, int>();
            foreach (InventorySlot invSlot in itemsList[inventory].Where(x => x != null && x.invItem.itemBase.id == itemId))
            {
                if (invSlot.amount + countFind <= count)
                {
                    slotList.Add(invSlot, invSlot.amount);
                    countFind += invSlot.amount;
                }
                else
                {
                    int itemCountNeeded = count - countFind;
                    slotList.Add(invSlot, itemCountNeeded);
                    countFind += itemCountNeeded;
                }
                if (countFind == count)
                    break;
            }
            if (countFind == count)
                return slotList;
            return null;
        }
        #endregion

        #region Création de packets basiques
        public static ServerPacket MakeInvSlotBase(int inventory, int slot, int vnum, int count)
        {
            ServerPacket packet = new ServerPacket(Outgoing.invSlot);
            packet.AppendInt(inventory);
            packet.AppendString(slot + "." + vnum + "." + count + ".0");
            return packet;
        }

        public static ServerPacket MakeEquipPacket(int inventory, int slot, int vnum, int rare, int upgrade)
        {
            ServerPacket packet = new ServerPacket(Outgoing.invSlot);
            packet.AppendInt(inventory);
            packet.AppendString(slot + "." + vnum + "." + rare + "." + upgrade + ".0");
            return packet;
        }
        #endregion
    }
}
