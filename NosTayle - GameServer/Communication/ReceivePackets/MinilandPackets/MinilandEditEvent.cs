using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.NosTale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.MinilandPackets
{
    internal sealed class MinilandEditEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Event.valuesCount >= 1)
            {
                switch (Convert.ToInt32(Event.GetValue(0)))
                {
                    case 1:
                        Session.GetPlayer().miniLand.EditMoto(Event.dataBrute.Substring(9));
                        break;
                    case 2:
                        if (Convert.ToInt32(Event.GetValue(1)) >= 0 && Convert.ToInt32(Event.GetValue(1)) <= 2 && Convert.ToInt32(Event.GetValue(1)) != Session.GetPlayer().miniLand.statue)
                            Session.GetPlayer().miniLand.EditStatue(Convert.ToInt32(Event.GetValue(1)));
                        break;
                }
            }
        }
    }
}
