using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Skills
{
    class SkillBuff
    {
        internal int rate;
        internal int buffId;

        public SkillBuff(int rate, int buffId)
        {
            this.rate = rate;
            this.buffId = buffId;
        }
    }
}
