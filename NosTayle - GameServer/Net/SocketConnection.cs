using NosTayleGameServer.Communication;
using NosTayleGameServer.Communication.Messages;
using NosTayleGameServer.NosTale.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.Net
{
    public sealed class SocketConnection : Socket, IDisposable
    {
        public delegate void ReceiveDelegate(ref byte[] Data, int lenght);
        private Session session;
        private bool disconnected;

        private byte[] Buffer;

        private AsyncCallback asyncCallback_0;
        private AsyncCallback asyncCallback_1;
        private SocketConnection.ReceiveDelegate receiveDelegate;

        public string Address;

        public SocketConnection(SocketInformation socketInformation_0)
            : base(socketInformation_0)
        {
            this.disconnected = false;
            this.Address = base.RemoteEndPoint.ToString().Split(new char[] { ':' })[0];
        }

        internal void Initialise(SocketConnection.ReceiveDelegate receiveDelegate, Session session)
        {
            this.Buffer = new byte[4096];
            this.session = session;
            this.asyncCallback_0 = new AsyncCallback(this.OnReceive);
            this.asyncCallback_1 = new AsyncCallback(this.OnSend);

            this.receiveDelegate = receiveDelegate;

            this.BeginReceive();
        }

        internal void CloseConnection()
        {
            try
            {
                this.Close();
                this.Dispose();
            }
            catch
            {
            }
        }

        internal void SendData(byte[] bytes)
        {
            if (!this.disconnected)
            {
                try
                {
                    base.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, this.asyncCallback_1, this);
                }
                catch
                {
                    this.CloseConnection();
                }
            }
        }

        private void OnSend(IAsyncResult ar)
        {
            if (!this.disconnected)
            {
                try
                {
                    base.EndSend(ar);
                }
                catch
                {
                    this.CloseConnection();
                }
            }
        }

        public void SendMessage(string str)
        {
            this.SendData(GameCrypto.Encrypt(str));
        }

        public void SendMessage(ServerPacket message)
        {
            if (message != null)
                this.SendData(message.GetBytes());
        }

        private void BeginReceive()
        {
            if (!this.disconnected)
            {
                try
                {
                    base.BeginReceive(this.Buffer, 0, 0x400, SocketFlags.None, this.asyncCallback_0, this);
                }
                catch (Exception)
                {
                    this.CloseConnection();
                }
            }
        }

        private void OnReceive(IAsyncResult ar)
        {
            if (!this.disconnected)
            {
                try
                {
                    int num = 0;

                    try
                    {
                        num = base.EndReceive(ar);
                    }
                    catch
                    {
                        this.CloseConnection();
                        return;
                    }

                    if (num > 0)
                    {
                        Array.Resize(ref this.Buffer, num);
                        byte[] array = (byte[])this.Buffer.Clone();
                        this.Buffer = new byte[4096];
                        this.BeginReceive();
                        this.StartReceiveMessage(ref array, num);
                    }
                    else
                        this.CloseConnection();
                }
                catch
                {
                    this.CloseConnection();
                }
            }
        }

        private void StartReceiveMessage(ref byte[] bytes, int lenght)
        {
            if (this.receiveDelegate != null)
            {
                this.receiveDelegate(ref bytes, lenght);
            }
        }

        public new void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private new void Dispose(bool bool_1)
        {
            if (!this.disconnected && bool_1)
            {
                this.disconnected = true;
                if (this.session != null)
                    this.session.GetChannel().DisconnectUser(this.session);
                try
                {
                    base.Shutdown(SocketShutdown.Both);
                    base.Close();
                    base.Dispose();
                }
                catch
                {
                }
                Array.Clear(this.Buffer, 0, this.Buffer.Length);
                this.Buffer = null;
                this.asyncCallback_0 = null;
                this.receiveDelegate = null;
            }
        }

        public bool isDisconnected()
        {
            return this.disconnected;
        }
    }
}
