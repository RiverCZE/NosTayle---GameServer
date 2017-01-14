using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Shops
{
    public class ShopItem
    {
        internal int id;
        internal int shopId;
        internal int itemId;
        internal string type;
        internal int price;
        internal int rare;
        internal int upgrade;
        internal int color;

        public ShopItem(int id, int shopId, int itemId, string type, int price, int rare, int upgrade, int color)
        {
            this.id = id;
            this.shopId = shopId;
            this.itemId = itemId;
            this.type = type;
            this.price = price;
            this.rare = rare;
            this.upgrade = upgrade;
            this.color = color;
        }
    }
}
