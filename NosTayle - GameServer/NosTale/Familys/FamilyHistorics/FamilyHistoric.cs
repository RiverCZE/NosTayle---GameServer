using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.NosTale.Entities.Players;
using NosTayleGameServer.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Familys.FamilyHistorics
{
    class FamilyHistoric
    {
        private List<HistoricItem> hisItems;

        public FamilyHistoric()
        {
            this.hisItems = new List<HistoricItem>();
        }

        public void AddToHis(HistoricItem hisItem)
        {
            lock (this.hisItems)
            {
                this.hisItems.Add(hisItem);
                if (this.hisItems.Count > 200)
                    this.hisItems.Remove(this.hisItems[0]);
            }
        }

        public void SendHis(Player user)
        {
            ServerPacket packet = new ServerPacket(Outgoing.hisPacket);
            packet.AppendInt(0);
            for (int i = this.hisItems.Count; i > 0; i--)
            {
                packet.AppendString(this.hisItems[i - 1].ToString());
                if (i != this.hisItems.Count && i % 50 == 0 && i - 1 != 0)
                {
                    user.SendPacket(packet);
                    packet = new ServerPacket(Outgoing.hisPacket);
                }
            }
            user.SendPacket(packet);
        }

        public void SaveHis(int familyId)
        {
            lock (this.hisItems)
            {
                using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("familyId", familyId);
                    dbClient.ExecuteQuery("DELETE FROM family_his_server" + GameServer.serverId + " WHERE familyId = @familyId;");
                    string req = "";
                    int i = 0;
                    foreach (HistoricItem hisItem in this.hisItems)
                    {
                        dbClient.AddParamWithValue(String.Format("text{0}", i), hisItem.text);
                        dbClient.AddParamWithValue(String.Format("date{0}", i), hisItem.date);
                        req += hisItem.GetSqlRequest(i);
                        i++;
                    }
                        if (req != "")
                            dbClient.ExecuteQuery(req);
                }
            }
        }
    }
}
