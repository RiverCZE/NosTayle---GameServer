using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Familys
{
    class FamilyMember
    {
        private int member_id;
        private int member_enterdate;
        private int member_level;
        private string member_name;
        private int member_class;
        private int member_rank;
        private int member_title;
        private int member_exp;
        private string member_intro;

        public FamilyMember(int member_id, int member_enterdate, int member_level, string member_name, int member_class, int member_rank, int member_title, int member_exp, string member_intro)
        {
            this.member_id = member_id;
            this.member_enterdate = member_enterdate;
            this.member_level = member_level;
            this.member_name = member_name;
            this.member_class = member_class;
            this.member_rank = member_rank;
            this.member_title = member_title;
            this.member_exp = member_exp;
            this.member_intro = member_intro;
        }

        public int GetMemberId()
        {
            return this.member_id;
        }

        public string GetName()
        {
            return this.member_name;
        }

        public int GetMemberRank()
        {
            return this.member_rank;
        }

        public string GetMemberForListing(int online)
        {
            return String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}", this.member_id, this.member_enterdate, this.member_name, this.member_level,
                this.member_class, this.member_rank, this.member_title, online);
        }

        public string GetStatutOnline(int online)
        {
            return String.Format("{0}|{1}", this.member_id, online);
        }

        public int GetRank()
        {
            return this.member_rank;
        }

        public string GetRankString(string languagePack)
        {
            switch (this.member_rank)
            {
                case 0:
                    return String.Format("({0})", GameServer.GetLanguage(languagePack, "rank.family.head"));
                case 1:
                    return String.Format("({0})", GameServer.GetLanguage(languagePack, "rank.family.assist"));
                case 2:
                    return String.Format("({0})", GameServer.GetLanguage(languagePack, "rank.family.guard"));
                default:
                    return String.Format("({0})", GameServer.GetLanguage(languagePack, "rank.family.member"));
            }
        }

        public string GetIntroForPacket()
        {
            if (this.member_intro == "")
                return "";
            else
                return String.Format("{0}|{1}", this.member_id, this.member_intro.Replace(' ', (char)11));
        }

        public ServerPacket GetIntroPacket()
        {
            ServerPacket packet = new ServerPacket(Outgoing.familyMembersIntro);
            packet.AppendString(String.Format("{0}|{1}", this.member_id, this.member_intro.Replace(' ', (char)11)));
            return packet;
        }

        public string GetIntro()
        {
            return this.member_intro;
        }

        public int GetEnterDate()
        {
            return this.member_enterdate;
        }

        public int GetTitle()
        {
            return this.member_title;
        }

        public int GetFxp()
        {
            return this.member_exp;
        }

        public void UpdateEnterDate(int enterdate) { this.member_enterdate = enterdate; }

        public void UpdateTitle(int title) { this.member_title = title; }

        public void UpdateFxp(int fxp) { this.member_exp = fxp; }

        public void UpdateIntro(string intro) { this.member_intro = intro; }

        public void UpdateRank(int rank) { this.member_rank = rank; }

        public void SaveMember(int familyId)
        {
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("charId", this.GetMemberId());
                dbClient.AddParamWithValue("familyId", familyId);
                dbClient.AddParamWithValue("familyEnterDate", this.GetEnterDate());
                dbClient.AddParamWithValue("familyRank", this.GetRank());
                dbClient.AddParamWithValue("familyTitle", this.GetTitle());
                dbClient.AddParamWithValue("familyExp", this.GetFxp());
                dbClient.AddParamWithValue("familyIntro", this.GetIntro());
                dbClient.ExecuteQuery("UPDATE chars_server" + GameServer.serverId + " SET "
                + "familyId = @familyId,"
                + "familyEnterDate = @familyEnterDate,"
                + "familyRank = @familyRank,"
                + "familyTitle = @familyTitle,"
                + "familyExp = @familyExp,"
                + "familyIntro = @familyIntro"
                + " WHERE charId = @charId LIMIT 1");
            }

        }
    }
}
