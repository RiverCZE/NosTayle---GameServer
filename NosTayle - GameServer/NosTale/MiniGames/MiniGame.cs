using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.NosTale.Entities.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.MiniGames
{
    class MiniGame
    {
        internal int gameId;
        internal int b0Min, b0Max, b1Min, b1Max, b2Min, b2Max, b3Min, b3Max, b4Min, b4Max, b5Min, b5Max;
        internal Dictionary<int, Dictionary<int, List<MiniGameAward>>> awards;

        public MiniGame(int gameId, string b0, string b1, string b2, string b3, string b4, string b5, string awards)
        {
            this.gameId = gameId;
            string[] bScore = b0.Split(';');
            this.b0Min = Convert.ToInt32(bScore[0]);
            this.b0Max = Convert.ToInt32(bScore[1]);
            bScore = b1.Split(';');
            this.b1Min = Convert.ToInt32(bScore[0]);
            this.b1Max = Convert.ToInt32(bScore[1]);
            bScore = b2.Split(';');
            this.b2Min = Convert.ToInt32(bScore[0]);
            this.b2Max = Convert.ToInt32(bScore[1]);
            bScore = b3.Split(';');
            this.b3Min = Convert.ToInt32(bScore[0]);
            this.b3Max = Convert.ToInt32(bScore[1]);
            bScore = b4.Split(';');
            this.b4Min = Convert.ToInt32(bScore[0]);
            this.b4Max = Convert.ToInt32(bScore[1]);
            bScore = b5.Split(';');
            this.b5Min = Convert.ToInt32(bScore[0]);
            this.b5Max = Convert.ToInt32(bScore[1]);

            string[] levels = awards.Split('$');
            int level = 1;
            foreach (string boxs in levels)
            {
                Dictionary<int, List<MiniGameAward>> awardsBoxsList = new Dictionary<int, List<MiniGameAward>>();
                string[] box = boxs.Split(':');
                for (int i = 0; i < box.Length; i++)
                {
                    string[] awardsBox = box[i].Split('^');
                    List<MiniGameAward> awardsList = new List<MiniGameAward>();
                    for (int i2 = 0; i2 < awardsBox.Length; i2++)
                    {
                        string[] award = awardsBox[i2].Split('.');
                        int awardItemId = Convert.ToInt32(award[0]);
                        int awardAmount = Convert.ToInt32(award[1]);
                        if (GameServer.GetItemsManager().itemList.ContainsKey(awardItemId))
                            awardsList.Add(new MiniGameAward(awardAmount, GameServer.GetItemsManager().itemList[awardItemId]));
                    }
                    awardsBoxsList.Add(i, awardsList);
                }
                this.awards = new Dictionary<int, Dictionary<int, List<MiniGameAward>>>();
                this.awards.Add(level, awardsBoxsList);
                level++;
            }
        }

        public int GetBMax(int score)
        {
            int bMax;
            if (score >= b5Min)
                bMax = 4;
            else if (score >= b4Min)
                bMax = 3;
            else if (score >= b3Min)
                bMax = 2;
            else if (score >= b2Min)
                bMax = 1;
            else if (score >= b1Min)
                bMax = 0;
            else
                bMax = -1;
            return bMax;
        }

        public void LookAwards(Player user, int score)
        {
            int bMax = GetBMax(score);
            if (bMax == -1)
            {
                user.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(user.languagePack, "message.minigame.noscore")));
                return;
            }
            ServerPacket packet = new ServerPacket("mlo_lv");
            packet.AppendInt(bMax);
            user.SendPacket(packet);
        }

        public void TakeBox(Player user, int level, int box)
        {
            if (awards.ContainsKey(level))
            {
                if (awards[level].ContainsKey(box))
                {
                    user.gamePoints -= 100;
                    user.SendPoints();
                    int randomAward = new Random().Next(0, awards[level][box].Count);
                    ServerPacket packet = new ServerPacket("mlo_rw");
                    packet.AppendInt(awards[level][box][randomAward].item.id);
                    packet.AppendInt(awards[level][box][randomAward].amount);
                    user.SendPacket(packet);
                    user.inventory.AddBasicItem(user, awards[level][box][randomAward].item.inventory, awards[level][box][randomAward].item.id, awards[level][box][randomAward].amount);
                }
            }
        }
    }
}
