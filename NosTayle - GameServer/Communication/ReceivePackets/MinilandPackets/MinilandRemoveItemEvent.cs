using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.MinilandPackets
{
    internal sealed class MinilandRemoveItemEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Event.valuesCount == 1)
                if (Session.GetPlayer().map == Session.GetPlayer().miniLand)
                    Session.GetPlayer().miniLand.RemoveItem(Convert.ToInt32(Event.GetValue(0)));
        }
    }
}
