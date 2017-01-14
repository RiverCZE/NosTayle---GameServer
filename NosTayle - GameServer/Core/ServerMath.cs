using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Core
{
    public static class ServerMath
    {
        public static bool StringToBool(string str)
        {
            if (str == "1")
                return true;
            return false;
        }

        public static int ConvertDigniToIco(int digni)
        {
            if (digni < -800)
            {
                return 6;
            }
            if (digni < -600)
            {
                return 5;
            }
            if (digni < -400)
            {
                return 4;
            }
            if (digni < -200)
            {
                return 3;
            }
            if (digni < -100)
            {
                return 2;
            }
            return 1;
        }

        public static int ConvertReputToIco(int reput)
        {
            int ico = 0;
            if (reput <= 50)
                ico = 1;
            else if (reput >= 51 && reput <= 150)
                ico = 2;
            else if (reput >= 151 && reput <= 250)
                ico = 3;
            else if (reput >= 251 && reput <= 500)
                ico = 4;
            else if (reput >= 501 && reput <= 750)
                ico = 5;
            else if (reput >= 751 && reput <= 1000)
                ico = 6;
            else if (reput >= 1001 && reput <= 2250)
                ico = 7;
            else if (reput >= 2251 && reput <= 3500)
                ico = 8;
            else if (reput >= 3501 && reput <= 5000)
                ico = 9;
            else if (reput >= 5001 && reput <= 9500)
                ico = 10;
            if (reput >= 9501 && reput <= 19000)
                ico = 11;
            else if (reput >= 19001 && reput <= 25000)
                ico = 12;
            else if (reput >= 25001 && reput <= 40000)
                ico = 13;
            else if (reput >= 40001 && reput <= 60000)
                ico = 14;
            else if (reput >= 60001 && reput <= 85000)
                ico = 15;
            else if (reput >= 85001 && reput <= 115000)
                ico = 16;
            else if (reput >= 115001 && reput <= 150000)
                ico = 17;
            else if (reput >= 150001 && reput <= 190000)
                ico = 18;
            else if (reput >= 190001 && reput <= 235000)
                ico = 19;
            else if (reput >= 235001 && reput <= 285000)
                ico = 20;
            else if (reput >= 285001 && reput <= 300000)
                ico = 21;
            else if (reput >= 300001 && reput <= 500000)
                ico = 22;
            else if (reput >= 500001 && reput <= 1500000)
                ico = 23;
            else if (reput >= 1500001 && reput <= 2500000)
                ico = 24;
            else if (reput >= 2500001 && reput <= 3750000)
                ico = 25;
            else if (reput >= 3750001 && reput <= 5000000)
                ico = 26;
            else if (reput >= 5000001)
                ico = 27;
            return ico;
        }

        public static int Percent(int min, int max)
        {
            int per = 0;
            if (max != 0)
            {
                per = min * 100 / max;
                if (per == 0 && min != 0)
                    per = 1;
            }
            return per;
        }

        public static int RealHp(int userClass, int level)
        {
            int baseHp = 1;
            int baseInc = 1;
            switch (userClass)
            {
                case 0:
                    {
                        baseHp = 221;
                        baseInc = 17;
                        int gauss = 136;
                        int hp = (int)((Math.Pow(level + 15, 2.0) + (double)level + 15.0) / 2.0 - (double)gauss + (double)baseHp);
                        return hp;
                    }
                case 1:
                    {
                        int hp = 946;
                        baseInc = 85;
                        int i = 16;
                        while (i <= level)
                        {
                            if (i % 5 == 2)
                            {
                                hp += baseInc / 2;
                                baseInc += 2;
                            }
                            else
                            {
                                hp += baseInc;
                                baseInc += 4;
                            }
                            ++i;
                        }
                        return hp;
                    }
                case 2:
                    {
                        int hp = 680;
                        int inc = 35;
                        int i = 16;
                        while (i <= level)
                        {
                            hp += inc;
                            ++inc;
                            if (i % 10 == 1 || i % 10 == 5 || i % 10 == 8)
                            {
                                hp += inc;
                                ++inc;
                            }
                            ++i;
                        }
                        return hp;
                    }
                case 3:
                    {
                        baseHp = 550;
                        int gauss = 465;
                        int hp = (int)((Math.Pow(level + 15, 2.0) + (double)level + 15.0) / 2.0 - (double)gauss + (double)baseHp);
                        return hp;
                    }
            }
            return 1;
        }

        public static int RealMp(int userClass, int level)
        {
            int mp = 1;
            switch (userClass)
            {
                case 0:
                    mp = 60;
                    int mpI = 9;
                    for (int i = 1; i < level; i++)
                    {
                        mp += mpI;
                        if (i % 4 <= 1) { mpI++; }
                    }
                    break;
                case 1:
                    mp = 200;
                    for (int i = 1; i < level; i++) mp += (5 * level >> 3);
                    break;
                case 2:
                    mp = 300;
                    for (int i = 1; i < level; i++) mp += (7 * level >> 3);
                    break;
                case 3:
                    mp = 500;
                    for (int i = 1; i < level; i++) mp += (10 * level >> 3);
                    break;
            }
            return mp;
        }

        public static int Sqr(int number)
        {
            return number * number;
        }

        #region MapTools
        public static double GetDistance(int x, int y, int x2, int y2)
        {
            return System.Math.Sqrt(Sqr(x2 - x) + Sqr(y2 - y));
        }
        public static Point GetFuturStep(Point start, Point end, int steps)
        {
            Point futurPoint;
            int newX = start.X;
            int newY = start.Y;
            if (start.X < end.Y)
            {
                newX = start.X + (steps);
                if (newX > end.X)
                    newX = end.X;
            }
            else if (start.X > end.X)
            {
                newX = start.X - (steps);
                if (newX < end.X)
                    newX = end.X;
            }
            if (start.Y < end.Y)
            {
                newY = start.Y + (steps);
                if (newY > end.Y)
                    newY = end.Y;
            }
            else if (start.Y > end.Y)
            {
                newY = start.Y - (steps);
                if (newY < end.Y)
                    newY = end.Y;
            }
            futurPoint = new Point(newX, newY);
            return futurPoint;
        }
        #endregion
    }
}
