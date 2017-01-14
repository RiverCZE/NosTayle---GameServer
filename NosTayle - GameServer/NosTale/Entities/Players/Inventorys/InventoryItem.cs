using NosTayleGameServer.NosTale.Items;
using NosTayleGameServer.NosTale.Items.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Entities.Players.Inventorys
{
    public class InventoryItem
    {
        internal Item item;
        internal bool isItem;
        internal bool isSp;
        internal bool isFairy;
        internal Specialist specialist;
        internal ItemBase itemBase;
        internal Fairy fairy;
        internal string type;
        internal int gId;

        public InventoryItem(object obj, string type)
        {
            this.type = type;
            switch (type)
            {
                case "item":
                    Item newItem = (Item)obj;
                    item = new Item(newItem, newItem.invPos, newItem.equiped);
                    isItem = true;
                    this.gId = newItem.id;
                    this.itemBase = newItem.itemBase;
                    break;
                case "specialist":
                    Specialist newSp = (Specialist)obj;
                    specialist = new Specialist(newSp);
                    isSp = true;
                    this.gId = newSp.spId;
                    this.itemBase = newSp.card;
                    break;
                case "fairy":
                    Fairy newFairy = (Fairy)obj;
                    fairy = new Fairy(newFairy);
                    isFairy = true;
                    this.gId = newFairy.fairyId;
                    this.itemBase = newFairy.itemBase;
                    break;
            }
        }

        public InventoryItem(InventoryItem inventoryItem)
        {
            this.type = inventoryItem.type;
            this.itemBase = inventoryItem.itemBase;
            this.isItem = inventoryItem.isItem;
            this.isSp = inventoryItem.isSp;
            this.isFairy = inventoryItem.isFairy;
            this.item = inventoryItem.item;
            this.specialist = inventoryItem.specialist;
            this.fairy = inventoryItem.fairy;
        }

        public bool UserClassCanUse(int userClass)
        {
            switch (userClass)
            {
                case 0:
                    return this.itemBase.adventer;
                case 1:
                    return this.itemBase.sword;
                case 2:
                    return this.itemBase.archer;
                case 3:
                    return this.itemBase.mage;
                default:
                    return false;

            }
        }

        public int GetUpgrade(int up)
        {
            switch (up)
            {
                case 1:
                    if (this.isItem)
                        if (this.item.color == 0)
                            return this.item.rare;
                        else
                            return this.item.color;
                    else
                        return 0;
                case 2:
                    if (this.isItem)
                        if (this.item.color == 0)
                            return this.item.upgrade;
                        else
                            return this.item.color;
                    else if (this.isSp)
                        return this.specialist.upgrade;
                    else
                        return 0;
            }
            return 0;
        }
    }
}
