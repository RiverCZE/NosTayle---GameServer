using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.ReceivePackets;
using NosTayleGameServer.Communication.ReceivePackets.CharsLoadedPackets;
using NosTayleGameServer.Communication.ReceivePackets.EntitiesPackets;
using NosTayleGameServer.Communication.ReceivePackets.FamilyPackets;
using NosTayleGameServer.Communication.ReceivePackets.FriendPackets;
using NosTayleGameServer.Communication.ReceivePackets.GroupPackets;
using NosTayleGameServer.Communication.ReceivePackets.GuriPackets;
using NosTayleGameServer.Communication.ReceivePackets.InventoryPackets;
using NosTayleGameServer.Communication.ReceivePackets.MinilandPackets;
using NosTayleGameServer.Communication.ReceivePackets.PlayersPackets;
using NosTayleGameServer.Communication.ReceivePackets.ShopPackets;
using NosTayleGameServer.Communication.ReceivePackets.SpecialistPackets;
using NosTayleGameServer.Communication.ReceivePackets.SpecialPackets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication
{
    class StaticMessageHandler
    {
        private static Dictionary<string, Interface> handlersIG;
        private static Dictionary<string, Interface> handlersNIG;

        internal static void HandlePacket(Session session, SessionMessage message)
        {
            try
            {

                if (session.GetPlayer() != null)
                {
                    if (handlersIG.ContainsKey(message.GetHeader()))
                        handlersIG[message.GetHeader()].Handle(session, message);
                    else if (message.GetHeader()[0] == '$' && message.GetHeader().Length > 1)
                        new CommandEvent().Handle(session, message);
                    else if (message.GetHeader()[0] == '/')
                        new WhisperEvent().Handle(session, message);
                    else if (message.GetHeader()[0] == ';')
                        new SayGroupEvent().Handle(session, message);
                    else if (message.GetHeader()[0] == ':')
                        new FamilyChatEvent().Handle(session, message);
                    else if (message.GetHeader()[0] == '#')
                        new SharpEvent().Handle(session, message);
                    else if (message.GetHeader()[0] == '%')
                        new FamilyCommandEvent().Handle(session, message);
                    else
                        Console.WriteLine("Unknow packet: {0}", message.dataBrute);
                }
                else if (handlersNIG.ContainsKey(message.GetHeader()))
                    handlersNIG[message.GetHeader()].Handle(session, message);
                /*else
                     Console.WriteLine("Unknow packet: {0}", message.GetHeader());*/
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        internal static void Initialize()
        {
            handlersNIG = new Dictionary<string, Interface>();
            handlersIG = new Dictionary<string, Interface>();
            RegisterPacketLibary();
        }

        internal static void RegisterPacketLibary()
        {

            //BASE PACKETS LIST
            handlersNIG.Add(Incoming.delChar, new DeleteCharacterEvent());
            handlersNIG.Add(Incoming.newChar, new CreateCharacterEvent());
            handlersNIG.Add(Incoming.selectChar, new SelectCharacterEvent());


            //GAME PACKET LIST
            handlersIG.Add(Incoming.guri, new GuriEvent());
            handlersIG.Add(Incoming.rest, new PlayerRestEvent());
            handlersIG.Add(Incoming.walk, new PlayerWalkEvent());
            handlersIG.Add(Incoming.reqInfo, new InfoRequestEvent());
            handlersIG.Add(Incoming.npcStat, new EntitieStatEvent());
            handlersIG.Add(Incoming.npcReq, new NpcRequestEvent());
            handlersIG.Add(Incoming.shopReq, new ShopRequestEvent());
            handlersIG.Add(Incoming.buyShop, new ShopBuyEvent());
            handlersIG.Add(Incoming.npcSpecialReq, new NpcSpecialRequestEvent());
            handlersIG.Add(Incoming.attackPacket, new PlayerAttackEvent()); //10 
            handlersIG.Add(Incoming.playerSay, new PlayerSayEvent());
            handlersIG.Add(Incoming.playerOnPortall, new PlayerWalkOnPortalEvent());
            handlersIG.Add(Incoming.spReq, new PutSpEvent());
            handlersIG.Add(Incoming.equipmentInfo, new EquipmentInfoEvent());
            handlersIG.Add(Incoming.moveItem, new MoveItemEvent());
            handlersIG.Add(Incoming.moveItemSpecial, new MoveItemSpecialEvent());
            handlersIG.Add(Incoming.useItem, new UseItemEvent());
            handlersIG.Add(Incoming.deleteItem, new DeletItemEvent());
            handlersIG.Add(Incoming.wearItem, new WearItemEvent());
            handlersIG.Add(Incoming.unWearItem, new UnwearItemEvent()); //20
            handlersIG.Add(Incoming.miniLandEdit, new MinilandEditEvent());
            handlersIG.Add(Incoming.addMLItem, new MinilandAddItemEvent());
            handlersIG.Add(Incoming.removeMlItem, new MinilandRemoveItemEvent());
            handlersIG.Add(Incoming.useMlObject, new MinilandUseObjectEvent());
            handlersIG.Add(Incoming.miniGame, new MinilandGameReqEvent());
            handlersIG.Add(Incoming.pulse, new PlayerPulseEvent());
            handlersIG.Add(Incoming.upgradeItem, new UpgradeItemEvent());
            handlersIG.Add(Incoming.basicGroupReq, new BasicGroupRequestEvent());
            handlersIG.Add(Incoming.leaveGroup, new LeaveGroupEvent());
            handlersIG.Add(Incoming.groupOption, new UpdateGroupOptionEvent());
            handlersIG.Add(Incoming.friendRequest, new FriendRequestEvent()); //30
            handlersIG.Add(Incoming.delFriend, new FriendDeleteEvent());
            handlersIG.Add(Incoming.friendMessage, new FriendMessageEvent());
            handlersIG.Add(Incoming.enterFriendMl, new FriendEnterMiniland());
            handlersIG.Add(Incoming.sBarSet, new PlayerSetSkillBarEvent());
            /*handlersIG.Add(Incoming.mtList, new StaticRequestHandler(PacketLib.mtList));
            handlersIG.Add(Incoming.uAS, new StaticRequestHandler(PacketLib.uAs));*/
            handlersIG.Add(Incoming.infoFamily, new InfoFamilyEvent());
            handlersIG.Add(Incoming.editIntroFamilyBoxe, new EditIntroFamilyEvent());
            handlersIG.Add(Incoming.updateFamilyMemberRank, new UpdateFamilyMemberRankEvent());
            handlersIG.Add(Incoming.updateFamilyPermissionRank, new UpdateFamilyPermissionRankEvent()); //40
            handlersIG.Add(Incoming.actualiseHis, new ActualiseFamilyHistoricRequestEvent());
            handlersIG.Add(Incoming.moveEntitie, new MoveNpcEvent());
            handlersIG.Add(Incoming.dropItem, new DropItemEvent());
            handlersIG.Add(Incoming.getDropItem, new GetDropItemEvent());
            handlersIG.Add(Incoming.script, new ScriptEvent());
        }
    }
}
