using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.GuriPackets
{
    internal sealed class GuriEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Event.valuesCount >= 1)
            {
                switch (Event.GetValue(0))
                {
                    case "3": //Vue scene
                        {
                            if (Event.valuesCount == 2)
                                Session.GetPlayer().questManager.GetScene(Session.GetPlayer(), Convert.ToInt32(Event.GetValue(1)));
                            Console.WriteLine(Event.dataBrute);
                        }
                        break;
                    case "4":
                        if (Event.valuesCount >= 2)
                        {
                            switch (Event.GetValue(1))
                            {
                                case "2":
                                    if (Event.valuesCount >= 5)
                                    {
                                        if (Event.GetValue(2) == "1" && Convert.ToInt32(Event.GetValue(3)) == Session.GetPlayer().id)
                                        {
                                            Session.GetPlayer().UpdatePresentation(Event.dataBrute.Substring(12 + Session.GetPlayer().id.ToString().Length));
                                        }
                                    }
                                    break;
                                case "3":
                                    if (Event.valuesCount >= 4)
                                        if (Event.GetValue(2) == "0" && Event.GetValue(3) == "0" && Event.dataBrute.Substring(13).Length <= 120)
                                            Session.GetChannel().SendSpeaker(Session.GetPlayer(), Event.dataBrute.Substring(13));
                                    break;
                            }
                        }
                        break;
                    case "5": //danse
                        if (Event.valuesCount >= 5)
                        {
                            bool enDance = false;
                            if (Event.GetValue(3) == "100" || Event.GetValue(3) == "-1")
                            {
                                enDance = true;
                            }
                            if (Event.GetValue(3) == "0" || Event.GetValue(3) == "100" || Event.GetValue(3) == "-1")
                            {
                                ServerPacket nPacket = new ServerPacket(Outgoing.guri);
                                nPacket.AppendInt((enDance ? 6 : 2));
                                nPacket.AppendInt(1);
                                nPacket.AppendInt(Session.GetPlayer().id);
                                if (enDance)
                                {
                                    nPacket.AppendInt(0);
                                    nPacket.AppendInt(0);
                                }
                                Session.GetPlayer().map.SendMap(nPacket);
                            }
                        }
                        break;
                    case "10": //SMILEY
                        if (Event.valuesCount == 4)
                        {
                            if (Convert.ToInt32(Event.GetValue(3)) >= 973 && Convert.ToInt32(Event.GetValue(3)) <= 999)
                            {
                                Session.GetPlayer().map.SendEffect(1, Session.GetPlayer().id, (Convert.ToInt32(Event.GetValue(3)) + 4099));
                            }
                        }
                        break;
                }
            }
        }
    }
}
