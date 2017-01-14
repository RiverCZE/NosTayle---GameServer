using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.PlayersPackets
{
    internal sealed class PlayerPulseEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Event.valuesCount == 2)
            {
                int newPulse = Convert.ToInt32(Event.GetValue(0));
                Session.GetPlayer().lastPulse += 60;
                DateTime receivePacket = DateTime.Now;
                TimeSpan tp = DateTime.Now.Subtract(Session.GetPlayer().lastPulseTime);
                if (tp.TotalSeconds >= 58 && tp.TotalSeconds <= 62 && newPulse == Session.GetPlayer().lastPulse)
                    Session.GetPlayer().lastPulseTime = receivePacket;
                else
                    Session.GetSock().CloseConnection();
            }
        }
    }
}
