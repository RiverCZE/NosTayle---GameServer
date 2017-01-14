using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.EntitiesPackets
{
    internal sealed class NpcSpecialRequestEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Event.valuesCount == 4)
                Session.GetPlayer().map.NRunReq(Session.GetPlayer(), Convert.ToInt32(Event.GetValue(0)), Convert.ToInt32(Event.GetValue(1)), Convert.ToInt32(Event.GetValue(2)), Convert.ToInt32(Event.GetValue(3)));
            else if (Event.valuesCount == 1)
            {
                switch (Convert.ToInt32(Event.GetValue(0)))
                {
                    case 18:
                        {
                            if (!Session.GetPlayer().map.specialMap)
                            {
                                ServerPacket packet = new ServerPacket(Outgoing.npcDialog);
                                packet.AppendInt(1);
                                packet.AppendInt(Session.GetPlayer().id);
                                packet.AppendInt(17);
                                Session.SendPacket(packet);
                            }
                        }
                        break;
                }
            }
        }
    }
}
