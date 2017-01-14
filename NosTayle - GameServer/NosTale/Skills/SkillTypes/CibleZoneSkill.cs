using NosTayleGameServer.Communication.Messages;
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
    class CibleZoneSkill
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
                skill.Attack(map, sender, cible, skillMiss, true);
                List<Player> players = map.playersManagers.GetPlayersList().FindAll(p => map.CanAttack(sender, p) && p != sender && p != cible && ServerMath.GetDistance(cible.x, cible.y, p.x, p.y) - 1 <= skill.skillBase.cells2 && !p.isDead);
                foreach (Player mCible in players)
                    skill.Attack(map, sender, mCible, false, false);
                List<Npc> monsters = map.monstersManager.GetNpcs().FindAll(p => map.CanAttack(sender, p) && ServerMath.GetDistance(cible.x, cible.y, p.x, p.y) - 1 <= skill.skillBase.cells2 && !p.isDead);
                foreach (Entitie mCible in monsters)
                    skill.Attack(map, sender, mCible, false, false);
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
