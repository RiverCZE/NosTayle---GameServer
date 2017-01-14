using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.Core;
using NosTayleGameServer.NosTale;
using NosTayleGameServer.NosTale.Entities.Players;
using NosTayleGameServer.NosTale.Entities.Players.Inventorys;
using NosTayleGameServer.NosTale.Familys;
using NosTayleGameServer.NosTale.Groups;
using NosTayleGameServer.NosTale.Maps;
using NosTayleGameServer.NosTale.Maps.SpecialMaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.SpecialPackets
{
    internal sealed class SharpEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            Player sender = Session.GetPlayer();
            string header = Event.dataBrute.Substring(1).Split('^')[0];
            string[] values = new string[0];
            if (Event.dataBrute.Split('^').Length > 1)
                values = Event.dataBrute.Substring(header.Length + 2).Split('^');
            switch (header)
            {
                case "sl":
                    if (values.Length == 1)
                    {
                        if (values[0] == "1")
                        {
                            if (sender.inAction)
                                return;
                            sender.PutSp();
                        }
                    }
                    break;
                case "b_i": //Supprimer un objet de l'inventaire
                    if (values.Length == 3)
                        if (values[2] == "1")
                            sender.inventory.RemoveItem(sender, Convert.ToInt32(values[0]), Convert.ToInt32(values[1]));
                    break;
                case "u_i": //Utilisation d'un objet de l'inventaire
                    if (values.Length == 5)
                    {
                        if (values[0] == "1" && Convert.ToInt32(values[1]) == sender.id)
                        {
                            if (Convert.ToInt32(values[3]) >= 0 && Convert.ToInt32(values[3]) <= 48 && sender.inventory.itemsList.ContainsKey(Convert.ToInt32(values[2])) && sender.inventory.itemsList[Convert.ToInt32(values[2])][Convert.ToInt32(values[3])] != null)
                            {
                                InventorySlot slot = sender.inventory.itemsList[Convert.ToInt32(values[2])][Convert.ToInt32(values[3])];
                                if (slot.invItem.isItem)
                                    slot.invItem.item.UseItem2(sender, Convert.ToInt32(values[2]), Convert.ToInt32(values[3]), Convert.ToInt32(values[4]));
                            }
                        }
                    }
                    break;
                case "mjoin": //Rejoindre un mini pays par un panneau
                    if (values.Length == 3)
                    {
                        Player owner = Session.GetChannel().GetPlayersManager().GetPlayerById(Convert.ToInt32(values[1]));
                        if (owner != null && owner.mlPanel != null && owner.mlPanel.mapId == sender.mapId)
                            owner.miniLand.EnterMiniLand(sender);
                    }
                    break;
                case "pjoin": //Accepter ou refuser une invitation de groupe
                    if (values.Length == 2)
                    {
                        Player iSender = sender.GetSession().GetChannel().GetPlayersManager().GetPlayerById(Convert.ToInt32(values[1]));
                        if (iSender != null && sender.groupRequest.Contains(iSender.id))
                        {
                            switch (values[0])
                            {
                                case "3":
                                    {
                                        sender.groupRequest.Remove(Convert.ToInt32(values[1]));
                                        if (sender.group == null && iSender.group == null)
                                        {
                                            Group group = new Group(iSender, sender);
                                            iSender.group = group;
                                            sender.group = group;
                                            if (sender.map == iSender.map)
                                            {

                                                ServerPacket packetG = new ServerPacket(Outgoing.groupStatue);
                                                packetG.AppendInt(1);
                                                packetG.AppendString("1." + iSender.id);
                                                packetG.AppendString("1." + sender.id);
                                                sender.map.SendMap(packetG, iSender.group.members);
                                            }
                                            else
                                            {
                                                ServerPacket packetG = new ServerPacket(Outgoing.groupStatue);
                                                packetG.AppendInt(1);
                                                packetG.AppendString("1." + iSender.id);
                                                iSender.map.SendMap(packetG, iSender.group.members);
                                                packetG = new ServerPacket(Outgoing.groupStatue);
                                                packetG.AppendInt(1);
                                                packetG.AppendString("1." + sender.id);
                                                sender.map.SendMap(packetG, sender.group.members);
                                            }
                                            group.SendGroupStatue();
                                            group.SendPinit();
                                            group.SendMemberStats();
                                            iSender.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(iSender.languagePack, "message.group.master")));
                                            return;
                                        }
                                        else
                                        {
                                            if (sender.group == null || iSender.group == null)
                                            {
                                                if (sender.group != null)
                                                {
                                                    if (sender.group.members.Count < 3)
                                                    {
                                                        iSender.group = sender.group;
                                                        iSender.group.members.Add(iSender);
                                                        ServerPacket packetG = new ServerPacket(Outgoing.groupStatue);
                                                        packetG.AppendInt(1);
                                                        packetG.AppendString("1." + iSender.id);
                                                        iSender.map.SendMap(packetG, iSender.group.members);
                                                        iSender.group.SendGroupStatue();
                                                        iSender.group.SendPinit();
                                                        iSender.group.SendMemberStats();
                                                        return;
                                                    }
                                                    else
                                                    {
                                                        sender.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(sender.languagePack, "error.group.invit.full")));
                                                        return;
                                                    }
                                                }
                                                else if (iSender.group != null)
                                                {
                                                    if (iSender.group.members.Count < 3)
                                                    {
                                                        sender.group = iSender.group;
                                                        sender.group.members.Add(sender);
                                                        ServerPacket packetG = new ServerPacket(Outgoing.groupStatue);
                                                        packetG.AppendInt(11);
                                                        packetG.AppendString("1." + sender.id);
                                                        sender.map.SendMap(packetG, sender.group.members);
                                                        sender.group.SendGroupStatue();
                                                        sender.group.SendPinit();
                                                        sender.group.SendMemberStats();
                                                    }
                                                    else
                                                        sender.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(sender.languagePack, "error.group.invit.full")));
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case "4":
                                    {
                                        sender.groupRequest.Remove(Convert.ToInt32(values[1]));
                                        iSender.SendPacket(GlobalMessage.MakeMessage(0, 0, 10, String.Format(GameServer.GetLanguage(iSender.languagePack, "message.group.invit.refuse"), sender.name)));
                                        return;
                                    }
                            }
                            sender.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(sender.languagePack, "message.group.invite.cantfind")));
                        }
                    }
                    break;
                case "gjoin": //Accepter ou refuser une invitation de famille
                    if (values.Length == 2)
                    {
                        Player iSender = Session.GetChannel().GetPlayersManager().GetPlayerById(Convert.ToInt32(values[1]));
                        if (iSender != null)
                        {
                            if (iSender.familyRequest.Contains(sender.id))
                            {
                                iSender.familyRequest.Remove(sender.id);
                                if (iSender.fMember.GetRank() == 2 && !iSender.family.GetPermission("gardCanInvit") || iSender.fMember.GetRank() > 3)
                                    return;
                                switch (values[0])
                                {
                                    case "1":
                                        if (sender.family == null && iSender.family != null)
                                            iSender.family.NewMember(iSender, sender);
                                        break;
                                    case "2":
                                        iSender.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(iSender.languagePack, "message.family.invit.refuse")));
                                        break;
                                }
                            }
                        }
                    }
                    break;
                case "gleave":
                    if (values.Length == 1)
                    {
                        if (values[0] == "1" && sender.family != null)
                        {
                            sender.family.LeaveFamily(sender);
                        }
                    }
                    break;
                case "fins": //Demande d'ami
                    if (values.Length == 2)
                    {
                        switch (values[0])
                        {
                            case "-1":
                                sender.friendList.AddFriend(sender, Convert.ToInt32(values[1]));
                                break;
                            case "-99":
                                if (sender.friendRequest.Contains(Convert.ToInt32(values[1])))
                                {
                                    sender.friendRequest.Remove(Convert.ToInt32(values[1]));
                                    Player reqSender = sender.GetSession().GetChannel().GetPlayersManager().GetPlayerById((Convert.ToInt32(values[1])));
                                    if (reqSender != null)
                                        reqSender.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(reqSender.languagePack, "message.you.blocked")));
                                }
                                break;
                        }
                    }
                    break;
                case "arena": //Entrer en arène
                    if (values.Length == 2 && !sender.map.specialMap)
                    {
                        switch (values[0])
                        {
                            case "0":
                                if (sender.gold >= 500)
                                {
                                    int newX = MapData.randomPoint(20, 35);
                                    int newY = MapData.randomPoint(20, 37);
                                    if ((newX == 28 && newY == 36) || (newX == 29 && newY == 35))
                                    {
                                        newX = 29;
                                        newY = 35;
                                    }
                                    Arena arena = sender.GetSession().GetChannel().arenas["individual"];
                                    sender.map.ChangeMapRequest(sender, arena, newX, newY);
                                    sender.gold -= 500;
                                    sender.SendGold();
                                }
                                else
                                    sender.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(sender.languagePack, "error.gold")));
                                break;
                            case "1":
                                if (sender.gold >= 1000)
                                {
                                    Arena arena = sender.GetSession().GetChannel().arenas["family"];
                                    int newX = MapData.randomPoint(28, 46);
                                    int newY = MapData.randomPoint(12, 28);
                                    sender.map.ChangeMapRequest(sender, arena, newX, newY);
                                    sender.gold -= 1000;
                                    sender.SendGold();
                                }
                                else
                                    sender.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(sender.languagePack, "error.gold")));
                                break;
                        }
                    }
                    break;
                case "revival":
                    if (values.Length == 1)
                    {
                        switch (values[0])
                        {
                            case "1":
                                if (sender.map.isArena)
                                {
                                    sender.rested = 0;
                                    sender.currentHp = 1;
                                    sender.currentMp = 1;
                                    sender.isDead = false;
                                    sender.canPvp = false;
                                    Map map = null;
                                    if (sender.GetSession().GetChannel().generalMaps.ContainsKey(sender.saveMap))
                                        map = sender.GetSession().GetChannel().generalMaps[sender.saveMap];
                                    else
                                    {
                                        sender.GetSession().GetChannel().generalMaps.Add(sender.saveMap, new Map(sender.saveMap, sender.GetSession().GetChannel()));
                                        map = sender.GetSession().GetChannel().generalMaps[sender.saveMap];
                                    }
                                    sender.map.ChangeMapRequest(sender, map, sender.saveX, sender.saveY);
                                }
                                break;
                            case "5":
                                if (sender.map.isArena)
                                {
                                    Arena arena = (Arena)sender.map;
                                    if (arena.arenaType != "individual")
                                        return;
                                    if (sender.gold >= 100)
                                    {
                                        sender.gold -= 100;
                                        sender.rested = 0;
                                        sender.currentHp = sender.maxHp;
                                        sender.currentMp = sender.maxMp;
                                        sender.isDead = false;
                                        sender.spawnTime = DateTime.Now;
                                        sender.canPvp = false;
                                        sender.SendStats(false);
                                        sender.SendGold();
                                        ServerPacket newPacket = new ServerPacket(Outgoing.teleport);
                                        newPacket.AppendInt(1);
                                        newPacket.AppendInt(sender.id);
                                        newPacket.AppendInt(sender.x);
                                        newPacket.AppendInt(sender.y);
                                        sender.map.SendMap(newPacket);
                                        newPacket = new ServerPacket(Outgoing.respawn);
                                        newPacket.AppendInt(1);
                                        newPacket.AppendInt(sender.id);
                                        newPacket.AppendInt(0);
                                        sender.map.SendMap(newPacket);
                                        return;
                                    }
                                    sender.rested = 0;
                                    sender.currentHp = 1;
                                    sender.currentMp = 1;
                                    sender.isDead = false;
                                    sender.canPvp = false;
                                    Map map = null;
                                    if (sender.GetSession().GetChannel().generalMaps.ContainsKey(sender.saveMap))
                                        map = sender.GetSession().GetChannel().generalMaps[sender.saveMap];
                                    else
                                    {
                                        sender.GetSession().GetChannel().generalMaps.Add(sender.saveMap, new Map(sender.saveMap, sender.GetSession().GetChannel()));
                                        map = sender.GetSession().GetChannel().generalMaps[sender.saveMap];
                                    }
                                    sender.map.ChangeMapRequest(sender, map, sender.saveX, sender.saveY);
                                }
                                break;
                            case "6":
                                {
                                    Arena arena = (Arena)sender.map;
                                    if (arena.arenaType != "family")
                                        return;
                                    if (sender.gold >= 200)
                                    {
                                        sender.gold -= 200;
                                        sender.rested = 0;
                                        sender.currentHp = sender.maxHp;
                                        sender.currentMp = sender.maxMp;
                                        sender.isDead = false;
                                        sender.spawnTime = DateTime.Now;
                                        sender.canPvp = false;
                                        sender.SendStats(false);
                                        sender.SendGold();
                                        ServerPacket newPacket = new ServerPacket(Outgoing.teleport);
                                        newPacket.AppendInt(1);
                                        newPacket.AppendInt(sender.id);
                                        newPacket.AppendInt(sender.x);
                                        newPacket.AppendInt(sender.y);
                                        sender.map.SendMap(newPacket);
                                        newPacket = new ServerPacket(Outgoing.respawn);
                                        newPacket.AppendInt(1);
                                        newPacket.AppendInt(sender.id);
                                        newPacket.AppendInt(0);
                                        sender.map.SendMap(newPacket);
                                        return;
                                    }
                                    sender.rested = 0;
                                    sender.currentHp = 1;
                                    sender.currentMp = 1;
                                    sender.isDead = false;
                                    sender.canPvp = false;
                                    Map map = null;
                                    if (sender.GetSession().GetChannel().generalMaps.ContainsKey(sender.saveMap))
                                        map = sender.GetSession().GetChannel().generalMaps[sender.saveMap];
                                    else
                                    {
                                        sender.GetSession().GetChannel().generalMaps.Add(sender.saveMap, new Map(sender.saveMap, sender.GetSession().GetChannel()));
                                        map = sender.GetSession().GetChannel().generalMaps[sender.saveMap];
                                    }
                                    sender.map.ChangeMapRequest(sender, map, sender.saveX, sender.saveY);
                                }
                                break;
                        }
                    }
                    break;
                case "glmk":
                    {
                        if (values.Length == 1)
                        {
                            Family.CreateFamily(Session.GetPlayer(), values[0]);
                        }
                    }
                    break;
                default:
                    Console.WriteLine(Event.dataBrute);
                    break;
            }
        }
    }
}
