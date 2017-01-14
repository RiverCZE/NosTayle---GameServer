using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Familys.FamilyHistorics
{
    class HistoricItem
    {
        internal string text;
        internal int date;

        public HistoricItem(string text, int date)
        {
            this.text = text;
            this.date = date;
        }

        new public string ToString()
        {
            int time = (int)((GameServer.Timestamp() - date) / 3600);
            return String.Format("{0}|{1}", text, time);
        }

        public string GetSqlRequest(int i)
        {
            return String.Format("INSERT INTO `family_his_server{0}`(`familyId`, `text`, `date`) VALUES (@familyId,@text{1},@date{2});", GameServer.serverId, i, i);
        }
    }
}
