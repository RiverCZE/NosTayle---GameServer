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
    internal sealed class SelectCharacterEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Session.GetAccount().onSelectUser)
                return;
            Session.GetAccount().onSelectUser = true;
            if (Event.valuesCount == 1)
            {
                if (Convert.ToInt32(Event.GetValue(0)) >= 0 && Convert.ToInt32(Event.GetValue(0)) <= 2)
                {
                    DataTable dataTable2 = null;
                    using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("id", Session.GetAccount().id);
                        dbClient.AddParamWithValue("pos", Event.GetValue(0));
                        dataTable2 = dbClient.ReadDataTable("SELECT * FROM chars_server" + GameServer.serverId + " WHERE accountId = @id AND pos = @pos;");
                    }
                    if (dataTable2.Rows.Count == 1)
                    {
                        DataRow charRow = dataTable2.Rows[0];
                        Session.SetPlayer(new NosTale.Entities.Players.Player(Session, (int)charRow["charId"], GameCrypto.GetRealString(charRow["name"].ToString()), Convert.ToInt32(charRow["faction"].ToString()), Convert.ToInt32(charRow["familyId"].ToString()), Convert.ToInt32(charRow["sex"].ToString()), (int)charRow["hairStyle"], (int)charRow["hairColor"], Convert.ToInt32(charRow["class"].ToString()), (int)charRow["level"], (int)charRow["jobLevel"], charRow["skills"].ToString(), (int)charRow["map"], (int)charRow["x"], (int)charRow["y"], false, Session.GetAccount().isGm, (int)charRow["weaponId"], (int)charRow["weapon2Id"], (int)charRow["armorId"], (int)charRow["maskId"], (int)charRow["fairyId"], (int)charRow["chapId"], (int)charRow["spId"], (int)charRow["gloveId"], (int)charRow["bootId"], (int)charRow["costumeBodyId"], (int)charRow["costumeHeadId"], 11, (int)charRow["hp"], (int)charRow["mp"], (int)charRow["exp"], (int)charRow["expJob"], (int)charRow["reputation"], (int)charRow["digniter"], (int)charRow["gold"], charRow["sBar"].ToString(), GameCrypto.GetRealString(charRow["pMessage"].ToString()), false, (int)charRow["mlStatue"], (int)charRow["mlVisitesTotal"], (int)charRow["mlVisitesDay"], GameCrypto.GetRealString(charRow["mlMotto"].ToString()), charRow["mlLastUpdateVisites"].ToString(), (int)charRow["arenaKill"], (int)charRow["arenaDead"], (int)charRow["admirationPoints"], ServerMath.StringToBool(charRow["muted"].ToString()), (int)charRow["onlineTime"]));
                        Session.GetChannel().AddPlayerOnline(Session.GetPlayer());
                        Session.GetAccount().LoadGame(Session);
                    }
                    else
                        Session.GetSock().CloseConnection();
                }
                else
                    Session.GetSock().CloseConnection();
            }
        }
    }
}
