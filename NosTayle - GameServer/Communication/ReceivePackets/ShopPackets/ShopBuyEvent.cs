using NosTayleGameServer.NosTale;
using NosTayleGameServer.NosTale.Entities.Npcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.ShopPackets
{
    internal sealed class ShopBuyEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Event.valuesCount == 4)
            {
                if (Event.GetValue(0) == "2")
                {
                    int npcId = Convert.ToInt32(Event.GetValue(1));
                    int itemId = Convert.ToInt32(Event.GetValue(2));
                    int amount = Convert.ToInt32(Event.GetValue(3));
                    if (Session.GetPlayer().map.npcsManager.NpcInListById(npcId) && GameServer.GetShopManager().shopItemsAll.ContainsKey(itemId))
                    {
                        Npc npc = Session.GetPlayer().map.npcsManager.GetNpcById(npcId);
                        if (npc.shopId == GameServer.GetShopManager().shopItemsAll[itemId])
                        {
                            if (npc.shop && GameServer.GetShopManager().shopList.ContainsKey(npc.shopId))
                                GameServer.GetShopManager().shopList[npc.shopId].BuyItem(Session.GetPlayer(), itemId, amount);
                        }
                        else if (npc.shopList.Contains(GameServer.GetShopManager().shopItemsAll[itemId].ToString()))
                        {
                            GameServer.GetShopManager().shopList[GameServer.GetShopManager().shopItemsAll[itemId]].BuyItem(Session.GetPlayer(), itemId, amount);
                        }
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error in BuyShopPacket(): {0}", Event.dataBrute);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
    }
}
