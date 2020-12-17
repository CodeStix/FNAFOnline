using LumiSoft.Media.Wave;
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
    public class VoiceClient : IDisposable
    {
        public VoiceIn VoiceIn { get; }
        public VoiceOut VoiceOut { get; }

        private Socket socket;
        private Thread receiveThread;

        public VoiceClient(WavInDevice audioInputDevice, WavOutDevice audioOutputDevice, IPAddress ipToConnect, ushort port, AudioSettings audioSettings)
        {
            Console.WriteLine($"Connecting to voice server on { ipToConnect.ToString() }...");

            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.ReceiveBufferSize = audioSettings.CalculateBufferSize();
                socket.Connect(ipToConnect, port);

                receiveThread = new Thread(new ThreadStart(ReceiveThread));
                receiveThread.Start();
            }
            catch(Exception e)
            {
                ErrorHandler.Instance.Error(e, 22);

                return;
            }
            
            Console.WriteLine("Enabling input/output audio devices...");

            try
            {
                VoiceOut = new VoiceOut(audioOutputDevice, audioSettings);
            }
            catch (Exception e)
            {
                ErrorHandler.Instance.Error(e, 23);
            }

            try
            {
                VoiceIn = new VoiceIn(audioInputDevice, AudioSettings.Default);
                VoiceIn.OnRecordAudio += VoiceIn_OnRecordAudio;
            }
            catch (Exception e)
            {
                ErrorHandler.Instance.Error(e, 24);
            }

            Console.WriteLine("Ready.");
        }

        ~VoiceClient()
        {
            Dispose();
        }

        private void VoiceIn_OnRecordAudio(byte[] uLawAudioBytes)
        {
            SendRaw(uLawAudioBytes, socket.RemoteEndPoint);
        }

        private void SendRaw(byte[] bytes, EndPoint to)
        {
            try
            {
                socket.SendTo(bytes, 0, bytes.Length, SocketFlags.None, to);
            }
            catch(Exception e)
            {
                ErrorHandler.Instance.Error(e, 25);
            }
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
                catch (Exception e)
                {
                    Console.WriteLine(from);

                    ErrorHandler.Instance.Error(e, 27);

                    continue;
                }

                if (size > 0)
                {
                    //Console.WriteLine($"Received { size } bytes from { from as IPEndPoint }");

                    VoiceOut.PlayAudio(buffer);
                }
            }
        }

        public void Dispose()
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            
            VoiceIn.Dispose();
            VoiceOut.Dispose();
        }
    }
}
