using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.ShopPackets
{
    internal sealed class ShopRequestEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Event.valuesCount == 4)
                if (Event.GetValue(1) == "0" && Event.GetValue(2) == "2")
                        Session.GetPlayer().map.NpcShopRequest(Session.GetPlayer(), Convert.ToInt32(Event.GetValue(0)), Convert.ToInt32(Event.GetValue(3)));
        }
    }
}
