using NosTayleGameServer.Communication.Headers;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.NosTale;
using NosTayleGameServer.NosTale.Entities.Players;
using NosTayleGameServer.NosTale.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication.ReceivePackets.SpecialPackets
{
    internal sealed class CommandEvent : Interface
    {
        private Dictionary<string, int> commands;
        public void Handle(Session Session, SessionMessage Event)
        {
            Player sender = Session.GetPlayer();
            string command = Event.dataBrute.Substring(1).Split(' ')[0];
            string[] values = new string[0];
            if (Event.dataBrute.Split(' ').Length > 1)
                values = Event.dataBrute.Substring(command.Length + 2).Split(' ');
            this.GetCommands(sender.languagePack);
            if (!this.commands.ContainsKey(command))
                return;
            switch (this.commands[command])
            {
                case 1: //Changement de langue
                    if (values.Length == 1)
                    {
                        if (GameServer.LanguageExist(values[0]))
                        {
                            sender.GetSession().GetAccount().languagePack = values[0];
                            sender.SendPacket(GlobalMessage.MakeMessage(0, 0, 12, GameServer.GetLanguage(sender.languagePack, "changelanguage")));
                        }
                        else
                            sender.SendPacket(GlobalMessage.MakeMessage(0, 0, 11, String.Format(GameServer.GetLanguage(sender.languagePack, "error.cantfindlang"), values[0])));
                    }
                    break;
                case 2: //Activation du mode GM
                    if (values.Length == 0 && sender.isGm)
                    {
                        if (!sender.gmActivate)
                        {
                            sender.gmActivate = true;
                            sender.SendPacket(GlobalMessage.MakeMessage(0, 0, 12, GameServer.GetLanguage(sender.languagePack, "gmmode.activate")));
                        }
                        else
                        {
                            sender.gmActivate = false;
                            sender.SendPacket(GlobalMessage.MakeMessage(0, 0, 11, GameServer.GetLanguage(sender.languagePack, "gmmode.disabled")));
                        }
                        sender.map.SendMap(sender.GmPacket());
                    }
                    break;
                case 3: //Modification de la vitesse
                    if (values.Length == 1 && sender.isGm)
                    {
                        if (Convert.ToInt32(values[0]) > 0 && Convert.ToInt32(values[0]) <= 50)
                        {
                            sender.speed = Convert.ToInt32(values[0]);
                            sender.speedUpdated = true;
                        }
                        else
                        {
                            sender.speedUpdated = false;
                            sender.ReCalculStatPoint();
                        }
                        sender.SendSpeed();
                    }
                    break;
                case 4: //Ajout d'or
                    if (values.Length == 2 && sender.isGm)
                    {
                        try
                        {
                            Player player = GameServer.GetPlayersManager().GetPlayerByName(values[0]);
                            if (player != null)
                            {
                                player.gold += Convert.ToInt32(values[1]);
                                player.SendGold();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    break;
                case 5: //Informations
                    {
                        sender.SendPacket(GlobalMessage.MakeMessage(0, 0, 10, "============================="));
                        sender.SendPacket(GlobalMessage.MakeMessage(0, 0, 6, "# NosFenix Server Info"));
                        sender.SendPacket(GlobalMessage.MakeMessage(0, 0, 6, "# Server DateTime: " + DateTime.Now.ToString("dd/MM/yyyy H:mm:ss")));
                        TimeSpan span = (TimeSpan)(DateTime.Now - GameServer.startedTime);
                        sender.SendPacket(GlobalMessage.MakeMessage(0, 0, 6, "# Server Online Time: " + ((span.Days == 0) ? "" : span.Days + ((span.Days == 1) ? " day, " : " days, ")) + ((span.Hours == 0) ? "" : span.Hours + ((span.Hours == 1) ? " hour, " : " hours, ")) + ((span.Minutes == 0) ? "" : span.Minutes + ((span.Minutes == 1) ? " minute and " : " minutes and ")) + span.Seconds + ((span.Seconds == 1) ? " second " : " seconds")));
                        sender.SendPacket(GlobalMessage.MakeMessage(0, 0, 6, "# Users online: " + GameServer.GetPlayersManager().playersCount));
                        sender.SendPacket(GlobalMessage.MakeMessage(0, 0, 6, "# Users on this channel: " + sender.GetSession().GetChannel().GetPlayersManager().playersCount));
                        sender.SendPacket(GlobalMessage.MakeMessage(0, 0, 6, "# Users on this map: " + sender.map.playersManagers.playersCount));
                        sender.SendPacket(GlobalMessage.MakeMessage(0, 0, 10, "============================="));
                    }
                    break;
                case 6: //Kick l'utilisateur
                    if (values.Length == 1 && sender.isGm)
                    {
                        try
                        {
                            Player player = GameServer.GetPlayersManager().GetPlayerByName(values[0]);
                            if (player != null && !player.isGm)
                                player.GetSession().GetSock().CloseConnection();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    break;
                case 7: //upSp
                    if (values.Length == 1)
                    {
                        try
                        {
                            if (sender.sp.spId != -1 && sender.spInUsing && Convert.ToInt32(values[0]) > 0 && Convert.ToInt32(values[0]) <= 99)
                                sender.sp.level = Convert.ToInt32(values[0]);
                            sender.SendLevel();
                        }
                        catch
                        {
                        }
                    }
                    break;
                case 8: //Gold
                    if (values.Length == 1 && sender.isGm)
                    {
                        try
                        {
                            if (sender.gold + Convert.ToInt32(values[0]) <= Int32.MaxValue)
                                sender.gold += Convert.ToInt32(values[0]);
                            else
                                sender.gold = Int32.MaxValue;
                            if (sender.gold < 0)
                                sender.gold = 0;
                            sender.SendGold();
                        }
                        catch
                        {
                        }
                    }
                    break;
                case 9:
                    if (values.Length == 3 && sender.isGm)
                    {
                        if (Convert.ToInt32(values[1]) < -2 || Convert.ToInt32(values[1]) > 8 || Convert.ToInt32(values[2]) < 0 || Convert.ToInt32(values[2]) > 10)
                            return;
                        switch (values[0])
                        {
                            case "1":
                                if (sender.weapon.id != -1)
                                {
                                    sender.weapon.rare = Convert.ToInt32(values[1]);
                                    sender.weapon.upgrade = Convert.ToInt32(values[2]);
                                }
                                break;
                            case "2":
                                if (sender.weapon2.id != -1)
                                {
                                    sender.weapon2.rare = Convert.ToInt32(values[1]);
                                    sender.weapon2.upgrade = Convert.ToInt32(values[2]);
                                }
                                break;
                            case "3":
                                if (sender.armor.id != -1)
                                {
                                    sender.armor.rare = Convert.ToInt32(values[1]);
                                    sender.armor.upgrade = Convert.ToInt32(values[2]);
                                }
                                break;
                        }
                        sender.SendStatPoints();
                        sender.SendEquipment();
                        sender.map.SendMap(sender.GetEqPacket());
                    }
                    break;
                case 10:
                    if (values.Length == 2 && sender.isGm)
                    {
                        if (sender.sp.spId != -1 && sender.spInUsing && Convert.ToInt32(values[0]) >= 0 && Convert.ToInt32(values[0]) <= 15)
                        {
                            sender.sp.upgrade = Convert.ToInt32(values[0]);
                            sender.sp.wingsType = Convert.ToInt32(values[1]);
                            sender.SendStatPoints();
                            sender.SendEquipment();
                            sender.map.SendMap(sender.GetEqPacket());
                            sender.map.SendMap(sender.GetMorphPacket());
                        }
                    }
                    break;
                case 11: //MUTE
                    if (values.Length == 1 && sender.isGm)
                    {
                        try
                        {
                            Player player = GameServer.GetPlayersManager().GetPlayerByName(values[0]);
                            if (player != null && !player.isGm)
                                if (player.isMuted)
                                    player.isMuted = false;
                                else player.isMuted = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    break;
                case 12: //Ban l'utilisateur
                    if (values.Length == 1 && sender.isGm)
                    {
                        try
                        {
                            Player player = GameServer.GetPlayersManager().GetPlayerByName(values[0]);
                            if (player != null && !player.isGm)
                            {
                                player.GetSession().GetAccount().banned = true;
                                player.GetSession().GetSock().CloseConnection();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    break;
                case 13: //Se téléporter
                    if (values.Length == 3 && sender.isGm)
                    {
                        try
                        {
                            if (GameServer.GetMapData(Convert.ToInt32(values[0])) != null)
                            {
                                Map map = null;
                                if (sender.GetSession().GetChannel().generalMaps.ContainsKey(Convert.ToInt32(values[0])))
                                {
                                    map = sender.GetSession().GetChannel().generalMaps[Convert.ToInt32(values[0])];
                                }
                                else
                                {
                                    sender.GetSession().GetChannel().generalMaps.Add(Convert.ToInt32(values[0]), new Map(Convert.ToInt32(values[0]), sender.GetSession().GetChannel()));
                                    map = sender.GetSession().GetChannel().generalMaps[Convert.ToInt32(values[0])];
                                }
                                sender.map.ChangeMapRequest(sender, map, Convert.ToInt32(values[1]), Convert.ToInt32(values[2]));
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    break;
                case 14: //Téléporter un joueur à sois
                    if (values.Length == 1 && sender.isGm)
                    {
                        try
                        {
                            Player player = GameServer.GetPlayersManager().GetPlayerByName(values[0]);
                            if (player != null && sender.map != null)
                                player.map.ChangeMapRequest(player, sender.map, sender.x, sender.y);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    break;
                case 15: //Se téléporter à un joueur
                    if (values.Length == 1 && sender.isGm)
                    {
                        try
                        {
                            Player player = GameServer.GetPlayersManager().GetPlayerByName(values[0]);
                            if (player != null && player.map != null)
                                sender.map.ChangeMapRequest(sender, player.map, player.x, player.y);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    break;
                case 16:
                    if (values.Length == 1 && sender.isGm)
                    {
                        try
                        {
                            sender.ModScale(Convert.ToInt32(values[0]));
                            ServerPacket packet = new ServerPacket(Outgoing.changeCharScale);
                            packet.AppendInt(1);
                            packet.AppendInt(sender.id);
                            packet.AppendInt(Convert.ToInt32(values[0]));
                            sender.map.SendMap(packet);
                        }
                        catch
                        {
                        }
                    }
                    break;
                case 17: //Annonce
                    if (values.Length >= 1 && sender.isGm)
                    {
                        GameServer.GetPlayersManager().SendToAll(GlobalMessage.MakeAlert(2, Event.dataBrute.Substring(command.Length + 2)));
                        GameServer.GetPlayersManager().SendToAll(GlobalMessage.MakeMessage(0, 0, 10, "{0}").ToString(), "message.announce", Event.dataBrute.Substring(command.Length + 2));
                    }
                    break;
            }
        }

        private void GetCommands(string languagePack)
        {
            this.commands = new Dictionary<string, int>();
            this.commands.Add(GameServer.GetLanguage(languagePack, "change.language.command"), 1);
            this.commands.Add(GameServer.GetLanguage(languagePack, "activate.gm"), 2);
            this.commands.Add(GameServer.GetLanguage(languagePack, "change.speed"), 3);
            this.commands.Add(GameServer.GetLanguage(languagePack, "add.gold"), 4);
            this.commands.Add("infos", 5);
            this.commands.Add(GameServer.GetLanguage(languagePack, "kick.player"), 6);
            this.commands.Add("upSp", 7);
            this.commands.Add("gold", 8);
            this.commands.Add("eq", 9);
            this.commands.Add("sUp", 10);
            this.commands.Add("mute", 11);
            this.commands.Add("ban", 12);
            this.commands.Add("port", 13);
            this.commands.Add("summon", 14);
            this.commands.Add("follow", 15);
            this.commands.Add("scale", 16);
            this.commands.Add("announce", 17);
            this.commands.Add("invisible", 18);
            this.commands.Add("visible", 19);
        }
    }
}
