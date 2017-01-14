using NosTayleGameServer.NosTale.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Entities.Bases
{
    public class EntitieBase
    {
        internal int id, level_base, type, agressive, element_type, element, hp, mp, attack_type, attack_min, attack_max, attack_cells, hitrate, critic_rate, critic_degats, corpDef, distDef, magicDef, dodge, fire_res, water_res, light_res, dark_res, exp, exp_job;
        internal bool canCapture, canMoved, canBeDeplaced;
        internal string name;

        //EFFECTS
        internal bool spawnDie;
        internal List<string> spawnMobs;

        internal List<BaseDropItem> drops;

        public EntitieBase(int id, string name, int level_base, int type, int agressive, bool canCapture, bool canMoved, bool canBeDeplaced, int element_type, int element, int hp, int mp, int attack_type, int attack_min, int attack_max, int attack_cells, int hitrate, int critic_rate, int critic_degats, int corpDef, int distDef, int magicDef, int dodge, int fire_res, int water_res, int light_res, int dark_res, int exp, int exp_job, string effects, string drops)
        {
            this.id = id;
            this.name = name;
            this.level_base = level_base;
            this.type = type;
            this.agressive = agressive;
            this.canCapture = canCapture;
            this.canMoved = canMoved;
            this.canBeDeplaced = canBeDeplaced;
            this.element_type = element_type;
            this.element = element;
            this.hp = hp;
            this.mp = mp;
            this.attack_type = attack_type;
            this.attack_min = attack_min;
            this.attack_max = attack_max;
            this.attack_cells = attack_cells;
            this.hitrate = hitrate;
            this.critic_rate = critic_rate;
            this.critic_degats = critic_degats;
            this.corpDef = corpDef;
            this.distDef = distDef;
            this.magicDef = magicDef;
            this.dodge = dodge;
            this.fire_res = fire_res;
            this.water_res = water_res;
            this.light_res = light_res;
            this.dark_res = dark_res;
            this.exp = exp;
            this.exp_job = exp_job;
            this.SetEffects(effects);
            this.SetDrops(drops);
        }

        private void SetEffects(string effectString)
        {
            string[] effects = effectString.Split(';');
            foreach (string effect in effects)
            {
                switch (effect.Split(':')[0])
                {
                    case "spawnDie":
                        {
                            this.spawnDie = true;
                            string[] mobs = effect.Split(':')[1].Split('^');
                            this.spawnMobs = new List<string>();
                            foreach (string mob in mobs)
                                this.spawnMobs.Add(mob);
                        }
                        break;
                }
            }
        }

        private void SetDrops(string drops)
        {
            this.drops = new List<BaseDropItem>();
            string[] drop = drops.Split('^');
            foreach(string d in drop)
            {
                string[] infos = d.Split('.');
                if(infos.Length == 5)
                {
                    this.drops.Add(new BaseDropItem(infos[0], Convert.ToInt32(infos[1]), Convert.ToInt32(infos[2]), Convert.ToInt32(infos[3]), Convert.ToInt32(infos[4])));
                }
            }
        }
    }
}
