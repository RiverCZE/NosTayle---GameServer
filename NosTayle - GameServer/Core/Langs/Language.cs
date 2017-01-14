using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Core.Langs
{
    class Language
    {
        private ConfigurationData serverTexts;
        private ConfigurationData entitieNames;
        private ConfigurationData skillNames;
        private ConfigurationData itemNames;

        public Language(string lang)
        {
            this.serverTexts = new ConfigurationData(lang);
            this.entitieNames = new ConfigurationData(String.Format("{0}.entitienames", lang));
            this.skillNames = new ConfigurationData(String.Format("{0}.skillnames", lang));
            this.itemNames = new ConfigurationData(String.Format("{0}.itemnames", lang));
        }

        public ConfigurationData GetServerTexts() { return this.serverTexts; }
        public ConfigurationData GetEntitieNames() { return this.entitieNames; }
        public ConfigurationData GetSkillNames() { return this.skillNames; }
        public ConfigurationData GetItemNames() { return this.itemNames; }
    }
}
