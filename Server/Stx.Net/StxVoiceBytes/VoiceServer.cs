using Stx.Logging;
using Stx.Utilities;
using Stx.Utilities.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Stx.Net.VoiceBytes
{
    public class VoiceServer : IDisposable
    {
        public List<VoiceRoom> Rooms { get; set; } = new List<VoiceRoom>();
        public List<IPEndPoint> ConnectedClients { get; private set; } = new List<IPEndPoint>();
        public Dictionary<IPEndPoint, byte> ClientOutIDs { get; } = new Dictionary<IPEndPoint, byte>();
        public bool EnableGlobalRoom { get; set; } = false;

        private UdpClient server;
        private ushort port;

        public ILogger Logger { get; set; } = new VoidLogger();

        public VoiceServer(ushort port)
        {
            this.port = port;

            IPAddress addrs = IPUtil.GetLocalIP(AddressFamily.InterNetwork);

            Logger.Log($"Starting voice server on { addrs.ToString() }:{ port } ...");

            server = new UdpClient(port);
            server.BeginReceive(new AsyncCallback(Receive), null);

            Logger.Log("Ready.");
        }

        ~VoiceServer()
        {
            server?.Close();
        }

        private void Receive(IAsyncResult e)
        {
            IPEndPoint from = new IPEndPoint(IPAddress.Any, 0);
            byte[] buffer = null;

            try
            {
                buffer = server.EndReceive(e, ref from);
            }
            catch
            {
                Logger.Log("A client has disconnected -> Server restart.");

                Restart();

                return;
            }

            if (!ConnectedClients.Contains(from))
                ConnectedClients.Add(from);

            if (!ClientOutIDs.ContainsKey(from))
                ClientOutIDs.Add(from, (byte)ClientOutIDs.Count);

            if (buffer?.Length > 0)
            {
                if (EnableGlobalRoom)
                {
                    SendRawAll(buffer, ConnectedClients.ToArray(), ClientOutIDs[from], from);
                }
                else
                {
                    VoiceRoom v = Rooms.FirstOrDefault((r) => r.AllowedClients.Contains(from));

                    if (v != null)
                        SendRawAll(buffer, v.AllowedClients.ToArray(), ClientOutIDs[from], from);
                }
            }

            try
            {
                server.BeginReceive(new AsyncCallback(Receive), null);
            }
            catch
            {
                Logger.Log("A client has disconnected -> Server restart.");

                Restart();
            }
        }

        private void Restart()
        {
            server.Close();
            server = null;

            ConnectedClients = new List<IPEndPoint>();

            server = new UdpClient(port);
            server.BeginReceive(new AsyncCallback(Receive), null);
        }

        private void SendRawAll(byte[] bytes, IPEndPoint[] to, byte id, IPEndPoint except = null)
        {
            foreach (IPEndPoint p in to)
                if (except == null || except.ToString() != p.ToString())
                    SendRaw(bytes, p, id);
        }

        private void SendRaw(byte[] bytes, IPEndPoint to, byte id)
        {
            byte[] b = new byte[bytes.Length + 1];
            bytes.CopyTo(b, 1);
            b[0] = id;
            server.Send(b, b.Length, to);
        }

        public void Dispose()
        {
            server.Close();
        }
    }

    public class VoiceRoom
    {
        public List<IPEndPoint> AllowedClients { get; set; } = new List<IPEndPoint>();
        public string RoomID { get; }

        public VoiceRoom()
        {
            RoomID = Guid.NewGuid().ToString();
        }
    }
}
