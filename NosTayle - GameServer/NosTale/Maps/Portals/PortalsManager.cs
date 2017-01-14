using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Maps.Portals
{
    class PortalsManager
    {
        internal Dictionary<int, Portal> portals = new Dictionary<int, Portal>();

        public void AddPortal(int id, Portal portal)
        {
            portals.Add(id, portal);
        }
    }
}
