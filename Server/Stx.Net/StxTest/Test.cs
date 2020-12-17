//#define STX_CLIENT_CONSOLE_TEST
//#define STX_VOICEBYTES_TEST
//#define STX_VOICEBYTES_NET_TEST
//#define STX_UDPCLIENT_TEST

using Imgur.API;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models;
using Stx.Collections.Concurrent;
using Stx.Logging;
using Stx.Net;
using Stx.Net.Achievements;
using Stx.Net.RoomBased;
using Stx.Serialization;
using Stx.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public static class TestProgram
{
    public static void Main(string[] args)
    {
        Dictionary<int, int> counter = new Dictionary<int, int>();

        for(int i = 0; i < 1000000000; i+=1)
        {
            int j = GenerateChecksum(i);

            if (counter.ContainsKey(j))
                counter[j]++;
            else
                counter.Add(j, 1);

            if (counter[j] > 1)
                Console.WriteLine($"{ i }: { j } ({ counter[j] } occurrences)");

            //if (i % 1000 == 0)
            //    Console.ReadKey();
        }

        Console.ResetColor();
        Console.WriteLine("----End----");
        Console.ReadKey();
    }

    private static int GenerateChecksum(int f)
    {
        const int key = 0x0f3782ea;

        f = (int)(f * 349.69721f);
        f -= key * (f % 109);
        f += key * (f % 42);

        f = ~f;

        byte[] b = BitConverter.GetBytes(f);
        byte[] bk = BitConverter.GetBytes(key);

        int l = 9;

        for (int i = 0; i < b.Length; i++)
        {
            b[b.Length - 1 - i] ^= bk[i];
            l -= i * (bk[i] + b[i] - 1);
        }

        f += (int)Math.Round(Math.Sin(l * 0.123f) * 2390.3f);

        return f;
    }

    public static void Maadsdfsdfsdfin(string[] args)
    {
        Stopwatch sw = new Stopwatch();

        const int Compares = 20000;

        for(int i = 0; i < Compares; i++)
        {
            string str1 = Guid.NewGuid().ToString();
            string str2 = Guid.NewGuid().ToString();

            sw.Start();

            int j = string.Compare(str1, str2, StringComparison.OrdinalIgnoreCase);

            sw.Stop();

            Console.WriteLine($"[{ i + 1 }/{ Compares }] { str1 } == { str2 } ? { j.ToString().PadLeft(4, ' ') } ({ sw.ElapsedMilliseconds } ms, { sw.ElapsedTicks } ticks)");
        }

        Console.WriteLine($"Total time taken for { Compares } compares: { sw.ElapsedTicks / 10000.0f } ms");
        Console.ReadKey();
    }

    public static void Madfsdfsdfsdfsdfin(string[] args)
    {
        foreach(string f in Directory.GetFiles(@".\Upload\","*.png"))
        {
            Console.WriteLine($"Uploading { f } to imgur...");

            UploadImage(f);

            break;
        }

        while(true)
        {
            Console.Write("Enter id of image to grasp: ");

            string str = Console.ReadLine();

            GetImage(str);
        }
    }

    public static ImgurClient client = new ImgurClient("50dac57b4589f6d", "523c33170f952d5af8464c4f00e51c44743efe46");

    public static void GetImage(string imageId)
    {
        try
        {
            var endpoint = new ImageEndpoint(client);
            var image = endpoint.GetImageAsync(imageId).GetAwaiter().GetResult();
            Console.WriteLine("Image retrieved. Image Url: " + image.Link);
        }
        catch (ImgurException imgurEx)
        {
            Console.WriteLine("An error occurred getting an image from Imgur.");
            Console.WriteLine(imgurEx.Message);
        }
    }

    public static void UploadImage(string fileName)
    {
        try
        {
            var endpoint = new ImageEndpoint(client);
            IImage image;
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                image = endpoint.UploadImageStreamAsync(fs).GetAwaiter().GetResult();
            }
            Console.WriteLine($"Image uploaded. Image Url: { image.Link }, Image Id: { image.Id }");
        }
        catch (ImgurException imgurEx)
        {
            Console.WriteLine("An error occurred uploading an image to Imgur.");
            Console.WriteLine(imgurEx.Message);
        }
    }

    public static void dfdfsfsMain(string[] args)
    {
        UniqueCodeGenerator ucg = new UniqueCodeGenerator(30);

        while (true)
        {
            Console.WriteLine(ucg.GetNextCode());
            
            //Thread.Sleep(20);
        }
    }

    public static void dMaisdfan(string[] args)
    {
        int[] index = new int[4];
        string chars = "abcdefghijklmnopqrstuvwxyz";

        int pow = 1;
        for(int i = 0; i < index.Length; i++)
            pow = pow * chars.Length;

        for(int i = 0; i < pow; i++)
        {
            for (int k = 0; k < index.Length; k++)
            {
                int x = i;

                for (int j = 0; j < k; j++)
                    x /= 26;

                index[k] = x % 26;
            }

            string code = string.Join("", index.Select((t) => chars[t]));

            Console.WriteLine(code);
        }

        Console.ReadKey();
    }

    public static void Masdfsdfin(string[] args)
    {
        Stx.Net.TaskScheduler nts = new Stx.Net.TaskScheduler();

        ThreadSafeData.MultiThreadOverride = false;

        nts.Repeat(() =>
        {
            Console.WriteLine("looping with 750 ms interval");
        }, 750);

        nts.RepeatLater(() =>
        {
            Console.WriteLine("6 seconds have passed, looping");
        }, 6000, 1000);

        nts.RunLater(() =>
        {
            Console.WriteLine("2 seconds have passed");
        }, 2000);

        nts.RunLater(() =>
        {
            Console.WriteLine("4 seconds have passed");
        }, 4000);

        Console.ReadKey();
    }

    public static void Masdasddfgfgain(string[] args)
    {
        Console.Write("Enter a name to feminify: ");
        string name = Console.ReadLine();

        string suffix = "a";
        int cutoff = 0;

        if (name.EndsWith("e"))
        {
            cutoff = 1;
            suffix = "ie";
        }
        else if (name.EndsWith("k"))
        {
            suffix = "kie";
        }

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine(name.Substring(0, name.Length - cutoff) + suffix);
        Console.ReadKey();
    }


    public static void Maisdfsdfn(string[] args)
    {
        ConsoleLogger logger = new ConsoleLogger();

        logger.Log("dit is een CriticalError", LoggedImportance.CriticalError);
        logger.Log("dit is een CriticalWarning", LoggedImportance.CriticalWarning);
        logger.Log("dit is een Debug", LoggedImportance.Debug);
        logger.Log("dit is een Information", LoggedImportance.Information);
        logger.Log("dit is een Successful", LoggedImportance.Successful);
        logger.Log("dit is een Warning", LoggedImportance.Warning);
        logger.LogException(new Exception("dikke error :O"), "ziektes");

        Console.ReadKey();
    }

    public static void Maidfsghn(string[] args)
    {
        ThreadSafeData.MultiThreadOverride = false;

        Console.ReadKey();
        Console.WriteLine("Creating first connection...");

        ConnectionStartInfo csi1 = new ConnectionStartInfo("localhost:1987", "", false);
        csi1.ClientName = "Client1";
        csi1.ClientID = "8bae63f3-2834-4e09-bc6f-36bc128706be";
        csi1.ApplicationName = "FNAFOnline";
        csi1.ApplicationVersion = "1.0";

        Client client1 = new Client(csi1);
        client1.OnConnected += (ft) =>
        {
            Console.WriteLine("Connected! First time? " + ft);
        };

        Console.ReadKey();
        Console.WriteLine("Creating second connection...");

        ConnectionStartInfo csi2 = new ConnectionStartInfo("localhost:1987", "", false);
        csi2.ClientName = "Client2";
        csi2.ClientID = "abf34297-2791-498d-83d9-0f164ea31bb0";
        csi2.ApplicationName = "FNAFOnline";
        csi2.ApplicationVersion = "1.0";

        Client client2 = new Client(csi2);
        client2.OnConnected += (ft) =>
        {
            Console.WriteLine("Connected! First time? " + ft);
        };

        Console.ReadKey();
        Console.WriteLine("Sending packet as other");
        RequestPacket p = new RequestPacket("abf34297-2791-498d-83d9-0f164ea31bb0", "Matchmaking", (e) => 
        {
            Console.WriteLine("Packet was answered: " + e);
        });
        p.Data.Add("Header", 0x9);
        client2.SendToServer(p.ToBytes(), BytesContentType.Packet);

        Console.ReadKey();
    }

    public static void Msdasdasdain(string[] args)
    {
        string[] names = File.ReadAllLines("names.txt");

        ThreadSafeData.MultiThreadOverride = false;

        Console.WriteLine("Press a key to create a connection...");

        for (int i = 0; i < names.Length; i++)
        {
            //Console.ReadKey();
            Thread.Sleep(2000);

            string name = names[i];
            string clientId = Guid.NewGuid().ToString();

            Console.WriteLine($"[{ i }] Connecting as { name }, id is { clientId }");

            new Connection(name, clientId);
        }
    }

    public class Connection
    {
        private Client client;
        private string name;

        private string[] requests = new string[] 
        {
            Requests.RequestPing,
            Requests.RequestMatchmaking,
            Requests.RequestClientIdentity,
            Requests.RequestClientInfo,
            Requests.RequestClientLevel,
            Requests.RequestCurrentRoom,
            Requests.RequestMatchmaking
        };

        public Connection(string name, string clientId)
        {
            this.name = name;

            ConnectionStartInfo csi = new ConnectionStartInfo("192.168.0.116:1987", "", false);
            csi.ClientName = name;
            csi.ClientID = clientId;
            csi.AuthorizationToken = "2656949b-e345-4796-b1ff-d4b666b5a682";
            csi.ConnectOnConstruct = false;
            csi.ApplicationName = "FNAFOnline";
            csi.ApplicationVersion = "1.0";

            client = new Client(csi);
            client.OnConnected += Client_OnConnected;
            client.Connect();
        }

        private void Client_OnConnected(bool firstTime)
        {
            DoStuff();
        }

        public async void DoStuff()
        {
            Random r = new Random();

            for(int i = 0; i < requests.Length; i++)
            {
                await Task.Delay(r.Next(0, 5000));

                client.SendToServer(new RequestPacket(client.NetworkID, requests[i], (e) => 
                {
                    Console.WriteLine($"Answer for { name }: { e }");
                }));
            }

            await Task.Delay(r.Next(0, 10000));

            client.Disconnect();
        }

    }

    public static void Maidfgdfgn(string[] args)
    {
        Console.WriteLine("Testing ConcurrentList...");
        ConcurrentList<string> list = new ConcurrentList<string>();

        Console.WriteLine("Creating test data...");

        Random r = new Random();

        for (int i = 0; i < 5; i++)
            list.Add(Guid.NewGuid().ToString());

        Task[] t = new Task[50];
        int actualCount = 5;

        Console.WriteLine("Starting tasks...");

        for (int i = 0; i < t.Length; i++)
        {
            t[i] = new Task(async (o) =>
            {
                while (true)
                {
                    await Task.Delay(250);

                    if ((int)o == 0)
                        Console.Title = actualCount.ToString();

                    if (r.Next(0, 100) < 6)
                    {
                        string str = Guid.NewGuid().ToString();

                        list.Add(str);
                        actualCount++;

                        Console.WriteLine($"Task { o } added to the list. " + str);
                    }
                    else if (list.Count > 20)
                    {
                        list.Clear();
                        actualCount = 0;
                        //list.Add(Guid.NewGuid().ToString());

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Task { o } cleared the list. ");
                        Console.ResetColor();
                    }
                    else if (r.Next(0, 100) < 4)
                    {
                        string str = list.GetRandom();

                        if (list.Remove(str))
                        {
                            actualCount--;

                            Console.WriteLine($"Task { o } removed something from the list. " + str);
                        }
                    }
                    else if (r.Next(0,100) < 3)
                    {
                        Console.WriteLine($"Task { o } is iterating through the list. ");

                        list.ForEach((e) =>
                        {
                            Console.Write('.');

                            Thread.Sleep(50);
                        });
                    }
                    else
                    {
                        Console.WriteLine($"Task { o } accesses the list. " + list.GetRandom());
                    }

                    if (actualCount != list.Count)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Actual count does not match! Stopping task " + o);
                        Console.ResetColor();
                        return;
                    }

                }
            },i);
            t[i].Start();

            Thread.Sleep(250);
        }

        Console.ReadKey();
    }

    public static void Mainasdasddfg(string[] args)
    {
        Console.WriteLine("Press any key to start...");
        Console.ReadKey();

        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine("Creating new client " + i);

            ConnectionStartInfo csi = new ConnectionStartInfo("192.168.0.116", "", false);
            csi.ApplicationName = "FNAFOnline";
            csi.ApplicationVersion = "1.0";
            csi.ConnectAsync = false;
            csi.ConnectOnConstruct = false;
            csi.ClientID = Guid.NewGuid().ToString();
            csi.AuthorizationToken = "d71ecff8-306c-42bf-8e99-96ac161b213d";

            Console.WriteLine("Client ID:" + csi.ClientID);

            Client c = new Client(csi);

            c.OnConnected += (ft) =>
            {
                Console.WriteLine("Connected! First time: " + ft);
            };
            c.OnDisconnected += (dr) =>
            {
                Console.WriteLine("Disconnected :(");
            };

            Console.WriteLine("Connecting...");
            c.ConnectSync();

            Console.WriteLine("Waiting...");
            Thread.Sleep(1500);
        }

        Console.ReadKey();
    }

    public static void Madfgsdfgsdfgin(string[] args)
    {
        string sending = "move 7,9,3";

        Console.WriteLine("Sending: " + sending);

        byte[] b = ByteWrapper.Wrap(Encoding.ASCII.GetBytes(sending), 9);

        Console.WriteLine("----------- SEND BY CLIENT -----------");
        /////// SEND BY CLIENT /\
        
        // Man in the middle
        string str = Encoding.ASCII.GetString(b, 4, b.Length - 4);
        Console.WriteLine(str);
        str = str.Replace("7","9");
        Array.Copy(Encoding.ASCII.GetBytes(str), 0, b, 4, Encoding.ASCII.GetByteCount(str));

        /////// RECEIVED BY SERVER \/
        Console.WriteLine("----------- RECEIVED BY SERVER -----------");

        ByteWrapper bw = ByteWrapper.UnWrap(b); //new byte[] { 0x5, 0x99, 0x87, 0x4a, 0xfe }
        Console.WriteLine("Received: " + Encoding.ASCII.GetString(bw.DataBuffer));
        Console.WriteLine("First: " + bw.FirstCrc16);
        Console.WriteLine("Last: " + bw.LastCrc16);
        Console.WriteLine("Integrity: " + bw.Integrity);
        Console.WriteLine("ContentType: " + bw.ContentType);
        Console.ReadKey();
    }

    public static void Mmmmmmain(string[] args)
    {
        int max = 100;
        //int spread = (int)Math.Ceiling(Math.Sqrt(max));

        int i = 0;
        for(int x = 0; i < max; x++)
            for(int y = 0; y < 10 && i < max; y++, i++)
            {
                Console.CursorTop = y * 2;
                Console.CursorLeft = x * 10;
                Console.Write(i);
                Console.Write(':');
            }

        int[] count = new int[max];
        int highest = 0;

        Console.ReadKey();

        while (true)
        {
            int val = XorShift.NextInt(0, max);
            int x = val / 10;
            int y = val % 10;

            count[val]++;

            if (count[val] > highest)
            {
                highest = count[val];
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.Black;
            }

            Console.CursorTop = y * 2;
            Console.CursorLeft = x * 10 + 4;
            Console.Write(count[val]);
            Console.ResetColor();


            //Console.WriteLine();
            //Thread.Sleep(50);
        }

        
    }

    private static int delay = 500;

    public static void Madiasdn(string[] args)
    {
        AsyncMethod();

        while (true)
        {
            string str = Console.ReadLine();

            delay = 1000 - str.Length * 25;


            Console.WriteLine($"Entered: { str };");
        }
    }

    private static async void AsyncMethod()
    {
        await Task.Delay(delay);

        Console.WriteLine("This is from task");

        await Task.Yield();

        AsyncMethod();
    }

    public static void Maindfg(string[] args)
    {
        string[] testStrings = { "piemel", "pannekoek", "supposed", "man", "brol", "testje", "yoooo", "jow wat is dit een lange string zeg holy tering kan hte nog langer>????" };

        foreach (string str in testStrings)
            Console.WriteLine($"{ str }: { StringUtil.StringToUniqueLong(str) }");

        Console.ReadKey();
    }

    public static void Mainaasdsd(string[] args)
    {
        XorShift.Initiate(18);

        for(int i = 0; true; i++)
        {
            Console.WriteLine($"Starting XorShift test { i } with state { XorShift.State }.");

            Console.WriteLine($"Test NextByte { XorShift.NextByte() }");
            Console.WriteLine($"Test NextInt { XorShift.NextInt() }");
            Console.WriteLine($"Test NextIntWow { XorShift.NextIntWow() }");
            Console.WriteLine($"Test NextLong { XorShift.NextLong() }");
            Console.WriteLine($"Test NextLongPlus { XorShift.NextLongPlus() }");
            Console.WriteLine($"Test NextLongStar { XorShift.NextLongStar() }");
            Console.WriteLine($"Test NextShort { XorShift.NextShort() }");

            Console.ReadKey();
        }
    }

    public static void Mafghin(string[] args)
    {
        StxNet.RegisterNetworkTypes();

        RequestPacket p1 = new RequestPacket("abcdef1234567890", "tyfus", (obj) => { });
        Packet p2 = new Packet("1234567890abcdef");
        Console.WriteLine("Input1: " + p1);
        Console.WriteLine("Input2: " + p2);

        byte[] buffer1 = ByteUtil.WrapSegmentedBytes(p1.ToBytes());
        byte[] buffer2 = ByteUtil.WrapSegmentedBytes(p2.ToBytes());

        byte[] buffer = new byte[4096];
        Array.Copy(buffer1, 0, buffer, 0, buffer1.Length);
        Array.Copy(buffer2, 0, buffer, buffer1.Length, buffer2.Length);

        // --> send --> receive

        byte[] received = buffer;

        foreach (byte[] b in ByteUtil.UnwrapSegmentedBytes(buffer))
        {
            Packet p = Bytifier.ObjectSize<Packet>(b, 0, b.Length);

            Console.WriteLine($"Unwrapped({ b.Length }): " + (p?.ToString() ?? "null"));
        }

        Console.ReadKey();
    }

    /*public static void Maind(string[] args)
    {
        string[] toHash = new string[] {
            "hoi ik ben stijn", "dit is wel een erg lange zin met erg veel spaties ow yeah.",
        "testje", "0", "P", "datWASeenLETTER", ";][1234356*&%.,;:", "vr33md3 t3k3n5", ""};

        //SR1Hasher hasher = new SR1Hasher(987, "dit is zout");

        Console.WriteLine($"Current minutes: { DateTime.UtcNow.Minute }");

        Stopwatch sw = new Stopwatch();
        sw.Start();

        foreach (string str in toHash)
        {
            Console.WriteLine($"Hash of '{ str.PadRight(35) }' : " + hasher.CalculateTemporaryHash(str));
        }

        sw.Stop();

        Console.WriteLine($"Ended. Took { sw.ElapsedMilliseconds } millis.");
        Console.ReadKey();
    }*/

    private class AsyncObject
    {
        public string Value { get; set; }

        public AsyncObject(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"A value of this object: { Value }";
        }
    }

    private class TestAsyncClass : ThreadSafeDataTransfer<AsyncObject>
    {
        public new void Transfer(AsyncObject data)
        {
            base.Transfer(data);
        }

        protected override void Received(AsyncObject data)
        {
            Console.WriteLine("Received data: " + data.ToString());
        }
    }

    /*public static void Main9(string[] args)
    {
        Console.ReadKey();

        BHashtable v = new BHashtable();

        v.Add("aiektes123", 889);

        Stopwatch sw = new Stopwatch();
        sw.Start();

        ByteHasher bh = new ByteHasher();

        foreach (string key in v.Keys)
            bh.AppendHash(key);
        foreach (object val in v.Values)
            bh.AppendHash(val);

        sw.Stop();

        Console.WriteLine($"Took { sw.ElapsedMilliseconds } milliseconds.");

        Console.WriteLine("Requires1: " + v.Requires<int>("ziektes123"));
        Console.WriteLine("Requires2: " + v.Requires<int, string>("ziektes123", "mheh"));

        Console.WriteLine("Hash: " + bh.GetHashCode());
        Console.ReadKey();
    }*/

    public static void Main0(string[] args)
    {
        Bytifier.Include<BList<Room>>();
        Bytifier.Include<BConcurrentList<Room>>();
        Bytifier.Include<MatchmakingQuery>();
        Bytifier.Include<MatchmakingQueryResult>();
        Bytifier.Include<Room>();
        Bytifier.Include<RoomTemplate>();
        //Bytifier.Include<RoomInfo>();
        Bytifier.Include<ClientIdentity>();
        Bytifier.Include<ClientInfo>();
        Bytifier.Include<ChatEntry>();
        Bytifier.Include<AchievementGrantedInfo>();
        Bytifier.IncludeEnum<ClientStatus>();
        Bytifier.IncludeEnum<ClientRoomStatus>();
        Bytifier.IncludeEnum<GameState>();
        Bytifier.IncludeEnum<ChatSourceType>();

        RoomTemplate rt = new RoomTemplate("ikbenroom", 3, null);
        Room p = new Room(rt, new ClientIdentity("jezus-id"));

        Console.WriteLine(p.ToString());

        byte[] buffer = Bytifier.Bytes(p);

        Console.WriteLine("Serialized size: " + StringUtil.FormatBytes(buffer.Length));

        Room p2 = Bytifier.Object<Room>(buffer);

        Console.WriteLine(p2.ToString());
        Console.ReadKey();
    }

    public static void Main1(string[] args)
    {
        string input = "testje";
        Console.Write(input);
        byte[] s = Encoding.ASCII.GetBytes(input);
        //s = ByteUtil.XorKey(s, 7583);
        //Console.Write("->" + Encoding.ASCII.GetString(s));
        //s = ByteUtil.XorKey(s, 7583);
        //Console.Write("->" + Encoding.ASCII.GetString(s));

        Bytifier.Include<ByteCouple>();
        Bytifier.Include<TestingObject>();
        Bytifier.Include<BList<string>>();
        Bytifier.Include<BDictionary<string, object>>();
        Bytifier.Include<BHashtable>();

        string path = @"C:\Users\Stijn Rogiest\Desktop\test.bbin";

        if (!File.Exists(path))
        {
            TestingObject to = new TestingObject();
            to.MyName = "DylanIsGay";
            to.NiceEnum = TestingObject.TestEnum.ValueSecond;

            Bytifile.ToFile(path, to);

            Console.WriteLine("Saved.");
            Console.ReadKey();
        }

        TestingObject fr = Bytifile.FromFile<TestingObject>(path);

        Console.WriteLine(fr.MyName);
        Console.WriteLine(fr.NiceEnum);
        Console.WriteLine(fr.TestInteger);

        Console.Write("Enter new name: ");
        fr.MyName = Console.ReadLine();

        Bytifile.ToFile(path, fr);

        Console.WriteLine("Saved.");
        Console.ReadKey();
    }

    public static void Main2(string[] args)
    {
        Bytifier.Include<ByteCouple>();
        Bytifier.Include<TestingObject>();
        Bytifier.Include<BList<string>>();
        Bytifier.Include<BDictionary<string, object>>();
        Bytifier.Include<BHashtable>();
        Bytifier.Include<BStack<string>>();
        Bytifier.Include<BQueue<float>>();
        Bytifier.Include<AgentID>();
        Bytifier.IncludeEnum<TestingObject.TestEnum>();
        Bytifier.StringEncoding = Encoding.Unicode;

        Console.WriteLine("Waiting...");
        Console.ReadKey();
        Console.WriteLine(" ---------- Serializing ---------- ");

        TestingObject to = new TestingObject();
        to.MyName = "Stijn Rogiest";
        to.NiceEnum = TestingObject.TestEnum.ValueNotLast;
        to.MoreInt = 123;
        to.AObject = TestingObject.TestEnum.Value1;
        to.ADate = DateTime.Now;
        to.Hash.Add("hoerb", TestingObject.TestEnum.ValueNotLast);
        to.Hash.Add("thisKey", new AgentID() { SecretName = "VerySecretYes" });
        to.Hash.Add("hoera", new ByteCouple(8, 9));

        //to.Arries = new object[] { 1, "hey", 45, true, 45 };
        /*to.TestList.Add("ziektes1First");
        to.TestList.Add("ziektes12");
        to.TestList.Add("ziektes123");
        to.TestList.Add("ziektes1234End");
        to.Table.Add("keytest", 1234);
        to.Table.Add("842665fc-bae0-4ac5-8bce-5ca9dc389b69", 696);
        to.Table.Add("467b41ef-be82-4862-9750-0c9994451624", 444);
        to.Table.Add("d0caa6b4-ad9c-4800-a787-7fecfb2e2e7c", "hey");
        to.Table.Add("a8aab18c-8f14-4966-a0c3-850db0c23a15", 1234);
        to.Hash.Add(5, 9.6f);
        to.HighStack.Push("name1");
        to.HighStack.Push("thisislastmustbefirst");
        to.WaitQueue.Enqueue(987f);
        to.WaitQueue.Enqueue(456.897f);
        to.WaitQueue.Enqueue(123f);*/
        to.SecretAgent.SecretName = "CodeStix";
        to.SecretAgent.SecretID = 696969;

        Stopwatch st = new Stopwatch();
        st.Start();

        byte[] full = /*ByteUtil.XorKey(*/Bytifier.Bytes(to)/*, 1)*/;

        st.Stop();

        Console.WriteLine("Serialized size: " + StringUtil.FormatBytes(full.Length));

        Console.WriteLine($"SERIALIZE: Took { st.ElapsedMilliseconds } millis.");

        Dictionary<byte, int> count = new Dictionary<byte, int>();
        int i = 0;
        foreach (byte b in full)
        {
            //Console.Write(Convert.ToString(b, 2).PadLeft(8, '0') + ' ');
            Console.Write(b.ToString().PadLeft(4, ' ') + ' ');

            if (!count.ContainsKey(b))
                count.Add(b, 0);

            count[b]++;

            if (i++ == 15)
            {
                Console.WriteLine();
                i = 0;
            }
        }
        Console.WriteLine();
        Console.WriteLine(" ---------- Serializing result ---------- ");

        foreach (KeyValuePair<byte, int> item in count.OrderBy(key => key.Value).Reverse())
        {
            Console.WriteLine($"Byte { item.Key } ({ Convert.ToString(item.Key, 2).PadLeft(8, '0') }): " + item.Value);
        }

        /* ------------------------------------------- */

        Console.ReadKey();

        Console.WriteLine(" ---------- Deserializing ---------- ");

        st.Restart();

        TestingObject basd = Bytifier.Object<TestingObject>(/*ByteUtil.XorKey(*/full/*, 1)*/);

        st.Stop();

        Console.WriteLine("TestInteger = " + basd.TestInteger);
        Console.WriteLine("MoreInt = " + basd.MoreInt);
        Console.WriteLine("AChar = " + basd.AChar);
        Console.WriteLine("Couple = " + basd.MyBytes.ToString());
        Console.WriteLine("NullCouple = " + (basd.NullBytes == null));
        Console.WriteLine("MyName = " + (basd.MyName));
        Console.WriteLine("AObject = " + (basd.AObject));
        Console.WriteLine("NiceEnum = " + (basd.NiceEnum));
        Console.WriteLine("GreatFloat = " + (basd.GreatFloat));
        Console.WriteLine("Arries.Length = " + basd.Arries.Length);
        Console.WriteLine("Arries[1] = " + basd.Arries[1]);
        Console.WriteLine("Hash[thisKey] = " + basd.Hash["thisKey"]);
        Console.WriteLine("Hash[hoera] = " + basd.Hash["hoera"]);
        Console.WriteLine("Hash[hoerb] = " + basd.Hash["hoerb"]);

        /*foreach(var t in basd.TestList)
            Console.WriteLine("\t- " + t);
        Console.WriteLine("Table[keytest] = " + basd.Table["keytest"]);
        Console.WriteLine("Table[d0caa6b4-ad9c-4800-a787-7fecfb2e2e7c] = " + basd.Table["d0caa6b4-ad9c-4800-a787-7fecfb2e2e7c"]);
        Console.WriteLine("Hash[5] = " + basd.Hash[5]);
        Console.WriteLine("HighStack.Pop() = " + basd.HighStack.Pop());
        Console.WriteLine("HighStack.Pop() = " + basd.HighStack.Pop());
        Console.WriteLine("WaitQueue.Dequeue() = " + basd.WaitQueue.Dequeue());
        Console.WriteLine("WaitQueue.Dequeue() = " + basd.WaitQueue.Dequeue());
        Console.WriteLine("WaitQueue.Dequeue() = " + basd.WaitQueue.Dequeue());*/
        Console.WriteLine("Agent = " + basd.SecretAgent.ToString());
        Console.WriteLine("ADate = " + basd.ADate);

        Console.WriteLine("Done.");

        st.Stop();

        Console.WriteLine($"DESERIALIZE: Took { st.ElapsedMilliseconds } millis.");

        Console.ReadKey();
    }

    public static void WriteByte(byte b)
    {
        Console.Write(Convert.ToString(b, 2).PadLeft(8, '0') + ' ');
    }

    public static void WriteLineByte(byte b)
    {
        Console.WriteLine(Convert.ToString(b, 2).PadLeft(8, '0'));
    }

    public class TestingObject : IByteDefined
    {
        public BHashtable Hash { get; set; } = new BHashtable();
        public AgentID SecretAgent { get; set; } = new AgentID();
        public ByteCouple MyBytes { get; set; } = new ByteCouple(23, 32);
        public int TestInteger { get; set; } = 90;

        [DoNotSerialize]
        public int MoreInt { get; set; } = 99999;

        [DoNotSerialize]
        public char AChar { get; set; } = 't';

        [DoNotSerialize]
        public ByteCouple NullBytes { get; set; } = null;

        public string MyName { get; set; } = "ThisIsMyName";
        public object AObject { get; set; }//new ByteCouple(224, 69);
        public TestEnum NiceEnum { get; set; } = TestEnum.Unknown;
        public float GreatFloat { get; set; } = 8654.32f;
        public int[] Arries { get; set; } = new int[] { 5, 9, 7 };
        public DateTime ADate { get; set; }

        /*public BList<string> TestList { get; set; } = new BList<string>();
        public BDictionary<string, object> Table { get; set; } = new BDictionary<string, object>();
        public BHashtable Hash { get; set; } = new BHashtable();
        public BStack<string> HighStack { get; set; } = new BStack<string>();
        public BQueue<float> WaitQueue { get; set; } = new BQueue<float>();*/

        public TestingObject()
        {
        }

        public enum TestEnum
        {
            NoValue,
            First1,
            Value1,
            ValueSecond,
            ValueNotLast,
            Unknown
        }
    }

    public class ByteCouple : IBytifiable<ByteCouple>
    {
        public byte FirstByte { get; set; }
        public byte SecondByte { get; set; }

        public ByteCouple()
        { }

        public ByteCouple(byte one, byte two)
        {
            FirstByte = one;
            SecondByte = two;
        }

        public void FromBytes(byte[] from)
        {
            FirstByte = from[0];
            SecondByte = from[1];
        }

        public byte[] ToBytes()
        {
            return new byte[] { FirstByte, SecondByte };
        }

        public override string ToString()
        {
            return $"({ FirstByte } : { SecondByte })";
        }
    }

    public class AgentID : IByteDefined
    {
        public int SecretID { get; set; } = 9878970;
        public string SecretName { get; set; } = "TopSecretName";

        public AgentID()
        { }

        public override string ToString()
        {
            return $"(SECRET; { SecretID } = { SecretName })";
        }
    }

    /*public static void Main0(string[] args)
    {
        //Serialize(new TestObject());

        RequestPacket p = new RequestPacket("thisIsASender", "Requested", (e) => { });
        p.Data.Add("test", 5.0f);

        byte[] sdf = p.ToNewBytes();

        Console.WriteLine("SERIALIZE");

        int i = 0;
        foreach (byte b in sdf)
        {
            Console.Write(Convert.ToString(b, 2).PadLeft(8, '0') + ' ');

            if (i++ == 3)
            {
                Console.WriteLine();
                i = 0;
            }
        }
        Console.WriteLine();

        Console.WriteLine("DESERIALIZE");

        new RequestPacket().FromNewBytes(sdf);

        Console.ReadKey();
    }

    public static byte[] Serialize<T>(T forInstance)
    {
        Type t = forInstance.GetType();

        if (t.IsGenericType)
        {
            Console.WriteLine("GENERIC: " + t.Name);
            Type[] generics = t.GetGenericArguments();

            if (t.Name.StartsWith("KeyValuePair"))
            {
                Console.WriteLine("Is KeyValuePair.");

                dynamic b = forInstance;

                Console.WriteLine("Key: " + b.Key);
                Console.WriteLine("Value: " + b.Value);

                Type[] targs = { b.Key.GetType(), b.Value.GetType() };
                Type constructed = typeof(SimpleKeyValuePair<,>).MakeGenericType(targs);

                return Serialize(Activator.CreateInstance(constructed, b.Key, b.Value));
            }
            else if (t.Name.Contains("Dictionary"))
            {
                Console.WriteLine("Is Dictionary.");
            }
        }

        t = forInstance.GetType();

        PropertyInfo[] properties = t.GetProperties();
        foreach (PropertyInfo property in properties)
        {
            //Console.WriteLine(property.Name + "...");

            object val = property.GetValue(forInstance);
            var valType = val?.GetType();
            bool isNull = val == null;

            if (!isNull && valType.IsArray)
            {
                Array arrVal = (Array)val;

                for (int i = 0; i < arrVal.Length; i++)
                {
                    //Console.WriteLine($"{i}: " + arrVal.GetValue(i));

                    SingeSerialize(arrVal.GetValue(i), property);
                }
            }
            else
            {
                SingeSerialize(val, property);
            }
        }

        return null;
    }

    private static void SingeSerialize(object val, PropertyInfo info = null)
    {
        var valType = val?.GetType();

        if (valType == null || valType.IsPrimitive || valType.IsEnum || valType == typeof(decimal) || valType == typeof(string))
        {
            Console.WriteLine((info?.Name ?? "?") + ": " + (val ?? "null"));
        }
        else
        {
            Console.WriteLine("Is not primitive: " + valType);
            Serialize(val);
        }
    }

    public class SimpleDictionary<T, U>
    {
        public ICollection<T> Keys { get; set; }
        public ICollection<U> Values { get; set; }
    }

    public class SimpleKeyValuePair<T, U>
    {
        public T Key { get; set; }
        public U Value { get; set; }

        public SimpleKeyValuePair(T key, U value)
        {
            Key = key;
            Value = value;
        }
    }*/
}

#if STX_UDPCLIENT_TEST

public static class TestProgram
{
    private static UdpClient c;
    private static List<IPEndPoint> connected = new List<IPEndPoint>();
    private static bool isClient = false;

    static void Main(string[] args)
    {
        ErrorHandler.Instance.MultiThread = false;
        ErrorHandler.Instance.OnError += (e) =>
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: " + e.ToString());
            Console.ForegroundColor = ConsoleColor.Gray;
        };

        string m = Console.ReadLine();

        if (m == "server")
        {
            isClient = false;

            Console.WriteLine("--- SERVER ---");

            c = new UdpClient(11987);
            c.BeginReceive(new AsyncCallback(Receive), null);

            while (true)
            {
                string input = Console.ReadLine();

                byte[] b = Encoding.ASCII.GetBytes(input);

                //c.Send(b, b.Length);

                foreach(var i in connected)
                    c.Send(b, b.Length, i);
            }
        }
        else
        {
            isClient = true;

            Console.WriteLine("--- CLIENT ---");

            IPAddress ip;
            ushort port;

            if (!IPUtil.ParseIPAndPort(Console.ReadLine(), out ip, out port, 11987))
            {
                Console.WriteLine("Invalid input.");
                Console.ReadKey();

                return;
            }

            c = new UdpClient();
            c.Connect(ip, port);
            c.BeginReceive(new AsyncCallback(Receive), null);

            while (true)
            {
                string input = Console.ReadLine();

                byte[] b = Encoding.ASCII.GetBytes(input);

                c.Send(b, b.Length);
            }
            // voiceClient = new VoiceClient(a, b, ip, port, AudioSettings.Default);
        }
    }

    private static void Receive(IAsyncResult e)
    {
        IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
        byte[] received = null;
        try
        {
            received = c.EndReceive(e, ref remote);
        }
        catch
        {
            if (isClient)
            {
                Console.WriteLine("Server has shut down");
                //client
                //// SERVER SHUTDOWN
            }
            else
            {
                Console.WriteLine("Error -> Restart");
                //server
                // a client has disconnected, restart
                c.Close();
                c = null;

                connected = new List<IPEndPoint>();

                c = new UdpClient(11987);
                c.BeginReceive(new AsyncCallback(Receive), null);
            }

            return;
        }

        if (!connected.Contains(remote))
            connected.Add(remote);

        Console.WriteLine($"Received { received?.Length } bytes from { remote }.");

        c.BeginReceive(new AsyncCallback(Receive), null);
    }
}

#endif

#if STX_VOICEBYTES_NET_TEST

public static class TestProgram
{
    static void Main(string[] args)
    {
        DebugHandler.Instance.MultiThread = false;
        DebugHandler.AutoWriteToConsole = true;

        var inputs = VoiceIn.GetAudioInputDevices();
        var outputs = VoiceOut.GetAudioOutputDevices();

        DebugHandler.Info("Inputs:");
        foreach (var i in inputs)
            Console.WriteLine("\t- " + i.Name);

        DebugHandler.Info("Outputs:");
        foreach (var i in outputs)
            Console.WriteLine("\t- " + i.Name);

        VoiceServer voiceServer;
        VoiceClient voiceClient;

        string m = Console.ReadLine();

        if (m == "server")
        {
            DebugHandler.Info("--- SERVER ---");

            voiceServer = new VoiceServer(11987);
            voiceServer.EnableGlobalRoom = true;
        }
        else
        {
            DebugHandler.Info("--- CLIENT ---");

            var a = inputs.First((e) => e.Name.Contains("Microfoon van hoofdtelefoon"));
            var b = outputs.First((e) => e.Name.Contains("Oortelefoon van hoofdtelefoon"));

            DebugHandler.Info("In: " + a.Name);
            DebugHandler.Info("Out: " + b.Name);

            IPAddress ip;
            ushort port;

            if (!IPUtil.ParseIPAndPort(Console.ReadLine(), out ip, out port, 11987))
            {
                DebugHandler.Warning("Invalid input.");
                Console.ReadKey();

                return;
            }

            voiceClient = new VoiceClient(a, b, ip, port, AudioSettings.Default);
        }

        while (true)
        {
            string input = Console.ReadLine();

            DebugHandler.Warning("this is async");
        }
    }
}

#endif

#if STX_VOICEBYTES_TEST

public static class TestProgram
{
    private static VoiceIn vIn;
    private static VoiceOut vOut;

    static void Main(string[] args)
    {
        var inputs = VoiceIn.GetAudioInputDevices();
        var outputs = VoiceOut.GetAudioOutputDevices();

        Console.WriteLine("Inputs:");
        foreach (var i in inputs)
            Console.WriteLine("\t- " + i.Name);

        Console.WriteLine("Outputs:");
        foreach (var i in outputs)
            Console.WriteLine("\t- " + i.Name);

        var a = inputs.First((e) => e.Name.Contains("Microfoon van hoofdtelefoon"));
        var b = outputs.First((e) => e.Name.Contains("Oortelefoon van hoofdtelefoon"));

        Console.WriteLine("In: " + a);
        Console.WriteLine("Out: " + b);

        vIn = new VoiceIn(a, AudioSettings.Default);
        vOut = new VoiceOut(b, AudioSettings.Default);

        vIn.OnRecordAudio += VIn_OnRecordAudio;

        while (true)
        {
            string input = Console.ReadLine();

            if (input == "reload")
            {
            }

            Console.WriteLine("this is async");
        }
    }

    private static void VIn_OnRecordAudio(byte[] uLawAudioBytes)
    {
        vOut.PlayAudio(uLawAudioBytes);
    }
}

#endif

#if STX_CLIENT_CONSOLE_TEST

public static class Program
{
    private static Client client;
    private static ServerFunctions serverFunctions;

    public const string APPLICATION_NAME = "FNAFOnline";
    public const string APPLICATION_VERSION = "1.0";

    static void Main(string[] args)
    {
        IPAddress addrs;
        if (args.Length >= 1)
        {
            addrs = IPAddress.Parse(args[0]);
        }
        else
        {
            Console.Write("Enter IP to connect to: ");
            addrs = IPAddress.Parse(Console.ReadLine());
        }

        int port = 1987;

        if (args.Length >= 2)
        {
            int.TryParse(args[1], out port);
        }

        string clientID = StxClient.Properties.Settings.Default.ClientID;

        if (string.IsNullOrEmpty(clientID))
        {
            clientID = Guid.NewGuid().ToString();

            Console.WriteLine("Creating your unique ID...");

            StxClient.Properties.Settings.Default.ClientID = clientID;
            StxClient.Properties.Settings.Default.Save();
        }

        Console.WriteLine("Your ID is " + clientID);

        ErrorHandler.Instance.OnError += Client_OnError;
        client = new Client(APPLICATION_NAME, APPLICATION_VERSION, addrs, port, "wachtwoord", clientID);
        client.OnReceived += Client_OnReceive;
        client.DataReceiver.AddHandler(new DataHandler<bool>("_FirstTime", (n) =>
        {
            Console.WriteLine("Server asks for your name:");
            string name = Console.ReadLine();

            RequestPacket rp = client.GetNewRequestPacket(Identifiers.REQUEST_SET_CLIENT_INFO, (obj) =>
            {
                Console.WriteLine("Server name set response: " + obj);
            });

            rp.Data.Add("Name", name);

            client.SendToServer(rp.ToBytes());
        }));

        serverFunctions = new ServerFunctions(client);

        while (true)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            string c;
            Packet p = client.GetNewPacket();

            Console.WriteLine("---- NEW PACKET ----");

            do
            {
                c = Console.ReadLine();

                if (c.Contains(":"))
                {
                    string[] s = c.Split(':');
                    data.Add(s[0], s[1]);
                    Console.WriteLine("DATA OK");
                }

                if (c.StartsWith("?"))
                {
                    c = c.Substring(1);
                    p = client.GetNewRequestPacket(c, (obj) =>
                    {
                        Console.WriteLine("--RESPONSE--");
                        Console.WriteLine(obj);
                    });
                    Console.WriteLine("REQUEST OK");
                }

                if (c.StartsWith("$"))
                {
                    c = c.Substring(1);
                    data.Add("RoomTemplate", new RoomTemplate(c, 2));
                    Console.WriteLine("NEW ROOM OK");
                }
            } while (c != ">");

            //Packet p = client.GetNewPacket();

            foreach (string key in data.Keys)
                p.Data.Add(key, data[key]);

            client.SendToServer(p.ToBytes());
        }
    }

    private static void Client_OnReceive(Packet p)
    {
        Console.WriteLine($"Received data: ");

        foreach (string key in p.Data.Keys)
        {
            Console.WriteLine($"{ key } = { p.Data[key] }");
        }
    }

    private static void Client_OnError(CodedException e)
    {
        Console.WriteLine($"Error(#{ e.ErrorCode }): { e.Error.Message }");
    }
}

#endif