using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.Core;
using NosTayleGameServer.NosTale.Entities.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Groups
{
    public class Group
    {
        internal int groupOwner, dropStatut;
        internal List<Player> members;
        internal int members_count
        {
            get
            {
                return this.members.Count;
            }
        }
        internal DateTime lastSendStat;
        private bool inCycle;

        public Group(Player owner, Player otherPlayer)
        {
            if ((owner != null && otherPlayer != null) && (owner.group == null && otherPlayer.group == null))
            {
                this.dropStatut = 1;
                this.members = new List<Player>();
                this.groupOwner = owner.id;
                this.members.Add(owner);
                this.members.Add(otherPlayer);
                owner.group = this;
                otherPlayer.group = this;
            }
        }

        public void MemberStatSend()
        {
            if (!this.inCycle)
            {
                this.inCycle = true;
                if (DateTime.Now.Subtract(this.lastSendStat).TotalMilliseconds >= 2500)
                {
                    this.SendMemberStats();
                }
                this.inCycle = false;
            }
        }

        public void SendMemberStats()
        {
            foreach (Player user in members)
            {
                int i = 0;
                foreach (Player member in members)
                {
                    i++;
                    if (member != user && !user.GetSession().GetSock().isDisconnected())
                    {
                        ServerPacket packet = new ServerPacket(Outgoing.pStat);
                        packet.AppendInt(1);
                        packet.AppendInt(member.id);
                        packet.AppendInt(i);
                        packet.AppendInt(ServerMath.Percent(member.currentHp, member.maxHp));
                        packet.AppendInt(ServerMath.Percent(member.currentMp, member.maxMp));
                        packet.AppendInt(member.currentHp);
                        packet.AppendInt(member.currentMp);
                        packet.AppendInt(member.userClass);
                        packet.AppendInt(member.gender);
                        packet.AppendInt(member.GetMorph());
                        user.SendPacket(packet);
                    }
                }
            }
            this.lastSendStat = DateTime.Now;
        }

        public void SendPinit(Player user)
        {
            if (this.members.Contains(user))
            {
                ServerPacket packet = new ServerPacket(Outgoing.pInit);
                packet.AppendInt(this.members.Count);
                int i = 0;
                foreach (Player member in members)
                {
                    i++;
                    packet.AppendString("1|" + member.id + "|" + i + "|" + member.level + "|" + member.name + "|11|" + member.gender + "|" + member.userClass + "|" + member.GetMorph());
                }
                user.SendPacket(packet);
            }
        }

        public void SendPinit()
        {
            foreach (Player user in members)
            {
                ServerPacket packet = new ServerPacket(Outgoing.pInit);
                packet.AppendInt(this.members.Count);
                int i = 0;
                foreach (Player member in members)
                {
                    i++;
                    packet.AppendString("1|" + member.id + "|" + i + "|" + member.level + "|" + member.name + "|11|" + member.gender + "|" + member.userClass + "|" + member.GetMorph());
                }
                user.SendPacket(packet);
            }
        }

        public void RemoveUser(Player user)
        {
            this.members.Remove(user);
            if (this.members.Count <= 1)
            {
                foreach (Player member in members)
                {
                    member.group = null;
                    ServerPacket packet = new ServerPacket(Outgoing.pInit);
                    packet.AppendInt(0);
                    member.SendPacket(packet);
                    member.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(member.languagePack, "message.group.destroyed")));
                    packet = new ServerPacket(Outgoing.groupStatue);
                    packet.AppendInt(-1);
                    packet.AppendString("1." + member.id);
                    member.map.SendMap(packet);
                }
            }
            else
            {
                if (user.id == this.groupOwner)
                {
                    Player newOwner = members[0];
                    this.groupOwner = newOwner.id;
                    newOwner.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(newOwner.languagePack, "message.group.newchief")));
                    foreach (Player p in this.members)
                    {
                        ServerPacket packet = new ServerPacket(Outgoing.groupStatue);
                        packet.AppendInt(1);
                        packet.AppendString("1." + p.id);
                        p.map.SendMap(p, packet, false);
                    }
                }
                this.SendPinit();
                this.SendMemberStats();
            }
            GC.Collect();
        }

        public void SendToGroup(Player user, ServerPacket message, bool userIncluded)
        {
            if (this.members.Contains(user))
            {
                foreach (Player member in members)
                {
                    if (user == member && !userIncluded)
                        continue;
                    member.SendPacket(message);
                }
            }
        }

        public void SendToAll(ServerPacket message)
        {
            foreach (Player member in members)
                member.SendPacket(message);
        }

        public void UpdateDrop()
        {
            foreach (Player member in members)
                member.SendPacket(GlobalMessage.MakeAlert(0, String.Format(GameServer.GetLanguage(member.languagePack, "message.drop.updated"), (this.dropStatut == 0 ? GameServer.GetLanguage(member.languagePack, "message.drop.updated.0") : GameServer.GetLanguage(member.languagePack, "message.drop.updated.1")))));
        }

        public void SendGroupStatue()
        {
            ServerPacket packet = new ServerPacket(Outgoing.groupStatue);
            packet.AppendInt(11);
            foreach (Player member in members)
            {
                packet.AppendString("1." + member.id);
            }
            this.SendToAll(packet);
        }
    }
}
