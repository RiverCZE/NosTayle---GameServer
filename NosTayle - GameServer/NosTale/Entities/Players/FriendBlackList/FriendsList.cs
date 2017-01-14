using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Entities.Players.FriendBlackList
{
    public class FriendsList
    {
        internal int userId;
        internal List<LPlayer> friends;

        public FriendsList(int userId)
        {
            this.userId = userId;
        }

        public void Initialize()
        {
            this.friends = new List<LPlayer>();
            DataTable dataTable;
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("@id", this.userId);
                dataTable = dbClient.ReadDataTable("SELECT * FROM friends_server" + GameServer.serverId + " WHERE userOne = @id ORDER BY id ASC;");
            }
            foreach (DataRow friendRow in dataTable.Rows)
                this.friends.Add(new LPlayer((int)friendRow["userTwo"], GameCrypto.GetRealString(friendRow["username"].ToString())));
        }

        #region Packets concernant la liste
        public ServerPacket InitList() //Packet d'envoie de la liste complète.
        {
            ServerPacket packet = new ServerPacket(Outgoing.friendInit);
            if (friends.Count > 0)
                foreach (LPlayer friend in friends)
                    packet.AppendString(friend.GetId() + "|0|" + (GameServer.GetPlayersManager().PlayerInListById(friend.GetId()) ? 1 : 0) + "|" + friend.GetName());
            return packet;
        }


        public ServerPacket FriendInfo()
        {
            ServerPacket packet = new ServerPacket(Outgoing.friendInfo);
            if (friends.Count > 0)
                foreach (LPlayer friend in friends)
                    packet.AppendString(friend.GetId() + "." + (GameServer.GetPlayersManager().PlayerInListById(friend.GetId()) ? 1 : 0));
            return packet;
        }
        #endregion


        public void SendConnexionToAll(string name)
        {
            if (friends.Count > 0)
            {
                foreach (LPlayer friend in friends)
                {
                    Player rFriend = GameServer.GetPlayersManager().GetPlayerById(friend.GetId());
                    if (rFriend != null)
                    {
                        rFriend.SendPacket(rFriend.GetFriendList().FriendInfo());
                        rFriend.SendPacket(GlobalMessage.MakeMessage(0, 0, 10, String.Format(GameServer.GetLanguage(rFriend.languagePack, "enter.friend.message"), name)));
                    }
                }
            }
        }


        public void SendDisconection()
        {
            if (friends.Count > 0)
                foreach (LPlayer friend in friends)
                {
                    Player rFriend = GameServer.GetPlayersManager().GetPlayerById(friend.GetId());
                    if (rFriend != null)
                        rFriend.SendPacket(rFriend.GetFriendList().FriendInfo());
                }
        }


        public LPlayer GetFriend(int fId)
        {
            return this.friends.Find(x => x.GetId() == fId);
        }

        public void AddFriend(Player friend, int friendId)
        {
            if (friend.friendRequest.Contains(friendId))
            {
                friend.friendRequest.Remove(friendId);
                if (this.friends.Count < 80)
                {
                    Player newFriend = friend.GetSession().GetChannel().GetPlayersManager().GetPlayerById(friendId);
                    if (newFriend != null)
                    {
                        if (newFriend.friendList.friends.Count < 80)
                        {
                            if (this.GetFriend(friendId) == null)
                            {
                                using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
                                {
                                    dbClient.AddParamWithValue("one_id", friend.id);
                                    dbClient.AddParamWithValue("two_id", friendId);
                                    dbClient.AddParamWithValue("username_one", friend.name);
                                    dbClient.AddParamWithValue("username_two", newFriend.name);
                                    dbClient.ExecuteQuery("INSERT INTO `friends_server" + GameServer.serverId + "`(`userOne`, `userTwo`, `username`) VALUES (@one_id,@two_id,@username_two)");
                                    dbClient.ExecuteQuery("INSERT INTO `friends_server" + GameServer.serverId + "`(`userOne`, `userTwo`, `username`) VALUES (@two_id,@one_id,@username_one)");
                                }
                                this.friends.Add(new LPlayer(friendId, newFriend.name));
                                newFriend.friendList.friends.Add(new LPlayer(friend.id, friend.name));
                                friend.SendPacket(this.FriendInfo());
                                friend.SendPacket(this.InitList());
                                newFriend.SendPacket(newFriend.friendList.FriendInfo());
                                newFriend.SendPacket(newFriend.friendList.InitList());
                                friend.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(friend.languagePack, "message.friendreq.ok")));
                                newFriend.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(newFriend.languagePack, "message.friendreq.ok")));
                                return;
                            }
                            friend.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(friend.languagePack, "error.friendreq.alreadyfriend")));
                            newFriend.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(newFriend.languagePack, "error.friendreq.alreadyfriend")));
                            return;
                        }
                        newFriend.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(newFriend.languagePack, "error.friendlist.full")));
                        return;
                    }
                    friend.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(friend.languagePack, "error.friendreq.nosender")));
                    return;
                }
                friend.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(friend.languagePack, "error.friendlist.full")));
            }
        }

        public void DelFriend(Player player, int friendId, bool firstDel)
        {
            LPlayer friend = this.GetFriend(friendId);
            if (friend != null)
            {
                if (firstDel)
                {
                    Player rFriend = GameServer.GetPlayersManager().GetPlayerById(friendId);
                    if (rFriend != null)
                    {
                        rFriend.GetFriendList().DelFriend(rFriend, player.GetId(), false);
                    }
                    using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("userOne", player.GetId());
                        dbClient.AddParamWithValue("userTwo", friendId);
                        dbClient.ExecuteQuery("DELETE FROM `friends_server" + GameServer.serverId + "` WHERE userOne = @userOne AND userTwo = @userTwo OR userOne = @userTwo AND userTwo = @userOne");
                    }
                }
                this.friends.Remove(friend);
            }
            player.SendPacket(this.InitList());
        }

        public void SendMessage(Player player, int friendId, string message)
        {
            Player receiver = GameServer.GetPlayersManager().GetPlayerById(friendId);
            if (receiver != null)
            {
                if (receiver.friendList.GetFriend(player.id) != null && this.GetFriend(friendId) != null)
                {
                    ServerPacket packet = new ServerPacket(Outgoing.friendMessage);
                    packet.AppendInt(player.id);
                    packet.AppendString(message);
                    receiver.SendPacket(packet);
                }
                return;
            }
            player.SendPacket(GlobalMessage.MakeAlert(0, "error.friendmessage.notfoundreceiver"));
        }

        public void GoToMiniLand(Player player, int friendId)
        {
            Player owner = player.GetSession().GetChannel().GetPlayersManager().GetPlayerById(friendId);
            if (owner != null)
            {
                if (owner.friendList.GetFriend(player.id) != null && this.GetFriend(friendId) != null)
                    player.map.ChangeMapRequest(player, owner.miniLand, 5, 8);
                return;
            }
            player.SendPacket(GlobalMessage.MakeAlert(0, "error.friendmessage.notfoundreceiver"));
        }
    }
}
