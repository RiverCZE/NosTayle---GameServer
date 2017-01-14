using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Core
{
    class WriteConsole
    {
        public static void WriteStructure(string entete, string corp, bool line = false)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[{0}] ", entete);
            Console.ForegroundColor = ConsoleColor.White;
            if (!line)
                Console.Write(corp);
            else
                Console.WriteLine(corp);
        }
    }
}
