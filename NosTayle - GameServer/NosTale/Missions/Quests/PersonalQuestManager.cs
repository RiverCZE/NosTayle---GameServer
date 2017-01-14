using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.NosTale.Entities.Players;
using NosTayleGameServer.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Missions.Quests
{
    class PersonalQuestManager
    {
        internal int charId;
        internal int lastVideoActView;
        internal int lastPrincipalQuestId;
        internal int needViewScene;
        public PersonalQuestManager(int charId)
        {
            this.charId = charId;
            DataTable questTable = null;
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("charId", charId);
                questTable = dbClient.ReadDataTable("SELECT * FROM char_quests_server" + GameServer.serverId + " WHERE charId = @charId;");
                if (questTable.Rows.Count == 1)
                {
                    DataRow questRow = questTable.Rows[0];
                    this.lastVideoActView = (int)questRow["lastVideoActView"];
                    this.lastPrincipalQuestId = (int)questRow["lastPrincipalQuestId"];
                }
                else
                    dbClient.ExecuteQuery("INSERT INTO `char_quests_server1`(`charId`) VALUES (@charId)");
            }
        }

        public void GetScene(Player player, int id)
        {
            if (id == 40 && this.needViewScene == 40)
            {
                this.needViewScene = 0;
                this.lastVideoActView = id;
                this.GetQuest(player);
            }
        }

        public void GetQuest(Player player)
        {
            if (this.lastPrincipalQuestId == 0)
            {
                if (this.lastVideoActView == 0)
                {
                    this.needViewScene = 40;
                    ServerPacket packet = new ServerPacket(Outgoing.actScene);
                    packet.AppendInt(40);
                    player.SendPacket(packet);
                }
                else
                {
                    ServerPacket packet = new ServerPacket(Outgoing.script);
                    packet.AppendInt(1);
                    packet.AppendInt(10);
                    player.SendPacket(packet);
                }
            }
        }

        public void Save()
        {
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("charId", charId);
                dbClient.AddParamWithValue("lastVideoActView", this.lastVideoActView);
                dbClient.AddParamWithValue("lastPrincipalQuestId", this.lastPrincipalQuestId);
                dbClient.ExecuteQuery("UPDATE `char_quests_server1` SET "
                        + "lastVideoActView = @lastVideoActView,"
                        + "lastPrincipalQuestId = @lastPrincipalQuestId WHERE charId = @charId");
            }
        }
    }
}
