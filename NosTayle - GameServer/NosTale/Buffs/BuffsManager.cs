using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Buffs
{
    static class BuffsManager
    {
        internal static Dictionary<int, Buff> buffs;
        internal static Random randomBuff = new Random();

        internal static bool BuffOk(int rate)
        {
            lock (randomBuff)
            {
                return randomBuff.Next(0, 100) < rate ? true : false;
            }
        }

        static BuffsManager()
        {
            BuffsManager.buffs = new Dictionary<int, Buff>();

            BuffsManager.buffs.Add(74, new Buff(74, "Œil du faucon", true, 3000, 0, "", "", false));
            BuffsManager.buffs.Add(75, new Buff(75, "Marcheur de vent", true, 4200, 2, "2 60", "0 60", true));
        }
    }
}
