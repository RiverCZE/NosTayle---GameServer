using NosTayleGameServer.NosTale.Entities.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.PlayersPackets
{
    internal sealed class PlayerWalkEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Event.valuesCount == 4 && Session.GetPlayer().rested != 1)
            {
                Player player = Session.GetPlayer();
                if (!player.map.IsBlockedZone(Convert.ToInt32(Event.GetValue(0)), Convert.ToInt32(Event.GetValue(1))) && player.rested != 1)
                {
                    player.Move(Convert.ToInt32(Event.GetValue(0)), Convert.ToInt32(Event.GetValue(1)), player.direction);
                    player.map.MoveEntitie(1, player.id, player.x, player.y, Convert.ToInt32(Event.GetValue(3)), player, false);
                    player.SendSpeed();
                }
                //else
                   //ReplaceUser
            }
        }
    }
}
