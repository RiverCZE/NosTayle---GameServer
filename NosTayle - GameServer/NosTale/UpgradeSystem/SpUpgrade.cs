using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.UpgradeSystem
{
    class SpUpgrade
    {
        internal int pWin;
        internal int pFail;
        internal int pDestroy;

        internal int points;

        internal int gold;
        internal int specialItemId;
        internal int specialItemId2;
        internal int penCount;
        internal int fMoonCount;
        internal int specialCount;

        public SpUpgrade(int upgrade)
        {
            switch (upgrade)
            {
                case 1:
                    {
                        this.pWin = 80;
                        this.pFail = 20;
                        this.pDestroy = 0;
                        this.points = 5;
                        this.gold = 200000;
                        this.specialItemId = 2283;
                        this.specialItemId2 = 2511;
                        this.fMoonCount = 1;
                        this.penCount = 3;
                        this.specialCount = 2;
                    }
                    break;
                case 2:
                    {
                        this.pWin = 75;
                        this.pFail = 25;
                        this.pDestroy = 0;
                        this.points = 10;
                        this.gold = 200000;
                        this.specialItemId = 2283;
                        this.specialItemId2 = 2511;
                        this.fMoonCount = 3;
                        this.penCount = 5;
                        this.specialCount = 4;
                    }
                    break;
                case 3:
                    {
                        this.pWin = 70;
                        this.pFail = 25;
                        this.pDestroy = 5;
                        this.points = 15;
                        this.gold = 200000;
                        this.specialItemId = 2283;
                        this.specialItemId2 = 2511;
                        this.fMoonCount = 5;
                        this.penCount = 8;
                        this.specialCount = 6;
                    }
                    break;
                case 4:
                    {
                        this.pWin = 60;
                        this.pFail = 30;
                        this.pDestroy = 10;
                        this.points = 20;
                        this.gold = 200000;
                        this.specialItemId = 2283;
                        this.specialItemId2 = 2511;
                        this.fMoonCount = 7;
                        this.penCount = 10;
                        this.specialCount = 8;
                    }
                    break;
                case 5:
                    {
                        this.pWin = 50;
                        this.pFail = 35;
                        this.pDestroy = 15;
                        this.points = 28;
                        this.gold = 200000;
                        this.specialItemId = 2283;
                        this.specialItemId2 = 2511;
                        this.fMoonCount = 10;
                        this.penCount = 15;
                        this.specialCount = 10;
                    }
                    break;
                case 6:
                    {
                        this.pWin = 40;
                        this.pFail = 40;
                        this.pDestroy = 20;
                        this.points = 36;
                        this.gold = 500000;
                        this.specialItemId = 2284;
                        this.specialItemId2 = 2512;
                        this.fMoonCount = 12;
                        this.penCount = 20;
                        this.specialCount = 1;
                    }
                    break;
                case 7:
                    {
                        this.pWin = 35;
                        this.pFail = 40;
                        this.pDestroy = 25;
                        this.points = 46;
                        this.gold = 500000;
                        this.specialItemId = 2284;
                        this.specialItemId2 = 2512;
                        this.fMoonCount = 14;
                        this.penCount = 25;
                        this.specialCount = 2;
                    }
                    break;
                case 8:
                    {
                        this.pWin = 30;
                        this.pFail = 40;
                        this.pDestroy = 30;
                        this.points = 56;
                        this.gold = 500000;
                        this.specialItemId = 2284;
                        this.specialItemId2 = 2512;
                        this.fMoonCount = 16;
                        this.penCount = 30;
                        this.specialCount = 3;
                    }
                    break;
                case 9:
                    {
                        this.pWin = 25;
                        this.pFail = 40;
                        this.pDestroy = 35;
                        this.points = 68;
                        this.gold = 500000;
                        this.specialItemId = 2284;
                        this.specialItemId2 = 2512;
                        this.fMoonCount = 18;
                        this.penCount = 35;
                        this.specialCount = 4;
                    }
                    break;
                case 10:
                    {
                        this.pWin = 20;
                        this.pFail = 40;
                        this.pDestroy = 40;
                        this.points = 80;
                        this.gold = 500000;
                        this.specialItemId = 2284;
                        this.specialItemId2 = 2512;
                        this.fMoonCount = 20;
                        this.penCount = 40;
                        this.specialCount = 5;
                    }
                    break;
                case 11:
                    {
                        this.pWin = 10;
                        this.pFail = 45;
                        this.pDestroy = 45;
                        this.points = 95;
                        this.gold = 1000000;
                        this.specialItemId = 2285;
                        this.specialItemId2 = 2513;
                        this.fMoonCount = 22;
                        this.penCount = 45;
                        this.specialCount = 1;
                    }
                    break;
                case 12:
                    {
                        this.pWin = 7;
                        this.pFail = 43;
                        this.pDestroy = 50;
                        this.points = 110;
                        this.gold = 1000000;
                        this.specialItemId = 2285;
                        this.specialItemId2 = 2513;
                        this.fMoonCount = 24;
                        this.penCount = 50;
                        this.specialCount = 2;
                    }
                    break;
                case 13:
                    {
                        this.pWin = 5;
                        this.pFail = 40;
                        this.pDestroy = 55;
                        this.points = 128;
                        this.gold = 1000000;
                        this.specialItemId = 2285;
                        this.specialItemId2 = 2513;
                        this.fMoonCount = 26;
                        this.penCount = 55;
                        this.specialCount = 3;
                    }
                    break;
                case 14:
                    {
                        this.pWin = 3;
                        this.pFail = 37;
                        this.pDestroy = 60;
                        this.points = 148;
                        this.gold = 1000000;
                        this.specialItemId = 2285;
                        this.specialItemId2 = 2513;
                        this.fMoonCount = 28;
                        this.penCount = 60;
                        this.specialCount = 4;
                    }
                    break;
                case 15:
                    {
                        this.pWin = 1;
                        this.pFail = 29;
                        this.pDestroy = 70;
                        this.points = 173;
                        this.gold = 1000000;
                        this.specialItemId = 2285;
                        this.specialItemId2 = 2513;
                        this.fMoonCount = 30;
                        this.penCount = 70;
                        this.specialCount = 5;
                    }
                    break;
            }
        }
    }
}
