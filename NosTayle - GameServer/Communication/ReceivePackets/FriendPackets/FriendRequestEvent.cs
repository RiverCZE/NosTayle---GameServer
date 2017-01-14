using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.NosTale;
using NosTayleGameServer.NosTale.Entities.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.FriendPackets
{
    internal sealed class FriendRequestEvent : Interface
    {
        public void Handle(Session Session, SessionMessage Event)
        {
            if (Event.valuesCount == 2)
            {
                if (Event.GetValue(0) == "1")
                {
                    Player otherPlayer = Session.GetPlayer().map.playersManagers.GetPlayerById(Convert.ToInt32(Event.GetValue(1)));
                    if (otherPlayer != null)
                    {
                        if (Session.GetPlayer() == otherPlayer)
                            return;
                        if (Session.GetPlayer().InBatle())
                        {
                            Session.SendPacket(GlobalMessage.MakeInfo(GameServer.GetLanguage(Session.GetPlayer().languagePack, "message.you.inbatle")));
                            return;
                        }
                        if (otherPlayer.InBatle())
                        {
                            Session.SendPacket(GlobalMessage.MakeInfo(String.Format(GameServer.GetLanguage(Session.GetPlayer().languagePack, "message.cible.inbatle"), otherPlayer.name)));
                            return;
                        }
                        if (!otherPlayer.friendRequest.Contains(Session.GetPlayer().id))
                            otherPlayer.friendRequest.Add(Session.GetPlayer().id);
                        otherPlayer.SendPacket(GlobalMessage.MakeDialog("#fins^-1^" + Session.GetPlayer().id, "#fins^-99^" + Session.GetPlayer().id, String.Format(GameServer.GetLanguage(otherPlayer.languagePack, "message.friend.request"), Session.GetPlayer().name)));
                    }
                }
            }
        }
    }
}
