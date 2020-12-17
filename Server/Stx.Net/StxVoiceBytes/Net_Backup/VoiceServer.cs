using Stx.Utilities;
using Stx.Utilities.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Stx.VoiceBytes.Net
{
    public class VoiceServer
    {
        private Socket socket;
        private Thread receiveThread;

        public List<VoiceRoom> Rooms { get; set; } = new List<VoiceRoom>();
        public List<EndPoint> ConnectedClients { get; } = new List<EndPoint>();
        public bool EnableGlobalRoom { get; set; } = false;

        public VoiceServer(ushort port, AudioSettings audioSettings)
        {
            IPAddress addrs = IPUtil.GetLocalIP(AddressFamily.InterNetwork);

            Console.WriteLine($"Starting voice server on { addrs.ToString() }...");

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            socket.ReceiveBufferSize = audioSettings.CalculateBufferSize();

            IPEndPoint local = new IPEndPoint(addrs, port);

            socket.Bind(local);

            receiveThread = new Thread(new ThreadStart(ReceiveThread));
            receiveThread.Start();

            Console.WriteLine("Ready.");
        }

        private void ReceiveThread()
        {
            while (true)
            {
                byte[] buffer = new byte[socket.ReceiveBufferSize];
                EndPoint from = new IPEndPoint(IPAddress.Any, 0);
                int size = 0;
                try
                {
                    size = socket.ReceiveFrom(buffer, 0, socket.ReceiveBufferSize, SocketFlags.None, ref from);
                }
                catch(Exception e)
                {
                    ErrorHandler.Instance.Error(e, 26);

                    

                    continue;
                }

                if (!ConnectedClients.Contains(from))
                    ConnectedClients.Add(from);

                if (size > 0 && buffer != null)
                {
                    //Console.WriteLine($"Received { size } bytes from { from as IPEndPoint }");

                    if (!EnableGlobalRoom)
                    {
                        foreach (VoiceRoom r in Rooms)
                        {
                            if (r.AllowedClients.Contains(from))
                            {
                                SendRawAll(buffer, r.AllowedClients.ToArray(), from);
                            }
                        }
                    }
                    else
                    {
                        //SendRaw(buffer, from);
                        SendRawAll(buffer, ConnectedClients.ToArray(), from);
                    }
                }
            }
        }

        private void SendRaw(byte[] bytes, EndPoint to)
        {
            try
            {
                socket.SendTo(bytes, 0, bytes.Length, SocketFlags.None, to);
            }
            catch
            {
                Console.WriteLine($"Client disconnected: " + to);

                // They diconnected.
                ConnectedClients.Remove(to);

                foreach (VoiceRoom r in Rooms)
                    r.AllowedClients.Remove(to);
            }
        }

        private void SendRawAll(byte[] bytes, EndPoint[] to, EndPoint except = null)
        {
            foreach (EndPoint p in to)
                if (except == null || except != p)
                    SendRaw(bytes, p);
        }
    }

    public class VoiceRoom
    {
        public List<EndPoint> AllowedClients { get; set; } = new List<EndPoint>();
    }
}
