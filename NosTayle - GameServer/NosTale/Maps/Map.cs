using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.NosTale.Channels;
using NosTayleGameServer.NosTale.Entities;
using NosTayleGameServer.NosTale.Entities.Bases;
using NosTayleGameServer.NosTale.Entities.Managers;
using NosTayleGameServer.NosTale.Entities.Npcs;
using NosTayleGameServer.NosTale.Entities.Players;
using NosTayleGameServer.NosTale.Items;
using NosTayleGameServer.NosTale.Items.Others;
using NosTayleGameServer.NosTale.Maps.Portals;
using NosTayleGameServer.NosTale.Missions.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Maps
{
    public class Map
    {
        internal Channel channel;
        internal int mapId;
        internal bool specialMap;
        internal PlayersManager playersManagers = new PlayersManager();
        internal NpcsManager npcsManager;
        internal NpcsManager monstersManager;
        internal PortalsManager portalsManager;
        internal Task cycleTask;
        internal bool isCycling;

        internal DropItem[] dropItems = new DropItem[1000];
        internal Random randomDrop = new Random();

        internal int randomRate
        {
            get
            {
                lock (this.randomDrop)
                    return this.randomDrop.Next(0, 100);
            }
        }

        //IDENTIFICATION
        internal bool isArena;

        public Map() { }

        public Map(int id, Channel channel)
        {
            this.channel = channel;
            this.mapId = id;
            if (GameServer.GetPortals().ContainsKey(mapId))
                portalsManager = GameServer.GetPortals()[mapId];
            else
                portalsManager = new PortalsManager();
            if (npcsManager == null)
                npcsManager = new NpcsManager(this.mapId, GameServer.GetNpcsManager().entitieList, true, this);
            if (monstersManager == null)
                monstersManager = new NpcsManager(this.mapId, GameServer.GetMonstersManager().entitieList, true, this);
            StartCycleTask();
        }

        public void StartCycleTask()
        {
            cycleTask = new Task(OnCycle);
            cycleTask.Start();
        }

        public void OnCycle()
        {
            while (true)
            {
                if (!this.isCycling)
                {
                    this.isCycling = true;
                    try
                    {
                        this.playersManagers.cycleTask();
                        this.npcsManager.OnCycle(this);
                        this.monstersManager.OnCycle(this);
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.ToString());
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    this.isCycling = false;
                }
                Thread.Sleep(25);
            }
        }

        public virtual void CyclePlayer(Player user)
        {

        }

        public void AddUser(Player user, bool firstEnter)
        {
            try
            {
                ServerPacket packet = null;
                if (firstEnter)
                {

                    user.inventory.LoadItems(user);
                    user.miniLand.LoadItems();
                    user.SendTit();
                    if (user.family != null)
                        user.family.SendFamilyMotto(user);
                    user.SendCharacterReputation();
                }
                user.SendCharacterInfo();
                user.SendEquipment();
                user.SendPacket(user.GetEqPacket());
                user.SendLevel();
                if (user.GetScale() != 10)
                {
                    packet = new ServerPacket(Outgoing.changeCharScale);
                    packet.AppendInt(1);
                    packet.AppendInt(user.id);
                    packet.AppendInt(user.GetScale());
                    user.SendPacket(packet);
                }
                if (firstEnter)
                    user.SendSkillPacket();
                user.SendPlaceOnMap();
                packet = new ServerPacket(Outgoing.mapIni);
                packet.AppendInt(0);
                packet.AppendInt(this.mapId);
                packet.AppendInt(1);
                user.SendPacket(packet);
                user.SendStatPoints();
                if (user.rested == 1)
                    user.rested = 0;
                user.SendStats(false);
                user.SendSpeed();
                user.SendFairy();
                packet = new ServerPacket(Outgoing.rsfi); //ACTS COMPLETE AND TS
                packet.AppendInt(1);
                packet.AppendInt(1);
                packet.AppendInt(0);
                packet.AppendInt(9);
                packet.AppendInt(0);
                packet.AppendInt(9);
                user.SendPacket(packet);
                user.SendSpPoints();
                packet = new ServerPacket(Outgoing.scr); //Unknow 
                packet.AppendInt(0);
                packet.AppendInt(0);
                packet.AppendInt(0);
                packet.AppendInt(0);
                packet.AppendInt(0);
                packet.AppendInt(0);
                user.SendPacket(packet);
                //MESSAGES BOX BAS 
                user.inventory.SendInvSlot(user);
                if (user.pet != null && user.locomotion == null)
                {
                    user.pet.x = user.x + 1;
                    user.pet.y = user.y - 1;
                    user.SendPacket(user.pet.NpcPacket(user.languagePack));
                }
                user.SendFamilyAffich(user);
                if (user.buffs.Exists(x => x.baseBuff.viewForm))
                    user.SendPacket(GlobalMessage.MakeGuri(0, user.type, user.id, user.buffs.FirstOrDefault(x => x.baseBuff.viewForm).baseBuff.guriStart));
                if (firstEnter)
                {
                    user.miniLand.SendUserMLInfos();
                    user.questManager.GetQuest(user);
                    user.LoadFamily(firstEnter); //FAMILY INFOS AND MEMBER LIST SEND CONNEXION
                    user.inventory.BuildAllInventory(user);
                    user.skillBar.SendBarToUser(user);
                    user.friendList.SendConnexionToAll(user.name);
                    user.SendPacket(user.friendList.InitList());
                }
                if (user.group != null)
                {
                    user.group.SendGroupStatue();
                    user.group.SendPinit(user);
                }
                else
                {
                    packet = new ServerPacket(Outgoing.pInit);
                    packet.AppendInt(0);
                    user.SendPacket(packet);
                }
                this.playersManagers.AddUser(user.id, user);
                this.SendOtherMapInfos(user);
                this.SendPortalsList(user);
                this.SendMonsters(user);
                this.SendNpcs(user);
                this.SendDropItem(user);
                this.SendPlayerList(user);
                user.spawnTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void ChangeMapRequest(Player player) //PORTAIL DEPLACEMENT
        {
            player.cibles = new List<Entitie>();
            player.arenaKillNow = 0;
            player.arenaDeadNow = 0;
            player.SendArenaScore(false);
            foreach (Portal portail in portalsManager.portals.Values)
            {
                if (player.x < (portail.x_pos + 2) && player.x > (portail.x_pos - 2)
                    && player.y < (portail.y_pos + 2) && player.y > (portail.y_pos - 2))
                {
                    this.RemoveUser(player);
                    if (player.pet != null)
                        this.RemoveNpc(player.pet);
                    player.groupRequest = new List<int>();
                    switch (portail.portal_dType)
                    {
                        case "default":
                            if (player.GetSession().GetChannel().generalMaps.ContainsKey(portail.mapDirection))
                            {
                                player.map = player.GetSession().GetChannel().generalMaps[portail.mapDirection];
                                player.mapId = portail.mapDirection;
                            }
                            else
                            {
                                player.GetSession().GetChannel().generalMaps.Add(portail.mapDirection, new Map(portail.mapDirection, player.GetSession().GetChannel()));
                                player.map = player.GetSession().GetChannel().generalMaps[portail.mapDirection];
                                player.mapId = portail.mapDirection;
                            }
                            player.x = portail.x_mapDirection;
                            player.y = portail.y_mapDirection;
                            break;
                        case "miniland_enter":
                            player.saveMap = player.map.mapId;
                            player.saveX = player.x;
                            player.saveY = player.y;
                            player.map = player.miniLand;
                            player.x = portail.x_mapDirection;
                            player.y = portail.y_mapDirection;
                            break;
                        case "special_exit":
                            if (player.GetSession().GetChannel().generalMaps.ContainsKey(player.saveMap))
                            {
                                player.map = player.GetSession().GetChannel().generalMaps[player.saveMap];
                                player.mapId = player.saveMap;
                            }
                            else
                            {
                                player.GetSession().GetChannel().generalMaps.Add(player.saveMap, new Map(player.saveMap, player.GetSession().GetChannel()));
                                player.map = player.GetSession().GetChannel().generalMaps[player.saveMap];
                                player.mapId = player.saveMap;
                            }
                            player.x = player.saveX;
                            player.y = player.saveY;
                            player.canPvp = false;
                            break;
                    }
                    player.map.AddUser(player, false);
                    return;
                }
            }
        }

        public void ChangeMapRequest(Player player, Map map, int x, int y) //TELEPORT DEPLACEMENT
        {
            player.cibles = new List<Entitie>();
            player.arenaKillNow = 0;
            player.arenaDeadNow = 0;
            player.SendArenaScore(false);
            if (map.specialMap)
            {
                player.saveMap = player.map.mapId;
                player.saveX = player.x;
                player.saveY = player.y;
            }
            this.RemoveUser(player);
            if (player.pet != null)
                this.RemoveNpc(player.pet);
            player.map = map;
            player.mapId = map.mapId;
            player.x = x;
            player.y = y;
            player.map.AddUser(player, false);
            return;
        }

        public void RemoveUser(Player user)
        {
            this.playersManagers.RemoveUser(user);
            ServerPacket packet = new ServerPacket(Outgoing.mapIni);
            if (!user.GetSession().GetSock().isDisconnected())
            {
                packet.AppendInt(0);
                packet.AppendInt(this.mapId);
                packet.AppendInt(0);
                user.SendPacket(packet);
                user.SendPacket(new ServerPacket(Outgoing.mapOut));
            }
            packet = new ServerPacket(Outgoing.outT);
            packet.AppendInt(1);
            packet.AppendInt(user.id);
            this.SendMap(user, packet, false);
        }

        public Npc AddNpc(Channel channel, int type, int vnum, int x, int y, int direction, int attackUpgrade, int defenseUpgrade, string infoType, Player owner = null, int dialogId = 0)
        {
            EntitieBase eBase = EntitieBaseManager.GetEntitieBase(vnum);
            Npc newNpc = new Npc(channel.NewNpcId(), vnum, "-", type, eBase.level_base, attackUpgrade, defenseUpgrade, this.mapId, x, y, direction, eBase.hp, eBase.mp, eBase.element_type, eBase.element, eBase.fire_res, eBase.water_res, eBase.light_res, eBase.dark_res, eBase.attack_min, eBase.attack_max, eBase.hitrate, eBase.corpDef, eBase.distDef, eBase.magicDef, eBase.dodge, 0, false, false, eBase.canMoved, false, 0, 0, "", 0, null, dialogId, null, infoType, owner, this);
            if (type == 2)
                lock (this.npcsManager.entitieList)
                    this.npcsManager.entitieList.Add(newNpc);
            else if (type == 3)
                lock (this.monstersManager.entitieList)
                    this.monstersManager.entitieList.Add(newNpc);
            List<Player> players = this.playersManagers.GetPlayersList();
            foreach (Player p in players)
                p.SendPacket(newNpc.NpcPacket(p.languagePack));
            return newNpc;
        }

        public void AddNpc(Npc npc)
        {
            if (npc.type == 2)
                lock (this.npcsManager.entitieList)
                    this.npcsManager.entitieList.Add(npc);
            else if (npc.type == 3)
                lock (this.monstersManager.entitieList)
                    this.monstersManager.entitieList.Add(npc);
        }

        public void RemoveNpc(Npc npc)
        {
            ServerPacket packet = new ServerPacket(Outgoing.outT);
            packet.AppendInt(npc.type);
            packet.AppendInt(npc.id);
            this.SendMap(packet);
            lock (this.npcsManager.entitieList)
                if (this.npcsManager.entitieList.Contains(npc))
                    this.npcsManager.entitieList.Remove(npc);
        }

        public void ExitAllUser(bool isMiniLand, int ownerId = 0)
        {
            this.playersManagers.ExitAllUsers(isMiniLand, this.specialMap, ownerId);
        }

        public void SendMap(Player player, ServerPacket packet, bool player_included)
        {
            this.playersManagers.SendToAll(player, packet, player_included);
        }

        public void SendMap(ServerPacket packet, List<Player> notReceivers)
        {
            this.playersManagers.SendToAll(packet, notReceivers);
        }

        public void SendMap(ServerPacket packet)
        {
            this.playersManagers.SendToAll(packet);
        }

        public virtual void SendOtherMapInfos(Player receiver) { }

        public void SendPortalsList(Player user)
        {
            if (portalsManager.portals.Count >= 1)
                foreach (Portal portal in portalsManager.portals.Values)
                    user.SendPacket(portal.GetPortalPacket());
        }

        public void SendPlayerList(Player user)
        {
            this.playersManagers.SendUserList(user);
        }

        public void SendNpcs(Player user)
        {
            if (this.npcsManager.npcCount >= 1)
                lock (this.monstersManager.entitieList)
                {
                    foreach (Npc npc in this.npcsManager.GetNpcs())
                        npc.SendNpc(user);
                }
        }

        public void SendMonsters(Player user)
        {
            if (this.monstersManager.npcCount >= 1)
                lock (this.monstersManager.entitieList)
                {
                    foreach (Npc monster in this.monstersManager.GetNpcs())
                        if (!monster.isDead)
                            monster.SendNpc(user);
                }
        }

        public void SendDropItem(Player user)
        {
            lock (this.dropItems)
            {
                foreach (DropItem item in this.dropItems.Where(x => x != null))
                    user.SendPacket(item.GetInPacket());
            }
        }

        public void SendEffect(int entitieType, int entitieId, int effectId)
        {
            ServerPacket packet = new ServerPacket(Outgoing.effectPacket);
            packet.AppendInt(entitieType);
            packet.AppendInt(entitieId);
            packet.AppendInt(effectId);
            this.SendMap(packet);
        }

        public void SendGuri(int guriType, int uType, int uId, string text)
        {
            ServerPacket packet = new ServerPacket(Outgoing.guri);
            packet.AppendInt(guriType);
            packet.AppendInt(uType);
            packet.AppendInt(uId);
            packet.AppendString(text);
            this.SendMap(packet);
        }

        public bool IsBlockedZone(int x, int y)
        {
            try
            {
                if (x > GameServer.GetMapData(this.mapId).x || x < 1 || y > GameServer.GetMapData(this.mapId).y || y < 1 || GameServer.GetMapData(this.mapId).grid[y, x] != 0)
                    return true;
            }
            catch { }
            return false;
        }

        public void AddDropItem(DropItem dItem)
        {
            lock (this.dropItems)
            {
                int newId = Array.IndexOf(this.dropItems, null);
                if (newId != -1)
                {
                    dItem.id = newId + 1;
                    dropItems[newId] = dItem;
                }
                else
                {
                    int lastSeconds = 0;
                    int index = 0;
                    for (int i = 0; i < this.dropItems.Length; i++)
                    {
                        if (DateTime.Now.Subtract(this.dropItems[i].dropedAt).TotalSeconds >= lastSeconds)
                        {
                            index = i;
                            lastSeconds = (int)DateTime.Now.Subtract(this.dropItems[i].dropedAt).TotalSeconds;
                        }
                    }
                    dItem.id = index + 1;
                    this.dropItems[index] = dItem;
                }
                this.SendMap(dItem.GetDropPacket());
            }
        }

        public void GetDropItem(Entitie entitie, int id)
        {
            Player player = null;
            if (entitie.type == 1)
                player = (Player)entitie;
            else if (entitie.type == 2 && ((Npc)entitie).isPet)
                player = ((Npc)entitie).owner;
            if (player == null)
                return;
            lock (player.inventory)
            {
                int realId = id - 1;
                if (realId >= 0 && realId < 1000 && this.dropItems[realId] != null && !this.dropItems[realId].inDrop)
                {
                    lock (this.dropItems[realId])
                    {
                        if (this.dropItems[realId].isItem)
                        {
                            Item item = this.dropItems[realId].item;
                            item.mustInsert = true;
                            if (item.id == -1)
                                item.id = GameServer.NewItemId();
                            if (item.itemBase.inventory == 0)
                            {
                                if (player.inventory.AddItem(player, 0, item, "item", 1, true) != -1)
                                {
                                    player.SendPacket(GlobalMessage.MakeMessage(player.id, player.type, 12, String.Format(GameServer.GetLanguage(player.languagePack, "message.inventory.dropitem"), GameServer.GetItemName(player.languagePack, item.itemBase.name), this.dropItems[realId].quantity)));
                                    player.SendPacket(GlobalMessage.MakeIcon(entitie.type, entitie.id, 1, item.itemBase.id));
                                    this.SendGetDrop(entitie.type, entitie.id, id);
                                    this.dropItems[realId] = null;
                                    return;
                                }
                                else
                                    player.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(player.languagePack, "message.inventory.full")));
                            }
                            else
                            {
                                try
                                {
                                    if (player.inventory.AddBasicItem(player, item.itemBase.inventory, item.itemBase.id, this.dropItems[realId].quantity))
                                    {
                                        player.SendPacket(GlobalMessage.MakeMessage(player.id, player.type, 12, String.Format(GameServer.GetLanguage(player.languagePack, "message.inventory.dropitem"), GameServer.GetItemName(player.languagePack, item.itemBase.name), this.dropItems[realId].quantity)));
                                        player.SendPacket(GlobalMessage.MakeIcon(entitie.type, entitie.id, 1, item.itemBase.id));
                                        this.SendGetDrop(entitie.type, entitie.id, id);
                                        this.dropItems[realId] = null;
                                        return;
                                    }
                                    else
                                        player.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(player.languagePack, "message.inventory.full")));
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                }
                            }
                        }
                        else if (this.dropItems[realId].isSp)
                        {
                            Specialist sp = this.dropItems[realId].sp;
                            sp.mustInsert = true;
                            if (sp.spId == -1)
                                sp.spId = GameServer.NewSpId();
                            if (player.inventory.AddItem(player, 0, sp, "specialist", 1, true) != -1)
                            {
                                player.SendPacket(GlobalMessage.MakeMessage(player.id, player.type, 12, String.Format(GameServer.GetLanguage(player.languagePack, "message.inventory.dropitem"), GameServer.GetItemName(player.languagePack, sp.card.name), this.dropItems[realId].quantity)));
                                player.SendPacket(GlobalMessage.MakeIcon(entitie.type, entitie.id, 1, sp.card.id));
                                this.SendGetDrop(entitie.type, entitie.id, id);
                                this.dropItems[realId] = null;
                                return;
                            }
                            else
                                player.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(player.languagePack, "message.inventory.full")));
                        }
                        else if (this.dropItems[realId].isFairy)
                        {
                            Fairy fairy = this.dropItems[realId].fairy;
                            fairy.mustInsert = true;
                            if (fairy.fairyId == -1)
                                fairy.fairyId = GameServer.NewFairyId();
                            if (player.inventory.AddItem(player, 0, fairy, "fairy", 1, true) != -1)
                            {
                                player.SendPacket(GlobalMessage.MakeMessage(player.id, player.type, 12, String.Format(GameServer.GetLanguage(player.languagePack, "message.inventory.dropitem"), GameServer.GetItemName(player.languagePack, fairy.itemBase.name), this.dropItems[realId].quantity)));
                                player.SendPacket(GlobalMessage.MakeIcon(entitie.type, entitie.id, 1, fairy.itemBase.id));
                                this.SendGetDrop(entitie.type, entitie.id, id);
                                this.dropItems[realId] = null;
                                return;
                            }
                            else
                                player.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(player.languagePack, "message.inventory.full")));
                        }
                        else if (this.dropItems[realId].isGold)
                        {
                            if ((double)(player.gold) + this.dropItems[realId].quantity <= Int32.MaxValue)
                            {
                                player.gold += this.dropItems[realId].quantity;
                                player.SendGold();
                                player.SendPacket(GlobalMessage.MakeMessage(player.id, player.type, 12, String.Format(GameServer.GetLanguage(player.languagePack, "message.inventory.dropitem"), GameServer.GetItemName(player.languagePack, "zts698e"), this.dropItems[realId].quantity)));
                                player.SendPacket(GlobalMessage.MakeIcon(entitie.type, entitie.id, 1, 1046));
                                this.SendGetDrop(entitie.type, entitie.id, id);
                                this.dropItems[realId] = null;
                                return;
                            }
                            else
                                player.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(player.languagePack, "message.inventory.full")));
                        }
                    }
                }
            }
        }

        public void SendGetDrop(int entitieType, int entitieId, int itemId)
        {
            ServerPacket packet = new ServerPacket(Outgoing.getDropItem);
            packet.AppendInt(entitieType);
            packet.AppendInt(entitieId);
            packet.AppendInt(itemId);
            packet.AppendInt(0);
            this.SendMap(packet);
        }
        #region Skills
        public virtual bool CanAttack(Entitie sender, Entitie cible)
        {
            if (sender.type == 3 && !((Npc)sender).isPet || cible.type == 3 && !((Npc)cible).isPet && sender != cible && !sender.isDead)
                return true;
            return false;
        }

        public virtual bool CanBuff(Entitie sender, Entitie cible)
        {
            if (sender.type == 1 && cible.type == 1)
                return true;
            return false;
        }

        public void SendEntitieAttack(int senderType, int senderId, int cibleType, int cibleId, string effect, int skillId)
        {
            ServerPacket packet = new ServerPacket(Outgoing.entitieAttack);
            packet.AppendInt(senderType);
            packet.AppendInt(senderId);
            packet.AppendInt(cibleType);
            packet.AppendInt(cibleId);
            packet.AppendString(effect);
            packet.AppendInt(skillId);
            this.SendMap(packet);
        }
        public void SendAttackEntitie(int attackerType, int attackerId, int cibleType, int cibleId, int skillId, int skillCharge, string attack_effect, int x, int y, int isAlive, int cibleHp, int degats, int typeDegats)
        {
            ServerPacket packet = new ServerPacket(Outgoing.attackEntitie);
            packet.AppendInt(attackerType);
            packet.AppendInt(attackerId);
            packet.AppendInt(cibleType);
            packet.AppendInt(cibleId);
            packet.AppendInt(skillId);
            packet.AppendInt(skillCharge);
            packet.AppendString(attack_effect);
            packet.AppendInt(x);
            packet.AppendInt(y);
            packet.AppendInt(isAlive);
            packet.AppendInt(cibleHp);
            packet.AppendInt(degats);
            packet.AppendInt(typeDegats);
            packet.AppendInt(0);
            this.SendMap(packet);
        }
        public virtual void EntitieDead(Entitie killer, Entitie entitie)
        {
            entitie.isDead = true;
            if (entitie.type == 2 || entitie.type == 3)
            {
                Npc npc = (Npc)entitie;
                npc.cibles = new List<Entitie>();
                if (npc.eBase.spawnDie)
                    npc.SpawnMobs(this);
                npc.deadTime = DateTime.Now;
                if (killer.type == 1)
                {
                    Player p = (Player)killer;
                    Random random = new Random();
                    foreach (BaseDropItem dItem in npc.eBase.drops)
                    {
                        if (dItem.type == "gold" || dItem.type == "item")
                        {
                            if (this.randomRate < dItem.rate)
                            {
                                ItemBase iBase = dItem.type == "item" ? GameServer.GetItemsManager().itemList[dItem.itemId] : null;
                                DropItem item = new DropItem(0, entitie.x + random.Next(-1, 2), entitie.y + random.Next(-1, 2), DateTime.Now, dItem.type == "gold", false, false, dItem.type == "item", false, random.Next(dItem.qtyMin, dItem.qtyMax + 1), dItem.type == "item" ? new Item(-1, 0, dItem.itemId, iBase.damageMin, iBase.damageMax, iBase.hitRate, iBase.corpDef, iBase.distDef, iBase.magicDef, iBase.dodge) : null, null, null, p.group, p.group == null ? p : null);
                                this.AddDropItem(item);
                            }
                        }
                    }
                }
            }
            else if (entitie.type == 1)
            {

            }
            if (killer.type == 1)
                ((Player)killer).AddExp(entitie.GetExpWin(), entitie.GetExpJobWin());
        }
        #endregion

        #region entities méthodes
        public void MoveEntitie(int type, int id, int x, int y, int speed)
        {
            ServerPacket packet = new ServerPacket(Outgoing.movPlayer);
            packet.AppendInt(type);
            packet.AppendInt(id);
            packet.AppendInt(x);
            packet.AppendInt(y);
            packet.AppendInt(speed);
            this.SendMap(packet);
        }

        public void MoveEntitie(int type, int id, int x, int y, int speed, Player user, bool sendUser)
        {
            ServerPacket packet = new ServerPacket(Outgoing.movPlayer);
            packet.AppendInt(type);
            packet.AppendInt(id);
            packet.AppendInt(x);
            packet.AppendInt(y);
            packet.AppendInt(speed);
            this.SendMap(user, packet, sendUser);
        }

        public void GetEntitieStat(Player user, int type, int id) //Envoie les hp d'une entité au joueur.
        {
            switch (type)
            {
                case 1:
                    Player otherPlayer = this.playersManagers.GetPlayerById(id);
                    if (otherPlayer != null)
                        user.SendPacket(otherPlayer.GetStats());
                    break;
                case 2:
                    Player master = this.playersManagers.GetPlayersList().FirstOrDefault(x => x.pet != null && x.pet.id == id);
                    if (master != null)
                        user.SendPacket(master.pet.GetStats());
                    else if (this.npcsManager.NpcInListById(id))
                    {
                        Npc npc = this.npcsManager.GetNpcById(id);
                        user.SendPacket(npc.GetStats());
                    }
                    break;
                case 3:
                    if (this.monstersManager.NpcInListById(id))
                        user.SendPacket(this.monstersManager.GetNpcById(id).GetStats());
                    break;
            }
        }

        public void NpcRequest(Player player, int type, int id)
        {
            switch (type)
            {
                case 2:
                    if (this.npcsManager.NpcInListById(id))
                    {
                        Npc npc = this.npcsManager.GetNpcById(id);
                        if (npc.typeInfo == "default")
                        {
                            ServerPacket packet = new ServerPacket(Outgoing.npcDialog);
                            packet.AppendInt(type);
                            packet.AppendInt(id);
                            packet.AppendInt(npc.dialogId);
                            player.SendPacket(packet);
                        }
                        else if (npc.typeInfo == "miniland_panel")
                            npc.PanelRequest(player);
                    }
                    break;
            }
        }

        public void NpcShopRequest(Player player, int shopId, int npcId)
        {
            if (this.npcsManager.NpcInListById(npcId))
            {
                Npc npc = this.npcsManager.GetNpcById(npcId);
                if (npc.shop && GameServer.GetShopManager().shopList.ContainsKey(shopId == 0 ? npc.shopId : shopId))
                    GameServer.GetShopManager().shopList[shopId == 0 ? npc.shopId : shopId].MakeShop(player, npcId);
            }
        }

        public void NRunReq(Player user, int type, int value, int unknown, int npcId)
        {
            switch (type)
            {
                case 1:
                    if (this.npcsManager.NpcInListById(npcId))
                    {
                        if (this.npcsManager.GetNpcById(npcId).nRunList.Contains(type.ToString()))
                        {
                            user.ChangeClass(value);
                        }
                    }
                    break;
                case 2:
                    if (this.npcsManager.NpcInListById(npcId))
                    {
                        if (this.npcsManager.GetNpcById(npcId).nRunList.Contains(type.ToString()))
                        {
                            ServerPacket packet = new ServerPacket("wopen");
                            packet.AppendInt(1);
                            packet.AppendInt(0);
                            user.SendPacket(packet);
                        }
                    }
                    break;
                case 12:
                    if (this.npcsManager.NpcInListById(npcId))
                    {
                        if (this.npcsManager.GetNpcById(npcId).nRunList.Contains(type.ToString()))
                        {
                            ServerPacket packet = new ServerPacket("wopen");
                            packet.AppendInt(value);
                            packet.AppendInt(0);
                            user.SendPacket(packet);
                        }
                    }
                    break;
                case 17:
                    if (npcId == user.id && !user.map.specialMap)
                    {
                        if (value == 0)
                        {
                            ServerPacket packet = new ServerPacket(Outgoing.userQuestion);
                            packet.AppendString(String.Format("#arena^0^1 {0}", GameServer.GetLanguage(user.languagePack, "arena.solo.confirm.enter")));
                            user.SendPacket(packet);
                        }
                        else if (value == 1)
                        {
                            ServerPacket packet = new ServerPacket(Outgoing.userQuestion);
                            packet.AppendString(String.Format("#arena^1^1 {0}", GameServer.GetLanguage(user.languagePack, "arena.family.confirm.enter")));
                            user.SendPacket(packet);
                        }
                    }
                    break;
                case 23:
                    if (this.npcsManager.NpcInListById(npcId))
                    {
                        if (this.npcsManager.GetNpcById(npcId).nRunList.Contains(type.ToString()))
                        {
                            if (value == 0)
                            {
                                if (user.family == null)
                                {
                                    if (user.group != null && user.group.members_count == 3)
                                    {
                                        if (user.gold >= 200000)
                                        {
                                            ServerPacket packet = new ServerPacket(Outgoing.box);
                                            packet.AppendString("#glmk^");
                                            packet.AppendInt(14);
                                            packet.AppendInt(1);
                                            packet.AppendString(GameServer.GetLanguage(user.languagePack, "family.entername"));
                                            user.SendPacket(packet);
                                        }
                                        else
                                            user.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(user.languagePack, "error.gold")));
                                    }
                                    else
                                        user.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(user.languagePack, "error.createfamily.membercount")));
                                }
                                else
                                    user.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(user.languagePack, "error.createfamily.memberhf")));
                            }
                            else if (value == 1)
                            {
                                if (user.family != null)
                                {
                                    if (user.fMember.GetRank() != 0)
                                        user.SendPacket(GlobalMessage.MakeDialog("#gleave^1", "#gleave^2", GameServer.GetLanguage(user.languagePack, "message.family.confirmleave")));
                                    else
                                        user.SendPacket(GlobalMessage.MakeDialog("#gleave^1", "#gleave^2", GameServer.GetLanguage(user.languagePack, "message.family.confirmleave.head")));
                                }
                            }
                        }
                    }
                    break;
                default:
                    Console.WriteLine("Unknow n_run : {0} {1}", type, value, unknown);
                    break;
            }
        }
        #endregion

        #region Invitations
        public void SendGroupRequest(Player sender, int userId)
        {
            Player receiver = this.playersManagers.GetPlayerById(userId);
            if (receiver != null)
            {
                if (receiver.group != null)
                {
                    if (sender.group != null)
                    {
                        if (sender.group == receiver.group)
                        {
                            sender.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(sender.languagePack, "error.group.same")));
                            return;
                        }
                        sender.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(sender.languagePack, "error.group.algroup")));
                        return;
                    }
                    else if (receiver.group.members.Count >= 3)
                    {
                        sender.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(sender.languagePack, "error.group.invit.full")));
                        return;
                    }
                }
                if (!receiver.groupRequest.Contains(sender.id))
                    receiver.groupRequest.Add(sender.id);
                receiver.SendPacket(GlobalMessage.MakeDialog(String.Format("#pjoin^3^{0}", sender.id), String.Format("#pjoin^4^{0}", sender.id), String.Format(GameServer.GetLanguage(receiver.languagePack, "message.group.invit.receive"), sender.name)));
                sender.SendPacket(GlobalMessage.MakeInfo(String.Format(GameServer.GetLanguage(sender.languagePack, "message.group.invit.send"), receiver.name)));
            }
        }
        #endregion
    }
}
