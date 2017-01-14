using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.InventoryPackets
{
    internal sealed class UseItemEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            /*if (Session.GetPlayer().locomotion != null)
                return;*/
            if (!Session.GetPlayer().inventory.isBlocked && Event.valuesCount >= 6)
                Session.GetPlayer().inventory.UseItem(Session.GetPlayer(), Convert.ToInt32(Event.GetValue(2)), Convert.ToInt32(Event.GetValue(3)));
        }
    }
}
