using NosTayleGameServer.NosTale.Entities.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Communication
{
    public class SessionMessage
    {
        private string header;
        private string[] values;
        internal string dataBrute;
        public int valuesCount
        {
            get
            {
                return this.values.Length;
            }
        }

        public SessionMessage(string data)
        {
            this.dataBrute = data;
            this.header = data.Split(' ')[0];
            if (data.Split(' ').Length > 1)
                this.values = data.Substring(this.header.Length + 1).Split(' ');
        }

        public string GetHeader() { return this.header; }
        public string GetValue(int v)
        {
            if (this.values[v] != null)
                return this.values[v];
            else
                return "";
        }

        public string[] GetValues()
        {
            return this.values;
        }
    }
}
