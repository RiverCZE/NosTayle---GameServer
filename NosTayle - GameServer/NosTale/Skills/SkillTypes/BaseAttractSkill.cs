using NosTayleGameServer.Core;
using NosTayleGameServer.NosTale.Entities;
using NosTayleGameServer.NosTale.Entities.Npcs;
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
    class BaseAttractSkill
    {
        internal static void UseSkill(Map map, Entitie sender, Entitie cible, EntitieSkill skill, int x, int y)
        {
            map.SendEntitieAttack(sender.type, sender.id, sender.type, sender.id, skill.skillBase.eff1, skill.skillBase.skillId);
            skill.lastSkillUse = DateTime.Now;
            Thread.Sleep((int)skill.skillBase.actionTime * 100);
            skill.notCharge = true;
            sender.currentMp -= skill.skillBase.costMp;
            if (sender.type == 1)
                ((Player)sender).SendStats(false);
            List<Player> usersMoved = new List<Player>();
            List<Npc> monstersMoved = new List<Npc>();
            List<Player> players = map.playersManagers.GetPlayersList().FindAll(p => (map.CanAttack(sender, p) || sender == p) && ServerMath.GetDistance(sender.x, sender.y, p.x, p.y) <= skill.skillBase.cells && !p.isDead);
            foreach (Player mCible in players)
            {
                if (mCible != sender)
                {
                    map.SendAttackEntitie(sender.type, sender.id, mCible.type, mCible.id, skill.skillBase.skillId, (int)skill.skillBase.charge, skill.skillBase.attack_effs[0], x, y, (mCible.currentHp <= 0 ? 0 : 1), ServerMath.Percent(mCible.currentHp, mCible.maxHp), 0, 5);
                    map.SendEffect(mCible.type, mCible.id, skill.skillBase.effectId);
                    usersMoved.Add(mCible);
                }
                else
                    map.SendAttackEntitie(sender.type, sender.id, sender.type, mCible.id, skill.skillBase.skillId, (int)skill.skillBase.charge, skill.skillBase.attack_effs[0], x, y, (mCible.currentHp <= 0 ? 0 : 1), ServerMath.Percent(mCible.currentHp, mCible.maxHp), 0, -2);
            }
            List<Npc> npcs = map.monstersManager.entitieList.FindAll(p => (map.CanAttack(sender, p) || sender == p) && ServerMath.GetDistance(sender.x, sender.y, p.x, p.y) <= skill.skillBase.cells && !p.isDead);
            foreach (Npc mCible in npcs)
            {
                if (mCible != sender)
                {
                    map.SendAttackEntitie(sender.type, sender.id, mCible.type, mCible.id, skill.skillBase.skillId, (int)skill.skillBase.charge, skill.skillBase.attack_effs[0], x, y, (mCible.currentHp <= 0 ? 0 : 1), ServerMath.Percent(mCible.currentHp, mCible.maxHp), 0, 5);
                    map.SendEffect(mCible.type, mCible.id, skill.skillBase.effectId);
                    monstersMoved.Add(mCible);
                }
                else
                    map.SendAttackEntitie(sender.type, sender.id, mCible.type, mCible.id, skill.skillBase.skillId, (int)skill.skillBase.charge, skill.skillBase.attack_effs[0], x, y, (mCible.currentHp <= 0 ? 0 : 1), ServerMath.Percent(mCible.currentHp, mCible.maxHp), 0, -2);
            }
            foreach (Player p in usersMoved)
                if (p.map == map)
                    skill.EntitieGoToUser(map, sender, p);
            foreach (Npc m in monstersMoved)
                skill.EntitieGoToUser(map, sender, m);
        }
    }
}
