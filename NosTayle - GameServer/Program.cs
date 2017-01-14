using NosTayleGameServer.NosTale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NosTayleGameServer
{
    class Program
    {
        static public bool serverStart = false;
        private delegate bool EventHandler(CtrlType sig);
        private enum CtrlType
        {
            CTRL_BREAK_EVENT = 1,
            CTRL_C_EVENT = 0,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }
        private static EventHandler closeDelegate;
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(Program.EventHandler handler, bool add);
        static void Main(string[] args)
        {
            GameServer.Initialize();
            Program.serverStart = true;
            Program.closeDelegate = (Program.EventHandler)Delegate.Combine(Program.closeDelegate, new Program.EventHandler(Program.closeMethod));
            Program.SetConsoleCtrlHandler(Program.closeDelegate, true);
            Console.Beep();
            while (true)
            {
                string command = Console.ReadLine();
                switch (command)
                {
                    case "reloadlang":
                        GameServer.UpdateLanguage();
                        break;
                    case "reloadmaps":
                        GameServer.LoadMapsData();
                        break;
                    case "clear":
                        Console.Clear();
                        break;
                }
            }
        }

        private static bool closeMethod(CtrlType enum0_0)
        {
            if (Program.serverStart)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("The server saving and disconnect all users, don't exit !");
                GameServer.Destroy();
            }
            return true;
        }
    }
}
