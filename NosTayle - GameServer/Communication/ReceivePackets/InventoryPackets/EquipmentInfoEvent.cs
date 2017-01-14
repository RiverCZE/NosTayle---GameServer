using NosTayleGameServer.NosTale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.InventoryPackets
{
    internal sealed class EquipmentInfoEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Event.valuesCount == 2)
            {
                if (Event.GetValue(0) == "0")
                    Session.GetPlayer().LookEquipment(Convert.ToInt32(Event.GetValue(1)));
                else if (Event.GetValue(0) == "2")
                    GameServer.GetItemsManager().GetItemInfo(Session, Convert.ToInt32(Event.GetValue(1)));
                else
                    Session.GetPlayer().inventory.GetItemInfo(Session.GetPlayer(), Convert.ToInt32(Event.GetValue(0)), Convert.ToInt32(Event.GetValue(1)));
            }
        }
    }
}
