using NosTayleGameServer.Communication;
using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.Core;
using NosTayleGameServer.NosTale.Buffs;
using NosTayleGameServer.NosTale.Entities.Npcs;
using NosTayleGameServer.NosTale.Entities.Players.FriendBlackList;
using NosTayleGameServer.NosTale.Entities.Players.Inventorys;
using NosTayleGameServer.NosTale.Familys;
using NosTayleGameServer.NosTale.Groups;
using NosTayleGameServer.NosTale.Items;
using NosTayleGameServer.NosTale.Items.Others;
using NosTayleGameServer.NosTale.Maps;
using NosTayleGameServer.NosTale.Maps.SpecialMaps;
using NosTayleGameServer.NosTale.Missions.Quests;
using NosTayleGameServer.NosTale.Skills;
using NosTayleGameServer.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NosTayleGameServer.NosTale.Entities.Players
{
    public class Player : Entitie, GlobalFunctions
    {
        private Session session;
        internal int faction;
        internal Family family;
        internal int gender;
        internal int hairStyle;
        internal int hairColor;
        internal int userClass;
        internal int jobLevel;
        internal Map map;
        internal int saveMap;
        internal int saveX;
        internal int saveY;
        internal bool spInUsing;
        internal bool isGm;
        internal bool gmActivate;
        internal bool isVisible = true;
        internal int attack1_type;
        internal int attack2_type;
        internal Item mask;
        internal Item weapon;
        internal Item weapon2;
        internal Item armor;
        internal Item chap;
        internal Specialist sp;
        internal Fairy fairy;
        internal Item glove;
        internal Item boot;
        internal Item costumeHead;
        internal Item costumeBody;
        internal string pMessage = "Pas de message de présentation.";
        internal int speed = 11;
        internal bool speedUpdated;
        internal int lastPortal = 0;
        internal int reputation;
        internal int digniter;
        internal int admirationPoints;
        internal int gold;
        internal bool winArena = false;
        internal Inventory inventory;
        internal int spWaitSecond = 0;
        internal DateTime lastSpWaitUpdate;
        internal DateTime lastStatThread = DateTime.Now;
        internal int lastPulse;
        internal DateTime lastPulseTime;
        internal int compPoints = 20000;
        internal DateTime lastMorph;
        internal DateTime connectedAt;
        internal int onlineTime;

        //Skills
        internal Dictionary<int, EntitieSkill> skills;
        internal SkillBar skillBar;

        //MiniLand
        internal Miniland miniLand;
        internal int gamePoints = 2000;
        internal Npc mlPanel = null;
        internal Npc pet = null;

        //Locomotions
        internal Locomotion locomotion;

        //Arena
        internal bool canPvp;
        internal int arenaKill;
        internal int arenaDead;
        internal int arenaKillNow;
        internal int arenaDeadNow;

        //MODERATION
        internal bool isMuted;

        //OTHER
        internal int action = -1;
        internal List<int> groupRequest = new List<int>();
        internal List<int> friendRequest = new List<int>();
        internal List<int> familyRequest = new List<int>();
        internal bool easySpUp = false;
        internal bool inMorph = false;
        internal int morphId;
        //internal List<SpecialEffect> specialEffects;
        internal FamilyMember fMember;
        internal bool familyDestroyed;
        internal string languagePack
        {
            get
            {
                return this.session.GetAccount().languagePack;
            }
        }
        internal int avatarScale = 10;
        internal int lastSendMessagesNumber;
        internal DateTime antiFloodTime;

        //MANAGERS
        internal FriendsList friendList;
        internal Group group;

        //delegates
        internal delegate void lastStatSend();
        private lastStatSend getStat;

        //Protection BETA
        internal DateTime upItemTime;

        //Quètes
        internal bool needFirstQuest;
        internal PersonalQuestManager questManager;


        public Player(Session session, int id, string name, int faction, int familyId, int gender, int hairStyle, int hairColor, int userClass, int level, int jobLevel, string skills, int map, int x, int y, bool spInUsing, bool isGm, int weaponId, int weapon2Id, int armorId, int maskId, int fairyId, int chapId, int spId, int gloveId, int bootId, int costumeBodyId, int costumeHeadId, int speed, int currentHp, int currentMp, int exp, int expJob, int reputation, int digniter, int gold, string sBar, string pMessage, bool winArena, int mlStatue, int mlVisitesTotal, int mlVisitesDay, string mlMotto, string mlLastUpdateVisites, int arenaKill, int arenaDead, int admirationPoints, bool isMuted, int onlineTime)
        {
            this.lastPulseTime = DateTime.Now;
            this.session = session;
            this.id = id;
            this.type = 1;
            this.name = name;
            this.faction = faction;
            if (familyId != -1)
                this.family = GameServer.GetFamily(familyId);
            if (familyId != -1 && this.family == null)
                this.familyDestroyed = true;
            this.gender = gender;
            this.hairStyle = hairStyle;
            this.hairColor = hairColor;
            this.userClass = userClass;
            this.level = level;
            this.jobLevel = jobLevel;
            if (!session.GetChannel().generalMaps.ContainsKey(map))
                session.GetChannel().generalMaps.Add(map, new Map(map, session.GetChannel()));
            this.skills = this.SkillsInitialize(userClass, skills);
            this.map = session.GetChannel().generalMaps[map];
            this.mapId = this.map.mapId;
            this.x = x;
            this.y = y;
            this.spInUsing = spInUsing;
            this.isGm = isGm;
            this.gmActivate = true;
            this.attack1_type = attack_type(this.userClass, 1);
            this.attack2_type = attack_type(this.userClass, 2);
            this.mask = new Item(maskId, this.id);
            this.weapon = new Item(weaponId, this.id);
            this.weapon2 = new Item(weapon2Id, this.id);
            this.armor = new Item(armorId, this.id);
            this.chap = new Item(chapId, this.id);
            this.sp = new Specialist(spId);
            this.fairy = new Fairy(fairyId);
            this.glove = new Item(gloveId, this.id);
            this.boot = new Item(bootId, this.id);
            this.costumeBody = new Item(costumeBodyId, this.id);
            this.costumeHead = new Item(costumeHeadId, this.id);
            this.speed = speed;
            this.currentHp = (currentHp > 0 ? currentHp : 1);
            this.currentMp = (currentMp > 0 ? currentMp : 1);
            this.maxHp = ServerMath.RealHp(this.userClass, this.level) + this.weapon.itemBase.upgradeHp + this.armor.itemBase.upgradeHp;
            this.maxMp = ServerMath.RealMp(this.userClass, this.level) + this.weapon.itemBase.upgradeMp + this.armor.itemBase.upgradeMp;
            this.exp = exp;
            this.expJob = expJob;
            this.reputation = reputation;
            this.digniter = digniter;
            this.gold = gold;
            this.skillBar = new SkillBar(sBar);
            this.winArena = winArena;
            this.pMessage = pMessage;
            this.friendList = new FriendsList(this.id);
            this.friendList.Initialize();
            this.inventory = new Inventory();

            this.miniLand = new Miniland(this, mlStatue, mlMotto, mlVisitesDay, mlVisitesTotal, mlLastUpdateVisites);
            this.arenaDead = arenaDead;
            this.arenaKill = arenaKill;
            this.admirationPoints = admirationPoints;
            //this.specialEffects = new List<SpecialEffect>();
            this.isMuted = isMuted;
            this.onlineTime = onlineTime;
            this.questManager = new PersonalQuestManager(this.id);
            if (this.onlineTime == 0 && this.userClass == 0)
            {
                if (this.weapon.id == -1)
                {
                    this.weapon = new Item(GameServer.NewItemId(), this.id, 1, 20, 28, 10, 0, 0, 0, 0);
                    this.weapon.mustInsert = true;
                }
                if (this.weapon2.id == -1)
                {
                    this.weapon2 = new Item(GameServer.NewItemId(), this.id, 8, 10, 30, 8, 0, 0, 0, 0);
                    this.weapon2.mustInsert = true;
                }
                if (this.armor.id == -1)
                {
                    this.armor = new Item(GameServer.NewItemId(), this.id, 12, 0, 0, 0, 10, 10, 10, 15);
                    this.armor.mustInsert = true;
                }
            }
            this.connectedAt = DateTime.Now;

            this.cibles = new List<Entitie>();

            //FAMILY
            this.LoadFMember();

            //PETS
            if (this.GetSession().GetAccount().name == "Mehdy")
                this.pet = new Npc(100000, 1433, "-", 2, 99, 10, 10, this.map.mapId, this.x + 1, this.y - 1, 2, 2500, 2500, 4, 4000, 200, 200, 200, 200, 20000, 40000, 2000, 2500, 2500, 2500, 2000, 0, false, false, false, false, 0, 0, "", 0, null, 0, null, "", this, this.map, true);

            //CREATE DELEGATE 
            this.getStat = new lastStatSend(SendStat);

            this.antiFloodTime = DateTime.Now;
        }

        public int attack_type(int userClass, int type)
        {
            if (type == 1)
            {
                switch (userClass)
                {
                    case 0:
                        return 0;
                    case 1:
                        return 0;
                    case 2:
                        return 1;
                    case 3:
                        return 2;
                }
            }
            else if (type == 2)
            {
                switch (userClass)
                {
                    case 0:
                        return 1;
                    case 1:
                        return 1;
                    case 2:
                        return 0;
                    case 3:
                        return 1;
                }
            }
            return 0;
        }

        private void SendStat()
        {
            TimeSpan timeSpan = DateTime.Now - lastStatThread;
            if (!this.isDead && !this.GetSession().GetSock().isDisconnected() && Math.Round((2500 - timeSpan.TotalMilliseconds)) <= 0)
            {
                this.SendStats(true);
                this.lastStatThread = DateTime.Now;
            }
        }

        #region Méthodes pour voir les informations sur le joueur
        public Session GetSession() { return this.session; }
        public int GetId() { return this.id; }
        public string GetName() { return this.name; }
        public FriendsList GetFriendList() { return this.friendList; }
        public bool InBatle()
        {
            if (DateTime.Now.Subtract(this.lastAttack).TotalSeconds >= 15)
                return false;
            return true;
        }
        #endregion

        #region Méthodes sur le socket/session
        public void SendPacket(ServerPacket packet)
        {
            this.session.GetSock().SendData(packet.GetBytes());
        }
        public void SendPacket(byte[] packet)
        {
            this.session.GetSock().SendData(packet);
        }
        public Item GetEquipeBySlot(int eqSlot)
        {
            switch (eqSlot)
            {
                case 0:
                    return this.weapon;
                case 1:
                    return this.armor;
                case 2:
                    return this.chap;
                case 3:
                    return this.glove;
                case 4:
                    return this.boot;
                case 5:
                    return this.weapon2;
                case 9:
                    return this.mask;
                case 13:
                    return this.costumeBody;
                case 14:
                    return this.costumeHead;
                default:
                    return new Item(-1, this.id);
            }
        }
        #endregion

        #region Envoie d'informations
        public void SendTit()
        {
            ServerPacket packet = new ServerPacket(Outgoing.tit);
            packet.AppendString((this.userClass == 0 ? "Aventurier" : (this.userClass == 1 ? "Épéiste" : (this.userClass == 2 ? "Archer" : "Sorcier"))));
            packet.AppendString(this.name);
            this.SendPacket(packet);
        }
        public void SendPlaceOnMap()
        {
            ServerPacket packet = new ServerPacket(Outgoing.placeUser);
            packet.AppendInt(this.id);
            packet.AppendInt(this.map.mapId);
            packet.AppendInt(this.x);
            packet.AppendInt(this.y);
            packet.AppendInt(2);
            packet.AppendInt(0);
            packet.AppendInt(0);
            packet.AppendInt(1);
            this.SendPacket(packet);
        }
        public void SendCharacterReputation()
        {
            ServerPacket packet = new ServerPacket(Outgoing.fd);
            packet.AppendInt(this.reputation);
            packet.AppendInt(ServerMath.ConvertReputToIco(this.reputation));
            packet.AppendInt(this.digniter);
            packet.AppendInt(ServerMath.ConvertDigniToIco(this.digniter));
            this.SendPacket(packet);
        }
        public void SendCharacterInfo()
        {
            ServerPacket packet = new ServerPacket(Outgoing.cInfo);
            packet.AppendString(this.name);
            packet.AppendString("-");
            packet.AppendInt((this.group != null ? 1 : -1));
            packet.AppendInt(this.family == null ? -1 : this.family.GetId());
            packet.AppendString(this.family == null ? "-" : this.family.MakeNameAndGrade(this.fMember, this.languagePack));
            packet.AppendInt(this.id);
            packet.AppendInt(((this.isGm && this.gmActivate) ? 2 : 0));
            packet.AppendInt(this.gender);
            packet.AppendInt(this.hairStyle);
            packet.AppendInt(this.hairColor);
            packet.AppendInt(this.userClass);
            packet.AppendInt(ServerMath.ConvertReputToIco(this.reputation));
            packet.AppendInt(this.admirationPoints);
            packet.AppendInt(this.GetMorph());
            packet.AppendInt(0);
            packet.AppendInt(this.family != null ? this.family.GetLevel() : 0);
            packet.AppendInt(this.locomotion == null ? this.spInUsing ? sp.upgrade : 0 : 0);
            packet.AppendInt(this.locomotion == null ? this.spInUsing ? sp.wingsType : 0 : 0);
            packet.AppendInt((this.winArena ? 1 : 0));
            this.SendPacket(packet);
        }
        public void SendEquipment()
        {
            ServerPacket packet = new ServerPacket(Outgoing.equip);
            packet.AppendString(this.weapon.upgrade + "" + this.weapon.rare);
            packet.AppendString(this.armor.upgrade + "" + this.armor.rare);

            if (this.weapon.id != -1)
                packet.AppendString("0." + this.weapon.itemBase.id + "." + this.weapon.rare + "." + this.weapon.upgrade + ".0");
            if (this.armor.id != -1)
                packet.AppendString("1." + this.armor.itemBase.id + "." + this.armor.rare + "." + this.armor.upgrade + ".0");
            if (this.chap.id != -1)
                packet.AppendString("2." + this.chap.itemBase.id + ".0." + this.chap.color + ".0");
            if (this.glove.id != -1)
                packet.AppendString("3." + this.glove.itemBase.id + ".0." + this.glove.upgrade + ".0");
            if (this.boot.id != -1)
                packet.AppendString("4." + this.boot.itemBase.id + ".0." + this.boot.upgrade + ".0");
            if (this.weapon2.id != -1)
                packet.AppendString("5." + this.weapon2.itemBase.id + "." + this.weapon2.rare + "." + this.weapon2.upgrade + ".0");
            if (this.mask.id != -1)
                packet.AppendString("9." + this.mask.itemBase.id + ".0.0.0");
            if (this.fairy.fairyId != -1)
                packet.AppendString("10." + this.fairy.itemBase.id + ".0.0.0");
            if (this.sp.spId != -1)
                packet.AppendString("12." + this.sp.card.id + ".0." + this.sp.upgrade + ".0");
            if (this.costumeBody.id != -1)
                packet.AppendString("13." + this.costumeBody.itemBase.id + ".0.0.0");
            if (this.costumeHead.id != -1)
                packet.AppendString("14." + this.costumeHead.itemBase.id + ".0.0.0");
            this.SendPacket(packet);
        }
        public void SendLevel()
        {
            ServerPacket packet = new ServerPacket(Outgoing.levelStat);
            packet.AppendInt(this.level);
            packet.AppendInt(this.exp);
            packet.AppendInt((!this.spInUsing ? this.jobLevel : this.sp.level));
            packet.AppendInt((!this.spInUsing ? this.expJob : 0));
            packet.AppendDouble(GameServer.GetLevelsManager().Levels[this.level]);
            packet.AppendDouble((!this.spInUsing ? GameServer.GetLevelsManager().jobLevels[this.jobLevel] : 1000000));
            packet.AppendInt(this.reputation);
            packet.AppendInt(this.compPoints);
            this.SendPacket(packet);
        }
        public void SendStats(bool regen)
        {
            if (regen)
            {
                if (this.currentHp < this.maxHp)
                {
                    this.currentHp += (int)((this.level) * (ServerMath.StringToBool(this.rested.ToString()) ? 3 : (!this.InBatle() ? 1.5 : 0)));
                    if (this.currentHp > this.maxHp)
                        this.currentHp = this.maxHp;
                }
                else if (this.currentHp > this.maxHp)
                    this.currentHp = this.maxHp;
                if (this.currentMp < this.maxMp)
                {
                    this.currentMp += (int)((this.level) * (ServerMath.StringToBool(this.rested.ToString()) ? 3 : (!this.InBatle() ? 1.5 : 0)));
                    if (this.currentMp > this.maxMp)
                        this.currentMp = this.maxMp;
                }
                else if (this.currentMp > this.maxMp)
                    this.currentMp = this.maxMp;
            }
            else
            {
                if (this.currentHp > this.maxHp)
                    this.currentHp = this.maxHp;
                if (this.currentMp > this.maxMp)
                    this.currentMp = this.maxMp;
            }
            ServerPacket packet = new ServerPacket(Outgoing.playerStat);
            packet.AppendInt(this.currentHp);
            packet.AppendInt(this.maxHp);
            packet.AppendInt(this.currentMp);
            packet.AppendInt(this.maxMp);
            packet.AppendInt(0);
            packet.AppendInt(0);
            this.SendPacket(packet);
        }
        public void SendStatPoints()
        {
            this.ReCalculStatPoint();
            ServerPacket packet = new ServerPacket(Outgoing.personnalStat);
            //ARME1
            packet.AppendInt(this.attack1_type);
            packet.AppendInt(this.attackUpgrade1);
            packet.AppendInt(this.damageMin1);
            packet.AppendInt(this.damageMax1);
            packet.AppendInt(this.hitRate1);
            packet.AppendInt(this.criticRate1);
            packet.AppendInt(this.criticDamage1);

            //ARME2
            packet.AppendInt(this.attack2_type);
            packet.AppendInt(this.attackUpgrade2);
            packet.AppendInt(this.damageMin2);
            packet.AppendInt(this.damageMax2);
            packet.AppendInt(this.hitRate2);
            packet.AppendInt(this.criticRate2);
            packet.AppendInt(this.criticDamage2);

            //ARMURE
            packet.AppendInt(this.defenseUpgrade);
            packet.AppendInt(this.corpDef);
            packet.AppendInt(this.dodge);
            packet.AppendInt(this.distDef);
            packet.AppendInt(this.dodge);
            packet.AppendInt(this.magicDef);

            //RESISTANCES
            packet.AppendInt(this.fireResist);
            packet.AppendInt(this.waterResist);
            packet.AppendInt(this.lightResist);
            packet.AppendInt(this.darknessResist);
            this.SendPacket(packet);

            this.UpdateHpMp();
            this.SendStats(false);
        }
        public void SendSpeed()
        {
            this.SendPacket(this.GetSpeedPacket());
        }
        public void SendFairy()
        {
            if (this.fairy.fairyId != -1)
                this.SendPacket(this.FairyPacket());
        }
        public void SendFaction()
        {
            ServerPacket packet = new ServerPacket(Outgoing.faction);
            packet.AppendInt(this.faction);
            this.SendPacket(packet);
        }
        public void LoadFamily(bool firstConnexion = false)
        {
            this.LoadFMember();
            if (this.family != null && this.family.IsMember(this))
            {
                if (this.faction != this.family.GetFaction())
                {
                    this.UpdateFaction(this.family.GetFaction());
                    this.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(this.languagePack, "message.family.updatefaction")));
                }
                this.family.SendInfos(this);
                this.family.SendMembersList(this);
                if (firstConnexion)
                {
                    this.family.AddMemberOnline(this);
                    this.family.AddToSendConnexions(this.id);
                    this.family.GetHis().SendHis(this);
                }
                this.family.SendMembersIntro(this);
            }
            this.SendFaction();
        }
        public void SendFamilyAffich(Player receiver)
        {
            receiver.SendPacket(this.GetFamilyAffichPacket(receiver.languagePack));
        }
        public void SendEffect(int type, int id, int effect)
        {
            ServerPacket packet = new ServerPacket(Outgoing.effectPacket);
            packet.AppendInt(type);
            packet.AppendInt(id);
            packet.AppendInt(effect);
            this.SendPacket(packet);
        }
        public void SendGuri(string infos)
        {
            ServerPacket packet = new ServerPacket(Outgoing.guri);
            packet.AppendString(infos);
            packet.AppendInt(this.id);
            this.SendPacket(packet);
        }
        public void SendGold()
        {
            ServerPacket packet = new ServerPacket(Outgoing.gold);
            packet.AppendInt(this.gold);
            packet.AppendInt(0);
            this.SendPacket(packet);
        }
        public void SendPlayerIn(Player receiver)
        {

            this.SendPacket(receiver.GetInEntitiePacket(this.languagePack));
            if (receiver.pet != null && receiver.locomotion == null)
                this.SendPacket(receiver.pet.NpcPacket(this.languagePack));
            lock (receiver.buffs)
            {
                if (receiver.buffs.Exists(x => x.baseBuff.viewForm))
                    this.SendPacket(GlobalMessage.MakeGuri(0, receiver.type, receiver.id, receiver.buffs.FirstOrDefault(x => x.baseBuff.viewForm).baseBuff.guriStart));
            }
            receiver.SendFamilyAffich(this);

            receiver.SendPacket(this.GetInEntitiePacket(receiver.languagePack));
            if (this.pet != null && this.locomotion == null)
                receiver.SendPacket(this.pet.NpcPacket(receiver.languagePack));
            lock(this.buffs)
            {
                if (this.buffs.Exists(x => x.baseBuff.viewForm))
                    receiver.SendPacket(GlobalMessage.MakeGuri(0, this.type, this.id, this.buffs.FirstOrDefault(x => x.baseBuff.viewForm).baseBuff.guriStart));
            }
            this.SendFamilyAffich(receiver);
        }
        public void LookEquipment(int equipmentSlot)
        {
            switch (equipmentSlot)
            {
                case 0:
                    if (this.weapon != null && this.weapon.id != -1)
                        this.weapon.GetEquipInfo(this);
                    break;
                case 1:
                    if (this.armor != null && this.armor.id != -1)
                        this.armor.GetEquipInfo(this);
                    break;
                case 2:
                    if (this.chap != null && this.chap.id != -1)
                        this.chap.GetEquipInfo(this);
                    break;
                case 3:
                    if (this.glove != null && this.glove.id != -1)
                        this.glove.GetEquipInfo(this);
                    break;
                case 4:
                    if (this.boot != null && this.boot.id != -1)
                        this.boot.GetEquipInfo(this);
                    break;
                case 5:
                    if (this.weapon2 != null && this.weapon2.id != -1)
                        this.weapon2.GetEquipInfo(this);
                    break;
                case 9:
                    if (this.mask != null && this.mask.id != -1)
                        this.mask.GetEquipInfo(this);
                    break;
                case 10:
                    if (this.fairy != null && this.fairy.fairyId != -1)
                        this.fairy.GetInfo(this);
                    break;
                case 12:
                    if (this.sp != null && this.sp.spId != -1)
                        this.SendPacket(sp.GetInfos(0));
                    break;
                case 13:
                    if (this.costumeBody != null && this.costumeBody.id != -1)
                        this.costumeBody.GetEquipInfo(this);
                    break;
                case 14:
                    if (this.costumeHead != null && this.costumeHead.id != -1)
                        this.costumeHead.GetEquipInfo(this);
                    break;
            }
        }
        public void SendPoints()
        {
            ServerPacket packet = new ServerPacket("mlpt");
            packet.AppendInt(this.gamePoints);
            packet.AppendInt(100);
            this.SendPacket(packet);
        }
        public void SendSkillPacket()
        {
            if (!this.spInUsing)
            {
                ServerPacket packet = new ServerPacket(Outgoing.skillList);
                packet.AppendInt(this.skills[0].skillBase.skillId);
                packet.AppendInt(this.skills[1].skillBase.skillId);
                foreach (EntitieSkill pSkill in this.skills.Values)
                    packet.AppendInt(pSkill.skillBase.skillId);
                packet.AppendInt(235);
                this.SendPacket(packet);
            }
            else
                this.SendPacket(this.sp.SkillsPacket());
        }
        #endregion

        #region Récupération d'informations
        public bool UnEquiped()
        {
            if (this.weapon.id == -1 && this.weapon2.id == -1 && this.armor.id == -1 && this.sp.spId == -1 && this.fairy.fairyId == -1 && this.mask.id == -1 && this.glove.id == -1 && this.boot.id == -1 && this.chap.id == -1 && this.costumeBody.id == -1 && this.costumeHead.id == -1)
                return true;
            return false;
        }
        public int GetMorph()
        {
            if (this.locomotion != null)
                return this.locomotion.GetMorphByGender(this.gender);
            else if (this.spInUsing)
                return this.sp.card.displayNum;
            else if (this.inMorph)
                return this.morphId;
            else
                return 0;
        }
        public ServerPacket GetEqPacket()
        {
            ServerPacket packet = new ServerPacket(Outgoing.eq);
            packet.AppendInt(this.id);
            packet.AppendInt(((this.isGm && this.gmActivate) ? 2 : 0));
            packet.AppendInt(this.gender);
            packet.AppendInt(this.hairStyle);
            packet.AppendInt(this.GetRealColor());
            packet.AppendInt(this.userClass);
            packet.AppendString(this.chap.itemBase.id + "." + this.armor.itemBase.id + "." + this.weapon.itemBase.id + "." + this.weapon2.itemBase.id + "." + this.mask.itemBase.id + "." + this.fairy.itemBase.id + "." + this.costumeBody.itemBase.id + "." + this.costumeHead.itemBase.id);
            packet.AppendString(this.weapon.upgrade.ToString() + this.weapon.rare.ToString());
            packet.AppendString(this.armor.upgrade.ToString() + this.armor.rare.ToString());
            return packet;
        }
        public int GetRealColor()
        {
            if (this.chap.id != -1 && this.costumeHead.id == -1)
                if (this.chap.itemBase.coloredItem)
                    return chap.color;
            return this.hairColor;
        }
        public ServerPacket FairyPacket()
        {
            ServerPacket packet = new ServerPacket(Outgoing.pairy);
            packet.AppendInt(1);
            packet.AppendInt(this.id);
            packet.AppendInt(4);
            packet.AppendInt(this.fairy.itemBase.element);
            packet.AppendInt(this.fairy.level);
            packet.AppendInt(this.fairy.itemBase.displayNum);
            return packet;
        }
        public void SendSpPoints()
        {
            if (this.sp.spId != -1)
            {
                ServerPacket packet = new ServerPacket(Outgoing.spPoints);
                packet.AppendInt(1000000);
                packet.AppendInt(1000000);
                packet.AppendInt(10000);
                packet.AppendInt(10000);
                this.SendPacket(packet);
            }
        }
        public ServerPacket GetSpeedPacket()
        {
            if (!this.speedUpdated)
                this.GetRealSpeed();
            ServerPacket packet = new ServerPacket(Outgoing.speedCond);
            packet.AppendInt(1);
            packet.AppendInt(this.id);
            packet.AppendInt(0);
            packet.AppendInt(0);
            packet.AppendInt(this.speed);
            return packet;
        }
        public ServerPacket GetFamilyAffichPacket(string languagePack)
        {
            ServerPacket packet = new ServerPacket(Outgoing.familyAffich);
            packet.AppendInt(this.type);
            packet.AppendInt(this.id);
            packet.AppendInt(this.family == null ? -1 : this.family.GetId());
            packet.AppendString(this.family == null ? "-" : this.family.MakeNameAndGrade(this.fMember, languagePack));
            packet.AppendInt(this.family == null ? 0 : this.family.GetLevel());
            return packet;
        }
        public ServerPacket GetInEntitiePacket(string languagePack)
        {
            ServerPacket packet = new ServerPacket(Outgoing.inEntitie);
            packet.AppendInt(1);
            packet.AppendString(this.name);
            packet.AppendString("-");
            packet.AppendInt(this.id);
            packet.AppendInt(this.x);
            packet.AppendInt(this.y);
            packet.AppendInt(this.direction);
            packet.AppendInt(this.isGm && this.gmActivate ? 2 : 0);
            packet.AppendInt(this.gender);
            packet.AppendInt(this.hairStyle);
            packet.AppendInt(this.GetRealColor());
            packet.AppendInt(this.userClass);
            packet.AppendString(this.chap.itemBase.id + "." + this.armor.itemBase.id + "." + this.weapon.itemBase.id + "." + this.weapon2.itemBase.id + "." + this.mask.itemBase.id + "." + this.fairy.itemBase.id + "." + this.costumeBody.itemBase.id + "." + this.costumeHead.itemBase.id);
            packet.AppendInt(ServerMath.Percent(this.currentHp, this.maxHp));
            packet.AppendInt(ServerMath.Percent(this.currentMp, this.maxMp));
            packet.AppendInt(this.rested);
            packet.AppendInt(this.group != null ? 1 : -1);
            packet.AppendInt(4);
            packet.AppendInt((this.fairy.fairyId != -1 ? this.fairy.itemBase.element : 0));
            packet.AppendInt(0);
            packet.AppendInt((this.fairy.fairyId != -1 ? this.fairy.itemBase.displayNum : 0));
            packet.AppendInt(0);
            packet.AppendInt(this.GetMorph());
            packet.AppendString(this.weapon.upgrade.ToString() + this.weapon.rare.ToString());
            packet.AppendString(this.armor.upgrade.ToString() + this.armor.rare.ToString());
            packet.AppendInt(this.family == null ? -1 : this.family.GetId());
            packet.AppendString(this.family == null ? "-" : this.family.MakeNameAndGrade(this.fMember, languagePack));
            packet.AppendInt(ServerMath.ConvertReputToIco(this.reputation));
            packet.AppendInt(0);
            packet.AppendInt(this.locomotion == null ? this.spInUsing ? this.sp.upgrade : 0 : 0);
            packet.AppendInt(0);
            packet.AppendInt(this.locomotion == null ? this.spInUsing ? this.sp.wingsType : 0 : 0);
            packet.AppendInt(this.level);
            packet.AppendInt(this.family == null ? 0 : this.family.GetLevel());
            packet.AppendInt(this.winArena ? 1 : 0);
            packet.AppendInt(this.admirationPoints);
            packet.AppendInt(this.GetScale());
            return packet;
        }
        public ServerPacket GetInfoWindow(string languagePack)
        {
            ServerPacket packet = new ServerPacket(Outgoing.entitieInfo);
            packet.AppendInt(this.level);
            packet.AppendString(this.name);
            packet.AppendInt(this.fairy.itemBase.element);
            packet.AppendInt(this.fairy.level);
            packet.AppendInt(this.userClass);
            packet.AppendInt(this.gender);
            packet.AppendInt(this.family != null ? this.family.GetId() : -1);
            packet.AppendString(this.family != null ? this.family.MakeNameAndGrade(this.fMember, languagePack) : "-");
            packet.AppendInt(ServerMath.ConvertReputToIco(this.reputation));
            packet.AppendInt(1);
            packet.AppendInt(((this.weapon.id != -1) ? 1 : 0));
            packet.AppendInt(this.weapon.rare);
            packet.AppendInt(this.weapon.upgrade);
            packet.AppendInt(((this.weapon2.id != -1) ? 1 : 0));
            packet.AppendInt(this.weapon2.rare);
            packet.AppendInt(this.weapon2.upgrade);
            packet.AppendInt(((this.armor.id != -1) ? 1 : 0));
            packet.AppendInt(this.armor.rare);
            packet.AppendInt(this.armor.upgrade);
            packet.AppendInt(this.arenaKill); //ACT4KILL
            packet.AppendInt(this.arenaDead); //ACT4DEAD
            packet.AppendInt(this.reputation);
            packet.AppendInt(0);
            packet.AppendInt(0);
            packet.AppendInt(0);
            packet.AppendInt(this.GetMorph());
            packet.AppendInt(0); //Arènes GG
            packet.AppendInt(0); //Arènes perdu
            packet.AppendInt(0); //Arènes abandon
            packet.AppendInt(0); //Points arène des maitres
            packet.AppendInt(10); //Entrés arène des maitres
            packet.AppendInt(this.admirationPoints);
            packet.AppendInt(0); //ACT4 POINTS
            packet.AppendInt(0); //HONORABLE ARME
            packet.AppendInt(0); //HONORABLE ARME2
            packet.AppendInt(0); //HONORABLE ARMURE
            packet.AppendString(this.pMessage);
            return packet;
        }
        public int GetScale()
        {
            return this.avatarScale;
        }
        public void SendArenaScore(bool onArena)
        {
            ServerPacket packet = new ServerPacket(Outgoing.arenaScore);
            packet.AppendInt(this.arenaKill);
            packet.AppendInt(this.arenaDead);
            packet.AppendInt(0);
            packet.AppendInt(this.arenaKillNow);
            packet.AppendInt(this.arenaDeadNow);
            packet.AppendInt(0);
            packet.AppendInt(0);
            packet.AppendInt(0);
            packet.AppendInt((onArena ? 0 : -1));
            this.SendPacket(packet);
        }
        public void SendSBar()
        {
            if (!this.spInUsing)
                this.skillBar.SendBarToUser(this);
            else
                this.sp.skillBar.SendBarToUser(this);
        }
        public ServerPacket GetMorphPacket()
        {
            ServerPacket packet = new ServerPacket(Outgoing.morph);
            packet.AppendInt(1);
            packet.AppendInt(this.id);
            packet.AppendInt(this.GetMorph());
            packet.AppendInt(this.locomotion != null ? 0 : this.spInUsing ? this.sp.upgrade : 0);
            packet.AppendInt(this.locomotion != null ? 0 : this.spInUsing ? this.sp.wingsType : 0);
            packet.AppendInt(this.winArena ? 1 : 0);
            return packet;
        }
        public Item GetWeapon(int type)
        {
            switch (type)
            {
                case 2:
                    return this.weapon2;
                default:
                    return this.weapon;
            }
        }
        public ServerPacket GetArenaScore(bool onArena)
        {
            ServerPacket packet = new ServerPacket(Outgoing.arenaScore);
            packet.AppendInt(this.arenaKill);
            packet.AppendInt(this.arenaDead);
            packet.AppendInt(0);
            packet.AppendInt(this.arenaKillNow);
            packet.AppendInt(this.arenaDeadNow);
            packet.AppendInt(0);
            packet.AppendInt(0);
            packet.AppendInt(0);
            packet.AppendInt((onArena ? 0 : -1));
            return packet;
        }
        #endregion

        #region Calcule d'informations
        public void UpdateHpMp()
        {
            this.maxHp = ServerMath.RealHp(this.userClass, this.level);
            this.maxMp = ServerMath.RealMp(this.userClass, this.level);
            if (this.spInUsing)
            {
                this.maxHp = this.maxHp + (int)((double)this.maxHp * (double)(Specialist.eneAjout[this.sp.statPoints] / 100.0));
                this.maxMp = this.maxMp + (int)((double)this.maxMp * (double)(Specialist.eneAjout[this.sp.statPoints] / 100.0));
            }
            this.maxHp += this.weapon.itemBase.upgradeHp + this.armor.itemBase.upgradeHp;
            this.maxMp += this.weapon.itemBase.upgradeMp + this.armor.itemBase.upgradeMp;
        }
        public void ReCalculStatPoint()
        {
            this.element_type = 0;
            this.element = 0;
            if (this.fairy.fairyId != -1)
            {
                this.element_type = this.fairy.itemBase.element;
                this.element = this.fairy.level;
            }
            this.fireResist = 0;
            this.waterResist = 0;
            this.lightResist = 0;
            this.darknessResist = 0;
            if (this.spInUsing)
            {
                this.fireResist += this.sp.card.fireResist;
                this.waterResist += this.sp.card.waterResist;
                this.lightResist += this.sp.card.ligthResist;
                this.darknessResist += this.sp.card.darknessResist;
            }
            if (!this.speedUpdated)
                this.GetRealSpeed();
            this.SendSpeed();

            this.damageMin1 = this.weapon.damageMin + ((this.level - 8 < 0) ? 0 : this.level - 8);
            this.damageMax1 = this.weapon.damageMax + ((this.level - 8 < 0) ? 0 : this.level - 8);
            this.hitRate1 = this.weapon.hitRate + (this.level * 2 + 54);
            this.criticRate1 = this.weapon.itemBase.criticChance;
            this.criticDamage1 = this.weapon.itemBase.criticDamage;
            this.attackUpgrade1 = this.weapon.upgrade;

            this.damageMin2 = this.weapon2.damageMin + ((this.level - 71 < 0) ? 0 : this.level - 71);
            this.damageMax2 = this.weapon2.damageMax + ((this.level - 71 < 0) ? 0 : this.level - 71);
            this.hitRate2 = this.weapon2.hitRate + ((this.level * 2) + 58);
            this.criticRate2 = this.weapon2.itemBase.criticChance;
            this.criticDamage2 = this.weapon2.itemBase.criticDamage;
            this.attackUpgrade2 = this.weapon2.upgrade;

            this.defenseUpgrade = this.armor.upgrade;
            this.corpDef = (this.armor.corpDef + this.level + 1) + this.glove.itemBase.corpDef + this.boot.itemBase.corpDef;
            this.dodge = (this.armor.dodge + this.level + 29) + this.glove.itemBase.dodge + this.boot.itemBase.dodge;
            this.distDef = (this.armor.distDef + ((this.level - 31 < 0) ? 0 : this.level - 31)) + this.glove.itemBase.distDef + this.boot.itemBase.distDef;
            this.magicDef = (this.armor.magicDef + ((this.level - 39 < 0) ? 0 : this.level - 39)) + this.glove.itemBase.magicDef + this.boot.itemBase.magicDef;

            this.fireResist += (this.glove.itemBase.fireResist * (1 + this.glove.upgrade)) + (this.boot.itemBase.fireResist * (1 + this.boot.upgrade));
            this.waterResist += (this.glove.itemBase.waterResist * (1 + this.glove.upgrade)) + (this.boot.itemBase.waterResist * (1 + this.boot.upgrade));
            this.lightResist += (this.glove.itemBase.ligthResist * (1 + this.glove.upgrade)) + (this.boot.itemBase.ligthResist * (1 + this.boot.upgrade));
            this.darknessResist += (this.glove.itemBase.darknessResist * (1 + this.glove.upgrade)) + (this.boot.itemBase.darknessResist * (1 + this.boot.upgrade));

            this.fireResist += this.armor.itemBase.upgradeFireRes + this.costumeHead.itemBase.upgradeFireRes + this.costumeBody.itemBase.upgradeFireRes;
            this.waterResist += this.armor.itemBase.upgradeWaterRes + this.costumeHead.itemBase.upgradeWaterRes + this.costumeBody.itemBase.upgradeWaterRes;
            this.lightResist += this.armor.itemBase.upgradeLigthRes + this.costumeHead.itemBase.upgradeLigthRes + this.costumeBody.itemBase.upgradeLigthRes;
            this.darknessResist += this.armor.itemBase.upgradeDarkRes + this.costumeHead.itemBase.upgradeDarkRes + this.costumeBody.itemBase.upgradeDarkRes;
        }
        public void GetRealSpeed()
        {
            if (this.locomotion != null)
            {
                this.speed = this.locomotion.speed;
                return;
            }
            int newSpeed = 11;
            if (this.spInUsing)
                newSpeed += this.sp.card.speed;
            newSpeed += this.costumeHead.itemBase.upgradeSpeed + this.costumeBody.itemBase.upgradeSpeed + this.boot.itemBase.speed;
            lock (this.buffs)
            {
                List<PersonalBuff> buffs = this.buffs.FindAll(x => x.baseBuff.speedUp > 0);
                if (buffs != null)
                    foreach (PersonalBuff buff in buffs)
                        newSpeed += buff.baseBuff.speedUp;
            }
            this.speed = newSpeed;
        }
        public void LoadFMember()
        {
            if (this.family != null)
                this.fMember = this.family.GetMemberById(this.id);
            else
                this.fMember = new FamilyMember(this.id, 0, 0, this.name, this.userClass, 3, 0, 0, "");
        }
        public ServerPacket GmPacket()
        {
            ServerPacket packet = new ServerPacket(Outgoing.gmMode);
            packet.AppendInt(this.id);
            packet.AppendInt(this.isGm && this.gmActivate ? 2 : 0);
            return packet;
        }
        internal Dictionary<int, EntitieSkill> SkillsInitialize(int uClass, string skillsText)
        {
            Dictionary<int, EntitieSkill> skillsList = new Dictionary<int, EntitieSkill>();
            skillsList.Add(0, new EntitieSkill(0, SkillsManager.skills[SkillsManager.classSkills[uClass][0]]));
            skillsList.Add(1, new EntitieSkill(1, SkillsManager.skills[SkillsManager.classSkills[uClass][1]]));
            string[] skillsTab = skillsText.Split(';');
            if (skillsText.Length > 0)
            {
                int i = 1;
                foreach (string skill in skillsTab)
                {
                    i++;
                    if (SkillsManager.skills.ContainsKey(Convert.ToInt32(skill)))
                    {
                        skillsList.Add(i, new EntitieSkill(i, SkillsManager.skills[Convert.ToInt32(skill)]));
                        Console.WriteLine(i);
                    }
                }
            }
            return skillsList;
        }
        #endregion

        #region Modification de valeurs
        public void ChangeClass(int newClass)
        {
            if (this.userClass == 0 && newClass > 0 && newClass <= 3)
            {
                if (this.level >= 15 && this.jobLevel >= 20)
                {
                    if (this.UnEquiped())
                    {
                        this.userClass = newClass;
                        //this.jobLevel = 1;
                        //FOR EVENT TEST
                        this.jobLevel = 80;
                        int weaponId = 0;
                        int weapon2Id = 0;
                        int armorId = 0;
                        switch (newClass)
                        {
                            case 1:
                                weaponId = 18;
                                weapon2Id = 68;
                                armorId = 94;
                                break;
                            case 2:
                                weaponId = 32;
                                weapon2Id = 78;
                                armorId = 107;
                                break;
                            case 3:
                                weaponId = 46;
                                weapon2Id = 86;
                                armorId = 120;
                                break;
                        }
                        using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("charId", this.id);
                            int newIdWeapon = GameServer.NewItemId();
                            int newIdArmor = GameServer.NewItemId();
                            int newIdWeapon2 = GameServer.NewItemId();

                            this.weapon = new Item(newIdWeapon, this.id, weaponId, 90, 110, 20, 0, 0, 0, 0, 0);
                            this.armor = new Item(newIdArmor, this.id, armorId, 0, 0, 0, 55, 50, 48, 75, 75);
                            this.weapon2 = new Item(newIdWeapon2, this.id, weapon2Id, 90, 100, 20, 0, 0, 0, 0, 0);
                            this.SendStatPoints();
                            List<Player> players = this.map.playersManagers.GetPlayersList();
                            this.SendTit();
                            this.SendLevel();
                            foreach (Player p in players)
                                p.SendPacket(this.GetEqPacket());
                            this.currentHp = this.maxHp;
                            this.currentMp = this.maxMp;
                            this.SendTit();
                            this.SendStats(false);
                            if (this.family == null)
                                this.UpdateFaction(new Random().Next(1, 3));
                            this.weapon.Save(this.id, this.session.GetAccount().id, 0, 1, 0, 1);
                            this.weapon2.Save(this.id, this.session.GetAccount().id, 0, 1, 0, 1);
                            this.armor.Save(this.id, this.session.GetAccount().id, 0, 1, 0, 1);
                            this.SaveData();
                        }
                    }
                    else
                        this.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(this.languagePack, "error.changeclass.equip")));
                }
                else
                    this.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(this.languagePack, "error.changeclass.level")));
            }
        }
        public void UpdateFaction(int faction)
        {
            this.faction = faction;
            if (faction != 0)
                this.SendEffect(1, this.id, (4799 + this.faction));
        }
        public void UpdatePresentation(string presentation)
        {
            ServerPacket result = this.inventory.DeleteOneItemByType(this, 1, "u_pmessage");
            if (result != null)
            {
                this.pMessage = presentation.Substring(0, (presentation.Length < 60 ? presentation.Length : 60));
                this.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(this.languagePack, "message.updatepresentation")));
                this.SendPacket(result);
            }
        }
        public void PutSp()
        {
            if (this.rested == 1)
                return;
            if (this.sp.spId != -1)
            {
                if (!spInUsing)
                {
                    /*if (this.getSkillsNotCharge())
                    {
                        user.send(Messages.MakeAlert(0, "Tu ne peux te transformer qu'après que les temps d'attente des compétences aient expiré."));
                        return;
                    }*/
                    if (this.sp.card.jobLevelReq > this.jobLevel || this.sp.card.icoReputReq > ServerMath.ConvertReputToIco(this.reputation))
                    {
                        this.SendPacket(GlobalMessage.MakeMessage(0, 0, 10, GameServer.GetLanguage(this.languagePack, "error.char.cantuseitem")));
                        return;
                    }
                    if (this.sp.card.element != this.fairy.itemBase.element && this.fairy.itemBase.element != 0 && this.sp.card.element != 0)
                    {
                        this.SendPacket(GlobalMessage.MakeAlert(4, GameServer.GetLanguage(this.languagePack, "error.sp.notsameelement")));
                        return;
                    }
                    if (DateTime.Now.Subtract(this.lastSpWaitUpdate).TotalSeconds >= this.spWaitSecond)
                    {
                        this.spInUsing = true;
                        this.map.SendMap(this.GetMorphPacket());
                        this.map.SendEffect(1, this.id, 196);
                        this.map.SendGuri(6, 1, this.id, "0 0");
                        this.SendLevel();
                        this.SendStatPoints();
                        this.SendSpeed();
                    }
                    else
                        this.SendPacket(GlobalMessage.MakeAlert(0, String.Format(GameServer.GetLanguage(this.languagePack, "error.sp.cantmorph"), Math.Round(this.spWaitSecond - DateTime.Now.Subtract(this.lastSpWaitUpdate).TotalSeconds))));
                }
                else
                {
                    this.spInUsing = false;
                    this.map.SendMap(this.GetMorphPacket());
                    this.map.SendGuri(6, 1, this.id, "0 0");
                    this.lastSpWaitUpdate = DateTime.Now;
                    //this.spWaitSecond = 30 + this.sp.getSecondsSkillsCharge(true);
                    this.spWaitSecond = 30;
                    this.spWaitSecond = ((int)(this.spWaitSecond / 2) <= 30 ? 30 : (int)(this.spWaitSecond / 2));
                    ServerPacket packet = new ServerPacket(Outgoing.spCharge);
                    packet.AppendInt(spWaitSecond);
                    this.SendPacket(packet);
                    this.SendPacket(GlobalMessage.MakeMessage(this.id, 0, 11, String.Format(GameServer.GetLanguage(this.languagePack, "message.sp.effectd"), this.spWaitSecond)));
                    this.SendLevel();
                    this.SendStatPoints();
                    this.SendSpeed();
                }
                this.lastMorph = DateTime.Now;
                this.SendSkillPacket();
                this.SendSBar();
            }
            else
                this.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(this.languagePack, "error.sp.notcard")));
        }
        public void AddExp(int exp, int exp_job)
        {
            // you can add exp *= 2; or other for update the rate.
            if (this.level == 1 && this.jobLevel == 1 && this.exp == 0 && this.expJob == 0)
            {
                //this.client.send(Messages.MakeMessage(0, 0, 12, "Un bonus te procure une énergie nouvelle."));
                //this.client.send(Messages.MakeMessage(0, 0, 12, "Pour gagner un niveau, tu dois remplir complètement la barre de niveau."));
                exp += exp / 20;
                exp_job += exp / 20;
				exp *= 20;
            }
            if (this.level < 99)
            {
                this.exp += exp;
                if (this.exp >= GameServer.levelsManager.Levels[this.level])
                {
                    this.exp -= (int)GameServer.levelsManager.Levels[this.level];
                    this.level++;
                    //this.client.send(Messages.MakeAlert(0, "Ton niveau a augmenté."));
                    this.map.SendEffect(1, this.id, 6);
                    this.map.SendEffect(1, this.id, 198);
                    this.UpdateHpMp();
                    this.currentHp = this.maxHp;
                    this.currentMp = this.maxMp;
                    this.SendStats(false);
                    this.UpLevel();
                    if (this.level >= 99)
                    {
                        //this.client.send(Messages.MakeAlert(0, "Tu as atteint le niveau maximum."));
                        this.SendLevel();
                        return;
                    }
                    while (this.exp >= GameServer.levelsManager.Levels[this.level])
                    {
                        this.exp -= (int)GameServer.levelsManager.Levels[this.level];
                        this.level++;
                        //this.client.send(Messages.MakeAlert(0, "Ton niveau a augmenté."));
                        this.map.SendEffect(1, this.id, 6);
                        this.map.SendEffect(1, this.id, 198);
                        this.UpdateHpMp();
                        this.currentHp = this.maxHp;
                        this.currentMp = this.maxMp;
                        this.SendStats(false);
                        this.UpLevel();
                        if (this.level >= 99)
                        {
                            //this.client.send(Messages.MakeAlert(0, "Tu as atteint le niveau maximum."));
                            this.SendLevel();
                            return;
                        }
                    }
                }
            }
            if (this.jobLevel < 80)
            {
                this.expJob += exp_job;
                if (this.expJob >= GameServer.levelsManager.jobLevels[this.jobLevel])
                {
                    this.expJob -= (int)GameServer.levelsManager.jobLevels[this.jobLevel];
                    this.jobLevel++;
                    //this.client.send(Messages.MakeAlert(0, "Le niveau de ton métier a augmenté."));
                    this.map.SendEffect(1, this.id, 6);
                    this.map.SendEffect(1, this.id, 198);
                    if (this.jobLevel >= 80)
                    {
                        //this.client.send(Messages.MakeAlert(0, "Tu as atteint le niveau maximum de métier."));
                        this.SendLevel();
                        return;
                    }
                    while (this.expJob >= GameServer.levelsManager.jobLevels[this.jobLevel])
                    {
                        this.expJob -= (int)GameServer.levelsManager.jobLevels[this.jobLevel];
                        this.jobLevel++;
                        //this.client.send(Messages.MakeAlert(0, "Le niveau de ton métier a augmenté."));
                        this.map.SendEffect(1, this.id, 6);
                        this.map.SendEffect(1, this.id, 198);
                        if (this.jobLevel >= 80)
                        {
                            //this.client.send(Messages.MakeAlert(0, "Tu as atteint le niveau maximum de métier."));
                            this.SendLevel();
                            return;
                        }
                    }
                }
            }
            this.SendLevel();
        }
        public void UpLevel()
        {
            if ((this.level < 80 && this.level >= 30 && this.level % 10 == 0) || (this.level >= 80))
                if (this.family != null)
                    this.family.AddToHis(new Familys.FamilyHistorics.HistoricItem(String.Format("6|{0}|{1}", this.name, this.level), GameServer.Timestamp()));
        }
        public void ModScale(int scale) { this.avatarScale = scale; }

        public override void AddBuff(Map map, PersonalBuff buff)
        {
            lock (this.buffs)
            {
                if (this.buffs.Exists(s => s.baseBuff.id == buff.baseBuff.id))
                    this.buffs.Remove(this.buffs.FirstOrDefault(s => s.baseBuff.id == buff.baseBuff.id));
                else if (buff.baseBuff.viewForm)
                    map.SendGuri(0, this.type, this.id, buff.baseBuff.guriStart);
                this.buffs.Add(buff);
            }
            map.SendMap(buff.GetPacket(this.type, this.id, false));
            this.SendPacket(GlobalMessage.MakeMessage(this.id, 1, 20, String.Format(GameServer.GetLanguage(this.languagePack, "message.new.buff"), buff.baseBuff.name)));
            if (buff.baseBuff.speedUp != 0 && this.type == 1)
                ((Player)this).SendSpeed();
        }
        #endregion

        #region Sauvegarde
        public void SaveData()
        {
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                if (this.map.specialMap)
                {
                    dbClient.AddParamWithValue("mapId", this.saveMap);
                    dbClient.AddParamWithValue("x", this.saveX);
                    dbClient.AddParamWithValue("y", this.saveY);
                }
                else
                {
                    dbClient.AddParamWithValue("mapId", this.map.mapId);
                    dbClient.AddParamWithValue("x", this.x);
                    dbClient.AddParamWithValue("y", this.y);
                }
                dbClient.AddParamWithValue("pMessage", this.pMessage);
                dbClient.AddParamWithValue("mlStatue", this.miniLand.statue);
                dbClient.AddParamWithValue("mlVisitesTotal", this.miniLand.visites);
                dbClient.AddParamWithValue("mlVisitesDay", this.miniLand.dayVisites);
                dbClient.AddParamWithValue("mlMotto", this.miniLand.motto);
                dbClient.AddParamWithValue("mlLastUpdateVisites", this.miniLand.lastUpdateVisites);
                dbClient.AddParamWithValue("arenaKill", this.arenaKill);
                dbClient.AddParamWithValue("arenaDead", this.arenaDead);
                dbClient.AddParamWithValue("sBar", this.skillBar.SBarToDbText());
                dbClient.AddParamWithValue("familyIntro", this.family != null ? this.fMember.GetIntro() : "");
                dbClient.AddParamWithValue("muted", this.isMuted ? '1' : '0');
                dbClient.AddParamWithValue("onlineTime", this.onlineTime + (int)(DateTime.Now.Subtract(this.connectedAt).TotalSeconds));
                dbClient.ExecuteQuery("UPDATE chars_server" + GameServer.serverId + " SET "
                + "faction = '" + this.faction + "',"
                + "class = '" + this.userClass + "',"
                + "level = '" + this.level + "',"
                + "joblevel = '" + this.jobLevel + "',"
                + "exp = '" + this.exp + "',"
                + "expJob = '" + this.expJob + "',"
                + "gold = '" + this.gold + "',"
                + "sBar = @sBar, reputation = '" + this.reputation + "',"
                + "digniter = '" + this.digniter + "',"
                + "map = @mapId, x = @x, y = @y, hp = '" + this.currentHp + "',"
                + "mp = '" + this.currentMp + "',"
                + "maskId = '" + this.mask.id + "',"
                + "fairyId = '" + this.fairy.fairyId + "',"
                + "chapId = '" + this.chap.id + "',"
                + "weaponId = '" + this.weapon.id + "',"
                + "weapon2Id = '" + this.weapon2.id + "',"
                + "armorId = '" + this.armor.id + "',"
                + "spId = '" + this.sp.spId + "',"
                + "gloveId = '" + this.glove.id + "',"
                + "bootId = '" + this.boot.id + "',"
                + "costumeBodyId = '" + this.costumeBody.id + "',"
                + "costumeHeadId = '" + this.costumeHead.id + "',"
                + "pMessage = @pMessage,"
                + "familyId = '" + (this.family != null ? this.family.GetId() : -1) + "',"
                + "familyEnterdate = '" + (this.family != null ? this.fMember.GetEnterDate() : 0) + "',"
                + "familyRank = '" + (this.family != null ? this.fMember.GetRank() : -1) + "',"
                + "familyTitle = '" + (this.family != null ? this.fMember.GetTitle() : 0) + "',"
                + "familyExp = '" + (this.family != null ? this.fMember.GetFxp() : 0) + "',"
                + "familyIntro = @familyIntro,"
                + "mlStatue = @mlStatue, mlVisitesTotal = @mlVisitesTotal, mlVisitesDay = @mlVisitesDay, mlMotto = @mlMotto, mlLastUpdateVisites = @mlLastUpdateVisites, arenaKill = @arenaKill, arenaDead = @arenaDead, muted = @muted, onlineTime = @onlineTime WHERE charId = " + this.id + " LIMIT 1");
            }
            this.questManager.Save();
        }
        #endregion

        #region Fonctions globales
        public ServerPacket GetStats()
        {
            ServerPacket packet = new ServerPacket(Outgoing.entitieStat);
            packet.AppendInt(1);
            packet.AppendInt(this.id);
            packet.AppendInt(this.level);
            packet.AppendInt(ServerMath.Percent(this.currentHp, this.maxHp));
            packet.AppendInt(ServerMath.Percent(this.currentMp, this.maxMp));
            packet.AppendInt(this.currentHp);
            packet.AppendInt(this.currentMp);
            return packet;
        }
        public void Move(int x, int y, int dir) { this.x = x; this.y = y; this.direction = dir; }
        public void OnCycle()
        {
            if (DateTime.Now.Subtract(this.lastPulseTime).TotalSeconds >= 90)
            {
                this.GetSession().GetSock().CloseConnection();
                return;
            }
            this.map.CyclePlayer(this);
            if (DateTime.Now.Subtract(this.antiFloodTime).TotalSeconds >= 5)
            {
                if (this.lastSendMessagesNumber > 10)
                    this.session.GetSock().CloseConnection();
                this.lastSendMessagesNumber = 0;
                this.antiFloodTime = DateTime.Now;
            }
            if (this.spInUsing)
            {
                if (this.sp.skills != null)
                {
                    foreach (EntitieSkill skill in this.sp.skills.Values)
                    {
                        if (skill.notCharge)
                        {
                            if (DateTime.Now.Subtract(skill.lastSkillUse).TotalMilliseconds >= skill.skillBase.charge * 100)
                            {
                                ServerPacket packet = new ServerPacket(Outgoing.skillCharged);
                                packet.AppendInt(skill.skillBaseId);
                                this.SendPacket(packet);
                                skill.notCharge = false;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (EntitieSkill skill in this.skills.Values)
                {
                    if (skill.notCharge)
                    {
                        if (DateTime.Now.Subtract(skill.lastSkillUse).TotalMilliseconds >= skill.skillBase.charge * 100)
                        {
                            ServerPacket packet = new ServerPacket(Outgoing.skillCharged);
                            packet.AppendInt(skill.skillBaseId);
                            this.SendPacket(packet);
                            skill.notCharge = false;
                        }
                    }
                }
            }
            lock (this.buffs)
            {
                BuffList pBuffs = new BuffList();
                foreach (PersonalBuff buff in this.buffs)
                {
                    if (DateTime.Now.Subtract(buff.addedAt).TotalMilliseconds >= buff.baseBuff.time * 100)
                        this.map.SendMap(buff.GetPacket(this.type, this.id, true));
                    else
                        pBuffs.Add(buff);
                }
                this.buffs = pBuffs;
            }
            this.getStat();
            if (DateTime.Now.Subtract(this.lastSpWaitUpdate).TotalSeconds >= this.spWaitSecond && this.spWaitSecond != 0)
            {
                this.spWaitSecond = 0;
                ServerPacket packet = new ServerPacket(Outgoing.spCharge);
                packet.AppendInt(0);
                this.SendPacket(packet);
                this.SendPacket(GlobalMessage.MakeMessage(this.id, 0, 11, GameServer.GetLanguage(this.languagePack, "message.sp.secondffect.disabled")));
            }
            if (this.familyDestroyed)
            {
                this.SendPacket(GlobalMessage.MakeModal(1, GameServer.GetLanguage(this.languagePack, "message.family.destroyed")));
                this.familyDestroyed = false;
            }
            if (this.group != null && this.group.groupOwner == this.id)
                this.group.MemberStatSend();
        }
        #endregion
    }
}
