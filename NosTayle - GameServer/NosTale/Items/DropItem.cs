using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.NosTale.Entities;
using NosTayleGameServer.NosTale.Groups;
using NosTayleGameServer.NosTale.Items.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Items
{
    public class DropItem
    {
        internal int id;
        internal int x, y;
        internal DateTime dropedAt;
        internal bool isGold, isSp, isFairy, isItem, isQuestItem;
        internal int quantity;
        internal Item item;
        internal Specialist sp;
        internal Fairy fairy;

        internal Group forGroup;
        internal Entitie forEntitie;

        internal bool inDrop;

        private int baseId
        {
            get
            {
                if (isItem)
                    return this.item.itemBase.id;
                if (isSp)
                    return this.sp.card.id;
                if (isFairy)
                    return this.fairy.itemBase.id;
                if (isGold)
                    return 1046;
                return 0;
            }
        }

        public DropItem(int id, int x, int y, DateTime dropedAt, bool isGold, bool isSp, bool isFairy, bool isItem, bool isQuestItem, int quantity, Item item, Specialist sp, Fairy fairy, Group forGroup, Entitie forEntitie)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            this.dropedAt = dropedAt;
            this.isGold = isGold;
            this.isSp = isSp;
            this.isFairy = isFairy;
            this.isItem = isItem;
            this.isQuestItem = isQuestItem;
            this.quantity = quantity;
            this.item = item;
            this.sp = sp;
            this.fairy = fairy;
            this.forGroup = forGroup;
            this.forEntitie = forEntitie;
        }

        public ServerPacket GetDropPacket()
        {
            ServerPacket packet = new ServerPacket(Outgoing.dropItem);
            packet.AppendInt(this.baseId);
            packet.AppendInt(this.id);
            packet.AppendInt(this.x);
            packet.AppendInt(this.y);
            packet.AppendInt(this.quantity);
            packet.AppendBool(this.isQuestItem);
            packet.AppendInt(this.forEntitie != null ? this.forEntitie.id : 0);
            return packet;
        }

        public ServerPacket GetInPacket()
        {
            ServerPacket packet = new ServerPacket(Outgoing.inEntitie);
            packet.AppendInt(9);
            packet.AppendInt(this.baseId);
            packet.AppendInt(this.id);
            packet.AppendInt(this.x);
            packet.AppendInt(this.y);
            packet.AppendInt(this.quantity);
            packet.AppendBool(this.isQuestItem);
            packet.AppendInt(0);
            packet.AppendInt(this.forEntitie != null ? this.forEntitie.id : 0);
            return packet;
        }
    }
}
