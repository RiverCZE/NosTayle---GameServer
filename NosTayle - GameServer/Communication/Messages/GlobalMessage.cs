using NosTayleGameServer.Communication.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.Messages
{
    public static class GlobalMessage
    {
        public static ServerPacket MakeMessage(int userId, int entitieType, int type, string message)
        {
            ServerPacket packet = new ServerPacket(Outgoing.simpleMessage);
            packet.AppendInt(entitieType);
            packet.AppendInt(userId);
            packet.AppendInt(type);
            packet.AppendString(message);
            return packet;
        }

        public static ServerPacket MakeAlert(int type, string alertMessage)
        {
            ServerPacket packet = new ServerPacket(Outgoing.bannerMessage);
            packet.AppendInt(type);
            packet.AppendString(alertMessage);
            return packet;
        }

        public static ServerPacket MakeInfo(string message)
        {
            ServerPacket packet = new ServerPacket(Outgoing.infoBox);
            packet.AppendString(message);
            return packet;
        }

        public static ServerPacket MakeDelay(int time, int type, string result)
        {
            ServerPacket packet = new ServerPacket(Outgoing.delay);
            packet.AppendInt(time);
            packet.AppendInt(type);
            packet.AppendString(result);
            return packet;
        }

        public static ServerPacket MakeShopEnd(int type)
        {
            ServerPacket packet = new ServerPacket(Outgoing.shopEnd);
            packet.AppendInt(type);
            return packet;
        }

        public static ServerPacket MakeShopMessage(int type, string message)
        {
            ServerPacket packet = new ServerPacket(Outgoing.shopMessage);
            packet.AppendInt(type);
            packet.AppendString(message);
            return packet;
        }

        public static ServerPacket MakeSpeak(int entitieType, int entitieId, int type, string name, string message)
        {
            ServerPacket packet = new ServerPacket(Outgoing.speakEntity);
            packet.AppendInt(entitieType);
            packet.AppendInt(entitieId);
            packet.AppendInt(type);
            packet.AppendString(name);
            packet.AppendString(message);
            return packet;
        }

        public static ServerPacket MakeModal(int type, string text)
        {
            ServerPacket packet = new ServerPacket(Outgoing.modal);
            packet.AppendInt(type);
            packet.AppendString(text);
            return packet;
        }

        public static ServerPacket MakeDialog(string action1, string action2, string info)
        {
            ServerPacket packet = new ServerPacket(Outgoing.dialog);
            packet.AppendString(action1);
            packet.AppendString(action2);
            packet.AppendString(info);
            return packet;
        }

        public static ServerPacket MakeGuri(int guriType, int uType, int uId, string text)
        {
            ServerPacket packet = new ServerPacket(Outgoing.guri);
            packet.AppendInt(guriType);
            packet.AppendInt(uType);
            packet.AppendInt(uId);
            packet.AppendString(text);
            return packet;
        }

        public static ServerPacket MakeIcon(int type, int id, int unknow, int itemId)
        {
            ServerPacket packet = new ServerPacket(Outgoing.icon);
            packet.AppendInt(type);
            packet.AppendInt(id);
            packet.AppendInt(unknow);
            packet.AppendInt(itemId);
            return packet;
        }
    }
}
