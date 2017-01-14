using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.FriendPackets
{
    internal sealed class FriendDeleteEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Event.valuesCount == 1)
                Session.GetPlayer().friendList.DelFriend(Session.GetPlayer(), Convert.ToInt32(Event.GetValue(0)), true);
        }
    }
}
