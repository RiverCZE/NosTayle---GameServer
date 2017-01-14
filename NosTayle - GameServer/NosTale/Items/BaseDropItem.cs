using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Items
{
    class BaseDropItem
    {
        internal string type;
        internal int itemId;
        internal int qtyMin;
        internal int qtyMax;
        internal int rate;

        public BaseDropItem(string type, int itemId, int qtyMin, int qtyMax, int rate)
        {
            this.type = type;
            this.itemId = itemId;
            this.qtyMin = qtyMin;
            this.qtyMax = qtyMax;
            this.rate = rate;
        }
    }
}
