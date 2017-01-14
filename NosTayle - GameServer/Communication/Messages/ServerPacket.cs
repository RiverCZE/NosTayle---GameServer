using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.Messages
{
    public class ServerPacket
    {
        private string packet;

        public ServerPacket(string header)
        {
            this.packet = header;
        }

        public void AppendString(string str)
        {
            this.packet += " " + str;
        }

        public void AppendInt(int number)
        {
            this.packet += " " + number.ToString();
        }

        public void AppendBool(bool value)
        {
            this.packet += " " + Convert.ToInt32(value).ToString();
        }

        public void AppendDouble(double number)
        {
            this.packet += " " + number.ToString();
        }

        public new string ToString()
        {
            return this.packet;
        }

        public byte[] GetBytes()
        {
            return GameCrypto.Encrypt(this.ToString());
        }
    }
}
