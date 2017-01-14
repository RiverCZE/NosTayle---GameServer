using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.FriendPackets
{
    internal sealed class FriendMessageEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Event.valuesCount >= 2)
                Session.GetPlayer().friendList.SendMessage(Session.GetPlayer(), Convert.ToInt32(Event.GetValue(0)), Event.dataBrute.Substring(Event.GetHeader().Length + Event.GetValue(0).Length + 2));
        }
    }
}
