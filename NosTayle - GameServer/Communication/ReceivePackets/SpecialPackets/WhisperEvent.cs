using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.NosTale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.SpecialPackets
{
    internal sealed class WhisperEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Event.valuesCount >= 1)
            {
                string name = Event.dataBrute.Split(' ')[0].Substring(1);
                string message = Event.dataBrute.Substring(2 + name.Length);
                if (!GameServer.SendWhisper(Session.GetPlayer(), name, message))
                    Session.SendPacket(GlobalMessage.MakeInfo(String.Format(GameServer.GetLanguage(Session.GetPlayer().languagePack, "error.user.offline"), name)));
            }
        }
    }
}
