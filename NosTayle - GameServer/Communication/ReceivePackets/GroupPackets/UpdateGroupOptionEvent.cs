using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.NosTale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.GroupPackets
{
    internal sealed class UpdateGroupOptionEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Event.valuesCount == 2)
            {
                switch (Event.GetValue(0))
                {
                    case "8":
                        if (Session.GetPlayer().group != null)
                        {
                            if (Session.GetPlayer().group.groupOwner == Session.GetPlayer().id)
                            {
                                if (Event.GetValue(1) != "0" && Event.GetValue(1) != "1")
                                    return;
                                Session.GetPlayer().group.dropStatut = Convert.ToInt32(Event.GetValue(1));
                                Session.GetPlayer().group.UpdateDrop();
                                return;
                            }
                            Session.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(Session.GetPlayer().languagePack, "error.group.cantupdoption")));
                        }
                        break;
                    default:
                        Console.WriteLine(Event.dataBrute);
                        break;
                }
            }
        }
    }
}
