using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.Core;
using NosTayleGameServer.NosTale.Buffs;
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
    class BuffSkill
    {
        internal static void UseSkill(Map map, Entitie sender, Entitie cible, EntitieSkill skill, int x, int y)
        {
            if (map.CanBuff(sender, cible))
            {
                map.SendEntitieAttack(sender.type, sender.id, sender.type, sender.id, skill.skillBase.eff1, skill.skillBase.skillId);
                skill.lastSkillUse = DateTime.Now;
                Thread.Sleep((int)skill.skillBase.actionTime * 100);
                skill.notCharge = true;
                sender.currentMp -= skill.skillBase.costMp;
                if (sender.type == 1)
                    ((Player)sender).SendStats(false);
                try
                {
                    foreach (SkillBuff sBuff in skill.skillBase.buffs)
                    {
                        if (BuffsManager.BuffOk(sBuff.rate))
                            cible.AddBuff(map, new PersonalBuff(BuffsManager.buffs[sBuff.buffId], sender.level, DateTime.Now));
                    }
                } catch { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("No Buff for skill id: {0}", skill.skillBaseId); Console.ForegroundColor = ConsoleColor.White; }
                map.SendAttackEntitie(sender.type, sender.id, sender.type, sender.id, skill.skillBase.skillId, (int)skill.skillBase.charge, skill.skillBase.attack_effs[0], x, y, (sender.currentHp <= 0 ? 0 : 1), ServerMath.Percent(sender.currentHp, sender.maxHp), 0, -2);
            }
            else if (sender.type == 1)
            {
                ServerPacket packet = new ServerPacket(Communication.Headers.Outgoing.cancel);
                packet.AppendInt(2);
                packet.AppendInt(cible.id);
                ((Player)sender).SendPacket(packet);
            }
        }
    }
}
