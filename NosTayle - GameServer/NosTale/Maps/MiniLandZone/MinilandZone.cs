using NosTayleGameServer.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Maps.MiniLandZone
{
    class MinilandZone
    {
        internal Dictionary<string, Point> points = new Dictionary<string, Point>();

        public MinilandZone(int zone)
        {
            switch (zone)
            {
                case 1:
                    {
                        int pACX = 2;
                        int pBDX = 17;
                        int pABY = 6;
                        int pCDY = 6;

                        int pX = 2;
                        while (pX >= pACX && pX <= pBDX)
                        {
                            int pY = 6;
                            while (pY >= pABY && pY <= pCDY)
                            {
                                string pKey = pX + ";" + pY;
                                this.points.Add(pKey, new Point(pX, pY));
                                pY++;
                            }
                            pX++;
                        }

                        pACX = 2;
                        pBDX = 17;
                        pABY = 9;
                        pCDY = 9;

                        pX = 2;
                        while (pX >= pACX && pX <= pBDX)
                        {
                            int pY = 9;
                            while (pY >= pABY && pY <= pCDY)
                            {
                                string pKey = pX + ";" + pY;
                                this.points.Add(pKey, new Point(pX, pY));
                                pY++;
                            }
                            pX++;
                        }

                        pACX = 2;
                        pBDX = 18;
                        pABY = 10;
                        pCDY = 15;

                        pX = 2;
                        while (pX >= pACX && pX <= pBDX)
                        {
                            int pY = 10;
                            while (pY >= pABY && pY <= pCDY)
                            {
                                string pKey = pX + ";" + pY;
                                this.points.Add(pKey, new Point(pX, pY));
                                pY++;
                            }
                            pX++;
                        }

                        pACX = 2;
                        pBDX = 18;
                        pABY = 2;
                        pCDY = 5;

                        pX = 2;
                        while (pX >= pACX && pX <= pBDX)
                        {
                            int pY = 2;
                            while (pY >= pABY && pY <= pCDY)
                            {
                                string pKey = pX + ";" + pY;
                                this.points.Add(pKey, new Point(pX, pY));
                                pY++;
                            }
                            pX++;
                        }
                    }
                    break;
                case 2:
                    {
                        int pACX = 21;
                        int pBDX = 33;
                        int pABY = 2;
                        int pCDY = 15;

                        int pX = 21;
                        while (pX >= pACX && pX <= pBDX)
                        {
                            int pY = 2;
                            while (pY >= pABY && pY <= pCDY)
                            {
                                string pKey = pX + ";" + pY;
                                this.points.Add(pKey, new Point(pX, pY));
                                pY++;
                            }
                            pX++;
                        }
                    }
                    break;
                case 3:
                    {
                        int pACX = 2;
                        int pBDX = 11;
                        int pABY = 18;
                        int pCDY = 29;

                        int pX = 2;
                        while (pX >= pACX && pX <= pBDX)
                        {
                            int pY = 18;
                            while (pY >= pABY && pY <= pCDY)
                            {
                                string pKey = pX + ";" + pY;
                                this.points.Add(pKey, new Point(pX, pY));
                                pY++;
                            }
                            pX++;
                        }
                        pACX = 12;
                        pBDX = 15;
                        pABY = 19;
                        pCDY = 29;

                        pX = 12;
                        while (pX >= pACX && pX <= pBDX)
                        {
                            int pY = 19;
                            while (pY >= pABY && pY <= pCDY)
                            {
                                string pKey = pX + ";" + pY;
                                this.points.Add(pKey, new Point(pX, pY));
                                pY++;
                            }
                            pX++;
                        }
                        pACX = 16;
                        pBDX = 26;
                        pABY = 18;
                        pCDY = 29;

                        pX = 16;
                        while (pX >= pACX && pX <= pBDX)
                        {
                            int pY = 18;
                            while (pY >= pABY && pY <= pCDY)
                            {
                                string pKey = pX + ";" + pY;
                                this.points.Add(pKey, new Point(pX, pY));
                                pY++;
                            }
                            pX++;
                        }
                        pACX = 27;
                        pBDX = 30;
                        pABY = 19;
                        pCDY = 29;

                        pX = 27;
                        while (pX >= pACX && pX <= pBDX)
                        {
                            int pY = 19;
                            while (pY >= pABY && pY <= pCDY)
                            {
                                string pKey = pX + ";" + pY;
                                this.points.Add(pKey, new Point(pX, pY));
                                pY++;
                            }
                            pX++;
                        }
                        pACX = 31;
                        pBDX = 33;
                        pABY = 18;
                        pCDY = 29;

                        pX = 31;
                        while (pX >= pACX && pX <= pBDX)
                        {
                            int pY = 18;
                            while (pY >= pABY && pY <= pCDY)
                            {
                                string pKey = pX + ";" + pY;
                                this.points.Add(pKey, new Point(pX, pY));
                                pY++;
                            }
                            pX++;
                        }
                    }
                    break;
            }
        }
    }
}
