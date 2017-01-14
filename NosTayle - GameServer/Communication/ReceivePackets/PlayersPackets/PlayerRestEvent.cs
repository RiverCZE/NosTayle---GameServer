using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.Core;
using NosTayleGameServer.NosTale.Entities.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.PlayersPackets
{
    internal sealed class PlayerRestEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            Player player = Session.GetPlayer();
            if (player.locomotion != null)
                return;
            if (player.inAction)
                return;
            player.rested = player.rested == 1 ? 0 : 1;
            ServerPacket packet = new ServerPacket(Outgoing.rest);
            packet.AppendInt(1);
            packet.AppendInt(player.id);
            packet.AppendInt(player.rested);
            player.map.SendMap(packet);
        }
    }
}
