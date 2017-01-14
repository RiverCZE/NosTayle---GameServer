using NosTayleGameServer.Core;
using NosTayleGameServer.NosTale.Entities;
using NosTayleGameServer.NosTale.Entities.Players;
using NosTayleGameServer.NosTale.Maps;
using NosTayleGameServer.NosTale.UpgradeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NosTayleGameServer.NosTale.Skills
{
    class EntitieSkill
    {
        internal Skill skillBase;
        internal int skillBaseId;
        internal static Random randomGenerator = new Random();
        internal bool notCharge;
        internal DateTime lastSkillUse;
        internal int lastAttackId;

        public EntitieSkill(int skillBaseId, Skill skill)
        {
            this.skillBaseId = skillBaseId;
            this.skillBase = skill;
            this.lastAttackId = 0;
        }

        public void Attack(Map map, Entitie sender, Entitie cible, bool skillFalse, bool firstCible = true, int x = 0, int y = 0, bool gotoUser = false)
        {
            lock (cible)
            {
                if (!skillFalse && !cible.isDead)
                {
                    bool critical = false;
                    int criticChance = (sender.GetCriticRate(this.skillBase.weaponId) + this.skillBase.upCritic <= 100 ? sender.GetCriticRate(this.skillBase.weaponId) + this.skillBase.upCritic : 100);
                    critical = (randomGenerator.Next(1, 101) <= criticChance ? true : false);
                    if (this.skillBase.attack_type == 3)
                        critical = false;
                    double attack_total = 0;
                    double attack_final = 0;
                    double attackBonus = (sender.GetAttUpgrade(this.skillBase.weaponId) >= 0 && sender.GetAttUpgrade(this.skillBase.weaponId) <= 10 ? ItemUpgradeManager.degatsUp[GetRealAttackUpgrade(sender.GetAttUpgrade(this.skillBase.weaponId), cible.GetDefUpgrade())] / 100.0 : 0);
                    double defenseBonus = (sender.GetAttUpgrade(this.skillBase.weaponId) >= 0 && sender.GetAttUpgrade(this.skillBase.weaponId) <= 10 ? ItemUpgradeManager.degatsUp[GetRealDefenseUpgrade(sender.GetAttUpgrade(this.skillBase.weaponId), cible.GetDefUpgrade())] / 100.0 : 0);
                    double degats = sender.RandomDegats(this.skillBase.weaponId);
                    double attack_base = this.skillBase.attack_upgrade + degats + (degats * attackBonus) + 15;
                    attack_base = attack_base - (cible.GetDef(this.skillBase.attack_type) * (1.0 + defenseBonus));
                    attack_final = (int)(critical ? (1.0 + (sender.GetCriticDamage(this.skillBase.weaponId) / 100.0)) * attack_base : attack_base);
                    if (attack_final <= 0)
                        attack_final = randomGenerator.Next(1, 5);
                    double element = 0;
                    if (sender.element_type > 0 && sender.element_type <= 4)
                    {
                        if (sender.element_type == this.skillBase.element)
                        {
                            element = this.skillBase.element_upgrade;
                            element += (attack_base + 100) * (sender.element / 100.0);
                            double cibleResistances = cible.GetResistance(this.skillBase.element);
                            element = element * (1.0 - ((1.0 - (cibleResistances / 100.0) > 0) ? cibleResistances / 100.0 : 0));
                            if (cible.element_type != 0)
                                element = element * (1.0 + (SkillsManager.GetBonusElement(this.skillBase.element, cible.element_type) / 100.0));
                            else
                                element = element * 1.3;
                        }
                    }
                    attack_total = attack_final + element + sender.GetMoral() - cible.GetMoral();
                    double hitrateProba = 0;
                    if (this.skillBase.attack_type != 3 && (sender.GetHitRate(this.skillBase.weaponId) + sender.GetMoral() - cible.GetMoral() + cible.GetDodge()) > 0)
                        hitrateProba = ((double)(sender.GetHitRate(this.skillBase.weaponId) + sender.GetMoral() - cible.GetMoral()) / (double)(sender.GetHitRate(this.skillBase.weaponId) + sender.GetMoral() - cible.GetMoral() + cible.GetDodge())) * 100.0;
                    else
                        hitrateProba = 100;
                    bool attackFail = false;
                    if (randomGenerator.Next(1, 101) > Math.Ceiling(hitrateProba))
                    {
                        attack_total = 0;
                        attackFail = true;
                    }
                    if (attack_total <= 0 && !attackFail)
                        attack_total = randomGenerator.Next(1, 5);
                    if (!cible.isDead)
                    {
                        if (cible.currentHp - (int)attack_total < 0)
                            cible.currentHp = 0;
                        else
                            cible.currentHp -= (int)attack_total;
                        sender.lastAttack = DateTime.Now;
                        cible.lastAttack = DateTime.Now;
                        if (cible.type == 1)
                            ((Player)cible).SendStats(false);
                        cible.rested = 0;
                        int touchType = GetToucheType(attackFail, critical, firstCible);
                        if (attackFail)
                        {
                            this.lastAttackId = 0;
                            map.SendAttackEntitie(sender.type, sender.id, cible.type, cible.id, this.skillBase.skillId, (int)this.skillBase.charge, this.skillBase.attack_effs[0], x, y, (cible.currentHp <= 0 ? 0 : 1), ServerMath.Percent(cible.currentHp, cible.maxHp), (int)attack_total, touchType);
                        }
                        else
                        {
                            if (gotoUser && cible.currentHp > 0)
                                this.EntitieGoToUser(map, sender, cible);
                            map.SendAttackEntitie(sender.type, sender.id, cible.type, cible.id, this.skillBase.skillId, (int)this.skillBase.charge, this.skillBase.attack_effs[lastAttackId], x, y, (cible.currentHp <= 0 ? 0 : 1), ServerMath.Percent(cible.currentHp, cible.maxHp), (int)attack_total, touchType);
                            this.lastAttackId++;
                            if (this.lastAttackId >= this.skillBase.attack_effs.Length)
                                this.lastAttackId = 0;
                        }
                        if (this.skillBase.effectId != 0)
                            map.SendEffect(cible.type, cible.id, this.skillBase.effectId);
                        if (cible.currentHp <= 0)
                            map.EntitieDead(sender, cible);
                    }
                    else
                        map.SendAttackEntitie(sender.type, sender.id, cible.type, cible.id, this.skillBase.skillId, (int)this.skillBase.charge, this.skillBase.attack_effs[lastAttackId], x, y, (cible.currentHp <= 0 ? 0 : 1), ServerMath.Percent(cible.currentHp, cible.maxHp), 0, 2);
                }
                else
                    map.SendAttackEntitie(sender.type, sender.id, cible.type, cible.id, this.skillBase.skillId, (int)this.skillBase.charge, this.skillBase.attack_effs[lastAttackId], x, y, (cible.currentHp <= 0 ? 0 : 1), ServerMath.Percent(cible.currentHp, cible.maxHp), 0, 2);
            }
        }

        public static int GetRealAttackUpgrade(int attackUpgrade, int defenseUpgrade)
        {
            return (attackUpgrade - defenseUpgrade > 0) ? attackUpgrade - defenseUpgrade : 0;
        }
        public static int GetRealDefenseUpgrade(int attackUpgrade, int defenseUpgrade)
        {
            return (defenseUpgrade - defenseUpgrade > 0) ? defenseUpgrade - defenseUpgrade : 0;
        }
        public static int GetToucheType(bool miss, bool critic, bool firstCible)
        {
            int toucheType = 0;
            if (firstCible)
            {
                if (miss)
                    toucheType = 4;
                else if (critic)
                    toucheType = 3;
                else
                    toucheType = 0;
            }
            else
            {
                if (miss)
                    toucheType = 7;
                else if (critic)
                    toucheType = 6;
                else
                    toucheType = 5;
            }
            return toucheType;
        }

        public int GetPos(int pX, int pY, int cX, int cY)
        {
            int pos = -1;
            if (pX == cX && pY == cY)
                return 0;
            if (pX == cX && pY < cY)
                pos = 0;
            else if (pX > cX && pY == cY)
                pos = 1;
            else if (pX == cX && pY > cY)
                pos = 2;
            else if (pX < cX && pY == cY)
                pos = 3;
            else if (pX < cX && pY < cY)
                pos = 4;
            else if (pX > cX && pY < cY)
                pos = 5;
            else if (pX > cX && pY > cY)
                pos = 6;
            else if (pX < cX && pY > cY)
                pos = 7;
            return pos;
        }

        public void EntitieGoToUser(Map map, Entitie sender, Entitie cible)
        {
            int pos = this.GetPos(sender.x, sender.y, cible.x, cible.y);
            if (pos == 0)
            {
                cible.x = sender.x;
                cible.y = sender.y + 1;
            }
            else if (pos == 1)
            {
                cible.x = sender.x - 1;
                cible.y = sender.y;
            }
            else if (pos == 2)
            {
                cible.x = sender.x;
                cible.y = sender.y - 1;
            }
            else if (pos == 3)
            {
                cible.x = sender.x + 1;
                cible.y = sender.y;
            }
            else if (pos == 4)
            {
                cible.x = sender.x + 1;
                cible.y = sender.y + 1;
            }
            else if (pos == 5)
            {
                cible.x = sender.x - 1;
                cible.y = sender.y + 1;
            }
            else if (pos == 6)
            {
                cible.x = sender.x - 1;
                cible.y = sender.y - 1;
            }
            else if (pos == 7)
            {
                cible.x = sender.x + 1;
                cible.y = sender.y - 1;
            }
            map.SendGuri(3, cible.type, cible.id, cible.x + " " + cible.y + " 3 8 2 -1");
        }
    }
}
