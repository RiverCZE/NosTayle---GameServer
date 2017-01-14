using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.Core;
using NosTayleGameServer.NosTale.Entities;
using NosTayleGameServer.NosTale.Entities.Players;
using NosTayleGameServer.NosTale.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Skills.SkillTypes
{
    class SpecialCibleSkill
    {
        internal static void UseSkill(Map map, Entitie sender, Entitie cible, EntitieSkill skill, int x, int y)
        {
            if (map.CanAttack(sender, cible) && !cible.isDead)
            {
                bool skillMiss = false;
                if (ServerMath.GetDistance(sender.x, sender.y, cible.x, cible.y) - 3 > skill.skillBase.cells || cible.isDead)
                    skillMiss = true;
                map.SendEntitieAttack(sender.type, sender.id, cible.type, cible.id, skill.skillBase.eff1, skill.skillBase.skillId);
                skill.lastSkillUse = DateTime.Now;
                Thread.Sleep((int)skill.skillBase.actionTime * 100);
                skill.notCharge = true;
                sender.currentMp -= skill.skillBase.costMp;
                if (sender.type == 1)
                    ((Player)sender).SendStats(false);
                skill.Attack(map, sender, cible, skillMiss, true, x, y, false);
                if (!skillMiss)
                {
                    try
                    {
                        if (sender.mtList.Length >= 1)
                        {
                            string[] mtList = sender.mtList.Split(' ');
                            if (Convert.ToInt32(mtList[0]) != 0)
                            {
                                for (int i = 1; i < Convert.ToInt32(mtList[0]) * 2; i++)
                                {
                                    int mType = Convert.ToInt32(mtList[i]);
                                    int mId = Convert.ToInt32(mtList[++i]);
                                    if (mType == 1)
                                    {
                                        Entitie mCible = map.playersManagers.GetPlayerById(mId);
                                        if (mCible != null)
                                        {
                                            if (map.CanAttack(sender, mCible))
                                                skill.Attack(map, sender, mCible, false, false, x, y, false);
                                        }
                                    }
                                    else if (mType == 3)
                                    {
                                        Entitie mCible = map.monstersManager.GetNpcById(mId);
                                        if (mCible != null)
                                        {
                                            if (map.CanAttack(sender, mCible))
                                                skill.Attack(map, sender, mCible, false, false, x, y, false);
                                        }
                                    }
                                }
                            }
                        }
                        sender.mtList = "";
                    }
                    catch
                    {
                    }
                }
            }
            if (sender.type == 1)
            {
                Player player = (Player)sender;
                ServerPacket packet = new ServerPacket(Outgoing.cancel);
                packet.AppendInt(2);
                packet.AppendInt(cible.id);
                player.SendPacket(packet);
            }
        }
    }
}
