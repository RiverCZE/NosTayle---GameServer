using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.NosTale;
using NosTayleGameServer.NosTale.Entities;
using NosTayleGameServer.NosTale.Entities.Players;
using NosTayleGameServer.NosTale.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.PlayersPackets
{
    internal sealed class PlayerAttackEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            try
            {
                if (Event.valuesCount >= 3)
                {
                    if (Session.GetPlayer().locomotion != null)
                    {
                        Session.GetPlayer().mtList = "";
                        ServerPacket nPacket = new ServerPacket(Outgoing.cancel);
                        nPacket.AppendInt(2);
                        nPacket.AppendString(Event.GetValue(2));
                        Session.SendPacket(nPacket);
                        return;
                    }
                    Player player = Session.GetPlayer();
                    if (!player.isDead && !player.inAction)
                    {
                        Entitie entitie = null;
                        switch (Event.GetValue(1))
                        {
                            case "1":
                                entitie = player.map.playersManagers.GetPlayerById(Convert.ToInt32(Event.GetValue(2)));
                                break;
                            case "2":
                                entitie = player.map.npcsManager.GetNpcById(Convert.ToInt32(Event.GetValue(2)));
                                break;
                            case "3":
                                entitie = player.map.monstersManager.GetNpcById(Convert.ToInt32(Event.GetValue(2)));
                                break;
                        }
                        int x = 0;
                        int y = 0;
                        if (Event.valuesCount == 5)
                        {
                            x = Convert.ToInt32(Event.GetValue(3));
                            y = Convert.ToInt32(Event.GetValue(4));
                        }

                        EntitieSkill skill = null;
                        if (player.spInUsing && player.sp != null && player.sp.spId != -1)
                        {
                            if (player.sp.skills.ContainsKey(Convert.ToInt32(Event.GetValue(0))))
                                if (entitie != null)
                                    skill = player.sp.skills[Convert.ToInt32(Event.GetValue(0))];

                        }
                        else
                        {
                            if (player.skills.ContainsKey(Convert.ToInt32(Event.GetValue(0))))
                                if (entitie != null)
                                    skill = player.skills[Convert.ToInt32(Event.GetValue(0))];
                        }

                        if (skill != null)
                        {
                            player.inAction = true;
                            if (DateTime.Now.Subtract(player.lastMorph).TotalSeconds < 4)
                            {
                                ServerPacket nPacket = new ServerPacket(Outgoing.cancel);
                                nPacket.AppendInt(2);
                                nPacket.AppendInt(entitie.id);
                                player.SendPacket(nPacket);
                                player.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(player.languagePack, "error.cantuseskill")));
                                player.inAction = false;
                                return;
                            }
                            if (skill.notCharge)
                            {
                                ServerPacket nPacket = new ServerPacket(Outgoing.cancel);
                                nPacket.AppendInt(2);
                                nPacket.AppendInt(entitie.id);
                                player.SendPacket(nPacket);
                                player.inAction = false;
                                return;
                            }
                            if (player.GetWeapon(skill.skillBase.weaponId) == null && skill.skillBase.attack_type == 2 || player.GetWeapon(skill.skillBase.weaponId).id == -1 && skill.skillBase.attack_type == 2)
                            {
                                ServerPacket nPacket = new ServerPacket(Outgoing.cancel);
                                nPacket.AppendInt(2);
                                nPacket.AppendInt(entitie.id);
                                player.SendPacket(nPacket);
                                player.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(player.languagePack, "error.noweapon")));
                                player.inAction = false;
                                return;
                            }
                            if (player.currentMp < skill.skillBase.costMp)
                            {
                                ServerPacket nPacket = new ServerPacket(Outgoing.cancel);
                                nPacket.AppendInt(2);
                                nPacket.AppendInt(entitie.id);
                                player.SendPacket(nPacket);
                                player.SendPacket(GlobalMessage.MakeMessage(0, 0, 10, GameServer.GetLanguage(player.languagePack, " error.skillmp")));
                                player.inAction = false;
                                return;
                            }
                            skill.skillBase.useSkill(player.map, player, entitie, skill, x, y);
                            player.inAction = false;
                            return;
                        }
                    }
                    Session.GetPlayer().mtList = "";
                    ServerPacket packet = new ServerPacket(Outgoing.cancel);
                    packet.AppendInt(2);
                    packet.AppendString(Event.GetValue(2));
                    Session.SendPacket(packet);
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }
    }
}
