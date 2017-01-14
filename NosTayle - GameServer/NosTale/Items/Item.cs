using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.Core;
using NosTayleGameServer.NosTale.Entities.Npcs;
using NosTayleGameServer.NosTale.Entities.Players;
using NosTayleGameServer.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Items
{
    public class Item
    {
        internal int id;
        internal int charId;
        internal ItemBase itemBase;
        internal int invPos;
        internal bool equiped;
        internal int rare;
        internal int upgrade;
        internal int color;
        internal int damageMin;
        internal int damageMax;
        internal int hitRate;
        internal int corpDef;
        internal int distDef;
        internal int magicDef;
        internal int dodge;
        internal bool rune;
        internal int runeCharId;
        internal bool nivFixed;

        //INVENTORY
        internal int inEquip;

        //SAVE
        internal bool mustInsert = false;

        //SPECIAL
        internal int x;
        internal int y;
        internal int durabilites;

        public Item(int id, int charId, int weaponId = -1)
        {
            if (id != -1)
            {
                DataTable dataTable = null;
                using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("id", id);
                    dbClient.AddParamWithValue("charId", charId);
                    dataTable = dbClient.ReadDataTable("SELECT * FROM items_server" + GameServer.serverId + " WHERE id = @id AND charId = @charId;");
                }
                if (dataTable.Rows.Count == 1)
                {
                    DataRow itemRow = dataTable.Rows[0];
                    this.id = (int)itemRow["id"];
                    this.charId = (int)itemRow["charId"];
                    this.itemBase = GameServer.GetItemsManager().itemList[(int)itemRow["itemId"]];
                    this.invPos = (int)itemRow["invPos"];
                    this.equiped = ServerMath.StringToBool(itemRow["equiped"].ToString());
                    this.rare = (int)itemRow["rare"];
                    this.upgrade = (int)itemRow["upgrade"];
                    this.color = (int)itemRow["color"];
                    this.damageMin = (int)itemRow["damageMin"];
                    this.damageMax = (int)itemRow["damageMax"];
                    this.hitRate = (int)itemRow["hitRate"];
                    this.corpDef = (int)itemRow["corpDef"];
                    this.distDef = (int)itemRow["distDef"];
                    this.magicDef = (int)itemRow["magicDef"];
                    this.dodge = (int)itemRow["dodge"];
                    this.rune = ServerMath.StringToBool(itemRow["rune"].ToString());
                    this.runeCharId = (int)itemRow["runeCharId"];
                    this.nivFixed = ServerMath.StringToBool(itemRow["fixed"].ToString());
                    this.inEquip = Convert.ToInt32(itemRow["inEquip"].ToString());
                    this.x = (int)itemRow["x"];
                    this.y = (int)itemRow["y"];
                    this.durabilites = (int)itemRow["durabilites"];
                }
                else
                {
                    this.id = -1;
                    this.charId = -1;
                    this.invPos = -1;
                    this.equiped = false;
                    this.rare = 0;
                    this.upgrade = 0;
                    this.color = -1;
                    this.damageMin = 0;
                    this.damageMax = 0;
                    this.corpDef = 0;
                    this.distDef = 0;
                    this.magicDef = 0;
                    this.dodge = 0;
                    this.rune = false;
                    this.runeCharId = -1;
                    this.nivFixed = false;
                    this.itemBase = new ItemBase(-1);
                }
            }
            else
            {
                this.id = -1;
                this.charId = -1;
                this.invPos = -1;
                this.equiped = false;
                this.rare = 0;
                this.upgrade = 0;
                this.color = -1;
                this.damageMin = 0;
                this.damageMax = 0;
                this.corpDef = 0;
                this.distDef = 0;
                this.magicDef = 0;
                this.dodge = 0;
                this.rune = false;
                this.runeCharId = -1;
                this.nivFixed = false;
                this.itemBase = new ItemBase(-1);
            }
        }

        public Item(Item item, int invPos, bool equiped)
        {
            this.id = item.id;
            this.charId = item.charId;
            this.itemBase = item.itemBase;
            this.invPos = invPos;
            this.equiped = equiped;
            this.rare = item.rare;
            this.upgrade = item.upgrade;
            this.color = item.color;
            this.damageMin = item.damageMin;
            this.damageMax = item.damageMax;
            this.hitRate = item.hitRate;
            this.corpDef = item.corpDef;
            this.distDef = item.distDef;
            this.magicDef = item.magicDef;
            this.dodge = item.dodge;
            this.rune = item.rune;
            this.runeCharId = item.runeCharId;
            this.nivFixed = item.nivFixed;
            this.inEquip = item.inEquip;
            this.mustInsert = item.mustInsert;
            this.x = item.x;
            this.y = item.y;
            this.durabilites = item.durabilites;
        }

        public Item(int id, int charId, int idItemBase, int damageMin, int damageMax, int hitRate, int corpDef, int distDef, int magicDef, int dodge, int rare = 0, int upgrade = 0, int color = 0, bool mustInsert = false, int x = 0, int y = 0, int durabilites = 0)
        {
            this.id = id;
            this.charId = charId;
            this.itemBase = GameServer.GetItemsManager().itemList[idItemBase];
            this.damageMin = damageMin;
            this.damageMax = damageMax;
            this.hitRate = hitRate;
            this.corpDef = corpDef;
            this.distDef = distDef;
            this.magicDef = magicDef;
            this.dodge = dodge;
            this.rare = rare;
            this.upgrade = upgrade;
            this.color = color;
            this.mustInsert = mustInsert;
            this.x = x;
            this.y = y;
            this.durabilites = durabilites;
        }

        public bool UseItem(Player user, int inventory, int slot)
        {
            switch (this.itemBase.type)
            {
                case "effect":
                    if (this.itemBase.effect != -1)
                    {
                        user.map.SendEffect(1, user.id, this.itemBase.effect);
                        return true;
                    }
                    break;
                case "witem":
                    {
                        user.upItemTime = DateTime.Now;
                        ServerPacket packet = new ServerPacket(Outgoing.speWindows);
                        packet.AppendInt(this.itemBase.wId);
                        packet.AppendInt(0);
                        user.SendPacket(packet);
                        return false;
                    }
                case "u_pmessage":
                    user.SendGuri("10 2 1");
                    return false;
                case "speaker":
                    user.SendGuri("10 3 1");
                    return false;
                case "ml_bell":
                    user.SendPacket(GlobalMessage.MakeDelay(5000, 7, "#u_i^1^" + user.id + "^" + inventory + "^" + slot + "^1"));
                    user.map.SendGuri(2, 1, user.id, "");
                    return false;
                case "ml_panel":
                    if (GameServer.villageMapId.Contains(user.map.mapId))
                    {
                        if (user.mlPanel == null)
                        {
                            Npc mlPanel = user.map.AddNpc(user.GetSession().GetChannel(), 2, this.itemBase.displayNum, user.x, user.y, 2, 0, 0, "miniland_panel", user, 10000);
                            user.mlPanel = mlPanel;
                            return true;
                        }
                        else
                        {
                            user.SendPacket(GlobalMessage.MakeMessage(0, 0, 10, String.Format(GameServer.GetLanguage(user.GetSession().GetAccount().languagePack, "error.mlpanel.alreadyexist"), user.mlPanel.GetHp())));
                            return false;
                        }
                    }
                    user.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(user.GetSession().GetAccount().languagePack, "error.mlpanel.onlyvilage")));
                    return false;
                case "wingsupd":
                    if (user.spInUsing)
                    {
                        if (user.sp.wingsType != itemBase.wingsId)
                        {

                            user.sp.wingsType = itemBase.wingsId;
                            user.map.SendMap(user.GetMorphPacket());
                            return true;
                        }
                        else
                            user.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(user.languagePack, "error.specialist.havewings")));
                    }
                    else
                        user.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(user.languagePack, "error.specialist.notequiped")));
                    return false;
                case "hcolorupd":
                    if (user.chap != null && user.chap.itemBase.coloredItem)
                    {
                        int newColor = ItemsManager.random.Next(1, 16);
                        if (newColor == user.chap.color)
                        {
                            if (newColor == 15)
                            {
                                newColor--;
                            }
                            else
                                newColor++;
                        }
                        user.chap.color = newColor;
                        user.map.SendMap(user.GetEqPacket());
                        user.SendEquipment();
                        return true;
                    }
                    user.SendPacket(GlobalMessage.MakeAlert(0, GameServer.GetLanguage(user.languagePack, "error.headcolor.notequiped")));
                    return false;
                case "locomotion":
                    if (user.locomotion == null)
                    {
                        user.SendPacket(GlobalMessage.MakeDelay(3000, 3, "#u_i^1^" + user.id + "^" + inventory + "^" + slot + "^2"));
                        user.map.SendGuri(2, 1, user.id, "");
                    }
                    else
                    {
                        user.locomotion = null;
                        if (user.pet != null)
                        {
                            user.pet.x = user.x + 1;
                            user.pet.y = user.y - 1;
                            user.map.SendMap(user.pet.NpcPacket(user.languagePack));
                        }
                        user.map.SendMap(user.GetMorphPacket());
                        user.GetRealSpeed();
                        user.map.SendMap(user.GetSpeedPacket());
                    }
                    return true;
            }
            return false;
        }

        public void UseItem2(Player user, int inventory, int slot, int unknow)
        {
            user.inventory.isBlocked = true;
            try
            {
                switch (this.itemBase.type)
                {
                    case "ml_bell":
                        if (user.map.specialMap)
                            return;
                        ServerPacket result = user.inventory.DeleteOneItemByTypeAndSlot(user, inventory, slot, "ml_bell");
                        if (result != null)
                        {
                            user.map.ChangeMapRequest(user, user.miniLand, 5, 8);
                            user.SendPacket(result);
                        }
                        break;
                    case "locomotion":
                        if (user.inventory.itemsList[inventory][slot] != null && user.inventory.itemsList[inventory][slot].invItem.item == this && user.locomotion == null)
                        {
                            user.locomotion = user.inventory.itemsList[inventory][slot].invItem.itemBase.locomotion;
                            if (user.pet != null)
                                user.map.RemoveNpc(user.pet);
                            user.map.SendMap(user.GetMorphPacket());
                            user.map.SendEffect(1, user.id, 196);
                            user.GetRealSpeed();
                            user.map.SendMap(user.GetSpeedPacket());
                        }
                        break;
                }
            }
            catch { }
            user.inventory.isBlocked = false;
        }

        public void Save(int charId, int accountId, int inWareHouse, int equiped, int slot, int amount = 1)
        {
            using (DatabaseClient dbClient = GameServer.GetDatabaseManager().GetClient())
            {
                if (this.mustInsert)
                {
                    dbClient.ExecuteQuery("INSERT INTO `items_server" + GameServer.serverId + "`(`id`, `charId`, `accountId`, `itemId`, `inWareHouse`, `invPos`, `amount`, `equiped`, `rare`, `upgrade`, `color`, `damageMin`, `damageMax`, `hitRate`, `corpDef`, `distDef`, `magicDef`, `dodge`, `rune`, `runeCharId`, `fixed`, `x`, `y`, `durabilites`) VALUES ('" + this.id + "','" + charId + "','" + accountId + "','" + this.itemBase.id + "','" + inWareHouse + "','" + slot + "','" + amount + "','" + equiped + "','" + this.rare + "','" + this.upgrade + "','" + this.color + "','" + this.damageMin + "','" + this.damageMax + "','" + this.hitRate + "','" + this.corpDef + "','" + this.distDef + "','" + this.magicDef + "','" + this.dodge + "','" + this.rune + "','" + this.runeCharId + "','" + this.nivFixed + "','" + this.x + "','" + this.y + "','" + this.durabilites + "')");
                    this.mustInsert = false;
                }
                else
                {
                    dbClient.ExecuteQuery("UPDATE `items_server" + GameServer.serverId + "` SET " +
                        "`charId`='" + charId + "'," +
                        "`accountId`='" + accountId + "'," +
                        "`itemId`='" + this.itemBase.id + "'," +
                        "`inWareHouse`='" + inWareHouse + "'," +
                        "`invPos`='" + slot + "'," +
                        "`amount`='" + amount + "'," +
                        "`equiped`='" + equiped + "'," +
                        "`rare`='" + this.rare + "'," +
                        "`upgrade`='" + this.upgrade + "'," +
                        "`color`='" + this.color + "'," +
                        "`damageMin`='" + this.damageMin + "'," +
                        "`damageMax`='" + this.damageMax + "'," +
                        "`hitRate`='" + this.hitRate + "'," +
                        "`corpDef`='" + this.corpDef + "'," +
                        "`distDef`='" + this.distDef + "'," +
                        "`magicDef`='" + this.magicDef + "'," +
                        "`dodge`='" + this.dodge + "'," +
                        "`rune`='" + this.rune + "'," +
                        "`runeCharId`='" + this.runeCharId + "'," +
                        "`fixed`='" + this.nivFixed + "'," +
                        "`x`='" + this.x + "'," +
                        "`y`='" + this.y + "'," +
                        "`durabilites`='" + this.durabilites + "'" +
                        " WHERE id = '" + this.id + "';");
                }
            }
        }

        public void GetEquipInfo(Player user)
        {
            switch (this.itemBase.eqSlot)
            {
                case 0:
                    if (this.itemBase.sword || this.itemBase.adventer)
                    {
                        ServerPacket packet = new ServerPacket(Outgoing.equipInfo);
                        packet.AppendInt(0);
                        packet.AppendInt(this.itemBase.id);
                        packet.AppendInt(this.rare);
                        packet.AppendInt(this.upgrade);
                        packet.AppendInt(Convert.ToInt32(this.nivFixed));
                        packet.AppendInt(this.itemBase.levelReq);
                        packet.AppendInt(this.damageMin);
                        packet.AppendInt(this.damageMax);
                        packet.AppendInt(this.hitRate);
                        packet.AppendInt(this.itemBase.criticChance);
                        packet.AppendInt(this.itemBase.criticDamage);
                        packet.AppendInt(0);
                        packet.AppendInt(100);
                        packet.AppendInt(this.itemBase.price);
                        packet.AppendInt(-1);
                        user.SendPacket(packet);
                    }
                    else if (this.itemBase.archer)
                    {
                        ServerPacket packet = new ServerPacket(Outgoing.equipInfo);
                        packet.AppendInt(1);
                        packet.AppendInt(this.itemBase.id);
                        packet.AppendInt(this.rare);
                        packet.AppendInt(this.upgrade);
                        packet.AppendInt(Convert.ToInt32(this.nivFixed));
                        packet.AppendInt(this.itemBase.levelReq);
                        packet.AppendInt(this.damageMin);
                        packet.AppendInt(this.damageMax);
                        packet.AppendInt(this.hitRate);
                        packet.AppendInt(this.itemBase.criticChance);
                        packet.AppendInt(this.itemBase.criticDamage);
                        packet.AppendInt(0);
                        packet.AppendInt(100);
                        packet.AppendInt(this.itemBase.price);
                        packet.AppendInt(-1);
                        user.SendPacket(packet);
                    }
                    else if (this.itemBase.mage)
                    {
                        ServerPacket packet = new ServerPacket(Outgoing.equipInfo);
                        packet.AppendInt(5);
                        packet.AppendInt(this.itemBase.id);
                        packet.AppendInt(this.rare);
                        packet.AppendInt(this.upgrade);
                        packet.AppendInt(Convert.ToInt32(this.nivFixed));
                        packet.AppendInt(this.itemBase.levelReq);
                        packet.AppendInt(this.damageMin);
                        packet.AppendInt(this.damageMax);
                        packet.AppendInt(this.hitRate);
                        packet.AppendInt(this.itemBase.criticChance);
                        packet.AppendInt(this.itemBase.criticDamage);
                        packet.AppendInt(0);
                        packet.AppendInt(100);
                        packet.AppendInt(this.itemBase.price);
                        packet.AppendInt(-1);
                        user.SendPacket(packet);
                    }
                    break;
                case 1:
                    {
                        ServerPacket packet = new ServerPacket(Outgoing.equipInfo);
                        packet.AppendInt(2);
                        packet.AppendInt(this.itemBase.id);
                        packet.AppendInt(this.rare);
                        packet.AppendInt(this.upgrade);
                        packet.AppendInt(Convert.ToInt32(this.nivFixed));
                        packet.AppendInt(this.itemBase.levelReq);
                        packet.AppendInt(this.corpDef);
                        packet.AppendInt(this.distDef);
                        packet.AppendInt(this.magicDef);
                        packet.AppendInt(this.dodge);
                        packet.AppendInt(this.itemBase.price);
                        packet.AppendString("-1 0 0 0");
                        user.SendPacket(packet);
                    }
                    break;
                case 2:
                    {
                        ServerPacket packet = new ServerPacket(Outgoing.equipInfo);
                        packet.AppendInt(3);
                        packet.AppendInt(this.itemBase.id);
                        packet.AppendInt(this.itemBase.levelReq);
                        packet.AppendInt(this.corpDef);
                        packet.AppendInt(this.distDef);
                        packet.AppendInt(this.magicDef);
                        packet.AppendInt(this.dodge);
                        packet.AppendInt(0);
                        packet.AppendInt(0);
                        packet.AppendInt(0);
                        packet.AppendInt(0);
                        packet.AppendInt(this.itemBase.price);
                        packet.AppendInt(this.color);
                        packet.AppendInt(0);
                        packet.AppendInt(-1);
                        user.SendPacket(packet);
                    }
                    break;
                case 3:
                    {
                        ServerPacket packet = new ServerPacket(Outgoing.equipInfo);
                        packet.AppendInt(3);
                        packet.AppendInt(this.itemBase.id);
                        packet.AppendInt(this.itemBase.levelReq);
                        packet.AppendInt(this.itemBase.corpDef);
                        packet.AppendInt(this.itemBase.distDef);
                        packet.AppendInt(this.itemBase.magicDef);
                        packet.AppendInt(this.itemBase.dodge);
                        packet.AppendInt(this.itemBase.fireResist * (1 + this.upgrade));
                        packet.AppendInt(this.itemBase.waterResist * (1 + this.upgrade));
                        packet.AppendInt(this.itemBase.ligthResist * (1 + this.upgrade));
                        packet.AppendInt(this.itemBase.darknessResist * (1 + this.upgrade));
                        packet.AppendInt(this.itemBase.price);
                        packet.AppendInt(this.upgrade);
                        packet.AppendInt(0);
                        packet.AppendInt(-1);
                        user.SendPacket(packet);
                    }
                    break;
                case 4:
                    {
                        ServerPacket packet = new ServerPacket(Outgoing.equipInfo);
                        packet.AppendInt(3);
                        packet.AppendInt(this.itemBase.id);
                        packet.AppendInt(this.itemBase.levelReq);
                        packet.AppendInt(this.itemBase.corpDef);
                        packet.AppendInt(this.itemBase.distDef);
                        packet.AppendInt(this.itemBase.magicDef);
                        packet.AppendInt(this.itemBase.dodge);
                        packet.AppendInt(this.itemBase.fireResist * (1 + this.upgrade));
                        packet.AppendInt(this.itemBase.waterResist * (1 + this.upgrade));
                        packet.AppendInt(this.itemBase.ligthResist * (1 + this.upgrade));
                        packet.AppendInt(this.itemBase.darknessResist * (1 + this.upgrade));
                        packet.AppendInt(this.itemBase.price);
                        packet.AppendInt(this.upgrade);
                        packet.AppendInt(0);
                        packet.AppendInt(-1);
                        user.SendPacket(packet);
                    }
                    break;
                case 5:
                    if (this.itemBase.sword || this.itemBase.adventer)
                    {
                        ServerPacket packet = new ServerPacket(Outgoing.equipInfo);
                        packet.AppendInt(1);
                        packet.AppendInt(this.itemBase.id);
                        packet.AppendInt(this.rare);
                        packet.AppendInt(this.upgrade);
                        packet.AppendInt(Convert.ToInt32(this.nivFixed));
                        packet.AppendInt(this.itemBase.levelReq);
                        packet.AppendInt(this.damageMin);
                        packet.AppendInt(this.damageMax);
                        packet.AppendInt(this.hitRate);
                        packet.AppendInt(this.itemBase.criticChance);
                        packet.AppendInt(this.itemBase.criticDamage);
                        packet.AppendInt(0);
                        packet.AppendInt(100);
                        packet.AppendInt(this.itemBase.price);
                        packet.AppendInt(-1);
                        user.SendPacket(packet);
                    }
                    else if (this.itemBase.archer)
                    {
                        ServerPacket packet = new ServerPacket(Outgoing.equipInfo);
                        packet.AppendInt(0);
                        packet.AppendInt(this.itemBase.id);
                        packet.AppendInt(this.rare);
                        packet.AppendInt(this.upgrade);
                        packet.AppendInt(Convert.ToInt32(this.nivFixed));
                        packet.AppendInt(this.itemBase.levelReq);
                        packet.AppendInt(this.damageMin);
                        packet.AppendInt(this.damageMax);
                        packet.AppendInt(this.hitRate);
                        packet.AppendInt(this.itemBase.criticChance);
                        packet.AppendInt(this.itemBase.criticDamage);
                        packet.AppendInt(0);
                        packet.AppendInt(100);
                        packet.AppendInt(this.itemBase.price);
                        packet.AppendInt(-1);
                        user.SendPacket(packet);
                    }
                    break;
                case 9:
                    {
                        ServerPacket packet = new ServerPacket(Outgoing.equipInfo);
                        packet.AppendInt(3);
                        packet.AppendInt(this.itemBase.id);
                        packet.AppendInt(this.itemBase.levelReq);
                        packet.AppendInt(this.corpDef);
                        packet.AppendInt(this.distDef);
                        packet.AppendInt(this.magicDef);
                        packet.AppendInt(this.dodge);
                        packet.AppendInt(0);
                        packet.AppendInt(0);
                        packet.AppendInt(0);
                        packet.AppendInt(0);
                        packet.AppendInt(this.itemBase.price);
                        packet.AppendInt(0);
                        packet.AppendInt(0);
                        packet.AppendInt(-1);
                        user.SendPacket(packet);
                    }
                    break;
                case 13:
                    {
                        ServerPacket packet = new ServerPacket(Outgoing.equipInfo);
                        packet.AppendInt(2);
                        packet.AppendInt(this.itemBase.id);
                        packet.AppendInt(0);
                        packet.AppendInt(0);
                        packet.AppendInt(0);
                        packet.AppendInt(this.itemBase.levelReq);
                        packet.AppendInt(this.corpDef);
                        packet.AppendInt(this.distDef);
                        packet.AppendInt(this.magicDef);
                        packet.AppendInt(this.dodge);
                        packet.AppendInt(this.itemBase.price);
                        packet.AppendString("-1 2 -1 0");
                        user.SendPacket(packet);
                    }
                    break;
                case 14:
                    {
                        ServerPacket packet = new ServerPacket(Outgoing.equipInfo);
                        packet.AppendInt(3);
                        packet.AppendInt(this.itemBase.id);
                        packet.AppendInt(this.itemBase.levelReq);
                        packet.AppendInt(this.corpDef);
                        packet.AppendInt(this.distDef);
                        packet.AppendInt(this.magicDef);
                        packet.AppendInt(this.dodge);
                        packet.AppendInt(this.itemBase.fireResist);
                        packet.AppendInt(this.itemBase.waterResist);
                        packet.AppendInt(this.itemBase.ligthResist);
                        packet.AppendInt(this.itemBase.darknessResist);
                        packet.AppendInt(this.itemBase.price);
                        packet.AppendInt(0);
                        packet.AppendInt(2);
                        packet.AppendInt(-1);
                        user.SendPacket(packet);
                    }
                    break;
            }
        }
    }
}
