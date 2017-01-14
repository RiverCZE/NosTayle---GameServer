using NosTayleGameServer.NosTale.Entities;
using NosTayleGameServer.NosTale.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NosTayleGameServer.NosTale.Skills.SkillsManager;

namespace NosTayleGameServer.NosTale.Skills
{
    class Skill
    {
        internal int skillId;
        internal string skillName;
        internal UseSkill useSkill;
        internal string eff1;
        internal string[] attack_effs;
        internal int cells;
        internal double actionTime;
        internal double charge;
        internal int costMp;
        internal int element;
        internal int attack_type;
        internal int weaponId;
        internal int attack_upgrade;
        internal int element_upgrade;
        internal int cells2;
        internal int upCritic;
        internal int reducMp;
        internal int effectId;
        internal int specialEffect;
        internal int specialEffectDuration;
        internal List<SkillBuff> buffs;

        public Skill() { }

        public Skill(int skillId, string skillName, UseSkill useSkill, string eff1, string[] attack_effs, bool attackEffectToAll, int cells, int cells2, int costMp, double actionTime, double charge, int element, int attack_type, int weaponId, int attack_upgrade, int element_upgrade, int upCritic, int reducMp, int effectId, int specialEffect, int specialEffectDuration, List<SkillBuff> skillBuffs)
        {
            this.skillId = skillId;
            this.skillName = skillName;
            this.useSkill = useSkill;
            this.eff1 = eff1;
            this.attack_effs = attack_effs;
            this.cells = cells;
            this.cells2 = cells2;
            this.actionTime = actionTime;
            this.charge = charge;
            this.costMp = costMp;
            this.element = element;
            this.attack_type = attack_type;
            this.weaponId = weaponId;
            this.attack_upgrade = attack_upgrade;
            this.element_upgrade = element_upgrade;
            this.upCritic = upCritic;
            this.reducMp = reducMp;
            this.effectId = effectId;
            this.specialEffect = specialEffect;
            this.specialEffectDuration = specialEffectDuration;
            this.buffs = skillBuffs;
        }
    }
}
