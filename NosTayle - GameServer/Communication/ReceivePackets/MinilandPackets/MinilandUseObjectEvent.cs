using NosTayleGameServer.NosTale.Entities.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.MinilandPackets
{
    internal sealed class MinilandUseObjectEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Event.valuesCount == 2)
            {
                Player owner = Session.GetChannel().GetPlayersManager().GetPlayerByName(Event.GetValue(0));
                if (owner != null)
                    if (owner.miniLand == Session.GetPlayer().map)
                        owner.miniLand.UseObj(Session.GetPlayer(), Convert.ToInt32(Event.GetValue(1)));
            }
        }
    }
}
