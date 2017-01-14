using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.NosTale.Entities.Players;
using NosTayleGameServer.NosTale.Items;
using NosTayleGameServer.NosTale.Items.Others;
using NosTayleGameServer.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Shops
{
    public class Shop
    {
        internal Dictionary<int, ShopItem> shopItems = new Dictionary<int, ShopItem>();
        internal int id;
        internal int type;
        internal string name;

        public Shop(int id, int type, string name)
        {
            this.id = id;
            this.type = type;
            this.name = name;
        }

        public void Initialize()
        {
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                DataTable dataTable2 = dbClient.ReadDataTable("SELECT * FROM shop_items WHERE shopId = " + this.id + ";");
                foreach (DataRow shopItemRow in dataTable2.Rows)
                {
                    GameServer.GetShopManager().shopItemsAll.Add((int)shopItemRow["id"], (int)shopItemRow["shopId"]);
                    this.shopItems.Add((int)shopItemRow["id"], new ShopItem((int)shopItemRow["id"], (int)shopItemRow["shopId"], (int)shopItemRow["itemId"], shopItemRow["type"].ToString(), (int)shopItemRow["price"], (int)shopItemRow["rare"], (int)shopItemRow["upgrade"], (int)shopItemRow["color"]));
                }
            }
        }

        public void MakeShop(Player user, int npcId)
        {
            ServerPacket packet = new ServerPacket(Outgoing.makeShop);
            packet.AppendInt(2);
            packet.AppendInt(npcId);
            packet.AppendInt(0);
            packet.AppendInt(100);
            foreach (ShopItem sItem in shopItems.Values)
            {
                if (GameServer.GetItemsManager().itemList.ContainsKey(sItem.itemId))
                {
                    ItemBase iBase = GameServer.GetItemsManager().itemList[sItem.itemId];
                    switch (iBase.inventory)
                    {
                        case 0:
                            packet.AppendString(iBase.inventory + "." + sItem.id + "." + iBase.id + "." + sItem.rare + "." + sItem.upgrade + "." + sItem.price);
                            break;
                        default:
                            packet.AppendString(iBase.inventory + "." + sItem.id + "." + iBase.id + ".-1." + sItem.price);
                            break;
                    }
                }
            }
            user.SendPacket(packet);
        }

        public void BuyItem(Player user, int itemId, int amount)
        {
            if (amount <= 0 || amount > 99)
                return;
            if (shopItems.ContainsKey(itemId))
            {
                ShopItem sItem = shopItems[itemId];
                ItemBase sBase = GameServer.GetItemsManager().itemList[sItem.itemId];
                int price = sItem.price * amount;
                if (sBase.inventory == 0 && amount > 1)
                {
                    return;
                }

                if (user.gold < price)
                {
                    user.SendPacket(GlobalMessage.MakeShopMessage(2, GameServer.GetLanguage(user.languagePack, "error.gold")));
                    return;
                }

                if (sBase.inventory == 0)
                {
                    switch (sBase.eqSlot)
                    {
                        case 10:
                            {
                                Fairy fairy = new Fairy(GameServer.NewFairyId(), user.id, sBase, 0, sBase.percentMin, true);
                                if (user.inventory.AddItem(user, 0, fairy, "fairy", 1, true) != -1)
                                {
                                    user.gold -= price;
                                    user.SendGold();
                                    user.SendPacket(GlobalMessage.MakeShopMessage(1, GameServer.GetLanguage(user.languagePack, "shop.buy.ok")));
                                }
                                else
                                    user.SendPacket(GlobalMessage.MakeShopMessage(2, GameServer.GetLanguage(user.languagePack, "shop.buy.noplace")));
                            }
                            break;
                        case 12:
                            {
                                Specialist sp = new Specialist(GameServer.NewSpId(), user.id, sBase.id, true, sItem.upgrade, 0, 1);
                                if (user.inventory.AddItem(user, 0, sp, "specialist", 1, true) != -1)
                                {
                                    user.gold -= price;
                                    user.SendGold();
                                    user.SendPacket(GlobalMessage.MakeShopMessage(1, GameServer.GetLanguage(user.languagePack, "shop.buy.ok")));
                                }
                                else
                                    user.SendPacket(GlobalMessage.MakeShopMessage(2, GameServer.GetLanguage(user.languagePack, "shop.buy.noplace")));
                            }
                            break;
                        default:
                            {
                                int durabilites = 0;
                                if (sBase.type.StartsWith("ml_game"))
                                    durabilites = 10000;
                                Item item = new Item(GameServer.NewItemId(), user.id, sBase.id, sBase.damageMin, sBase.damageMax, sBase.hitRate, sBase.corpDef, sBase.distDef, sBase.magicDef, sBase.dodge, sItem.rare, sItem.upgrade, 0, false, 0, 0, durabilites);
                                item.mustInsert = true;
                                if (user.inventory.AddItem(user, 0, item, "item", 1, true) != -1)
                                {
                                    user.gold -= price;
                                    user.SendGold();
                                    user.SendPacket(GlobalMessage.MakeShopMessage(1, GameServer.GetLanguage(user.languagePack, "shop.buy.ok")));
                                }
                                else
                                    user.SendPacket(GlobalMessage.MakeShopMessage(2, GameServer.GetLanguage(user.languagePack, "shop.buy.noplace")));
                            }
                            break;
                    }
                }
                else if (sBase.inventory == 3)
                {
                    int durabilites = 0;
                    if (sBase.type.StartsWith("ml_game"))
                    {
                        durabilites = 10000;
                    }
                    Item item = new Item(GameServer.NewItemId(), user.id, sBase.id, sBase.damageMin, sBase.damageMax, sBase.hitRate, sBase.corpDef, sBase.distDef, sBase.magicDef, sBase.dodge, sItem.rare, sItem.upgrade, 0, false, 0, 0, durabilites);
                    item.mustInsert = true;
                    int result = user.inventory.AddItem(user, 3, item, "item", 1, true);
                    if (result != -1)
                    {
                        if (result != -2)
                        {
                            user.gold -= price;
                            user.SendGold();
                            user.SendPacket(GlobalMessage.MakeShopMessage(1, GameServer.GetLanguage(user.languagePack, "shop.buy.ok")));
                        }
                        else
                            user.SendPacket(GlobalMessage.MakeShopMessage(2, GameServer.GetLanguage(user.languagePack, "shop.buy.alreadyhave")));
                    }
                    else
                        user.SendPacket(GlobalMessage.MakeShopMessage(2, GameServer.GetLanguage(user.languagePack, "shop.buy.noplace")));
                }
                else
                {
                    try
                    {
                        if (user.inventory.AddBasicItem(user, sBase.inventory, sBase.id, amount))
                        {
                            user.gold -= price;
                            user.SendGold();
                            user.SendPacket(GlobalMessage.MakeShopMessage(1, GameServer.GetLanguage(user.languagePack, "shop.buy.ok")));
                        }
                        else
                            user.SendPacket(GlobalMessage.MakeShopMessage(2, GameServer.GetLanguage(user.languagePack, "shop.buy.noplace")));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

            }
        }
    }
}
