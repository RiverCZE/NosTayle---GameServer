using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.InventoryPackets
{
    internal sealed class DropItemEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Event.valuesCount == 3)
            {
                try
                {
                    int inventory = Convert.ToInt32(Event.GetValue(0));
                    int slot = Convert.ToInt32(Event.GetValue(1));
                    int qty = Convert.ToInt32(Event.GetValue(2));
                    if (inventory >= 0 && inventory <= 2)
                    {
                        if (inventory == 0 && qty != 1)
                            return;
                        else if (qty < 1 || qty > 99)
                            return;
                        Session.GetPlayer().inventory.DropItem(Session.GetPlayer(), inventory, slot, qty);
                    }
                }
                catch { }
            }
        }
    }
}
