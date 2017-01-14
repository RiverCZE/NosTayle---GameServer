using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.Core;
using NosTayleGameServer.NosTale.Entities.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Missions.Quests
{
    class QuestManager
    {
        internal Dictionary<int, BaseQuest> quests;

        public QuestManager()
        {
            this.quests = new Dictionary<int, BaseQuest>();
            WriteConsole.WriteStructure("GAME", "Load quest list...");
            this.quests.Add(1, new BaseQuest(1, true, 1997, 1, 2));
            this.quests.Add(2, new BaseQuest(2, true, 1500, 2, 3));
            Console.WriteLine("{0} quests loads !", quests.Count);
        }
    }
}
