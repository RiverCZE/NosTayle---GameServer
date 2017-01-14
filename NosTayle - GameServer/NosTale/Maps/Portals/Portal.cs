using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Maps.Portals
{
    class Portal
    {
        internal int id;
        internal int mapId;
        internal int title;
        internal int type;
        internal string portal_dType;
        internal int x_pos;
        internal int y_pos;
        internal int mapDirection;
        internal int x_mapDirection;
        internal int y_mapDirection;

        public Portal(int id, int type, string portal_dType, int mapId, int title, int x_pos, int y_pos, int mapDirection, int x_mapDirection, int y_mapDirection)
        {
            this.id = id;
            this.type = type;
            this.portal_dType = portal_dType;
            this.mapId = mapId;
            this.title = title;
            this.x_pos = x_pos;
            this.y_pos = y_pos;
            this.mapDirection = mapDirection;
            this.x_mapDirection = x_mapDirection;
            this.y_mapDirection = y_mapDirection;
        }

        public ServerPacket GetPortalPacket()
        {
            ServerPacket packet = new ServerPacket(Outgoing.portal);
            packet.AppendInt(this.x_pos);
            packet.AppendInt(this.y_pos);
            packet.AppendInt(this.title);
            packet.AppendInt(this.type);
            packet.AppendInt(0);
            return packet;
        }

        public ServerPacket GetPortalDelPacket()
        {
            ServerPacket packet = new ServerPacket(Outgoing.portal);
            packet.AppendInt(this.x_pos);
            packet.AppendInt(this.y_pos);
            packet.AppendInt(this.title);
            packet.AppendInt(this.type);
            packet.AppendInt(0);
            packet.AppendInt(1);
            return packet;
        }
    }
}
