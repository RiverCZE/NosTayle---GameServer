using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.Headers
{
    public static class Incoming
    {
        internal static string delChar;
        internal static string newChar;
        internal static string selectChar;
        internal static string guri;
        internal static string rest;
        internal static string walk;
        internal static string reqInfo;
        internal static string npcStat;
        internal static string npcReq;
        internal static string shopReq;
        internal static string buyShop;
        internal static string npcSpecialReq;
        internal static string attackPacket;
        internal static string playerSay;
        internal static string playerOnPortall;
        internal static string spReq;
        internal static string equipmentInfo;
        internal static string moveItem;
        internal static string moveItemSpecial;
        internal static string useItem;
        internal static string deleteItem;
        internal static string wearItem;
        internal static string unWearItem;
        internal static string miniLandEdit;
        internal static string addMLItem;
        internal static string removeMlItem;
        internal static string useMlObject;
        internal static string miniGame;
        internal static string pulse;
        internal static string upgradeItem;
        internal static string basicGroupReq;
        internal static string leaveGroup;
        internal static string groupOption;
        internal static string friendRequest;
        internal static string delFriend;
        internal static string friendMessage;
        internal static string enterFriendMl;
        internal static string sBarSet;
        internal static string mtList;
        internal static string uAS;
        internal static string infoFamily;
        internal static string editIntroFamilyBoxe;
        internal static string updateFamilyMemberRank;
        internal static string updateFamilyPermissionRank;
        internal static string actualiseHis;
        internal static string moveEntitie;
        internal static string dropItem;
        internal static string getDropItem;
        internal static string script;

        static Incoming()
        {
            Incoming.delChar = "Char_DEL";
            Incoming.newChar = "Char_NEW";
            Incoming.selectChar = "select";
            Incoming.guri = "guri";
            Incoming.rest = "rest";
            Incoming.walk = "walk";
            Incoming.reqInfo = "req_info";
            Incoming.npcStat = "ncif";
            Incoming.npcReq = "npc_req";
            Incoming.shopReq = "shopping";
            Incoming.buyShop = "buy";
            Incoming.npcSpecialReq = "n_run";
            Incoming.attackPacket = "u_s";
            Incoming.playerSay = "say";
            Incoming.playerOnPortall = "preq";
            Incoming.spReq = "sl";
            Incoming.equipmentInfo = "eqinfo";
            Incoming.moveItem = "mvi";
            Incoming.moveItemSpecial = "mve";
            Incoming.useItem = "u_i";
            Incoming.deleteItem = "b_i";
            Incoming.wearItem = "wear";
            Incoming.unWearItem = "remove";
            Incoming.miniLandEdit = "mledit";
            Incoming.addMLItem = "addobj";
            Incoming.removeMlItem = "rmvobj";
            Incoming.useMlObject = "useobj";
            Incoming.miniGame = "mg";
            Incoming.pulse = "pulse";
            Incoming.upgradeItem = "up_gr";
            Incoming.basicGroupReq = "pjoin";
            Incoming.leaveGroup = "pleave";
            Incoming.groupOption = "gop";
            Incoming.friendRequest = "fins";
            Incoming.delFriend = "fdel";
            Incoming.friendMessage = "btk";
            Incoming.enterFriendMl = "mjoin";
            Incoming.sBarSet = "qset";
            Incoming.mtList = "mtlist";
            Incoming.uAS = "u_as";
            Incoming.infoFamily = "glist";
            Incoming.editIntroFamilyBoxe = "today_cts";
            Incoming.updateFamilyMemberRank = "fmg";
            Incoming.updateFamilyPermissionRank = "fauth";
            Incoming.actualiseHis = "fhis_cts";
            Incoming.moveEntitie = "ptctl";
            Incoming.dropItem = "put";
            Incoming.getDropItem = "get";
            Incoming.script = "script";
        }
    }
}
