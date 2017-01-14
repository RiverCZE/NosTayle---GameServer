using NosTayleGameServer.NosTale.Buffs;
using NosTayleGameServer.NosTale.Entities.Players;
using NosTayleGameServer.NosTale.Maps;
using NosTayleGameServer.NosTale.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Entities
{
    public class Entitie
    {
        internal int id;
        internal int type;
        internal string name;
        internal int level;
        internal int attackUpgrade1;
        internal int attackUpgrade2;
        internal int defenseUpgrade;
        internal int mapId;
        internal int x;
        internal int y;
        internal int direction;
        internal int rested = 0;
        internal int currentHp;
        internal int currentMp;
        internal int maxHp;
        internal int maxMp;
        internal int exp;
        internal int expJob;
        internal int element_type;
        internal int element;
        internal int fireResist;
        internal int waterResist;
        internal int lightResist;
        internal int darknessResist;
        internal int damageMin1;
        internal int damageMax1;
        internal int hitRate1;
        internal int criticRate1;
        internal int criticDamage1;
        internal int damageMin2;
        internal int damageMax2;
        internal int hitRate2;
        internal int criticRate2;
        internal int criticDamage2;
        internal int corpDef;
        internal int distDef;
        internal int magicDef;
        internal int dodge;
        internal bool isDead;
        internal DateTime deadTime;
        internal bool inAction;
        internal List<Entitie> cibles;
        internal bool deleted;
        internal DateTime lastAttack;
        internal string mtList;

        internal BuffList buffs = new BuffList();

        internal DateTime spawnTime;

        public int RandomDegats(int weapon = 1)
        {
            if (weapon == 1)
                return EntitieSkill.randomGenerator.Next(this.damageMin1, this.damageMax1);
            return EntitieSkill.randomGenerator.Next(this.damageMin2, this.damageMax2);
        }

        public int GetHitRate(int weapon = 1)
        {
            if (weapon == 1)
                return this.hitRate1;
            return this.hitRate2;
        }

        public int GetCriticRate(int weapon = 1)
        {
            if (weapon == 1)
                return this.criticRate1;
            return this.criticRate2;
        }

        public int GetCriticDamage(int weapon = 1)
        {
            if (weapon == 1)
                return this.criticDamage1;
            return this.criticDamage2;
        }

        public int GetAttUpgrade(int weapon = 1)
        {
            if (weapon == 1)
                return this.attackUpgrade1;
            return this.attackUpgrade2;
        }

        public int GetDefUpgrade(int weapon = 1)
        {
            return defenseUpgrade;
        }

        public int GetDef(int type)
        {
            switch (type)
            {
                case 2:
                    return this.distDef;
                case 3:
                    return this.magicDef;
                default:
                    return this.corpDef;
            }
        }

        public int GetDodge()
        {
            return this.dodge;
        }

        public int GetResistance(int rId)
        {
            switch (rId)
            {
                case 1:
                    return this.fireResist;
                case 2:
                    return this.waterResist;
                case 3:
                    return this.lightResist;
                case 4:
                    return this.darknessResist;
                default:
                    return 0;

            }
        }

        virtual public int GetExpWin()
        {
            return 0;
        }

        virtual public int GetExpJobWin()
        {
            return this.level * 10;
        }

        public int GetMoral()
        {
            return this.level;
        }

        public virtual void AddBuff(Map map, PersonalBuff buff)
        {
            lock (this.buffs)
            {
                if (this.buffs.Exists(s => s.baseBuff.id == buff.baseBuff.id))
                    this.buffs.Remove(this.buffs.FirstOrDefault(s => s.baseBuff.id == buff.baseBuff.id));
                else if (buff.baseBuff.viewForm)
                    map.SendGuri(0, this.type, this.id, buff.baseBuff.guriStart);
                this.buffs.Add(buff);
            }
            map.SendMap(buff.GetPacket(this.type, this.id, false));
        }

        public Entitie() { }
    }
}
