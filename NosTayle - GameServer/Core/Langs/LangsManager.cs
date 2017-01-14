using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Core.Langs
{
    public class LangsManager
    {
        internal Dictionary<string, Language> langsServer;

        public LangsManager()
        {
            WriteConsole.WriteStructure("GAME", "Load language packs...");
            this.langsServer = new Dictionary<string, Language>();
            var files = from file in Directory.EnumerateFiles(@".\\langs", "*", SearchOption.AllDirectories)
                        select new
                        {
                            File = file,
                        };

            foreach (var f in files)
                if (f.File.Split('.').Length == 2)
                    this.langsServer.Add(f.File.Substring(".\\langs\\".Length + 1), new Language(f.File));
            Console.WriteLine("{0} language packs loads !", this.langsServer.Count);
        }

        #region Recherches de texte
        public string GetText(string lang, string key)
        {
            if (this.langsServer.ContainsKey(lang))
                return this.langsServer[lang].GetServerTexts().GetText(key);
            else
                return "unknowLangagePack";
        }

        public string GetItemName(string lang, string item)
        {
            if (this.langsServer.ContainsKey(lang))
                return this.langsServer[lang].GetItemNames().GetText(item);
            else
                return "unknowLangagePack";
        }

        public string GetNpcName(string lang, string npc)
        {
            if (this.langsServer.ContainsKey(lang))
                return this.langsServer[lang].GetEntitieNames().GetText(npc);
            else
                return "unknowLangagePack";
        }

        public string GetSkillName(string lang, string skill)
        {
            if (this.langsServer.ContainsKey(lang))
                return this.langsServer[lang].GetSkillNames().GetText(skill);
            else
                return "unknowLangagePack";
        }
        #endregion
    }
}
