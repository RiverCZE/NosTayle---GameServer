using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.NosTale.Entities.Players;
using NosTayleGameServer.NosTale.Familys.FamilyHistorics;
using NosTayleGameServer.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Familys
{
    class Family
    {
        private int id;
        private string name;
        private int level;
        private int faction;
        private int membersMax;
        private string motto;
        private int exp;
        private int expMonth;
        private int chiefGender;
        private bool gardCanInvit, gardCanAvert, gardCanShout, gardCanLookHis, memberCanLookHis;
        private int gardAccessWarehouse, memberAccessWarehouse;
        private FamilyHistoric historic;
        private DateTime lastSaveHis;

        private FamilyMember chief;

        private int membersCount;

        private List<int> newOnlineSend;

        private List<Player> membersOnline;

        internal bool isCycling;

        //LOCKED
        private Object lockedObject = new Object();

        public Family(int id, string name, int level, int faction, int membersMax, string motto, int exp, int expMonth, int chiefGender, bool gardCanInvit, bool gardCanAvert, bool gardCanShout, bool gardCanLookHis,
            int gardAccessWarehouse, bool memberCanLookHis, int memberAccessWarehouse)
        {
            this.id = id;
            this.name = name;
            this.level = level;
            this.membersMax = membersMax;
            this.motto = motto;
            this.exp = exp;
            this.expMonth = expMonth;
            this.membersOnline = new List<Player>();
            this.newOnlineSend = new List<int>();
            this.gardCanInvit = gardCanInvit;
            this.gardCanAvert = gardCanAvert;
            this.gardCanShout = gardCanShout;
            this.gardCanLookHis = gardCanLookHis;
            this.gardAccessWarehouse = gardAccessWarehouse;
            this.memberCanLookHis = memberCanLookHis;
            this.memberAccessWarehouse = memberAccessWarehouse;

            //OTHER
            this.chief = this.LoadChief();
            this.UpdateMembersCount();
            this.LoadHis();
            this.lastSaveHis = DateTime.Now;
        }

        public static Family LoadFamily(int id)
        {
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                DataTable dataTable = null;
                dbClient.AddParamWithValue("id", id);
                dataTable = dbClient.ReadDataTable("SELECT * FROM familys_server" + GameServer.serverId + " WHERE id = @id;");
                if (dataTable.Rows.Count == 1)
                {
                    DataRow row = dataTable.Rows[0];
                    return new Family((int)row["id"], GameCrypto.GetRealString(row["name"].ToString()), (int)row["level"], (int)row["faction"], (int)row["membersMax"], GameCrypto.GetRealString(row["motto"].ToString()), (int)row["exp"], (int)row["expMonth"], (int)row["chiefGender"], (bool)row["gardCanInvit"], (bool)row["gardCanAvert"], (bool)row["gardCanShout"], (bool)row["gardCanLookHis"], (int)row["gardAccessWarehouse"], (bool)row["memberCanLookHis"], (int)row["memberAccessWarehouse"]);
                }
            }
            return null;
        }

        public static void CreateFamily(Player chief, string name)
        {
            try
            {
                if (name.Length > 14)
                    return;
                Player member1 = null;
                Player member2 = null;
                if (chief.group != null && chief.group.members_count == 3)
                {
                    bool haveFamily = false;
                    bool notSameMap = false;
                    lock (chief.group.members)
                    {
                        foreach (Player member in chief.group.members)
                        {
                            if (member.family != null)
                            { haveFamily = true; break; }
                            if (member.map != chief.map)
                            { notSameMap = true; break; }
                        }
                    }
                    if (haveFamily)
                    {
                        chief.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(chief.languagePack, "error.createfamily.memberhf")));
                        return;
                    }
                    if (notSameMap)
                    {
                        chief.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(chief.languagePack, "error.createfamily.membernm")));
                        return;
                    }
                    if (chief.gold < 200000)
                    {
                        chief.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(chief.languagePack, "error.gold")));
                        return;
                    }
                    Family family;
                    using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
                    {
                        DataTable dataTable = null;
                        dbClient.AddParamWithValue("name", name);
                        dataTable = dbClient.ReadDataTable("SELECT id FROM familys_server" + GameServer.serverId + " WHERE name = @name;");
                        if (dataTable.Rows.Count < 1)
                        {
                            chief.gold -= 200000;
                            int familyId = GameServer.NewFamilyId();
                            dbClient.AddParamWithValue("familyId", familyId);
                            dbClient.AddParamWithValue("chiefGender", chief.gender);
                            dbClient.AddParamWithValue("chiefId", chief.id);
                            dbClient.AddParamWithValue("chiefName", chief.name);
                            dbClient.AddParamWithValue("chiefLevel", chief.level);
                            lock (chief.group.members)
                            {
                                foreach (Player m in chief.group.members)
                                    if (m != chief)
                                        if (member1 == null)
                                            member1 = m;
                                        else
                                            member2 = m;
                            }
                            dbClient.ExecuteQuery("INSERT INTO `familys_server" + GameServer.serverId + "`(`id`, `name`, `chiefGender`) VALUES (@familyId,@name,@chiefGender);"
                             + "UPDATE chars_server" + GameServer.serverId + " SET familyId = -1 WHERE familyId=@familyId;");
                            family = GameServer.GetFamily(familyId);
                        }
                        else
                        {
                            chief.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(chief.languagePack, "error.createfamily.namealreadyused")));
                            return;
                        }
                    }
                    chief.fMember = new FamilyMember(chief.id, 06102015, chief.level, chief.name, chief.userClass, 0, 0, 0, "");
                    chief.family = family;
                    chief.SaveData();
                    chief.fMember.SaveMember(family.GetId());
                    family.AddMemberOnline(chief);
                    member1.fMember = new FamilyMember(member1.id, 06102015, member1.level, member1.name, member1.userClass, 1, 0, 0, "");
                    member1.family = family;
                    member1.SaveData();
                    member1.fMember.SaveMember(family.GetId());
                    family.AddMemberOnline(member1);
                    member2.fMember = new FamilyMember(member2.id, 06102015, member2.level, member2.name, member2.userClass, 1, 0, 0, "");
                    member2.family = family;
                    member2.SaveData();
                    member2.fMember.SaveMember(family.GetId());
                    family.AddMemberOnline(member2);
                    family.UpdateMembersCount();
                    family.chief = family.LoadChief();
                    chief.LoadFamily();
                    member1.LoadFamily();
                    member2.LoadFamily();
                    family.SendAffichFamilyToAll();
                    GameServer.GetPlayersManager().SendToAll(GlobalMessage.MakeAlert(0, "").ToString(), "message.familycreated", name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void onCycle()
        {
            this.isCycling = true;
            this.SendAllNewsConnexions();
            this.SendDisconnexion();
            if (DateTime.Now.Subtract(this.lastSaveHis).TotalMinutes >= 5)
            {
                this.lastSaveHis = DateTime.Now;
                this.historic.SaveHis(this.id);
            }
            this.isCycling = false;
        }

        public int GetMembersCount()
        {
            return this.membersCount;
        }

        public void UpdateMembersCount()
        {
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                DataTable dataTable = null;
                dbClient.AddParamWithValue("familyId", this.id);
                dataTable = dbClient.ReadDataTable("SELECT charId FROM chars_server" + GameServer.serverId + " WHERE familyId = @familyId;");
                if (dataTable.Rows.Count > 0)
                    this.membersCount = dataTable.Rows.Count;
                else
                    this.membersCount = 0;
            }
        }

        public int GetMemberCountByRank(int rank)
        {
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                DataTable dataTable = null;
                dbClient.AddParamWithValue("familyId", this.id);
                dbClient.AddParamWithValue("familyRank", rank);
                dataTable = dbClient.ReadDataTable("SELECT charId FROM chars_server" + GameServer.serverId + " WHERE familyId = @familyId AND familyRank = @familyRank;");
                if (dataTable.Rows.Count > 0)
                    return dataTable.Rows.Count;
                else
                    return 0;
            }
        }

        public FamilyHistoric GetHis()
        {
            return this.historic;
        }

        public int GetId()
        {
            return this.id;
        }

        public string GetName()
        {
            return this.name;
        }

        public int GetLevel()
        {
            return this.level;
        }

        public void UpdateLevel(int level) { this.level = level; this.UpFamily(); }

        public int GetFaction() { return this.faction; }

        public string MakeNameAndGrade(FamilyMember fMember, string languagePacket)
        {
            return String.Format("{0}{1}", this.GetName(), fMember.GetRankString(languagePacket));
        }

        public void SendInfos(Player user) //ginfo packet informations de base sur la famille
        {
            FamilyMember chief = this.GetChief();
            ServerPacket packet = new ServerPacket(Outgoing.familyInfosBase);
            packet.AppendString(this.name); //FamilyName
            packet.AppendString(chief != null ? chief.GetName() : "default"); //ChiefName
            packet.AppendInt(this.chiefGender); //ChiefSexe 
            packet.AppendInt(this.level); //familyLevel
            packet.AppendInt(this.exp); //familyExp
            packet.AppendInt(1000000); //expRequire
            packet.AppendInt(this.membersCount); //membersCount
            packet.AppendInt(this.membersMax); //membersMax
            packet.AppendInt(user.fMember != null ? user.fMember.GetMemberRank() : 3); //memberRank
            packet.AppendInt(Convert.ToInt32(this.gardCanInvit)); //GardcanInvite
            packet.AppendInt(Convert.ToInt32(this.gardCanAvert)); //GardCanAvert
            packet.AppendInt(Convert.ToInt32(this.gardCanShout)); //GardCanShout
            packet.AppendInt(Convert.ToInt32(this.gardCanLookHis)); //GardCanLookHisto
            packet.AppendInt(this.gardAccessWarehouse); //GardAccessWareHouse
            packet.AppendInt(Convert.ToInt32(this.memberCanLookHis)); //MemberCanLookHisto
            packet.AppendInt(this.memberAccessWarehouse); //MemberAccessWareHouse
            packet.AppendString(this.motto.Replace(' ', '^')); //FamilyMessage
            user.SendPacket(packet);
        }

        public bool IsMember(Player player)
        {
            bool isMember = false;
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                DataTable dataTable = null;
                dbClient.AddParamWithValue("id", player.id);
                dbClient.AddParamWithValue("familyId", this.id);
                dataTable = dbClient.ReadDataTable("SELECT charId FROM chars_server" + GameServer.serverId + " WHERE charId = @id AND familyId = @familyId;");
                if (dataTable.Rows.Count == 1)
                    isMember = true;
            }
            return isMember;
        }

        public void SendFamilyMotto(Player user)
        {
            if (this.motto != "")
                user.SendPacket(GlobalMessage.MakeInfo(this.FamilyMessage()));
        }

        public string FamilyMessage()
        {
            return "--- Family Message ---" + Environment.NewLine
                + this.motto;
        }

        public void SendAtMembersOnline(ServerPacket message)
        {
            lock (this.lockedObject)
            {
                foreach (Player member in this.membersOnline)
                    member.SendPacket(message);
            }
        }

        public void AddMemberOnline(Player player)
        {
            FamilyMember member = this.GetMemberById(player.id);
            lock (this.lockedObject)
            {
                if (this.membersOnline.Contains(player))
                    this.membersOnline.Remove(player);
                this.membersOnline.Add(player);
            }
        }

        public bool MemberOnlineById(int member_id)
        {
            return this.membersOnline.Exists(x => x.id == member_id);
        }

        public FamilyMember GetMemberById(int member_id)
        {
            Player member = this.GetMemberOnlineById(member_id);
            if (member != null)
                return member.fMember;
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                DataTable dataTable = null;
                dbClient.AddParamWithValue("id", member_id);
                dbClient.AddParamWithValue("familyId", this.id);
                dataTable = dbClient.ReadDataTable("SELECT name,level,class,familyEnterDate,familyRank,familyTitle,familyExp,familyIntro FROM chars_server" + GameServer.serverId + " WHERE charId = @id AND familyId = @familyId;");
                if (dataTable.Rows.Count == 1)
                    return new FamilyMember(member_id, (int)dataTable.Rows[0]["familyEnterDate"], (int)dataTable.Rows[0]["level"], GameCrypto.GetRealString(dataTable.Rows[0]["name"].ToString()), Convert.ToInt32(dataTable.Rows[0]["class"].ToString()), (int)dataTable.Rows[0]["familyRank"], (int)dataTable.Rows[0]["familyTitle"], (int)dataTable.Rows[0]["familyExp"], GameCrypto.GetRealString(dataTable.Rows[0]["familyIntro"].ToString()));
            }
            return null;
        }

        public FamilyMember GetMemberByName(string name)
        {
            Player member = this.GetMemberOnlineByName(name);
            if (member != null)
                return member.fMember;
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                DataTable dataTable = null;
                dbClient.AddParamWithValue("name", name);
                dbClient.AddParamWithValue("familyId", this.id);
                dataTable = dbClient.ReadDataTable("SELECT charId,name,level,class,familyEnterDate,familyRank,familyTitle,familyExp,familyIntro FROM chars_server" + GameServer.serverId + " WHERE name = @name AND familyId = @familyId;");
                if (dataTable.Rows.Count == 1)
                    return new FamilyMember((int)dataTable.Rows[0]["charId"], (int)dataTable.Rows[0]["familyEnterDate"], (int)dataTable.Rows[0]["level"], GameCrypto.GetRealString(dataTable.Rows[0]["name"].ToString()), Convert.ToInt32(dataTable.Rows[0]["class"].ToString()), (int)dataTable.Rows[0]["familyRank"], (int)dataTable.Rows[0]["familyTitle"], (int)dataTable.Rows[0]["familyExp"], GameCrypto.GetRealString(dataTable.Rows[0]["familyIntro"].ToString()));
            }
            return null;
        }

        public FamilyMember LoadChief()
        {
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                DataTable dataTable = null;
                dbClient.AddParamWithValue("familyRank", 0);
                dbClient.AddParamWithValue("familyId", this.id);
                dataTable = dbClient.ReadDataTable("SELECT charId,name,level,class,familyEnterDate,familyRank,familyTitle,familyExp,familyIntro FROM chars_server" + GameServer.serverId + " WHERE familyRank = @familyRank AND familyId = @familyId;");
                if (dataTable.Rows.Count == 1)
                    return new FamilyMember((int)dataTable.Rows[0]["charId"], (int)dataTable.Rows[0]["familyEnterDate"], (int)dataTable.Rows[0]["level"], GameCrypto.GetRealString(dataTable.Rows[0]["name"].ToString()), Convert.ToInt32(dataTable.Rows[0]["class"].ToString()), (int)dataTable.Rows[0]["familyRank"], (int)dataTable.Rows[0]["familyTitle"], (int)dataTable.Rows[0]["familyExp"], GameCrypto.GetRealString(dataTable.Rows[0]["familyIntro"].ToString()));
            }
            return null;
        }

        public FamilyMember GetChief()
        {
            return this.chief;
        }

        public Player GetMemberOnlineById(int id)
        {
            return this.membersOnline.Find(x => x.id == id);
        }

        public Player GetMemberOnlineByName(string name)
        {
            return this.membersOnline.Find(x => x.name == name);
        }

        public void SendMembersList(Player user)
        {
            user.SendPacket(this.MembersListPacket());
        }

        public void AddToHis(HistoricItem hisItem)
        {
            this.historic.AddToHis(hisItem);
            this.SendAtMembersOnline(new ServerPacket(Outgoing.hisMustBeActualised));
        }

        public void LoadHis()
        {
            this.historic = new FamilyHistoric();
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                DataTable dataTable = null;
                dbClient.AddParamWithValue("familyId", this.id);
                dataTable = dbClient.ReadDataTable(String.Format("SELECT text,date FROM family_his_server{0} WHERE familyId = @familyId ORDER BY date ASC;", GameServer.serverId));
                foreach (DataRow row in dataTable.Rows)
                    this.historic.AddToHis(new HistoricItem(row["text"].ToString(), (int)row["date"]));
            }
        }

        public void UpFamily()
        {
            this.SendAffichFamilyToAll();
            lock (this.lockedObject)
            {
                foreach (Player member in this.membersOnline)
                    member.SendPacket(GlobalMessage.MakeAlert(0, String.Format(GameServer.GetLanguage(member.languagePack, "message.familyup"), this.level)));
            }
            this.AddToHis(new HistoricItem(String.Format("5|{0}", this.level), GameServer.Timestamp()));
        }

        public ServerPacket MembersListPacket()
        {
            ServerPacket packet = new ServerPacket(Outgoing.familyMembers);
            packet.AppendInt(0);
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                DataTable dataTable = null;
                dbClient.AddParamWithValue("familyId", this.id);
                dataTable = dbClient.ReadDataTable("SELECT charId,name,level,class,familyEnterDate,familyRank,familyTitle,familyExp,familyIntro FROM chars_server" + GameServer.serverId + " WHERE familyId = @familyId;");
                foreach (DataRow row in dataTable.Rows)
                {
                    FamilyMember fMember = new FamilyMember((int)row["charId"], (int)row["familyEnterDate"], (int)row["level"], GameCrypto.GetRealString(row["name"].ToString()), Convert.ToInt32(row["class"].ToString()), (int)row["familyRank"], (int)row["familyTitle"], (int)row["familyExp"], GameCrypto.GetRealString(row["familyIntro"].ToString()));
                    packet.AppendString(fMember.GetMemberForListing(Convert.ToInt32(this.MemberOnlineById(fMember.GetMemberId()))));
                }
            }
            return packet;
        }

        public void AddToSendConnexions(int id)
        {
            lock (this.newOnlineSend)
                this.newOnlineSend.Add(id);
        }

        public void SendAllNewsConnexions()
        {
            if (this.newOnlineSend.Count > 0)
            {
                ServerPacket packet = new ServerPacket(Outgoing.familyUpdateConnexion);
                List<int> connexionSended = new List<int>();
                lock (this.newOnlineSend)
                {
                    foreach (int id in this.newOnlineSend)
                    {
                        FamilyMember member = this.GetMemberById(id);
                        lock (this.lockedObject)
                        {
                            foreach (Player m in this.membersOnline)
                                m.SendPacket(GlobalMessage.MakeMessage(0, 0, 10, String.Format(GameServer.GetLanguage(m.languagePack, "message.familynewconnection"), member.GetName(), member.GetRankString(m.languagePack))));
                        }
                        int isOnline = Convert.ToInt32(this.MemberOnlineById(member.GetMemberId()));
                        packet.AppendString(member.GetStatutOnline(isOnline));
                        connexionSended.Add(id);
                    }
                }
                foreach (int id in connexionSended)
                    this.newOnlineSend.Remove(id);
                this.SendAtMembersOnline(packet);
            }
        }

        public ServerPacket GetMembersIntro()
        {
            ServerPacket packet = new ServerPacket(Outgoing.familyMembersIntro);
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                DataTable dataTable = null;
                dbClient.AddParamWithValue("familyId", this.id);
                dataTable = dbClient.ReadDataTable("SELECT charId,name,level,class,familyEnterDate,familyRank,familyTitle,familyExp,familyIntro FROM chars_server" + GameServer.serverId + " WHERE familyId = @familyId;");
                foreach (DataRow row in dataTable.Rows)
                {
                    FamilyMember fMember = new FamilyMember((int)row["charId"], (int)row["familyEnterDate"], (int)row["level"], GameCrypto.GetRealString(row["name"].ToString()), Convert.ToInt32(row["class"].ToString()), (int)row["familyRank"], (int)row["familyTitle"], (int)row["familyExp"], GameCrypto.GetRealString(row["familyIntro"].ToString()));
                    if (fMember.GetIntroForPacket() != "")
                        packet.AppendString(fMember.GetIntroForPacket());
                }
            }
            return packet;
        }

        public void SendMembersIntro(Player user)
        {
            user.SendPacket(this.GetMembersIntro());
        }

        public void MemberSay(Player sender, string message)
        {
            ServerPacket packetSameChannel = GlobalMessage.MakeMessage(-1, 1, 6, String.Format("[{0}]:{1}", sender.name, message));
            ServerPacket speakEntitie = GlobalMessage.MakeSpeak(1, sender.id, 1, sender.name, message);
            lock (this.lockedObject)
            {
                foreach (Player member in this.membersOnline)
                {
                    if (member.GetSession().GetChannel() == sender.GetSession().GetChannel())
                        member.SendPacket(packetSameChannel);
                    else
                        member.SendPacket(GlobalMessage.MakeMessage(-1, 1, 6, String.Format(GameServer.GetLanguage(member.languagePack, "message.familysayoc"), sender.GetSession().GetChannel().id, sender.name, message)));
                    if (member.map == sender.map)
                        member.SendPacket(speakEntitie);
                }
            }
        }

        public void SendDisconnexion()
        {
            ServerPacket packet = new ServerPacket(Outgoing.familyUpdateConnexion);
            string disconnection = "";
            List<Player> deletedPlayers = new List<Player>();
            List<Player> players = GameServer.GetPlayersManager().GetPlayersList();
            lock (this.lockedObject)
            {
                foreach (Player player in this.membersOnline)
                {
                    FamilyMember member = this.GetMemberById(player.id);
                    if (!players.Exists(x => x.id == member.GetMemberId() && x.family != null && x.family == this))
                    {
                        disconnection += String.Format(" {0}", member.GetStatutOnline(0));
                        deletedPlayers.Add(player);
                    }
                }
            }
            foreach (Player player in deletedPlayers)
            {
                lock (this.membersOnline)
                    this.membersOnline.Remove(player);
            }
            if (disconnection.Length > 3)
            {
                packet.AppendString(disconnection);
                this.SendAtMembersOnline(packet);
            }
        }

        public void UpdateMoto(string motto)
        {
            this.motto = motto;
            if (this.motto != "")
                this.SendAtMembersOnline(GlobalMessage.MakeInfo(this.FamilyMessage()));
        }

        public void UpdateIntroMember(Player member, string intro)
        {
            member.fMember.UpdateIntro(intro);
            member.fMember.SaveMember(this.id);
            this.SendAtMembersOnline(member.fMember.GetIntroPacket());
            this.AddToHis(new HistoricItem(String.Format("1|{0}|{1}", member.name, intro), GameServer.Timestamp()));
            this.SendAtMembersOnline(new ServerPacket(Outgoing.hisMustBeActualised));
        }

        public void SendAffichFamilyToAll()
        {
            lock (this.lockedObject)
            {
                foreach (Player member in this.membersOnline)
                {
                    List<Player> players = member.map.playersManagers.GetPlayersList();
                    lock (players)
                        foreach (Player player in players)
                            player.SendPacket(member.GetFamilyAffichPacket(player.languagePack));
                }
            }
        }

        public void FamilyInfosRequest(Player sender, int i1, int i2)
        {
            FamilyMember member = this.GetMemberById(sender.id);
            if (member == null)
                return;
            switch (i1)
            {
                case 0:
                    {
                        switch (i2)
                        {
                            case 2:
                                this.SendInfos(sender);
                                break;
                        }
                    }
                    break;
            }
        }

        public bool GetPermission(string perm)
        {
            switch (perm)
            {
                case "gardCanInvit":
                    return this.gardCanInvit;
                default:
                    return false;
            }
        }

        public void SendInfosBaseToAll()
        {
            lock (this.lockedObject)
                foreach (Player m in this.membersOnline)
                    this.SendInfos(m);
        }

        public void UpdateMemberRank(Player sender, int rank_id, int member_id)
        {
            if (sender.fMember.GetRank() > 1)
                return;
            if (sender.id == member_id)
                return;
            Player receiver = this.GetMemberOnlineById(member_id);
            FamilyMember fMember = null;
            fMember = this.GetMemberById(member_id);
            if (fMember == null)
                return;
            if (fMember.GetRank() == rank_id)
                return;
            if (fMember.GetRank() <= sender.fMember.GetRank())
            {
                sender.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(sender.languagePack, "error.cantupdfrank")));
                return;
            }
            if (rank_id == 0 && sender.fMember.GetRank() == 0)
            {
                if (fMember.GetRank() != 1)
                {
                    sender.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(sender.languagePack, "error.franknassit")));
                    return;
                }
                else
                {
                    sender.fMember.UpdateRank(1);
                    sender.fMember.SaveMember(this.id);
                    fMember.UpdateRank(rank_id);
                    fMember.SaveMember(this.id);
                    if (sender != null)
                    {
                        this.SendAffichFamilyToAll();
                        this.SendInfos(sender);
                    }
                    if (receiver != null)
                    {
                        this.SendAffichFamilyToAll();
                        this.SendInfos(receiver);
                    }
                    this.chief = this.LoadChief();
                    this.SendAtMembersOnline(this.MembersListPacket());
                    sender.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(sender.languagePack, "message.family.changehead")));
                    this.AddToHis(new HistoricItem(String.Format("9|{0}|1|{0}", sender.name), GameServer.Timestamp()));
                }
            }
            else if (rank_id == 1 && sender.fMember.GetRank() == 0)
            {
                if (this.GetMemberCountByRank(1) >= 2)
                {
                    sender.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(sender.languagePack, "error.family.cantaddassist")));
                    return;
                }
                else
                {
                    fMember.UpdateRank(rank_id);
                    fMember.SaveMember(this.id);
                    if (receiver != null)
                    {
                        this.SendAffichFamilyToAll();
                        this.SendInfos(receiver);
                    }
                    this.SendAtMembersOnline(this.MembersListPacket());
                    sender.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(sender.languagePack, "message.family.newassit")));
                }

            }
            else if (rank_id > 1 && rank_id < 4 && sender.fMember.GetRank() < 2)
            {
                fMember.UpdateRank(rank_id);
                fMember.SaveMember(this.id);
                if (receiver != null)
                {
                    this.SendAffichFamilyToAll();
                    this.SendInfos(receiver);
                }
                this.SendAtMembersOnline(this.MembersListPacket());
                if (rank_id == 2)
                    sender.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(sender.languagePack, "message.family.newguard")));
                else
                    sender.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(sender.languagePack, "message.family.kick")));
            }
            this.AddToHis(new HistoricItem(String.Format("9|{0}|{1}|{2}", sender.name, rank_id, fMember.GetName()), GameServer.Timestamp()));
        }

        public void InvitFamily(Player sender, string invitName)
        {
            if (sender.name == invitName || sender.fMember.GetRank() > 2 && !this.gardCanInvit)
                return;
            if (this.membersCount < this.membersMax)
            {
                Player invitPlayer = sender.GetSession().GetChannel().GetPlayersManager().GetPlayerByName(invitName);
                if (invitPlayer != null)
                {
                    if (invitPlayer.family == null)
                    {
                        if (!sender.familyRequest.Contains(invitPlayer.id))
                            sender.familyRequest.Add(invitPlayer.id);
                        invitPlayer.SendPacket(GlobalMessage.MakeDialog(String.Format("#gjoin^1^{0}", sender.id), String.Format("#gjoin^2^{0}", sender.id), String.Format(GameServer.GetLanguage(invitPlayer.languagePack, "message.receive.familyinvit"), this.name)));
                        sender.SendPacket(GlobalMessage.MakeInfo(String.Format(GameServer.GetLanguage(sender.languagePack, "message.invit.send"), invitName)));
                    }
                    else
                        sender.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(sender.languagePack, "error.family.havefamily")));
                }
                else
                    sender.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(sender.languagePack, "error.character.notfound")));
            }
            else
                sender.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(sender.languagePack, "error.family.full")));
        }

        public void NewMember(Player sender, Player member)
        {
            if (this.membersCount < this.membersMax)
            {
                FamilyMember fMember = new FamilyMember(member.id, 0, member.level, member.name, member.userClass, 3, 0, 0, "");
                if (member != null)
                {
                    lock (this.lockedObject)
                        this.membersOnline.Add(member);
                    member.family = this;
                    member.fMember = fMember;
                    this.SendAffichFamilyToAll();
                }
                fMember.SaveMember(this.id);
                this.UpdateMembersCount();
                this.SendAffichFamilyToAll();
                this.SendAtMembersOnline(this.MembersListPacket());
                this.SendAtMembersOnline(this.GetMembersIntro());
                //SEND EXP
                this.historic.SendHis(member);
                this.SendInfosBaseToAll();
                lock (this.lockedObject)
                {
                    foreach (Player m in this.membersOnline)
                        m.SendPacket(GlobalMessage.MakeAlert(0, String.Format(GameServer.GetLanguage(m.languagePack, "message.family.newmemberalert"), member.name)));
                }
                this.AddToHis(new HistoricItem(String.Format("11|{0}|{1}", sender.name, member.name), GameServer.Timestamp()));
                if (member.faction != this.GetFaction())
                {
                    member.UpdateFaction(this.GetFaction());
                    member.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(member.languagePack, "message.family.updatefaction")));
                }
            }
            else
                member.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(member.languagePack, "error.family.full")));
        }

        public void KickFamily(Player sender, string kickName)
        {
            if (sender.fMember.GetRank() > 1 || sender.name == kickName)
                return;
            FamilyMember fMember = this.GetMemberByName(kickName);
            Player member = this.GetMemberOnlineByName(kickName);
            if (fMember == null)
            {
                sender.SendPacket(GlobalMessage.MakeInfo(String.Format(GameServer.GetLanguage(sender.languagePack, "error.cantfindplayer"), kickName)));
                return;
            }
            if (fMember.GetRank() > sender.fMember.GetRank())
            {
                fMember.UpdateEnterDate(0);
                fMember.UpdateFxp(0);
                fMember.UpdateIntro("");
                fMember.UpdateRank(-1);
                fMember.UpdateTitle(0);
                if (member != null)
                {
                    lock (this.membersOnline)
                        this.membersOnline.Remove(member);
                    member.family = null;
                    member.fMember = fMember;
                    member.map.SendMap(member.GetFamilyAffichPacket(member.languagePack));
                    member.SendPacket(GlobalMessage.MakeModal(0, GameServer.GetLanguage(member.languagePack, "message.family.kick")));
                }
                fMember.SaveMember(-1);
                this.UpdateMembersCount();
                this.SendAffichFamilyToAll();
                this.SendAtMembersOnline(this.MembersListPacket());
                this.SendAtMembersOnline(this.GetMembersIntro());
                //SEND FXP
                this.SendInfosBaseToAll();
                sender.SendPacket(GlobalMessage.MakeModal(0, GameServer.GetLanguage(sender.languagePack, "message.family.kick")));
                this.AddToHis(new HistoricItem(String.Format("10|{0}", fMember.GetName()), GameServer.Timestamp()));
            }
            else
                sender.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(sender.languagePack, "error.family.cantkick")));
        }

        public void LeaveFamily(Player sender)
        {
            if (sender.fMember.GetRank() != 0)
            {
                sender.fMember.UpdateEnterDate(0);
                sender.fMember.UpdateFxp(0);
                sender.fMember.UpdateIntro("");
                sender.fMember.UpdateRank(-1);
                sender.fMember.UpdateTitle(0);
                lock (this.lockedObject)
                    this.membersOnline.Remove(sender);
                sender.family = null;
                sender.map.SendMap(sender.GetFamilyAffichPacket(sender.languagePack));
                sender.SendPacket(GlobalMessage.MakeModal(1, GameServer.GetLanguage(sender.languagePack, "message.leavefamily")));
                sender.fMember.SaveMember(-1);
                this.UpdateMembersCount();
                this.SendAffichFamilyToAll();
                this.SendAtMembersOnline(this.MembersListPacket());
                this.SendAtMembersOnline(this.GetMembersIntro());
                //SEND FXP
                this.SendInfosBaseToAll();
                this.AddToHis(new HistoricItem(String.Format("10|{0}", sender.name), GameServer.Timestamp()));
            }
            else
            {
                lock (this.lockedObject)
                {
                    foreach (Player member in this.membersOnline)
                    {
                        member.fMember.UpdateEnterDate(0);
                        member.fMember.UpdateFxp(0);
                        member.fMember.UpdateIntro("");
                        member.fMember.UpdateRank(-1);
                        member.fMember.UpdateTitle(0);
                        member.family = null;
                        member.map.SendMap(member.GetFamilyAffichPacket(member.languagePack));
                        member.SendPacket(GlobalMessage.MakeModal(1, GameServer.GetLanguage(member.languagePack, "message.family.destroyed")));
                        member.fMember.SaveMember(-1);
                    }
                }
                using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("id", this.id);
                    dbClient.ExecuteQuery("DELETE FROM familys_server" + GameServer.serverId + " WHERE id = @id;");
                    dbClient.ExecuteQuery("DELETE FROM family_his_server" + GameServer.serverId + " WHERE familyId = @id");
                }
                GameServer.DeleteFamily(this);
            }
        }

        public void UpdateRankPermission(Player sender, int rank_id, int permission_id, int value)
        {
            if (sender.fMember.GetRank() > 1)
                return;
            bool permChanged = true;
            switch (rank_id)
            {
                case 2:
                    switch (permission_id)
                    {
                        case 0:
                            if (value == 1)
                                this.gardCanInvit = true;
                            else
                                this.gardCanInvit = false;
                            break;
                        case 1:
                            if (value == 1)
                                this.gardCanAvert = true;
                            else
                                this.gardCanAvert = false;
                            break;
                        case 2:
                            if (value == 1)
                                this.gardCanShout = true;
                            else
                                this.gardCanShout = false;
                            break;
                        case 3:
                            if (value == 1)
                                this.gardCanLookHis = true;
                            else
                                this.gardCanLookHis = false;
                            break;
                        case 4:
                            if (value >= 0 && value < 3)
                                this.gardAccessWarehouse = value;
                            break;
                        default:
                            permChanged = false;
                            break;
                    }
                    break;
                case 3:
                    switch (permission_id)
                    {
                        case 0:
                            if (value == 1)
                                this.memberCanLookHis = true;
                            else
                                this.memberCanLookHis = false;
                            break;
                        case 1:
                            if (value >= 0 && value < 3)
                                this.memberAccessWarehouse = value;
                            break;
                        default:
                            permChanged = false;
                            break;
                    }
                    break;
                default:
                    permChanged = false;
                    break;
            }
            if (permChanged)
            {
                sender.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(sender.languagePack, "message.family.updateperm")));
                switch (rank_id)
                {
                    case 2:
                        {
                            switch (permission_id)
                            {
                                case 3:
                                    this.AddToHis(new HistoricItem(String.Format("8|{0}|{1}|{2}|{3}", sender.name, rank_id, 4, value), GameServer.Timestamp()));
                                    break;
                                case 4:
                                    if (value == 0)
                                        this.AddToHis(new HistoricItem(String.Format("8|{0}|{1}|{2}|{3}", sender.name, rank_id, 7, 1), GameServer.Timestamp()));
                                    else if (value == 1)
                                        this.AddToHis(new HistoricItem(String.Format("8|{0}|{1}|{2}|{3}", sender.name, rank_id, 6, 1), GameServer.Timestamp()));
                                    else if (value == 2)
                                        this.AddToHis(new HistoricItem(String.Format("8|{0}|{1}|{2}|{3}", sender.name, rank_id, 5, 1), GameServer.Timestamp()));
                                    break;
                                default:
                                    this.AddToHis(new HistoricItem(String.Format("8|{0}|{1}|{2}|{3}", sender.name, rank_id, (permission_id + 1), value), GameServer.Timestamp()));
                                    break;
                            }
                        }
                        break;
                    case 3:
                        switch (permission_id)
                        {
                            case 0:
                                this.AddToHis(new HistoricItem(String.Format("8|{0}|{1}|{2}|{3}", sender.name, rank_id, 4, value), GameServer.Timestamp()));
                                break;
                            case 1:
                                if (value == 0)
                                    this.AddToHis(new HistoricItem(String.Format("8|{0}|{1}|{2}|{3}", sender.name, rank_id, 7, 1), GameServer.Timestamp()));
                                else if (value == 1)
                                    this.AddToHis(new HistoricItem(String.Format("8|{0}|{1}|{2}|{3}", sender.name, rank_id, 6, 1), GameServer.Timestamp()));
                                else if (value == 2)
                                    this.AddToHis(new HistoricItem(String.Format("8|{0}|{1}|{2}|{3}", sender.name, rank_id, 5, 1), GameServer.Timestamp()));
                                break;
                        }
                        break;
                }

            }
        }

        public void FamilyCommand(Player sender, string data)
        {
            if (data.Length < 2)
                return;
            string[] decoupeCommand = data.Split(' ');
            FamilyMember member = this.GetMemberById(sender.id);
            if (member == null)
                return;
            switch (decoupeCommand[0])
            {
                case "Cridefamille":
                    if (data.Length >= 14 && (member.GetRank() < 2 || member.GetRank() == 2 && this.gardCanShout))
                        lock (this.lockedObject)
                        {
                            foreach (Player m in this.membersOnline)
                                m.SendPacket(GlobalMessage.MakeAlert(0, String.Format(GameServer.GetLanguage(m.languagePack, "message.family.cry"), data.Substring(13).Substring(0, data.Substring(13).Length < 50 ? data.Substring(13).Length : 50))));
                        }
                    break;
                case "Avertissement":
                    if (data.Length >= 14 && (member.GetRank() < 2 || member.GetRank() == 2 && this.gardCanAvert))
                    {
                        this.UpdateMoto(data.Substring(14).Substring(0, data.Substring(14).Length < 50 ? data.Substring(14).Length : 50));
                        this.SendInfos(sender);
                    }
                    break;
                case "Aujourd'hui":
                    if (data.Length >= 12)
                        this.UpdateIntroMember(sender, data.Substring(12).Substring(0, data.Substring(12).Length < 50 ? data.Substring(12).Length : 50));
                    break;
                case "Rejetdefamille":
                    if (data.Length >= 15)
                        this.KickFamily(sender, data.Substring(15));
                    break;
                case "Invitationdefamille":
                    if (data.Length >= 20)
                        this.InvitFamily(sender, data.Substring(20));
                    break;
                case "Congédefamille":
                    if (data.Length == 14)
                        if (sender.fMember.GetRank() > 0)
                            sender.SendPacket(GlobalMessage.MakeDialog("#gleave^1", "#gleave^2", GameServer.GetLanguage(sender.languagePack, "message.family.confirmleave")));
                        else
                            sender.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(sender.languagePack, "error.family.headcantleave")));
                    break;
            }
        }

        public void LookEditIntroBoxe(Player user)
        {
            user.SendPacket(new ServerPacket(Outgoing.familyIntroBoxe));
        }
    }
}
