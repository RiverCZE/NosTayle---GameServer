using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Buffs
{
    public class Buff
    {
        internal int id;
        internal string name;
        internal bool isGood;
        internal int time;
        internal int speedUp;
        internal string guriStart, guriEnd;
        internal bool viewForm;

        public Buff(int id, string name, bool isGood, int time, int speedUp, string guriStart, string guriEnd, bool viewForm)
        {
            this.id = id;
            this.name = name;
            this.isGood = isGood;
            this.time = time;
            this.speedUp = speedUp;
            this.guriStart = guriStart;
            this.guriEnd = guriEnd;
            this.viewForm = viewForm;
        }
    }
}
