using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Entities.Players.FriendBlackList
{
    public class LPlayer
    {
        private int pId;
        private string pName;

        public LPlayer(int pId, string pName)
        {
            this.pId = pId;
            this.pName = pName;
        }

        public int GetId() { return this.pId; }
        public string GetName() { return this.pName; }
    }
}
