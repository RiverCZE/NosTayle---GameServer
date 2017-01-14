using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.NosTale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.PlayersPackets
{
    internal sealed class PlayerSayEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Event.dataBrute.Length > 4 && Event.dataBrute.Substring(4).Length <= 60)
            {
                Session.GetPlayer().lastSendMessagesNumber++;
                if (Session.GetPlayer().isMuted)
                {
                    Session.GetPlayer().map.SendMap(GlobalMessage.MakeSpeak(1, Session.GetPlayer().id, 1, Session.GetPlayer().name, GameServer.GetLanguage(Session.GetPlayer().languagePack, "error.immuted")));
                    return;
                }
                Session.GetPlayer().map.SendMap(Session.GetPlayer(), GlobalMessage.MakeMessage(Session.GetPlayer().id, Session.GetPlayer().type, 0, Event.dataBrute.Substring(4)), false);
            }
        }
    }
}
