using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.UpgradeSystem
{
    class ItemUpgrade
    {
        internal int pWin;
        internal int pFix;
        internal int pDestroy;

        internal int gold;
        internal int specialItemId;
        internal int cellaCount;
        internal int specialCount;

        public ItemUpgrade(int upgrade)
        {
            switch (upgrade)
            {
                case 1:
                    {
                        this.pWin = 100;
                        this.pFix = 0;
                        this.pDestroy = 0;
                        this.gold = 500;
                        this.specialItemId = 1015;
                        this.cellaCount = 20;
                        this.specialCount = 1;
                    }
                    break;
                case 2:
                    {
                        this.pWin = 100;
                        this.pFix = 0;
                        this.pDestroy = 0;
                        this.gold = 1500;
                        this.specialItemId = 1015;
                        this.cellaCount = 50;
                        this.specialCount = 1;
                    }
                    break;
                case 3:
                    {
                        this.pWin = 90;
                        this.pFix = 10;
                        this.pDestroy = 0;
                        this.gold = 3000;
                        this.specialItemId = 1015;
                        this.cellaCount = 80;
                        this.specialCount = 2;
                    }
                    break;
                case 4:
                    {
                        this.pWin = 80;
                        this.pFix = 15;
                        this.pDestroy = 5;
                        this.gold = 10000;
                        this.specialItemId = 1015;
                        this.cellaCount = 120;
                        this.specialCount = 2;
                    }
                    break;
                case 5:
                    {
                        this.pWin = 60;
                        this.pFix = 20;
                        this.pDestroy = 20;
                        this.gold = 30000;
                        this.specialItemId = 1015;
                        this.cellaCount = 160;
                        this.specialCount = 3;
                    }
                    break;
                case 6:
                    {
                        this.pWin = 40;
                        this.pFix = 20;
                        this.pDestroy = 40;
                        this.gold = 80000;
                        this.specialItemId = 1016;
                        this.cellaCount = 220;
                        this.specialCount = 1;
                    }
                    break;
                case 7:
                    {
                        this.pWin = 20;
                        this.pFix = 20;
                        this.pDestroy = 60;
                        this.gold = 150000;
                        this.specialItemId = 1016;
                        this.cellaCount = 280;
                        this.specialCount = 1;
                    }
                    break;
                case 8:
                    {
                        this.pWin = 10;
                        this.pFix = 20;
                        this.pDestroy = 70;
                        this.gold = 400000;
                        this.specialItemId = 1016;
                        this.cellaCount = 380;
                        this.specialCount = 2;
                    }
                    break;
                case 9:
                    {
                        this.pWin = 5;
                        this.pFix = 20;
                        this.pDestroy = 75;
                        this.gold = 700000;
                        this.specialItemId = 1016;
                        this.cellaCount = 480;
                        this.specialCount = 2;
                    }
                    break;
                case 10:
                    {
                        this.pWin = 1;
                        this.pFix = 20;
                        this.pDestroy = 79;
                        this.gold = 1000000;
                        this.specialItemId = 1016;
                        this.cellaCount = 600;
                        this.specialCount = 3;
                    }
                    break;
            }
        }
    }
}
