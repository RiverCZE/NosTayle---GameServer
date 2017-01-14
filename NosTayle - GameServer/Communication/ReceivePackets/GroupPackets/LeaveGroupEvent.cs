using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.GroupPackets
{
    internal sealed class LeaveGroupEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Session.GetPlayer().group != null)
            {
                Session.GetPlayer().group.RemoveUser(Session.GetPlayer());
                Session.GetPlayer().group = null;
                ServerPacket packet = new ServerPacket(Outgoing.pInit);
                packet.AppendInt(0);
                Session.SendPacket(packet);
                packet = new ServerPacket(Outgoing.groupStatue);
                packet.AppendInt(-1);
                packet.AppendString("1." + Session.GetPlayer().id);
                Session.GetPlayer().map.SendMap(packet);
            }
        }
    }
}
