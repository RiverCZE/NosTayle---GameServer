using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.Core;
using NosTayleGameServer.NosTale.Entities.Players;
using NosTayleGameServer.NosTale.Skills;
using NosTayleGameServer.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Items.Others
{
    public class Specialist
    {
        internal int spId;
        internal int charId;
        internal ItemBase card;
        internal int invPos;
        internal bool inEquip;
        internal bool equiped;
        internal int upgrade;
        internal int wingsType;
        internal int level;
        internal bool isDead;
        internal int fireResistUpgrade;
        internal int waterResistUpgrade;
        internal int ligthResistUpgrade;
        internal int darknessResistUpgrade;
        internal int attackPoints;
        internal int defensePoints;
        internal int elementPoints;
        internal int statPoints;
        internal int totalPoints;

        //SKILLS
        internal Dictionary<int, EntitieSkill> skills;
        internal SkillBar skillBar;

        //POINTS
        internal static List<int> attConso = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 31, 34, 37, 40, 43, 46, 49, 52, 55, 58, 61, 64, 67, 70, 73, 76, 79, 82, 85, 88, 92, 96, 100, 104, 108, 112, 116, 120, 124, 128, 132, 136, 140, 144, 148, 152, 156, 160, 164, 168, 173, 178, 183, 188, 193, 198, 203, 208, 213, 218, 223, 228, 233, 238, 243, 248, 253, 258, 263, 268, 274, 280, 286, 292, 298, 304, 310, 316, 322, 328, 334, 341, 348, 355, 362, 369, 376, 383, 391, 400, 410 };
        internal static List<int> defConso = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40, 42, 44, 46, 48, 51, 54, 57, 60, 63, 66, 69, 72, 75, 78, 81, 85, 89, 93, 97, 101, 105, 109, 113, 117, 121, 125, 129, 133, 137, 141, 145, 149, 153, 157, 161, 166, 171, 176, 181, 186, 191, 196, 201, 206, 211, 216, 221, 226, 231, 236, 242, 248, 254, 260, 266, 272, 278, 284, 290, 297, 304, 311, 318, 325, 332, 339, 346, 353, 360, 368, 376, 384, 392, 400, 410 };
        internal static List<int> eleConso = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40, 43, 46, 49, 52, 55, 58, 61, 64, 67, 70, 74, 78, 82, 86, 90, 94, 98, 102, 106, 110, 115, 120, 125, 130, 135, 140, 145, 150, 155, 160, 165, 170, 175, 180, 185, 190, 195, 200, 205, 210, 216, 222, 228, 234, 240, 246, 252, 258, 264, 270, 277, 284, 291, 298, 305, 312, 319, 326, 333, 340, 347, 354, 361, 368, 375, 382, 389, 396, 403, 410 };
        internal static List<int> eneConso = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40, 42, 44, 46, 48, 50, 53, 56, 59, 62, 65, 68, 71, 74, 77, 80, 83, 86, 89, 92, 95, 98, 101, 104, 107, 110, 114, 118, 122, 126, 130, 134, 138, 142, 146, 150, 155, 160, 165, 170, 175, 180, 185, 190, 195, 200, 206, 212, 218, 224, 230, 236, 242, 248, 254, 260, 267, 274, 281, 288, 295, 302, 309, 316, 323, 330, 338, 346, 354, 362, 370, 378, 386, 394, 402, 410 };
        internal static List<int> eneAjout = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 52, 54, 56, 58, 60, 62, 64, 66, 68, 70, 72, 74, 76, 78, 80, 82, 84, 86, 88, 90, 92, 94, 96, 98, 100, 102, 104, 106, 108, 110, 112, 114, 116, 118, 120, 122, 124, 126, 128, 130, 132, 134, 136, 138, 140, 142, 144, 146, 148, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150 };

        //SAVE 
        internal bool mustInsert = false;

        public Specialist(int spId)
        {
            DataTable dataTable = null;
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("spId", spId);
                dataTable = dbClient.ReadDataTable("SELECT * FROM sps_server" + GameServer.serverId + " WHERE spId = @spId;");
            }
            if (dataTable.Rows.Count == 1)
            {
                DataRow spRow = dataTable.Rows[0];
                this.spId = (int)spRow["spId"];
                this.charId = (int)spRow["charId"];
                this.card = GameServer.GetItemsManager().itemList[(int)spRow["cardId"]];
                this.invPos = (int)spRow["invPos"];
                this.inEquip = ServerMath.StringToBool(spRow["inEquip"].ToString());
                this.equiped = ServerMath.StringToBool(spRow["equiped"].ToString());
                this.upgrade = (int)spRow["upgrade"];
                this.wingsType = (int)spRow["wingsType"];
                this.level = (int)spRow["level"];
                this.fireResistUpgrade = (int)spRow["fireResistUpgrade"];
                this.waterResistUpgrade = (int)spRow["waterResistUpgrade"];
                this.ligthResistUpgrade = (int)spRow["ligthResistUpgrade"];
                this.darknessResistUpgrade = (int)spRow["darknessResistUpgrade"];
                this.attackPoints = (int)spRow["attackPoints"];
                this.defensePoints = (int)spRow["defensePoints"];
                this.elementPoints = (int)spRow["elementPoints"]; ;
                this.statPoints = (int)spRow["statPoints"];
                this.skillBar = new SkillBar(spRow["sBar"].ToString());
                this.totalPoints = this.CalculePoints();
                this.skills = this.SkillsInitialize();
            }
            else
            {
                this.spId = -1;
                this.upgrade = 0;
                this.level = 0;
                this.fireResistUpgrade = 0;
                this.waterResistUpgrade = 0;
                this.ligthResistUpgrade = 0;
                this.darknessResistUpgrade = 0;
                this.attackPoints = 0;
                this.defensePoints = 0;
                this.elementPoints = 0;
                this.statPoints = 0;
                this.totalPoints = 0;
                this.card = new ItemBase(-1);
                this.skillBar = new SkillBar("");
            }
        }

        public Specialist(Specialist sp)
        {
            this.spId = sp.spId;
            this.charId = sp.charId;
            this.card = sp.card;
            this.invPos = sp.invPos;
            this.inEquip = sp.inEquip;
            this.equiped = sp.equiped;
            this.upgrade = sp.upgrade;
            this.wingsType = sp.wingsType;
            this.level = sp.level;
            this.fireResistUpgrade = sp.fireResistUpgrade;
            this.waterResistUpgrade = sp.waterResistUpgrade;
            this.ligthResistUpgrade = sp.ligthResistUpgrade;
            this.darknessResistUpgrade = sp.darknessResistUpgrade;
            this.attackPoints = sp.attackPoints;
            this.defensePoints = sp.defensePoints;
            this.elementPoints = sp.elementPoints;
            this.statPoints = sp.statPoints;
            this.totalPoints = sp.CalculePoints();
            this.mustInsert = sp.mustInsert;
            this.skills = sp.skills;
            this.skillBar = sp.skillBar;
        }

        public Specialist(int spId, int charId, int cardId, bool inEquip = true, int upgrade = 0, int wingsType = 0, int level = 0)
        {
            this.spId = spId;
            this.charId = charId;
            this.card = GameServer.GetItemsManager().itemList[cardId];
            this.inEquip = inEquip;
            this.upgrade = upgrade;
            this.wingsType = wingsType;
            this.level = level;
            this.totalPoints = this.CalculePoints();
            this.skills = this.SkillsInitialize();
            this.mustInsert = true;
            this.skillBar = new SkillBar("");
        }

        public int CalculePoints()
        {
            int result = 0;
            int total_points = (this.level > 20 ? this.level * 3 : 0) + (new UpgradeSystem.SpUpgrade(this.upgrade).points);
            result = total_points - attConso[this.attackPoints] - defConso[this.defensePoints] - eleConso[this.elementPoints] - eneConso[this.statPoints];
            return result;
        }

        public void UpdatePoints(Player user, int att, int def, int ele, int stat)
        {
            this.totalPoints = this.CalculePoints();
            if (attConso.IndexOf(attConso[this.attackPoints] + att) > 100 || defConso.IndexOf(defConso[this.defensePoints] + def) > 100 || eleConso.IndexOf(eleConso[this.elementPoints] + ele) > 100 || eneConso.IndexOf(eneConso[this.statPoints] + stat) > 100)
            {
                return;
            }
            if (this.totalPoints >= (att + def + ele + stat))
            {
                this.attackPoints = attConso.IndexOf(attConso[this.attackPoints] + att);
                this.defensePoints = defConso.IndexOf(defConso[this.defensePoints] + def);
                this.elementPoints = eleConso.IndexOf(eleConso[this.elementPoints] + ele);
                this.statPoints = eneConso.IndexOf(eneConso[this.statPoints] + stat);
            }
            this.totalPoints = this.CalculePoints();
            user.SendPacket(this.GetInfos(2));
            user.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(user.languagePack, "message.specialist.newstatu")));
            user.SendStatPoints();
        }

        public ServerPacket GetInfos(int page)
        {
            ServerPacket packet = new ServerPacket(Outgoing.spInfo);
            packet.AppendInt(page);
            packet.AppendInt(this.card.id);
            packet.AppendInt(this.card.displayNum);
            packet.AppendInt(this.level);
            packet.AppendInt(this.card.jobLevelReq);
            packet.AppendInt(this.card.icoReputReq);
            packet.AppendInt(0);
            packet.AppendInt(this.card.speed);
            packet.AppendInt(0);
            packet.AppendInt(0);
            packet.AppendInt(0);
            packet.AppendInt(0);
            packet.AppendInt(0);
            packet.AppendInt(3);
            packet.AppendInt(this.card.fireResist);
            packet.AppendInt(this.card.waterResist);
            packet.AppendInt(this.card.ligthResist);
            packet.AppendInt(this.card.darknessResist);
            packet.AppendInt(0);
            packet.AppendInt(10000000);
            packet.AppendString(this.GetSkills());
            packet.AppendString("-1");
            packet.AppendInt(this.spId);
            packet.AppendInt(this.totalPoints);
            packet.AppendInt(this.attackPoints);
            packet.AppendInt(this.defensePoints);
            packet.AppendInt(this.elementPoints);
            packet.AppendInt(this.statPoints);
            packet.AppendInt(this.upgrade);
            packet.AppendInt(-1);
            packet.AppendInt(12);
            packet.AppendInt((this.isDead ? 1 : 0));
            packet.AppendString("0 0 0 0 0 0 0 0 0 0 0 0 0");
            return packet;
        }

        public string GetSkills()
        {
            string iSkills = "-1";
            if (this.skills.Count > 1)
            {
                iSkills = "";
                for (int i = 1; i < this.skills.Count; i++)
                {
                    iSkills += this.skills[i].skillBase.skillId;
                    if (this.skills.Count > i + 1)
                        iSkills += ".";
                }
            }
            return iSkills;
        }

        public void SaveSP(int charId, int accountId, int inWareHouse, int equiped, int slot, int inEquip)
        {
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("sBar", this.skillBar.SBarToDbText());
                if (this.mustInsert)
                {
                    try
                    {
                        dbClient.ExecuteQuery("INSERT INTO `sps_server" + GameServer.serverId + "`(`spId`, `charId`, `accountId`, `cardId`, `inWareHouse`, `invPos`, `inEquip`, `equiped`, `upgrade`, `wingsType`, `level`, `fireResistUpgrade`, `waterResistUpgrade`, `ligthResistUpgrade`, `darknessResistUpgrade`, `attackPoints`, `defensePoints`, `elementPoints`, `statPoints`, `sbar`) VALUES ('" + this.spId + "','" + charId + "', '" + accountId + "','" + this.card.id + "','" + inWareHouse + "','" + slot + "','" + inEquip + "','" + equiped + "','" + this.upgrade + "','" + this.wingsType + "','" + this.level + "','" + this.fireResistUpgrade + "','" + this.waterResistUpgrade + "','" + this.ligthResistUpgrade + "','" + this.darknessResistUpgrade + "','" + this.attackPoints + "','" + this.defensePoints + "','" + this.elementPoints + "','" + this.statPoints + "',@sbar)");
                        this.mustInsert = false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                else
                {
                    dbClient.ExecuteQuery("UPDATE sps_server" + GameServer.serverId + " SET " +
                        "charId = '" + charId + "', " +
                        "accountId = '" + accountId + "', " +
                        "inWareHouse = '" + inWareHouse + "', " +
                        "invPos = '" + slot + "', " +
                        "inEquip = '" + inEquip + "', " +
                        "equiped = '" + equiped + "', " +
                        "upgrade = '" + this.upgrade + "', " +
                        "wingsType = '" + this.wingsType + "', " +
                        "level = '" + this.level + "', " +
                        "attackPoints = '" + this.attackPoints + "', " +
                        "defensePoints = '" + this.defensePoints + "', " +
                        "elementPoints = '" + this.elementPoints + "', " +
                        "statPoints = '" + this.statPoints + "', " +
                        "sBar = @sBar " +
                        "WHERE spId = '" + this.spId + "' LIMIT 1");
                }
            }
        }

        internal Dictionary<int, EntitieSkill> SkillsInitialize()
        {
            Dictionary<int, EntitieSkill> skillsList = new Dictionary<int, EntitieSkill>();
            if (this.card.skills != null)
            {
                if (this.card.skills.ContainsKey(0))
                    skillsList.Add(0, new EntitieSkill(0, SkillsManager.skills[this.card.skills[0]]));
                if (this.card.skills.ContainsKey(1))
                    skillsList.Add(1, new EntitieSkill(1, SkillsManager.skills[this.card.skills[1]]));
                if (this.card.skills.ContainsKey(2))
                    skillsList.Add(2, new EntitieSkill(2, SkillsManager.skills[this.card.skills[2]]));
                if (this.card.skills.ContainsKey(3))
                    skillsList.Add(3, new EntitieSkill(3, SkillsManager.skills[this.card.skills[3]]));
                if (this.card.skills.ContainsKey(4))
                    skillsList.Add(4, new EntitieSkill(4, SkillsManager.skills[this.card.skills[4]]));
                if (this.card.skills.ContainsKey(5))
                    skillsList.Add(5, new EntitieSkill(5, SkillsManager.skills[this.card.skills[5]]));
                for (int i = 6; i < this.card.skills.Count; i++)
                {
                    if (this.level >= (20 / (this.card.skills.Count - 6)) * (i - 5))
                    {
                        skillsList.Add(i, new EntitieSkill(i, SkillsManager.skills[this.card.skills[i]]));
                    }
                }
            }
            return skillsList;
        }

        public ServerPacket SkillsPacket()
        {
            ServerPacket packet = new ServerPacket(Outgoing.skillList);
            if (this.skills.Count > 0)
            {
                packet.AppendInt(this.skills[0].skillBase.skillId);
                packet.AppendInt(this.skills[0].skillBase.skillId);
                packet.AppendInt(this.skills[0].skillBase.skillId);
                for (int i = 1; i < this.skills.Count; i++)
                    packet.AppendInt(this.skills[i].skillBase.skillId);
            }
            return packet;
        }
    }
}
