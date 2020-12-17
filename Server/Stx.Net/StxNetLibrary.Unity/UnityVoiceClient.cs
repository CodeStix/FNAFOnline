using Stx.Utilities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Stx.Net.VoiceBytes.Unity
{
    public class UnityVoiceClient : MonoBehaviour
    {
        public string host = "yourVoiceServer:port";
        public KeyCode pushToTalk = KeyCode.None;

        public Dictionary<int, AudioSource> VoiceOuts { get; private set; }
        public bool EnableIn { get; set; } = true;
        public bool EnableOut { get; set; } = true;

        private UdpClient client;
        private new AudioClip audio;
        private int lastSample;
        private volatile Queue<AudioClipInfo> queue = new Queue<AudioClipInfo>();
        private float sendTime = 0f;
        private bool enableRecord = false;

        public static Logging.ILogger Logger { get; set; } = StxNet.DefaultLogger;

        void Start()
        {
            Logger.Log("Connecting to voice server...");

            IPAddress ip;
            ushort port;

            IPUtil.ParseIPAndPort(host, out ip, out port, 11987);

            try
            {
                client = new UdpClient();
                client.Connect(ip, port);
                client.BeginReceive(new AsyncCallback(Receive), null);
            }
            catch (Exception e)
            {
                Logger.LogException(e, "Could not connect to voice server");

                return;
            }

            VoiceOuts = new Dictionary<int, AudioSource>();

            int minFreq, maxFreq;
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);

            if (minFreq < 8000)
                minFreq = 8000;

            audio = Microphone.Start(null, true, 100, minFreq);
            while (Microphone.GetPosition(null) < 0) ;

            enableRecord = true;

            Logger.Log("Connected to voice server!", Stx.Logging.LoggedImportance.Successful);
        }

        void FixedUpdate()
        {
            if (enableRecord)
            {
                sendTime += Time.fixedDeltaTime;

                if (sendTime >= 0.1f)
                {
                    int pos = Microphone.GetPosition(null);
                    int diff = pos - lastSample;

                    if (diff > 0)
                    {
                        float[] samples = new float[diff * audio.channels];
                        audio.GetData(samples, lastSample);
                        byte[] ba = ToByteArray(samples);

                        byte[] b = new byte[ba.Length + 2];
                        ba.CopyTo(b, 2);
                        b[0] = (byte)audio.channels;
                        b[1] = (byte)(audio.frequency / 1000);

                        if (EnableIn && (pushToTalk == KeyCode.None || Input.GetKey(pushToTalk)))
                            client.Send(b, b.Length);
                    }

                    lastSample = pos;

                    sendTime = 0f;
                }
            }

            while (queue.Count > 0)
            {
                AudioClipInfo i = queue.Dequeue();

                if (!VoiceOuts.ContainsKey(i.id))
                {
                    AudioSource a = gameObject.AddComponent<AudioSource>();
                    a.playOnAwake = false;
                    a.loop = false;

                    VoiceOuts.Add(i.id, a);
                }

                AudioSource source = VoiceOuts[i.id];

                source.clip = i.Create();

                if (!source.isPlaying)
                    source.Play();
            }
        }

        public byte[] ToByteArray(float[] floatArray)
        {
            const int Step = 4;
            int len = floatArray.Length * Step;
            byte[] byteArray = new byte[len];
            int pos = 0;
            foreach (float f in floatArray)
            {
                byte[] data = BitConverter.GetBytes(f);
                Array.Copy(data, 0, byteArray, pos, Step);
                pos += Step;
            }
            return byteArray;
        }

        public float[] ToFloatArray(byte[] byteArray)
        {
            const int Step = 4;
            int len = byteArray.Length / Step;
            float[] floatArray = new float[len];
            for (int i = 0; i < byteArray.Length; i += Step)
            {
                floatArray[i / Step] = BitConverter.ToSingle(byteArray, i);
            }
            return floatArray;
        }

        private void OnDisable()
        {
            client.Close();
        }

        private void Receive(IAsyncResult e)
        {
            IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
            byte[] buffer = null;

            try
            {
                buffer = client.EndReceive(e, ref remote);
            }
            catch
            {
                Logger.Log("VoiceServer has been shut down.", Stx.Logging.LoggedImportance.CriticalWarning);

                client.Close();

                return;
            }

            if (buffer?.Length > 0)
            {
                byte id = buffer[0];
                byte channels = buffer[1];
                int freq = buffer[2] * 1000;
                byte[] b = new byte[buffer.Length - 3];
                Array.ConstrainedCopy(buffer, 3, b, 0, buffer.Length - 3);
                float[] f = ToFloatArray(b);

                if (EnableOut)
                    queue.Enqueue(new AudioClipInfo(id, f, f.Length, channels, freq, false));
            }

            client.BeginReceive(new AsyncCallback(Receive), null);
        }

        class AudioClipInfo
        {
            public byte id;
            public float[] data;
            public int length;
            public int channels;
            public int freq;
            public bool stream;

            public const string Prefix = "audio";

            public AudioClipInfo(byte id, float[] data, int length, int channels, int freq, bool stream)
            {
                this.id = id;
                this.data = data;
                this.length = length;
                this.channels = channels;
                this.freq = freq;
                this.stream = stream;
            }

            public AudioClip Create()
            {
                AudioClip c = AudioClip.Create(Prefix + id, data.Length, channels, freq, stream);
                c.SetData(data, 0);
                return c;
            }
        }
    }
}
