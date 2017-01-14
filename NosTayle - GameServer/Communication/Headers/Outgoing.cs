﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.Headers
{
    public static class Outgoing
    {
        public static string charList;
        public static string charListStart;
        public static string charListEnd;
        public static string pBaseInfo;
        public static string levelStat;
        public static string personnalStat;
        public static string inEntitie;
        public static string placeUser;
        public static string mapIni;
        public static string simpleMessage;
        public static string bannerMessage;
        public static string speakEntity;
        public static string infoBox;
        public static string speWindows;
        public static string effectPacket;
        public static string spInfo;
        public static string playerStat;
        public static string updateHp;
        public static string guri;
        public static string rest;
        public static string movPlayer;
        public static string cancel;
        public static string dialog;
        public static string groupStatue;
        public static string pInit;
        public static string pStat;
        public static string friendInit;
        public static string friendInfo;
        public static string friendMessage;
        public static string box;
        public static string skillList;
        public static string invSlot;
        public static string npcDialog;
        public static string userQuestion;
        public static string respawn;
        public static string teleport;
        public static string arenaScore;
        public static string portal;
        public static string skillCharged;
        public static string cBar;
        public static string sBar;
        public static string invSlotCount;
        public static string inv;
        public static string delay;
        public static string eq;
        public static string pairy;
        public static string spPoints;
        public static string outT;
        public static string mapOut;
        public static string morph;
        public static string spCharge;
        public static string tit;
        public static string shop;
        public static string gold;
        public static string shopEnd;
        public static string miniLandInfo;
        public static string success;
        public static string makeShop;
        public static string shopMessage;
        public static string speedCond;
        public static string equip;
        public static string fd;
        public static string cInfo;
        public static string equipInfo;
        public static string entitieInfo;
        public static string minilandObjList;
        public static string rsfi;
        public static string scr;
        public static string faction;
        public static string mlvisitesenter;
        public static string entitieStat;
        public static string mltObj;
        public static string mlEff;
        public static string mlObj;
        public static string entitieDie;
        public static string entitieAttack;
        public static string attackEntitie;
        public static string entitieSpecialAttack;
        public static string zoneAttack;
        public static string specialEffect;
        public static string familyInfosBase;
        public static string familyAffich;
        public static string familyMembers;
        public static string familyUpdateConnexion;
        public static string familyMembersIntro;
        public static string modal;
        public static string familyIntroBoxe;
        public static string hisMustBeActualised;
        public static string hisPacket;
        public static string gmMode;
        public static string changeCharScale;
        public static string buff;
        public static string dropItem;
        public static string icon;
        public static string getDropItem;
        public static string actScene;
        public static string script;

        static Outgoing()
        {
            Outgoing.charList = "clist";
            Outgoing.charListStart = "clist_start";
            Outgoing.charListEnd = "clist_end";
            Outgoing.pBaseInfo = "tit";
            Outgoing.levelStat = "lev";
            Outgoing.personnalStat = "sc";
            Outgoing.inEntitie = "in";
            Outgoing.placeUser = "at";
            Outgoing.mapIni = "c_map";
            Outgoing.simpleMessage = "say";
            Outgoing.bannerMessage = "msg";
            Outgoing.speakEntity = "spk";
            Outgoing.infoBox = "info";
            Outgoing.speWindows = "wopen";
            Outgoing.effectPacket = "eff";
            Outgoing.spInfo = "slinfo";
            Outgoing.playerStat = "stat";
            Outgoing.updateHp = "rc";
            Outgoing.guri = "guri";
            Outgoing.rest = "rest";
            Outgoing.movPlayer = "mv";
            Outgoing.cancel = "cancel";
            Outgoing.dialog = "dlg";
            Outgoing.groupStatue = "pidx";
            Outgoing.pInit = "pinit";
            Outgoing.pStat = "pst";
            Outgoing.friendInit = "finit";
            Outgoing.friendInfo = "finfo";
            Outgoing.friendMessage = "talk";
            Outgoing.box = "inbox";
            Outgoing.skillList = "ski";
            Outgoing.invSlot = "ivn";
            Outgoing.npcDialog = "npc_req";
            Outgoing.userQuestion = "qna";
            Outgoing.respawn = "revive";
            Outgoing.teleport = "tp";
            Outgoing.arenaScore = "ascr";
            Outgoing.portal = "gp";
            Outgoing.skillCharged = "sr";
            Outgoing.cBar = "qslot";
            Outgoing.sBar = "qset";
            Outgoing.invSlotCount = "exts";
            Outgoing.inv = "inv";
            Outgoing.delay = "delay";
            Outgoing.eq = "eq";
            Outgoing.pairy = "pairy";
            Outgoing.spPoints = "sp";
            Outgoing.outT = "out";
            Outgoing.mapOut = "mapout";
            Outgoing.morph = "c_mode";
            Outgoing.spCharge = "sd";
            Outgoing.tit = "tit";
            Outgoing.shop = "shop";
            Outgoing.gold = "gold";
            Outgoing.shopEnd = "shop_end";
            Outgoing.miniLandInfo = "mlinfo";
            Outgoing.success = "success";
            Outgoing.makeShop = "n_inv";
            Outgoing.shopMessage = "s_memo";
            Outgoing.speedCond = "cond";
            Outgoing.equip = "equip";
            Outgoing.fd = "fd";
            Outgoing.cInfo = "c_info";
            Outgoing.equipInfo = "e_info";
            Outgoing.entitieInfo = "tc_info";
            Outgoing.minilandObjList = "mlobjlst";
            Outgoing.rsfi = "rsfi";
            Outgoing.scr = "scr";
            Outgoing.faction = "fs";
            Outgoing.mlvisitesenter = "mlinfobr";
            Outgoing.entitieStat = "st";
            Outgoing.mltObj = "mltobj";
            Outgoing.mlEff = "eff_g";
            Outgoing.mlObj = "mlobj";
            Outgoing.entitieDie = "die";
            Outgoing.entitieAttack = "ct";
            Outgoing.attackEntitie = "su";
            Outgoing.entitieSpecialAttack = "ct_n";
            Outgoing.zoneAttack = "bs";
            Outgoing.specialEffect = "eff_ob";
            Outgoing.familyInfosBase = "ginfo";
            Outgoing.familyAffich = "gidx";
            Outgoing.familyMembers = "gmbr";
            Outgoing.familyUpdateConnexion = "gcon";
            Outgoing.familyMembersIntro = "gmsg";
            Outgoing.modal = "modal";
            Outgoing.familyIntroBoxe = "today_stc";
            Outgoing.hisMustBeActualised = "fhis_stc";
            Outgoing.hisPacket = "ghis";
            Outgoing.gmMode = "gmapr";
            Outgoing.changeCharScale = "char_sc";
            Outgoing.buff = "bf";
            Outgoing.dropItem = "drop";
            Outgoing.icon = "icon";
            Outgoing.getDropItem = "get";
            Outgoing.actScene = "scene";
            Outgoing.script = "script";
        }
    }
}
