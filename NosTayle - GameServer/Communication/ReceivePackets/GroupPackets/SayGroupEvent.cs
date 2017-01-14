using NosTayleGameServer.Communication.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.GroupPackets
{
    internal sealed class SayGroupEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Session.GetPlayer().group != null)
                Session.GetPlayer().group.SendToAll(GlobalMessage.MakeSpeak(1, Session.GetPlayer().id, 3, Session.GetPlayer().name, Event.dataBrute.Substring(1)));
        }
    }
}
