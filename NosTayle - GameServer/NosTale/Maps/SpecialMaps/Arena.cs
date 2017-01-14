using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.NosTale.Entities;
using NosTayleGameServer.NosTale.Entities.Managers;
using NosTayleGameServer.NosTale.Entities.Npcs;
using NosTayleGameServer.NosTale.Entities.Players;
using NosTayleGameServer.NosTale.Maps.Portals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Maps.SpecialMaps
{
    class Arena : Map
    {
        internal string arenaType;
        public Arena(int id, string arenaType)
        {
            this.specialMap = true;
            this.isArena = true;
            this.mapId = id;
            this.arenaType = arenaType;
            portalsManager = new PortalsManager();
            if (arenaType == "individual")
                this.portalsManager.AddPortal(1, new Portal(1, 0, "special_exit", this.mapId, 4998, 37, 15, 0, 0, 0));
            else if (arenaType == "family")
                this.portalsManager.AddPortal(1, new Portal(1, 0, "special_exit", this.mapId, 4998, 38, 3, 0, 0, 0));
            if (npcsManager == null)
                npcsManager = new NpcsManager(this.mapId, GameServer.GetNpcsManager().entitieList, true, this);
            if (monstersManager == null)
                monstersManager = new NpcsManager(this.mapId, GameServer.GetMonstersManager().entitieList, true, this);
            StartCycleTask();
        }

        override public void SendOtherMapInfos(Player receiver)
        {
            if (receiver.group == null)
                receiver.SendArenaScore(true);
            else
                receiver.group.SendToAll(receiver.GetArenaScore(true));
        }

        #region Attaque
        override public bool CanAttack(Entitie sender, Entitie cible)
        {
            if (sender == cible || sender.isDead)
                return false;
            if (sender.type == 3 && !((Npc)sender).isPet || cible.type == 3 && !((Npc)cible).isPet && sender != cible && !sender.isDead)
                return true;
            if (arenaType == "individual")
            {
                if (sender.type == 1 && cible.type == 1)
                {
                    Player player = (Player)sender;
                    Player cPlayer = (Player)cible;
                    if ((player.canPvp && cPlayer.canPvp) && (player.group != null && cPlayer.group != null ? (player.group != cPlayer.group ? true : false) : true))
                        return true;
                }
            }
            else if (arenaType == "family")
            {
                if (sender.type == 1 && cible.type == 1)
                {
                    Player player = (Player)sender;
                    Player cPlayer = (Player)cible;
                    if (player.canPvp && cPlayer.canPvp && player.family != cPlayer.family)
                        return true;
                }
            }
            return false;
        }

        override public void EntitieDead(Entitie killer, Entitie entitie)
        {
            entitie.isDead = true;
            string text = String.Format("Mort : [{0}] by [{1}]", entitie.name, killer.name);
            this.SendMap(GlobalMessage.MakeAlert(0, text));
            this.SendMap(GlobalMessage.MakeMessage(0, 0, 10, text));
            if (killer.type == 1)
            {
                if (entitie.type == 1)
                {
                    Player player = (Player)killer;
                    player.arenaKill++;
                    player.arenaKillNow++;
                    player.SendPacket(GlobalMessage.MakeMessage(0, 0, 10, String.Format("[Score] KILL:{0}, DEAD:{1}", player.arenaKillNow, player.arenaDeadNow)));
                    if (player.group != null)
                        player.group.SendToAll(player.GetArenaScore(true));
                    else
                        player.SendArenaScore(true);
                }
            }
            if (entitie.type == 1)
            {
                Player player = (Player)entitie;
                if (killer.type == 1)
                {
                    player.arenaDead++;
                    player.arenaDeadNow++;
                    player.SendPacket(GlobalMessage.MakeMessage(0, 0, 10, String.Format("[Score] KILL:{0}, DEAD:{1}", player.arenaKillNow, player.arenaDeadNow)));
                    if (player.group != null)
                        player.group.SendToAll(player.GetArenaScore(true));
                    else
                        player.SendArenaScore(true);
                }
                if (arenaType == "individual")
                    player.SendPacket(GlobalMessage.MakeDialog("#revival^5", "#revival^1", GameServer.GetLanguage(player.languagePack, "user.arena.individual.revive")));
                else if (arenaType == "family")
                    player.SendPacket(GlobalMessage.MakeDialog("#revival^6", "#revival^1", GameServer.GetLanguage(player.languagePack, "user.arena.family.revive")));
            }
        }

        override public void CyclePlayer(Player user)
        {
            if (!user.canPvp && DateTime.Now.Subtract(user.spawnTime).TotalSeconds >= 3)
            {
                user.canPvp = true;
                user.SendPacket(GlobalMessage.MakeMessage(0, 0, 10, GameServer.GetLanguage(user.languagePack, "user.canpvp")));
            }
        }
        #endregion
    }
}
