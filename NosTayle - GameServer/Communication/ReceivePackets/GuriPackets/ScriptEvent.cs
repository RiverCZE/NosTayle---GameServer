using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.GuriPackets
{
    internal sealed class ScriptEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Event.valuesCount == 3)
            {
                if (Event.GetValue(0) == "0")
                    if (Event.GetValue(1) == "1")
                        if (Event.GetValue(2) == "10")
                        {
                            ServerPacket packet = new ServerPacket(Outgoing.script);
                            packet.AppendInt(1);
                            packet.AppendInt(20);
                            Session.SendPacket(packet);
                        }
            }
            Console.WriteLine(Event.dataBrute);
        }
    }
}
