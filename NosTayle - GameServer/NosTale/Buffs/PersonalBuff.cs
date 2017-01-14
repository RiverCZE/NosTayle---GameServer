using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Buffs
{
    public class PersonalBuff
    {
        internal Buff baseBuff;
        internal int level;
        internal DateTime addedAt;

        public PersonalBuff(Buff baseBuff, int level, DateTime addedAt)
        {
            this.baseBuff = baseBuff;
            this.level = level;
            this.addedAt = addedAt;
        }

        public ServerPacket GetPacket(int type, int id, bool finish)
        {
            ServerPacket packet = new ServerPacket(Outgoing.buff);
            packet.AppendInt(type);
            packet.AppendInt(id);
            packet.AppendString(String.Format("0.{0}.{1}", this.baseBuff.id, finish ? 0 : this.baseBuff.time));
            packet.AppendInt(this.level);
            return packet;
        }
    }
}
