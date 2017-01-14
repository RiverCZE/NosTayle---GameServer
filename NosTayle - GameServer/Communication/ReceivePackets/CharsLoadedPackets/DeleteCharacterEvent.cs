using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.Core;
using NosTayleGameServer.NosTale;
using NosTayleGameServer.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.CharsLoadedPackets
{
    internal sealed class DeleteCharacterEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Event.valuesCount == 2)
            {
                if (MD5.MD5Hash(Event.GetValue(1)) == Session.GetAccount().password)
                {
                    if (Convert.ToInt32(Event.GetValue(0)) >= 0 && Convert.ToInt32(Event.GetValue(0)) <= 2)
                    {
                        using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("id", Session.GetAccount().id);
                            dbClient.AddParamWithValue("pos", Event.GetValue(0));
                            DataTable dataTable = dbClient.ReadDataTable("SELECT * FROM chars_server" + GameServer.serverId + " WHERE accountId = @id AND pos = @pos;");
                            if (dataTable.Rows.Count != 0)
                            {
                                DataRow charRow = dataTable.Rows[0];
                                dbClient.ExecuteQuery("DELETE FROM chars_server" + GameServer.serverId + " WHERE accountId = @id AND pos = @pos;");
                                dbClient.AddParamWithValue("charId", charRow["charId"]);
                                dbClient.ExecuteQuery("DELETE FROM items_server" + GameServer.serverId + " WHERE charId = @charId;");
                            }
                            Session.SendPacket(new ServerPacket(Outgoing.success));
                            Session.GetAccount().LoadChars(Session);
                        }
                    }
                }
                else
                    Session.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(Session.GetAccount().languagePack, "wrong.password")));
            }
        }
    }
}
