using NosTayleGameServer.Core;
using NosTayleGameServer.NosTale.Items.Others;
using NosTayleGameServer.NosTale.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Items
{
    public class ItemBase
    {
        internal int id;
        internal string name;
        internal int inventory;
        internal int eqSlot;
        internal string type;
        internal bool coloredItem;
        internal int displayNum;
        internal int element;
        internal int percentMin;
        internal int percentMax;
        internal int damageMin;
        internal int damageMax;
        internal int corpDef;
        internal int distDef;
        internal int magicDef;
        internal int dodge;
        internal int hitRate;
        internal int criticChance;
        internal int criticDamage;
        internal int fireResist;
        internal int waterResist;
        internal int ligthResist;
        internal int darknessResist;
        internal int speed;
        internal int price;
        internal int levelReq;
        internal int jobLevelReq;
        internal int icoReputReq;
        internal int effect;
        internal bool adventer;
        internal bool sword;
        internal bool mage;
        internal bool archer;
        internal bool deleteOnUse;

        //EXTRADATA PARAMETERS
        internal int upgradeHp;
        internal int upgradeMp;
        internal int upgradeFireRes;
        internal int upgradeWaterRes;
        internal int upgradeLigthRes;
        internal int upgradeDarkRes;
        internal int upgradeSpeed;
        internal int timeOut;
        internal int mlZone;
        internal int height;
        internal int width;
        internal int gameId;
        internal int gameLevel;
        internal int wSlot;
        internal int wId;
        internal int wingsId;

        //Locomotions
        internal Locomotion locomotion;

        //Sp skills
        public Dictionary<int, int> skills;

        public bool isSp
        {
            get
            {
                if (type == "user_specialist")
                    return true;
                return false;
            }
        }

        public ItemBase(int id, string name, int inventory, int eqSlot, string type, bool coloredItem, int displayNum, int element, int percentMin, int percentMax, int damageMin, int damageMax, int corpDef, int distDef, int magicDef, int dodge, int hitRate, int criticChance, int criticDamage, int fireResist, int waterResist, int ligthResist, int darknessResist, int speed, int price, int levelReq, int jobLevelReq, int icoReputReq, string classes, int effect, bool deleteOnUse, string extraData)
        {
            this.id = id;
            this.name = name;
            this.inventory = inventory;
            this.eqSlot = eqSlot;
            this.coloredItem = coloredItem;
            this.type = type;
            this.displayNum = displayNum;
            this.element = element;
            this.percentMin = percentMin;
            this.percentMax = percentMax;
            this.damageMin = damageMin;
            this.damageMax = damageMax;
            this.corpDef = corpDef;
            this.distDef = distDef;
            this.magicDef = magicDef;
            this.dodge = dodge;
            this.hitRate = hitRate;
            this.criticChance = criticChance;
            this.criticDamage = criticDamage;
            this.fireResist = fireResist;
            this.waterResist = waterResist;
            this.ligthResist = ligthResist;
            this.darknessResist = darknessResist;
            this.speed = speed;
            this.price = price;
            this.levelReq = levelReq;
            this.jobLevelReq = jobLevelReq;
            this.icoReputReq = icoReputReq;
            this.adventer = ServerMath.StringToBool(classes.Split('.')[0]);
            this.sword = ServerMath.StringToBool(classes.Split('.')[1]);
            this.archer = ServerMath.StringToBool(classes.Split('.')[2]);
            this.mage = ServerMath.StringToBool(classes.Split('.')[3]);
            this.effect = effect;
            this.deleteOnUse = deleteOnUse;
            this.ExtraDataCut(extraData);
            if (this.type == "locomotion")
                this.locomotion = Locomotion.GetLocomotion(this.displayNum);
            if (this.isSp)
            {
                if (SkillsManager.spsSkills.ContainsKey(this.displayNum))
                    this.skills = SkillsManager.spsSkills[this.displayNum];
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("No skills for SP: {0}", this.displayNum);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }

        public ItemBase(int id)
        {
            this.id = -1;
            this.name = "-1";
            this.inventory = -1;
            this.eqSlot = -1;
            this.displayNum = -1;
            this.damageMin = 0;
            this.damageMax = 0;
            this.criticChance = 0;
            this.criticDamage = 0;
            this.price = 0;
            this.levelReq = 0;
            this.adventer = true;
            this.sword = true;
            this.mage = true;
            this.archer = true;
            this.effect = -1;
            this.deleteOnUse = false;
            this.element = 0;
        }

        public void ExtraDataCut(string extraData)
        {
            string[] data = extraData.Split('^');
            foreach (string dataCom in data)
            {
                if (dataCom.Split(':').Length == 2)
                {
                    string option = dataCom.Split(':')[0];
                    string value = dataCom.Split(':')[1];
                    switch (option)
                    {
                        case "upgradeHP":
                            this.upgradeHp = Convert.ToInt32(value);
                            break;
                        case "upgradeMP":
                            this.upgradeMp = Convert.ToInt32(value);
                            break;
                        case "upgradeResGeneral":
                            upgradeFireRes += Convert.ToInt32(value);
                            upgradeWaterRes += Convert.ToInt32(value);
                            upgradeLigthRes += Convert.ToInt32(value);
                            upgradeDarkRes += Convert.ToInt32(value);
                            break;
                        case "upgradeFireResist":
                            upgradeFireRes += Convert.ToInt32(value);
                            break;
                        case "upgradeWaterResist":
                            upgradeWaterRes += Convert.ToInt32(value);
                            break;
                        case "upgradeLigthResist":
                            upgradeLigthRes += Convert.ToInt32(value);
                            break;
                        case "upgradeDarkResist":
                            upgradeDarkRes += Convert.ToInt32(value);
                            break;
                        case "upgradeSpeed":
                            upgradeSpeed += Convert.ToInt32(value);
                            break;
                        case "timeOut":
                            timeOut = Convert.ToInt32(value);
                            break;
                        case "mlZone":
                            mlZone = Convert.ToInt32(value);
                            break;
                        case "height":
                            height = Convert.ToInt32(value);
                            break;
                        case "width":
                            width = Convert.ToInt32(value);
                            break;
                        case "gameId":
                            gameId = Convert.ToInt32(value);
                            break;
                        case "gameLevel":
                            gameLevel = Convert.ToInt32(value);
                            break;
                        case "wSlot":
                            wSlot = Convert.ToInt32(value);
                            break;
                        case "wid":
                            wId = Convert.ToInt32(value);
                            break;
                        case "wingsId":
                            wingsId = Convert.ToInt32(value);
                            break;
                    }
                }
            }
        }
    }
}
