using NosTayleGameServer.NosTale.Channels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Net
{
    public class SocketListener
    {
        private Socket socket;
        private Channel channel;
        private bool listening;
        private AsyncCallback asyncCallback_0;
        private int processId = Process.GetCurrentProcess().Id;

        public SocketListener(string ip, string port, Channel channel)
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint localEP = new IPEndPoint(IPAddress.Parse(ip), Convert.ToInt32(port));
            this.socket.Bind(localEP);
            this.socket.Listen(1000);
            this.asyncCallback_0 = new AsyncCallback(this.ReceiveNewConnexion);
            this.channel = channel;
        }

        public void StartListening()
        {
            if (!this.listening)
            {
                this.listening = true;
                this.socket.BeginAccept(this.asyncCallback_0, this.socket);
            }
        }

        public void StopListening()
        {
            if (this.listening)
            {
                this.listening = false;
                this.socket.Close();
            }
        }

        private void RestartListening()
        {
            try
            {
                this.socket.BeginAccept(this.asyncCallback_0, this.socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void ReceiveNewConnexion(IAsyncResult iasyncResult_0)
        {
            if (this.listening)
            {
                try
                {
                    Socket socket = ((Socket)iasyncResult_0.AsyncState).EndAccept(iasyncResult_0);
                    this.channel.ReceiveNewConnection(socket.DuplicateAndClose(this.processId));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    this.RestartListening();
                }
            }
        }
    }
}
