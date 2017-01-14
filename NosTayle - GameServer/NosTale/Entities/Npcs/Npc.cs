using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.Core;
using NosTayleGameServer.NosTale.Entities.Bases;
using NosTayleGameServer.NosTale.Entities.Managers;
using NosTayleGameServer.NosTale.Entities.Players;
using NosTayleGameServer.NosTale.Maps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Entities.Npcs
{
    public class Npc : Entitie, GlobalFunctions
    {
        internal EntitieBase eBase;
        internal int vnum;
        internal bool isMonster;
        internal int baseX;
        internal int baseY;
        internal int respawnTime;
        internal bool strictRespawn;
        internal bool strictBase;
        internal bool botMove;
        internal bool shop;
        internal int shopType;
        internal int menuType;
        internal string shopName;
        internal int shopId;
        internal int dialogId;
        internal Map map;
        internal bool isPet;

        internal string[] shopList;
        internal string[] nRunList;

        internal Player owner;
        internal string typeInfo = "default";
        private DateTime lastWalk;
        private int timeForWalkingNext;
        private DateTime lastWalkForCible;

        //ATTACK INFOS

        new internal int critic_rate1 { set { critic_rate1 = value; } get { return this.eBase.critic_rate; } }
        new internal int critic_damage1 { set { critic_damage1 = value; } get { return this.eBase.critic_degats; } }

        //delegate
        internal delegate void randomWalk(Map map);
        private randomWalk rWalk;
        internal delegate bool statuVerif();
        internal statuVerif sVerif;
        internal delegate void cibleEntitie(Map map);
        internal cibleEntitie cEntitie;


        public Npc(int id, int vnum, string name, int type, int level, int attackUpgrade, int defenseUpgrade, int mapId, int x, int y, int direction,
            int maxHp, int maxMp, int element_type, int element, int fireResist, int waterResist, int lightResist, int darknessResist, int damageMin,
            int damageMax, int hitrate, int corpDef, int distDef, int magicDef, int dodge, int respawnTime, bool strictRespawn, bool strictBase,
            bool botMove, bool shop, int shopType, int menuType, string shopName, int shopId, string[] shopList, int dialogId, string[] nRunList, string typeInfo, Player owner = null, Map map = null, bool isPet = false)
        {
            this.id = id;
            this.vnum = vnum;
            this.eBase = EntitieBaseManager.GetEntitieBase(vnum);
            this.name = name;
            this.type = type;
            this.level = level;
            this.attackUpgrade1 = attackUpgrade;
            this.defenseUpgrade = defenseUpgrade;
            this.mapId = mapId;
            this.baseX = x;
            this.baseY = y;
            this.x = x;
            this.y = y;
            this.direction = direction;
            this.maxHp = maxHp;
            this.maxMp = maxMp;
            this.currentHp = this.maxHp;
            this.currentMp = this.maxMp;
            this.element_type = element_type;
            this.element = element;
            this.fireResist = fireResist;
            this.waterResist = waterResist;
            this.lightResist = lightResist;
            this.darknessResist = darknessResist;
            this.damageMin1 = damageMin;
            this.damageMax1 = damageMax;
            this.hitRate1 = hitrate;
            this.corpDef = corpDef;
            this.distDef = distDef;
            this.magicDef = magicDef;
            this.dodge = dodge;
            this.respawnTime = respawnTime;
            this.strictRespawn = strictRespawn;
            this.strictBase = strictBase;
            this.botMove = botMove;
            this.shop = shop;
            this.shopType = shopType;
            this.menuType = menuType;
            this.shopName = shopName;
            this.shopId = shopId;
            this.shopList = shopList;
            this.dialogId = dialogId;
            this.nRunList = nRunList;
            this.typeInfo = typeInfo;
            this.owner = owner;
            this.map = map;
            this.isPet = isPet;

            this.cibles = new List<Entitie>();

            this.spawnTime = DateTime.Now;

            //GENERALITES
            this.timeForWalkingNext = NpcsManager.Random(1500, 3000);
            this.rWalk = new randomWalk(RandomWalking);
            this.sVerif = new statuVerif(VerifStatu);
            this.cEntitie = new cibleEntitie(CibleEntitieMap);
        }

        public Npc(Npc npcCopy, Map map)
        {
            this.id = npcCopy.id;
            this.vnum = npcCopy.vnum;
            this.eBase = npcCopy.eBase;
            this.name = npcCopy.name;
            this.type = npcCopy.type;
            this.level = npcCopy.level;
            this.attackUpgrade1 = npcCopy.attackUpgrade1;
            this.defenseUpgrade = npcCopy.defenseUpgrade;
            this.mapId = npcCopy.mapId;
            this.baseX = npcCopy.baseX;
            this.baseY = npcCopy.baseY;
            this.x = npcCopy.x;
            this.y = npcCopy.y;
            this.direction = npcCopy.direction;
            this.maxHp = npcCopy.maxHp;
            this.maxMp = npcCopy.maxMp;
            this.currentHp = npcCopy.maxHp;
            this.currentMp = npcCopy.maxMp;
            this.element_type = npcCopy.element_type;
            this.element = npcCopy.element;
            this.fireResist = npcCopy.fireResist;
            this.waterResist = npcCopy.waterResist;
            this.lightResist = npcCopy.lightResist;
            this.darknessResist = npcCopy.darknessResist;
            this.damageMin1 = npcCopy.damageMin1;
            this.damageMax1 = npcCopy.damageMax1;
            this.hitRate1 = npcCopy.hitRate1;
            this.corpDef = npcCopy.corpDef;
            this.distDef = npcCopy.distDef;
            this.magicDef = npcCopy.magicDef;
            this.dodge = npcCopy.dodge;
            this.isDead = npcCopy.isDead;
            this.inAction = npcCopy.inAction;
            this.respawnTime = npcCopy.respawnTime;
            this.strictRespawn = npcCopy.strictRespawn;
            this.strictBase = npcCopy.strictBase;
            this.botMove = npcCopy.botMove;
            this.shop = npcCopy.shop;
            this.shopType = npcCopy.shopType;
            this.menuType = npcCopy.menuType;
            this.shopName = npcCopy.shopName;
            this.shopId = npcCopy.shopId;
            this.shopList = npcCopy.shopList;
            this.dialogId = npcCopy.dialogId;
            this.nRunList = npcCopy.nRunList;
            this.typeInfo = npcCopy.typeInfo;
            this.owner = npcCopy.owner;
            this.map = map;
            this.isPet = npcCopy.isPet;

            this.cibles = new List<Entitie>();

            this.spawnTime = DateTime.Now;

            //GENERALITES
            this.timeForWalkingNext = NpcsManager.Random(1500, 3000);
            this.rWalk = new randomWalk(RandomWalking);
            this.sVerif = new statuVerif(VerifStatu);
            this.cEntitie = new cibleEntitie(CibleEntitieMap);
        }

        public void OnCycle(Map map)
        {
            if (this.typeInfo == "miniland_panel" && DateTime.Now.Subtract(this.spawnTime).TotalSeconds >= this.maxHp)
            {
                if (this.owner != null)
                    owner.mlPanel = null;
                map.RemoveNpc(this);
            }
            if (this.isDead)
            {
                if (this.respawnTime > 0)
                {
                    if (DateTime.Now.Subtract(this.deadTime).TotalSeconds >= this.respawnTime)
                    {
                        this.currentHp = this.maxHp;
                        this.currentMp = this.maxMp;
                        this.x = this.baseX;
                        this.y = this.baseY;
                        this.isDead = false;
                        map.SendMap(this.NpcPacket("fr"));
                        ServerPacket newPacket = new ServerPacket(Outgoing.respawn);
                        newPacket.AppendInt(this.type);
                        newPacket.AppendInt(this.id);
                        newPacket.AppendInt(0);
                        map.SendMap(newPacket);
                    }
                }
                else
                    map.RemoveNpc(this);
            }
            cEntitie(map);
            rWalk(map);
        }

        public void CibleEntitieMap(Map map)
        {
            if (this.type == 3 && this.eBase.agressive == 1 && this.cibles.Count == 0)
            {
                foreach (Player user in map.playersManagers.GetPlayersList())
                {
                    if (ServerMath.GetDistance(this.x, this.y, user.x, user.y) <= 6)
                    {
                        this.lastAttack = DateTime.Now;
                        this.cibles.Add(user);
                        user.cibles.Add(this);
                        user.SendEffect(this.type, this.id, 5000);
                    }
                }
            }
        }

        public void RandomWalking(Map map)
        {
            lock (this)
            {
                if (this.cibles.Count == 0)
                {
                    TimeSpan timeSpan = DateTime.Now - lastWalk;
                    if (this.botMove && this.eBase.canMoved && !this.isDead && Math.Round((timeForWalkingNext - timeSpan.TotalMilliseconds)) <= 0)
                    {
                        this.lastWalk = DateTime.Now;
                        this.timeForWalkingNext = NpcsManager.Random(200, 3000);
                        //Generate Coords
                        bool notwalkable = false;
                        int gen_x, gen_y;
                        int tries = 0;

                        do
                        {
                            gen_x = (this.strictBase ? this.baseX : this.x);
                            gen_y = (this.strictBase ? this.baseY : this.y);

                            int x_min = NpcsManager.Random(0, 10);
                            int y_min = NpcsManager.Random(0, 10);

                            if (x_min > 6) { gen_x -= 2; } else if (x_min < 4) { gen_x += 2; }
                            if (y_min > 6) { gen_y -= 2; } else if (y_min < 4) { gen_y += 2; }

                            notwalkable = map.IsBlockedZone(gen_x, gen_y);

                            if ((int)ServerMath.GetDistance(this.x, this.y, gen_x, gen_y) > 2)
                                notwalkable = true;
                            if (this.x == gen_x && this.y == gen_y)
                                notwalkable = true;
                            tries++;
                        } while (notwalkable && tries < 5);
                        if (!notwalkable)
                        {
                            this.Move(gen_x, gen_y, 2);
                            map.MoveEntitie(this.type, this.id, this.x, this.y, 4);
                        }
                    }
                }
                else if (this.eBase.canMoved && !this.isDead)
                {
                    try
                    {
                        for (int i = 0; i < this.cibles.Count; i++)
                        {
                            if (this.cibles.ElementAtOrDefault(i) != null)
                            {
                                Entitie cible = this.cibles[i];
                                if (cible == null || cible.deleted || cible.isDead)
                                {
                                    this.cibles.Remove(cible);
                                    cible.cibles.Remove(this);
                                    continue;
                                }
                                if (cible.type == 1)
                                {
                                    Player c = (Player)cible;
                                    if (c.map != map || !c.cibles.Contains(this))
                                    {
                                        this.cibles.Remove(cible);
                                        c.cibles.Remove(this);
                                        continue;
                                    }
                                }
                                if (Math.Round(ServerMath.GetDistance(this.x, this.y, cible.x, cible.y)) > 15 && 8000 - ((TimeSpan)(DateTime.Now - lastAttack)).TotalMilliseconds <= 0)
                                {
                                    this.cibles.Remove(cible);
                                    cible.cibles.Remove(this);
                                    continue;
                                }
                                else if (Math.Round(ServerMath.GetDistance(this.x, this.y, cible.x, cible.y)) <= eBase.attack_cells)
                                {
                                    this.lastWalkForCible = DateTime.Now;
                                    if (5000 - ((TimeSpan)(DateTime.Now - lastAttack)).TotalMilliseconds <= 0)
                                    {
                                        this.lastAttack = DateTime.Now;
                                        //ATTAQUE
                                        //EntitieSkill.attackBase(map, this, cible, 200, eBase.attack_cells, eBase.attack_type);
                                    }
                                }
                                else if (Math.Round(ServerMath.GetDistance(this.x, this.y, cible.x, cible.y)) > eBase.attack_cells && 1000 - ((TimeSpan)(DateTime.Now - lastWalkForCible)).TotalMilliseconds <= 0)
                                {
                                    this.lastWalkForCible = DateTime.Now;
                                    Point futurStep = ServerMath.GetFuturStep(new Point(this.x, this.y), new Point(cible.x, cible.y), 1);
                                    if (!map.IsBlockedZone(futurStep.X, futurStep.Y))
                                    {
                                        this.Move(futurStep.X, futurStep.Y, 2);
                                        map.MoveEntitie(this.type, this.id, this.x, this.y, 4);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
        }

        public void SpawnMobs(Map map)
        {
            foreach (string mob in this.eBase.spawnMobs)
            {
                for (int i = 0; i < Convert.ToInt32(mob.Split('.')[0]); i++)
                    map.AddNpc(map.channel, this.type, Convert.ToInt32(mob.Split('.')[1]), this.x, this.y, this.direction, Convert.ToInt32(mob.Split('.')[2]), Convert.ToInt32(mob.Split('.')[3]), "default");
            }
        }

        public bool VerifStatu()
        {
            if (this.isDead)
                if (this.respawnTime == 0)
                    return true;
            return false;
        }

        public void GetShop(Player user)
        {
            if (GameServer.GetShopManager().shopList.ContainsKey(this.shopId))
                GameServer.GetShopManager().shopList[this.shopId].MakeShop(user, this.id);
        }

        public void SendNpc(Player user)
        {
            if (!this.isDead)
            {
                user.SendPacket(this.NpcPacket(user.languagePack));
                if (this.shop)
                {
                    ServerPacket packet = new ServerPacket(Outgoing.shop);
                    packet.AppendInt(this.type);
                    packet.AppendInt(this.id);
                    packet.AppendInt(Convert.ToInt32(this.shop));
                    packet.AppendInt(this.menuType);
                    packet.AppendInt(this.shopType);
                    packet.AppendString(this.shopName);
                    user.SendPacket(packet);
                }
            }
        }

        public ServerPacket NpcPacket(string languagePack)
        {
            ServerPacket packet = new ServerPacket(Outgoing.inEntitie);
            packet.AppendInt(this.type);
            packet.AppendInt(this.vnum);
            packet.AppendInt(this.id);
            packet.AppendInt(this.x);
            packet.AppendInt(this.y);
            packet.AppendInt(this.direction);
            packet.AppendInt(this.GetHpPercent());
            packet.AppendInt(this.GetMpPercent());
            packet.AppendInt(this.dialogId);
            packet.AppendInt(0);
            packet.AppendInt(this.isPet ? 3 : 0);
            packet.AppendInt(this.isPet ? this.owner.id : -1);
            packet.AppendString("1 0 -1 " + (this.typeInfo != "miniland_panel" ? this.name : GameServer.GetLanguage(languagePack, "name.miniland.panel")) + " 0 -1 0 0 0 0 0 0 0 20");
            return packet;
        }

        public int GetHp()
        {
            int hp = 0;
            switch (this.typeInfo)
            {
                case "miniland_panel":
                    {
                        hp = (int)Math.Round(this.maxHp - DateTime.Now.Subtract(this.spawnTime).TotalSeconds);
                    }
                    break;
                default:
                    hp = this.currentHp;
                    break;
            }
            this.currentHp = hp;
            return this.currentHp;
        }

        public int GetHpPercent()
        {
            int hp = 0;
            switch (this.typeInfo)
            {
                case "miniland_panel":
                    {
                        TimeSpan timeSpawn = DateTime.Now - this.spawnTime;
                        hp = (int)Math.Round(this.maxHp - DateTime.Now.Subtract(this.spawnTime).TotalSeconds);
                    }
                    break;
                default:
                    hp = this.currentHp;
                    break;
            }
            this.currentHp = hp;
            return ServerMath.Percent(this.currentHp, this.maxHp);
        }

        public int GetMpPercent()
        {
            int mp = 0;
            switch (this.type)
            {
                default:
                    mp = this.currentMp;
                    break;
            }
            this.currentMp = mp;
            return ServerMath.Percent(this.currentMp, this.maxMp);
        }

        public void PanelRequest(Player player)
        {
            if (player == this.owner)
            {
                player.map.ChangeMapRequest(player, player.miniLand, 5, 8);
                return;
            }
            if (!this.owner.GetSession().GetSock().isDisconnected())
            {
                switch (this.owner.miniLand.statue)
                {
                    case 0:
                        ServerPacket packet = new ServerPacket(Outgoing.userQuestion);
                        packet.AppendString("#mjoin^1^" + this.owner.id + "^1  " + this.owner.name + " : " + this.owner.miniLand.motto);
                        player.SendPacket(packet);
                        break;
                    case 1:
                        player.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(player.languagePack, "message.miniland.private")));
                        break;
                    case 2:
                        player.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(player.languagePack, "message.miniland.locked")));
                        break;
                }
            }
        }

        //GlobalFunctions
        public override int GetExpWin()
        {
            return this.eBase.exp;
        }

        public override int GetExpJobWin()
        {
            return this.eBase.exp_job;
        }

        public ServerPacket GetStats()
        {
            ServerPacket packet = new ServerPacket(Outgoing.entitieStat);
            packet.AppendInt(this.type);
            packet.AppendInt(this.id);
            packet.AppendInt(this.level);
            packet.AppendInt(this.GetHpPercent());
            packet.AppendInt(this.GetMpPercent());
            packet.AppendInt(this.currentHp);
            packet.AppendInt(this.currentMp);
            return packet;
        }

        public void Die(Map map)
        {
            /*this.cibles = new List<Entitie>();
            this.isDead = true;
            if (this.eBase.spawnDie)
                this.spawnMobs(map);
            if (this.respawnTime != 0)
                new Thread(new ParameterizedThreadStart(this.respawnThread)).Start(map);*/
        }

        public void Move(int x, int y, int dir)
        {
            this.x = x;
            this.y = y;
            this.direction = dir;
        }
    }
}