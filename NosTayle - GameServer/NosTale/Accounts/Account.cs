using NosTayleGameServer.Communication;
using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.Core;
using NosTayleGameServer.NosTale.Items;
using NosTayleGameServer.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Accounts
{
    public class Account
    {
        public string name;
        public string password;
        public int id;
        public bool isGm;
        public string ip;
        public bool online;
        public bool onSelectUser;
        public string languagePack;
        public bool banned;

        public Account(string ip)
        {
            this.name = null;
            this.password = null;
            this.id = 0;
            this.isGm = false;
            this.ip = ip;
            this.online = false;
            this.languagePack = "fr";
        }

        public void LoadChars(Session session) //A revoir
        {
            DataTable dataTable = null;
            ServerPacket packet = new ServerPacket(Outgoing.charListStart);
            packet.AppendInt(0);
            session.GetSock().Send(packet.GetBytes());
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("id", this.id);
                dataTable = dbClient.ReadDataTable("SELECT * FROM chars_server" + GameServer.serverId + " WHERE accountId = @id;");
            }
            foreach (DataRow charRow in dataTable.Rows)
            {
                string mask = "-1";
                if (charRow["maskId"].ToString() != "-1")
                {
                    using (DatabaseClient dbClient2 = GameServer.GetDatabaseManager().GetClient())
                    {
                        dbClient2.AddParamWithValue("charId", charRow["charId"].ToString());
                        dbClient2.AddParamWithValue("maskId", charRow["maskId"].ToString());
                        dataTable = dbClient2.ReadDataTable("SELECT * FROM items_server" + GameServer.serverId + " WHERE id = @maskId AND charId = @charId AND equiped='1';");
                        if (dataTable.Rows.Count == 1)
                        {
                            DataRow maskRow = dataTable.Rows[0];
                            mask = GameServer.GetItemsManager().itemList[Convert.ToInt32(maskRow["itemId"].ToString())].id.ToString();
                        }
                        else
                        {
                            dbClient2.ExecuteQuery("UPDATE chars_server" + GameServer.serverId + " SET maskId = -1 WHERE charId = @charId;");
                        }
                    }
                }
                ItemBase chap = null;
                string chapColor = "-1";
                if (charRow["chapId"].ToString() != "-1")
                {
                    using (DatabaseClient dbClient2 = GameServer.GetDatabaseManager().GetClient())
                    {
                        dbClient2.AddParamWithValue("charId", charRow["charId"].ToString());
                        dbClient2.AddParamWithValue("chapId", charRow["chapId"].ToString());
                        dataTable = dbClient2.ReadDataTable("SELECT * FROM items_server" + GameServer.serverId + " WHERE id = @chapId AND charId = @charId AND equiped='1';");
                        if (dataTable.Rows.Count == 1)
                        {
                            DataRow chapRow = dataTable.Rows[0];
                            if (GameServer.GetItemsManager().itemList.ContainsKey(Convert.ToInt32(chapRow["itemId"].ToString())))
                            {
                                chap = GameServer.GetItemsManager().itemList[Convert.ToInt32(chapRow["itemId"].ToString())];
                                if (chap.coloredItem)
                                {
                                    chapColor = chapRow["color"].ToString();
                                }
                            }
                        }
                        else
                        {
                            dbClient2.ExecuteQuery("UPDATE chars_server" + GameServer.serverId + " SET chapId = -1 WHERE charId = @charId;");
                        }
                    }
                }
                string weapon = "-1";
                if (charRow["weaponId"].ToString() != "-1")
                {
                    using (DatabaseClient dbClient2 = GameServer.GetDatabaseManager().GetClient())
                    {
                        dbClient2.AddParamWithValue("charId", charRow["charId"].ToString());
                        dbClient2.AddParamWithValue("weaponId", charRow["weaponId"].ToString());
                        dataTable = dbClient2.ReadDataTable("SELECT * FROM items_server" + GameServer.serverId + " WHERE id = @weaponId AND charId = @charId AND equiped='1';");
                        if (dataTable.Rows.Count == 1)
                        {
                            DataRow weaponRow = dataTable.Rows[0];
                            weapon = GameServer.GetItemsManager().itemList[Convert.ToInt32(weaponRow["itemId"].ToString())].id.ToString();
                        }
                        else
                        {
                            dbClient2.ExecuteQuery("UPDATE chars_server" + GameServer.serverId + " SET weaponId = -1 WHERE charId = @charId;");
                        }
                    }
                }
                string armor = "-1";
                if (charRow["armorId"].ToString() != "-1")
                {
                    using (DatabaseClient dbClient2 = GameServer.GetDatabaseManager().GetClient())
                    {
                        dbClient2.AddParamWithValue("charId", charRow["charId"].ToString());
                        dbClient2.AddParamWithValue("armorId", charRow["armorId"].ToString());
                        dataTable = dbClient2.ReadDataTable("SELECT * FROM items_server" + GameServer.serverId + " WHERE id = @armorId AND charId = @charId AND equiped='1';");
                        if (dataTable.Rows.Count == 1)
                        {
                            DataRow armorRow = dataTable.Rows[0];
                            armor = GameServer.GetItemsManager().itemList[Convert.ToInt32(armorRow["itemId"].ToString())].id.ToString();
                        }
                        else
                        {
                            dbClient2.ExecuteQuery("UPDATE chars_server" + GameServer.serverId + " SET armorId = -1 WHERE charId = @charId;");
                        }
                    }
                }
                string costumeBody = "-1";
                if (charRow["costumeBodyId"].ToString() != "-1")
                {
                    using (DatabaseClient dbClient2 = GameServer.GetDatabaseManager().GetClient())
                    {
                        dbClient2.AddParamWithValue("charId", charRow["charId"].ToString());
                        dbClient2.AddParamWithValue("costumeBodyId", charRow["costumeBodyId"].ToString());
                        dataTable = dbClient2.ReadDataTable("SELECT * FROM items_server" + GameServer.serverId + " WHERE id = @costumeBodyId AND charId = @charId AND equiped='1';");
                        if (dataTable.Rows.Count == 1)
                        {
                            DataRow armorRow = dataTable.Rows[0];
                            costumeBody = GameServer.GetItemsManager().itemList[Convert.ToInt32(armorRow["itemId"].ToString())].id.ToString();
                        }
                        else
                        {
                            dbClient2.ExecuteQuery("UPDATE chars_server" + GameServer.serverId + " SET costumeBodyId = -1 WHERE charId = @charId;");
                        }
                    }
                }
                string costumeHead = "-1";
                if (charRow["costumeHeadId"].ToString() != "-1")
                {
                    using (DatabaseClient dbClient2 = GameServer.GetDatabaseManager().GetClient())
                    {
                        dbClient2.AddParamWithValue("charId", charRow["charId"].ToString());
                        dbClient2.AddParamWithValue("costumeHeadId", charRow["costumeHeadId"].ToString());
                        dataTable = dbClient2.ReadDataTable("SELECT * FROM items_server" + GameServer.serverId + " WHERE id = @costumeHeadId AND charId = @charId AND equiped='1';");
                        if (dataTable.Rows.Count == 1)
                        {
                            DataRow armorRow = dataTable.Rows[0];
                            costumeHead = GameServer.GetItemsManager().itemList[Convert.ToInt32(armorRow["itemId"].ToString())].id.ToString();
                        }
                        else
                        {
                            dbClient2.ExecuteQuery("UPDATE chars_server" + GameServer.serverId + " SET costumeHeadId = -1 WHERE charId = @charId;");
                        }
                    }
                }
                packet = new ServerPacket(Outgoing.charList);
                packet.AppendString(charRow["pos"].ToString());
                packet.AppendString(GameCrypto.GetRealString(charRow["name"].ToString()));
                packet.AppendInt(0);
                packet.AppendString(charRow["sex"].ToString());
                packet.AppendString(charRow["hairStyle"].ToString());
                packet.AppendString(charRow["hairColor"].ToString());
                packet.AppendInt(0);
                packet.AppendString(charRow["class"].ToString());
                packet.AppendString(charRow["level"].ToString());
                packet.AppendString((chap != null ? chap.id.ToString() : "-1") + "." + armor + "." + weapon + ".-1." + mask + ".-1." + costumeBody + "." + costumeHead + " 2 3 4 -1.-1");
                packet.AppendString(chapColor);
                packet.AppendInt(0);
                session.GetSock().Send(packet.GetBytes());
            }
            session.SendPacket(new ServerPacket(Outgoing.charListEnd).GetBytes());
        }

        public void LoadGame(Session session)
        {
            session.SendPacket(new ServerPacket("OK"));
            /*user.player.ActivatePulse();*/
            session.GetPlayer().map.AddUser(session.GetPlayer(), true);
        }

        public static bool NoIllegalChar(string str)
        {
            for (int i = 0; i < str.Length; i++)
                if (str[i] == '[' || str[i] == ']' || str[i] == '@' || str[i] == '|' || str[i] == ':' || str[i] == '!' || str[i] == '(' || str[i] == ')' || str[i] == (char)132 || str[i] == ' ' || str[i] == (char)160)
                    return true;
            return false;
        }
    }
}
