using NosTayleGameServer.NosTale.Entities.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.EntitiesPackets
{
    internal sealed class InfoRequestEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Event.valuesCount == 2)
            {
                switch (Event.GetValue(0))
                {
                    case "1":
                        Player otherPlayer = Session.GetPlayer().map.playersManagers.GetPlayerById(Convert.ToInt32(Event.GetValue(1)));
                        if (otherPlayer != null)
                            Session.GetPlayer().SendPacket(otherPlayer.GetInfoWindow(Session.GetPlayer().languagePack));
                        break;
                }
            }
        }
    }
}
