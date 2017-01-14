using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets
{
    interface Interface
    {
        void Handle(Session session, SessionMessage Event);
    }
}
