using NosTayleGameServer.Communication.AdminTools;
using NosTayleGameServer.Core;
using NosTayleGameServer.NosTale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NosTayleGameServer.Net
{
    class GmServerListener
    {
        public Socket _socket;
        public string ip;
        public int port;
        public List<AdminSession> admSessions;
        public Task cycleTask;
        public bool cycled;
        public DateTime lastSendInfoServer;

        public GmServerListener(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            this.admSessions = new List<AdminSession>();
            try
            {
                this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this._socket.Bind(new IPEndPoint(IPAddress.Parse(this.ip), this.port));
                this._socket.Listen(100);
                this._socket.BeginAccept(new AsyncCallback(this.NewConnection), this._socket);
                WriteConsole.WriteStructure("ADMINTOOLS", String.Format("GM Server listening on {0}:{1}", ip, port), true);
                this.cycleTask = new Task(this.OnCycle);
                this.cycleTask.Start();
            }
            catch (Exception ex)
            {
                throw new Exception("Could not set up GmServer socket:\n" + ex.ToString());
            }
        }
        public void NewConnection(IAsyncResult iasyncResult_0)
        {
            try
            {
                Socket socket = ((Socket)iasyncResult_0.AsyncState).EndAccept(iasyncResult_0);
                this.admSessions.Add(new AdminSession(socket));
            }
            catch (Exception)
            {
            }
            this._socket.BeginAccept(new AsyncCallback(this.NewConnection), this._socket);
        }
        public void OnCycle()
        {
            while (true)
            {
                if (!this.cycled)
                {
                    this.cycled = true;
                    List<AdminSession> adms = new List<AdminSession>(this.admSessions);
                    if (DateTime.Now.Subtract(this.lastSendInfoServer).TotalSeconds > 5)
                    {
                        foreach (AdminSession adm in adms)
                        {
                            if (adm.connectedAtAdmin)
                                adm.SendMessage(String.Format("sinfo {0} {1}", GameServer.GetPlayersManager().playersCount, this.admSessions.FindAll(x => x.connectedAtAdmin == true).Count));
                        }
                        this.lastSendInfoServer = DateTime.Now;
                    }
                    this.cycled = false;
                }
                Thread.Sleep(100);
            }
        }
    }
}
