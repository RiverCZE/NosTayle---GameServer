using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Missions.Quests
{
    class BaseQuest
    {
        internal int id;
        internal int noteId;
        internal bool principalQuest;
        internal int type;
        internal int nextQuestId;

        internal bool sendVideo;
        internal int videoId;
        internal int startDialogId;
        internal int finishScriptId;
        internal int notCompletedDialogId;

        public BaseQuest(int id, bool principalQuest, int noteId, int type, int nextQuestId)
        {
            this.id = id;
            this.principalQuest = principalQuest;
            this.noteId = noteId;
            this.type = type;
            this.nextQuestId = nextQuestId;
        }
    }
}
