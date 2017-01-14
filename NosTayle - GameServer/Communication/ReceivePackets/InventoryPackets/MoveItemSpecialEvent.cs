using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.InventoryPackets
{
    internal sealed class MoveItemSpecialEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (!Session.GetPlayer().inventory.isBlocked && Event.valuesCount == 4)
                Session.GetPlayer().inventory.MoveItemSpecial(Session.GetPlayer(), Convert.ToInt32(Event.GetValue(0)), Convert.ToInt32(Event.GetValue(1)), Convert.ToInt32(Event.GetValue(2)), Convert.ToInt32(Event.GetValue(3)));
        }
    }
}
