using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.NosTale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.FriendPackets
{
    internal sealed class FriendEnterMiniland : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Session.GetPlayer().map.specialMap)
            {
                Session.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(Session.GetPlayer().languagePack, "message.cant.here")));
                return;
            }
            if (Event.valuesCount == 2)
                if (Event.GetValue(0) == "1")
                    Session.GetPlayer().friendList.GoToMiniLand(Session.GetPlayer(), Convert.ToInt32(Event.GetValue(1)));
        }
    }
}
