using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.PlayersPackets
{
    internal sealed class UpgradeItemEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            NosTayleGameServer.NosTale.UpgradeSystem.UpgradeController.GetUp(Session.GetPlayer(), Event.GetValues());
        }
    }
}
