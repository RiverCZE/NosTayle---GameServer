using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.PlayersPackets
{
    internal sealed class MoveNpcEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Event.valuesCount == 7 && Session.GetPlayer().pet != null)
            {
                if (Event.GetValue(0) == Session.GetPlayer().map.mapId.ToString() &&  Event.GetValue(2) == Session.GetPlayer().pet.id.ToString() && Session.GetPlayer().locomotion == null)
                {
                    Session.GetPlayer().pet.x = Convert.ToInt32(Event.GetValue(3));
                    Session.GetPlayer().pet.y = Convert.ToInt32(Event.GetValue(4));
                    Session.GetPlayer().map.MoveEntitie(2, Session.GetPlayer().pet.id, Session.GetPlayer().pet.x, Session.GetPlayer().pet.y, 9, Session.GetPlayer(), false);
                }
            }
        }
    }
}
