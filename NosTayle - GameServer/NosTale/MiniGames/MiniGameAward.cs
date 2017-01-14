using NosTayleGameServer.NosTale.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.MiniGames
{
    class MiniGameAward
    {
        internal int amount;
        internal ItemBase item;

        public MiniGameAward(int amount, ItemBase item)
        {
            this.amount = amount;
            this.item = item;
        }
    }
}
