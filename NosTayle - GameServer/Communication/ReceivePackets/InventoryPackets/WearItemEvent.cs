using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.InventoryPackets
{
    internal sealed class WearItemEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (!Session.GetPlayer().inventory.isBlocked && Event.valuesCount == 2)
                Session.GetPlayer().inventory.WearItem(Session.GetPlayer(), Convert.ToInt32(Event.GetValue(0)), Convert.ToInt32(Event.GetValue(1)));
        }
    }
}
