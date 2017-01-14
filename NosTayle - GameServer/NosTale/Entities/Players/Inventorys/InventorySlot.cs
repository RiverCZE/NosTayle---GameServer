using NosTayleGameServer.NosTale.Items;
using NosTayleGameServer.NosTale.Items.Others;
using NosTayleGameServer.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Entities.Players.Inventorys
{
    public class InventorySlot
    {
        internal int idSlot;
        internal InventoryItem invItem;
        internal int amount = 1;
        internal bool Updated = false;

        public InventorySlot(int slotId, int amount)
        {
            this.idSlot = slotId;
            this.amount = amount;
        }

        public InventorySlot(InventorySlot invSlot)
        {
            this.idSlot = invSlot.idSlot;
            this.invItem = invSlot.invItem;
            this.amount = invSlot.amount;
            this.Updated = invSlot.Updated;
        }

        public void SaveSlot(int playerId, int accountId)
        {
            if (this.invItem.isItem)
                this.invItem.item.Save(playerId, playerId, 0, 0, this.idSlot, this.amount);
            else if (this.invItem.isSp)
                this.invItem.specialist.SaveSP(playerId, accountId, 0, 0, this.idSlot, this.invItem.specialist.inEquip ? 1 : 0);
            else if (this.invItem.isFairy)
                this.invItem.fairy.Save(playerId, accountId, 0, 0, this.idSlot);
        }

        public void DeleteSlot()
        {
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                if (this.invItem.isItem)
                {
                    Item item = this.invItem.item;
                    dbClient.ExecuteQuery("DELETE FROM items_server" + GameServer.serverId + " WHERE id =  '" + item.id + "';");
                }
                else if (this.invItem.isSp)
                {
                    Specialist sp = this.invItem.specialist;
                    dbClient.ExecuteQuery("DELETE FROM sps_server" + GameServer.serverId + " WHERE spId =  '" + sp.spId + "';");
                }
                else if (this.invItem.isFairy)
                {
                    Fairy fairy = this.invItem.fairy;
                    dbClient.ExecuteQuery("DELETE FROM fairies_server" + GameServer.serverId + " WHERE fairyId =  '" + fairy.fairyId + "';");
                }
            }
        }

        public int DeleteSlotCount(int count)
        {
            if (amount <= 0)
                return 0;
            if (count <= amount)
            {
                try
                {
                    using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
                    {
                        string item_del = "";
                        InventoryItem inventoryItem = invItem;
                        this.amount -= count;
                        if (inventoryItem.isItem)
                        {
                            if (this.amount < 1)
                                item_del = "DELETE FROM items_server" + GameServer.serverId + " WHERE id =  '" + inventoryItem.item.id + "';";
                        }
                        else if (inventoryItem.isSp)
                        {
                            Specialist sp = inventoryItem.specialist;
                            dbClient.ExecuteQuery("DELETE FROM sps_server" + GameServer.serverId + " WHERE spId =  '" + sp.spId + "';");
                        }
                        else if (inventoryItem.isFairy)
                        {
                            Fairy fairy = inventoryItem.fairy;
                            dbClient.ExecuteQuery("DELETE FROM fairies_server" + GameServer.serverId + " WHERE fairyId =  '" + fairy.fairyId + "';");
                        }
                        if (item_del != "")
                            dbClient.ExecuteQuery(item_del);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            this.Updated = true;
            return this.amount;
        }
    }
}
