using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.FamilyPackets
{
    internal sealed class UpdateFamilyPermissionRankEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Session.GetPlayer().family != null && Event.valuesCount == 3)
                Session.GetPlayer().family.UpdateRankPermission(Session.GetPlayer(), Convert.ToInt32(Event.GetValue(0)), Convert.ToInt32(Event.GetValue(1)), Convert.ToInt32(Event.GetValue(2)));
        }
    }
}
