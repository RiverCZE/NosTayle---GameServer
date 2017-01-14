using NosTayleGameServer.Communication.Messages;
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
    internal sealed class CreateCharacterEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            try
            {
                if (Event.valuesCount == 5)
                {
                    if ((Event.GetValue(1) != "0" && Event.GetValue(1) != "1" && Event.GetValue(1) != "2") || (Event.GetValue(2) != "0" && Event.GetValue(2) != "1") || (Event.GetValue(3) != "0" && Event.GetValue(3) != "1") || (Convert.ToInt32(Event.GetValue(4)) < 0 || Convert.ToInt32(Event.GetValue(4)) > 9))
                    {
                        Session.SendPacket(GlobalMessage.MakeInfo("ERROR"));
                    }
                    if (Event.GetValue(0).Length > 3 && Event.GetValue(0).Length < 15)
                    {
                        DataTable dataTable = null;
                        Console.WriteLine((byte)Event.GetValue(0)[0]);
                        using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("name", Event.GetValue(0));
                            dbClient.AddParamWithValue("id", Session.GetAccount().id);
                            dbClient.AddParamWithValue("pos", Event.GetValue(1));
                            dataTable = dbClient.ReadDataTable("SELECT * FROM chars_server" + GameServer.serverId + " WHERE name = @name OR accountId = @id AND pos = @pos;");
                        }
                        if (dataTable.Rows.Count < 1)
                        {
                            if (!NosTayleGameServer.NosTale.Accounts.Account.NoIllegalChar(Event.GetValue(0)))
                            {
                                using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
                                {
                                    dbClient.AddParamWithValue("name", Event.GetValue(0));
                                    dbClient.AddParamWithValue("id", Session.GetAccount().id);
                                    dbClient.AddParamWithValue("pos", Event.GetValue(1));
                                    dbClient.AddParamWithValue("sex", Event.GetValue(2));
                                    dbClient.AddParamWithValue("hairStyle", Event.GetValue(3));
                                    dbClient.AddParamWithValue("hairColor", Event.GetValue(4));
                                    dbClient.ExecuteQuery("INSERT INTO `chars_server" + GameServer.serverId + "`(`accountId`, `pos`, `name`, `sex`, `hairStyle`, `hairColor`, `map`, `x`, `y`) VALUES (@id,@pos,@name,@sex,@hairStyle,@hairColor,'1','" + new Random().Next(75, 84) + "','" + new Random().Next(111, 121) + "');");
                                    Session.GetAccount().LoadChars(Session);
                                }
                            }
                            else
                                Session.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(Session.GetAccount().languagePack, "error.charname.illegalchar")));
                        }
                        else
                            Session.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(Session.GetAccount().languagePack, "error.charname.alreadyused")));
                    }
                    else
                        Session.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(Session.GetAccount().languagePack, "error.charname.length")));
                }
            }
            catch {  Session.SendPacket(GlobalMessage.MakeInfo("ERROR!")); }
        }
    }
}
