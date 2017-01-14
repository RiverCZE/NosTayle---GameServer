using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.PlayersPackets
{
    internal sealed class PlayerWalkOnPortalEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            Session.GetPlayer().map.ChangeMapRequest(Session.GetPlayer());
        }
    }
}
