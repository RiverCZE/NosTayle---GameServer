using NosTayleGameServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Levels
{
    class LevelsManager
    {
        public Dictionary<int, double> Levels = new Dictionary<int, double>();
        public Dictionary<int, double> jobLevels = new Dictionary<int, double>();

        public LevelsManager(int lvlMax, int jobLvlMax)
        {
            WriteConsole.WriteStructure("GAME", "Load levels...");
            double exp = 360;
            int lvl = 1;
            for (lvl = 1; lvl <= lvlMax; lvl++)
            {
                if (lvl != 1)
                    exp += (int)(2 * lvl * 578 * (lvl / 2) * lvl / 9.40);
                Levels.Add(lvl, exp);
            }
            Console.WriteLine(Levels.Count + " levels loads!");
            WriteConsole.WriteStructure("GAME", "Load job levels...");
            exp = 2200;
            lvl = 1;
            for (lvl = 1; lvl <= jobLvlMax; lvl++)
            {
                if (lvl != 1)
                    exp += (int)(2 * lvl * 120.4);
                jobLevels.Add(lvl, exp);
            }
            Console.WriteLine(jobLevels.Count + " job levels loads!");
        }
    }
}
