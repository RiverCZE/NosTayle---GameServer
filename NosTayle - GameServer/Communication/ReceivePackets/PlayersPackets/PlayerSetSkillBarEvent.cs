using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.PlayersPackets
{
    internal sealed class PlayerSetSkillBarEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Event.valuesCount == 3 || Event.valuesCount == 5)
                if (Event.valuesCount == 5 && (Event.GetValue(0) == "0" || Event.GetValue(0) == "1" || Event.GetValue(0) == "2"))
                    if (Event.GetValue(0) == "2")
                        if (Session.GetPlayer().spInUsing)
                            Session.GetPlayer().sp.skillBar.MoveSBar(Session.GetPlayer(), Convert.ToInt32(Event.GetValue(1)), Convert.ToInt32(Event.GetValue(2)), Convert.ToInt32(Event.GetValue(3)), Convert.ToInt32(Event.GetValue(4)));
                        else
                            Session.GetPlayer().skillBar.MoveSBar(Session.GetPlayer(), Convert.ToInt32(Event.GetValue(1)), Convert.ToInt32(Event.GetValue(2)), Convert.ToInt32(Event.GetValue(3)), Convert.ToInt32(Event.GetValue(4)));
                    else
                        if (Session.GetPlayer().spInUsing)
                            Session.GetPlayer().sp.skillBar.AddSbar(Session.GetPlayer(), Convert.ToInt32(Event.GetValue(0)), Convert.ToInt32(Event.GetValue(1)), Convert.ToInt32(Event.GetValue(2)), Convert.ToInt32(Event.GetValue(3)), Convert.ToInt32(Event.GetValue(4)));
                        else
                            Session.GetPlayer().skillBar.AddSbar(Session.GetPlayer(), Convert.ToInt32(Event.GetValue(0)), Convert.ToInt32(Event.GetValue(1)), Convert.ToInt32(Event.GetValue(2)), Convert.ToInt32(Event.GetValue(3)), Convert.ToInt32(Event.GetValue(4)));
                else if (Event.GetValue(0) == "3")
                    if (Session.GetPlayer().spInUsing)
                        Session.GetPlayer().sp.skillBar.DelSBar(Session.GetPlayer(), Convert.ToInt32(Event.GetValue(1)), Convert.ToInt32(Event.GetValue(2)));
                    else
                        Session.GetPlayer().skillBar.DelSBar(Session.GetPlayer(), Convert.ToInt32(Event.GetValue(1)), Convert.ToInt32(Event.GetValue(2)));
        }
    }
}
