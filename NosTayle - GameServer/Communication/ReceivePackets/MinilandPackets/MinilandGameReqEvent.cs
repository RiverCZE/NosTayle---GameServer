using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.MinilandPackets
{
    internal sealed class MinilandGameReqEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Session.GetPlayer().map is NosTayleGameServer.NosTale.Maps.SpecialMaps.Miniland && Event.valuesCount >= 1)
            {
                NosTayleGameServer.NosTale.Maps.SpecialMaps.Miniland miniland = (NosTayleGameServer.NosTale.Maps.SpecialMaps.Miniland)Session.GetPlayer().map;
                if (!miniland.GameAction(Session.GetPlayer(), Event.GetValues()))
                    Console.WriteLine(Event.dataBrute);
            }
        }
    }
}
