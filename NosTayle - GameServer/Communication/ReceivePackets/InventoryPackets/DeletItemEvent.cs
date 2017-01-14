using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.NosTale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.InventoryPackets
{
    internal sealed class DeletItemEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (!Session.GetPlayer().inventory.isBlocked && Event.valuesCount == 2)
                Session.SendPacket(GlobalMessage.MakeDialog(String.Format("#b_i^{0}^{1}^1", Convert.ToInt32(Event.GetValue(0)), Convert.ToInt32(Event.GetValue(1))), "#b_i^Cancel",
                    GameServer.GetLanguage(Session.GetPlayer().languagePack, "message.inventory.confirm.delet")));
        }
    }
}
