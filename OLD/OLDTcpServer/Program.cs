using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TcpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("TcpServer.exe [--silent] ( --server [:port] | --client host[:port] )");
                Console.WriteLine("Default port is 1987.");
                Console.WriteLine($"Derver host is { GetLocalIP() }.");
                Console.WriteLine("Simple Tcp Data Stream application by Stijn Rogiest (c) 2018. v1.0");
                return;
            }

            string j = string.Join(" ", args).Trim().ToLower();

            bool silent = false;
            bool isServer = false;

            if (j.Contains("--silent"))
                silent = true;

            if (j.Contains("--server"))
                isServer = true;

            string hostPort = args.Length == 1 ? args[0] : j.Substring(j.LastIndexOf(' ')).Trim();
            string[] s = hostPort.Split(':');

            int port = 1987;

            if (s.Length > 1)
                port = int.Parse(s[1]);

            string host = s[0];

            if (isServer)
                host = GetLocalIP();

            try
            {
                StartStream(isServer, host, port, silent);
            }
            catch (Exception e)
            {
                Console.WriteLine("!" + e.Message);
                Environment.Exit(0);
            }
        }

        private static void StartStream(bool isServer, string host, int port, bool silent = false)
        {
            TcpStream s = new TcpStream(isServer, host, port);

            s.OnException += (e) =>
            {
                Console.WriteLine("!" + e.Message);
                Environment.Exit(0);
            };

            if (!silent)
                Console.WriteLine("Connecting...");

            while (!s.Connected) ;

            if (!silent)
                Console.WriteLine("Connected!");
            else
                Console.WriteLine(".");

            new Thread(new ThreadStart(() => {

                while (true)
                {
                    while (s.received.Count <= 0) ;

                    Console.WriteLine((silent ? "" : Environment.NewLine + "<<<") + s.received.Dequeue());
                }

            })).Start();

            while (true)
            {
                if (!silent)
                    Console.Write(">>>");

                s.toSend.Enqueue(Console.ReadLine());
            }
        }

        private static string GetLocalIP()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint.Address.ToString();
            }
        }
    }
}
