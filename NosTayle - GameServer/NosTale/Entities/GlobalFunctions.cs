using NosTayleGameServer.Communication.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Entities
{
    interface GlobalFunctions
    {
        ServerPacket GetStats();
        void Move(int x, int y, int dir);
    }
}
