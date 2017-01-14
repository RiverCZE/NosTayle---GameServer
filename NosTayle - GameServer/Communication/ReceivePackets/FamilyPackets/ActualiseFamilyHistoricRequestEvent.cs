using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.FamilyPackets
{
    internal sealed class ActualiseFamilyHistoricRequestEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Session.GetPlayer().family != null)
                Session.GetPlayer().family.GetHis().SendHis(Session.GetPlayer());
        }
    }
}
