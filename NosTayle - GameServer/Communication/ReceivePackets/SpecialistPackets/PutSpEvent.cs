using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.NosTale;
using NosTayleGameServer.NosTale.Entities.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.SpecialistPackets
{
    internal sealed class PutSpEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            Player sender = Session.GetPlayer();
            if (Event.valuesCount >= 1)
            {
                switch (Event.GetValue(0))
                {
                    case "0":
                        if (!sender.spInUsing)
                        {
                            if (sender.sp.spId != -1)
                            {
                                /*if (this.user.player.getSkillsNotCharge())
                                {
                                    this.user.send(Messages.MakeAlert(0, "Tu ne peux te transformer qu'après que les temps d'attente des compétences aient expiré."));
                                    return;
                                }*/
                                if (DateTime.Now.Subtract(sender.lastSpWaitUpdate).TotalSeconds >= sender.spWaitSecond)
                                {
                                    sender.SendPacket(GlobalMessage.MakeDelay(5000, 3, "#sl^1"));
                                    sender.map.SendGuri(2, 1, sender.id, "");
                                }
                                else
                                    sender.SendPacket(GlobalMessage.MakeAlert(0, String.Format(GameServer.GetLanguage(sender.languagePack, "error.sp.cantmorph"), Math.Round(sender.spWaitSecond - DateTime.Now.Subtract(sender.lastSpWaitUpdate).TotalSeconds))));
                            }
                            else
                                sender.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(sender.languagePack, "error.sp.notcard")));
                        }
                        else
                        {
                            if (sender.inAction)
                                return;
                            sender.PutSp();
                        }
                        break;
                }
            }
        }
    }
}
