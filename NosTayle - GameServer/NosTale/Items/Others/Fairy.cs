using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.Core;
using NosTayleGameServer.NosTale.Entities.Players;
using NosTayleGameServer.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Items.Others
{
    public class Fairy
    {
        internal int fairyId;
        internal int charId;
        internal int invPos;
        internal bool equiped;
        internal ItemBase itemBase;
        internal int exp;
        internal int level;

        //SAVE
        internal bool mustInsert = false;

        public Fairy(int fairyId)
        {
            DataTable dataTable = null;
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("fairyId", fairyId);
                dataTable = dbClient.ReadDataTable("SELECT * FROM fairies_server" + GameServer.serverId + " WHERE fairyId = @fairyId;");
            }
            if (dataTable.Rows.Count == 1)
            {
                DataRow fairyRow = dataTable.Rows[0];
                this.fairyId = (int)fairyRow["fairyId"];
                this.charId = (int)fairyRow["charId"];
                this.invPos = (int)fairyRow["invPos"];
                this.equiped = ServerMath.StringToBool(fairyRow["equiped"].ToString());
                this.itemBase = GameServer.GetItemsManager().itemList[(int)fairyRow["itemId"]];
                this.exp = (int)fairyRow["exp"];
                this.level = (int)fairyRow["level"];
            }
            else
            {
                this.fairyId = -1;
                this.itemBase = new ItemBase(-1);
            }
        }

        public Fairy(Fairy fairy)
        {
            this.fairyId = fairy.fairyId;
            this.charId = fairy.charId;
            this.invPos = fairy.invPos;
            this.itemBase = fairy.itemBase;
            this.exp = fairy.exp;
            this.level = fairy.level;
            this.mustInsert = fairy.mustInsert;
        }

        public Fairy(int fairyId, int charId, ItemBase itemBase, int exp, int level, bool mustInsert)
        {
            this.fairyId = fairyId;
            this.charId = charId;
            this.itemBase = itemBase;
            this.exp = exp;
            this.level = level;
            this.mustInsert = mustInsert;
        }

        public void Save(int charId, int accountId, int inWareHouse, int equiped, int slot)
        {
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                if (this.mustInsert)
                {
                        dbClient.ExecuteQuery("INSERT INTO `fairies_server" + GameServer.serverId + "`(`fairyId`, `charId`, `accountId`, `inWareHouse`, `invPos`, `equiped`, `itemId`, `exp`, `level`) VALUES ('" + this.fairyId + "','" + charId + "','" + accountId + "','" + inWareHouse + "','" + slot + "','" + equiped + "','" + this.itemBase.id + "','" + this.exp + "','" + this.level + "')");
                        this.mustInsert = false;
                }
                else
                {
                    dbClient.ExecuteQuery("UPDATE fairies_server" + GameServer.serverId + " SET " +
                        "charId = '" + charId + "', " +
                        "accountId = '" + accountId + "', " +
                        "inWareHouse = '" + inWareHouse + "', " +
                        "invPos = '" + slot + "', " +
                        "equiped = '" + equiped + "', " +
                        "exp = '" + this.exp + "', " +
                        "level = '" + this.level + "' " +
                        "WHERE fairyId = '" + this.fairyId + "' LIMIT 1");
                }
            }
        }

        public void GetInfo(Player player)
        {
            ServerPacket packet = new ServerPacket(Outgoing.equipInfo);
            packet.AppendInt(4);
            packet.AppendInt(this.itemBase.id);
            packet.AppendInt(this.itemBase.element);
            packet.AppendInt(this.level);
            packet.AppendInt(0);
            packet.AppendInt(1);
            packet.AppendInt(0);
            packet.AppendInt(0);
            player.SendPacket(packet);
        }
    }
}
