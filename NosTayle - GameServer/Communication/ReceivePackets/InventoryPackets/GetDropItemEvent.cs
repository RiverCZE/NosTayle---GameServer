using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.InventoryPackets
{
    internal sealed class GetDropItemEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Event.valuesCount == 3)
            {
                try
                {
                    int entitieType = Convert.ToInt32(Event.GetValue(0));
                    int entitieId = Convert.ToInt32(Event.GetValue(1));
                    int itemId = Convert.ToInt32(Event.GetValue(2));
                    Session.GetPlayer().map.GetDropItem(Session.GetPlayer(), itemId);
                }
                catch { }
            }
        }
    }
}
