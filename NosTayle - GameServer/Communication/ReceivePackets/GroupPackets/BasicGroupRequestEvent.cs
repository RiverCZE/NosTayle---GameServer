using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.NosTale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.GroupPackets
{
    internal sealed class BasicGroupRequestEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Event.valuesCount == 2)
            {
                switch (Event.GetValue(0))
                {
                    case "0":
                        if (Session.GetPlayer().group == null)
                            Session.GetPlayer().map.SendGroupRequest(Session.GetPlayer(), Convert.ToInt32(Event.GetValue(1)));
                        break;
                    case "1":
                        if (Session.GetPlayer().group != null)
                        {
                            if (Session.GetPlayer().group.members.Count < 3)
                                Session.GetPlayer().map.SendGroupRequest(Session.GetPlayer(), Convert.ToInt32(Event.GetValue(1)));
                            else
                                Session.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(Session.GetPlayer().languagePack, "error.group.full")));
                        }
                        break;
                }
            }
        }
    }
}
