using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Entities.Players
{
    class SkillBar
    {
        internal List<string[]> slots;

        public SkillBar(string skillList)
        {
            this.slots = new List<string[]>();
            this.slots.Add(new string[10] { "0.255.-1", "0.255.-1", "0.255.-1", "0.255.-1", "0.255.-1", "0.255.-1", "0.255.-1", "0.255.-1", "0.255.-1", "0.255.-1" });
            this.slots.Add(new string[10] { "0.255.-1", "0.255.-1", "0.255.-1", "0.255.-1", "0.255.-1", "0.255.-1", "0.255.-1", "0.255.-1", "0.255.-1", "0.255.-1" });
            this.slots.Add(new string[10] { "0.255.-1", "0.255.-1", "0.255.-1", "0.255.-1", "0.255.-1", "0.255.-1", "0.255.-1", "0.255.-1", "0.255.-1", "0.255.-1" });
            int slotId = 0;
            foreach (string slot in skillList.Split(':'))
            {
                if (slotId >= 3)
                    break;
                if (slot.Length >= 5)
                {
                    string[] s = slot.Split(' ');
                    for (int i = 0; i < (s.Length > 10 ? 10 : s.Length); i++)
                    {
                        if (s[i].Split('.').Length == 3)
                        {
                            this.slots[slotId][i] = s[i];
                        }
                    }
                }
                slotId++;
            }
        }

        public string SBarToDbText()
        {
            string sBar = "";
            for (int i = 0; i < this.slots.Count; i++)
            {
                if (i == 1)
                    sBar += ":";
                for (int i2 = 0; i2 < this.slots[i].Length; i2++)
                {
                    sBar += this.slots[i][i2];
                    if (i2 != this.slots[i].Length - 1)
                        sBar += " ";
                }
                if (i == 1)
                    sBar += ":";
            }
            return sBar;
        }

        public void SendBarToUser(Player player)
        {
            for (int i = 0; i < this.slots.Count; i++)
            {
                ServerPacket packet = new ServerPacket(Outgoing.cBar);
                packet.AppendInt(i);
                foreach (string s in this.slots[i])
                    packet.AppendString(s);
                player.SendPacket(packet);
            }
        }

        public void AddSbar(Player player, int type, int sBarId, int sBarCase, int itype, int value)
        {
            if (sBarId >= 0 && sBarId <= 2 && sBarCase >= 0 && sBarCase <= 9)
            {
                if (type == 0 && itype != 1 && itype != 2)
                    return;
                this.slots[sBarId][sBarCase] = type + "." + itype + "." + value;
                ServerPacket packet = new ServerPacket(Outgoing.sBar);
                packet.AppendInt(sBarId);
                packet.AppendInt(sBarCase);
                packet.AppendString(type + "." + itype + "." + value + ".0");
                player.SendPacket(packet);
            }
        }

        public void DelSBar(Player player, int sBarId, int sBarCase)
        {
            if (sBarId >= 0 && sBarId <= 2 && sBarCase >= 0 && sBarCase <= 9)
            {
                this.slots[sBarId][sBarCase] = "0.255.-1";
                ServerPacket packet = new ServerPacket(Outgoing.sBar);
                packet.AppendInt(sBarId);
                packet.AppendInt(sBarCase);
                packet.AppendString("0.255.-1.0");
                player.SendPacket(packet);
            }
        }

        public void MoveSBar(Player player, int sBarIdTarget, int sBarCaseTarget, int sBarId, int sBarCase)
        {
            if (sBarId >= 0 && sBarId <= 2 && sBarCase >= 0 && sBarCase <= 9 && sBarIdTarget >= 0 && sBarIdTarget <= 2 && sBarCaseTarget >= 0 && sBarCaseTarget <= 9)
            {
                string[] sBarTargetCase = this.slots[sBarIdTarget][sBarCaseTarget].Split('.');
                string[] sBar = this.slots[sBarId][sBarCase].Split('.');
                string sBarCopy = this.slots[sBarId][sBarCase];
                string sBarTargetCopy = this.slots[sBarIdTarget][sBarCaseTarget];
                if (sBarTargetCase.Length == 3 && sBar.Length == 3)
                {
                    this.slots[sBarId][sBarCase] = sBarTargetCopy;
                    this.slots[sBarIdTarget][sBarCaseTarget] = sBarCopy;
                    ServerPacket packet = new ServerPacket(Outgoing.sBar);
                    packet.AppendInt(sBarId);
                    packet.AppendInt(sBarCase);
                    packet.AppendString(sBarTargetCase[0] + "." + sBarTargetCase[1] + "." + sBarTargetCase[2] + ".0");
                    player.SendPacket(packet);

                    packet = new ServerPacket(Outgoing.sBar);
                    packet.AppendInt(sBarIdTarget);
                    packet.AppendInt(sBarCaseTarget);
                    packet.AppendString(sBar[0] + "." + sBar[1] + "." + sBar[2] + ".0");
                    player.SendPacket(packet);
                }
            }
        }
    }
}
