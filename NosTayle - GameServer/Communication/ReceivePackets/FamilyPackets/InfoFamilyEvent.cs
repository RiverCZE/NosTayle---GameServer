using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.FamilyPackets
{
    internal sealed class InfoFamilyEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Session.GetPlayer().family != null && Event.valuesCount == 2)
                Session.GetPlayer().family.FamilyInfosRequest(Session.GetPlayer(), Convert.ToInt32(Event.GetValue(0)), Convert.ToInt32(Event.GetValue(1)));
        }
    }
}
