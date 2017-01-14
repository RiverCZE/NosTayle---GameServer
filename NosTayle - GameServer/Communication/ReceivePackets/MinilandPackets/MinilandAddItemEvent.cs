using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.MinilandPackets
{
    internal sealed class MinilandAddItemEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Event.valuesCount == 3)
                if (Session.GetPlayer().map == Session.GetPlayer().miniLand)
                    Session.GetPlayer().miniLand.AddItem(Convert.ToInt32(Event.GetValue(0)), Convert.ToInt32(Event.GetValue(1)), Convert.ToInt32(Event.GetValue(2)));
        }
    }
}
