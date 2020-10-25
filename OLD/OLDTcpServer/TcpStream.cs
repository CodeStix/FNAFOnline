using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class TcpStream
{
    public bool IsServer { get; private set; }
    public string Host { get; private set; }
    public int Port { get; private set; }
    public bool Connected { get; private set; }

    public volatile Queue<string> toSend = new Queue<string>();
    public volatile Queue<string> received = new Queue<string>();

    public delegate void OnExceptionDelegate(Exception e);
    public event OnExceptionDelegate OnException;

    private Thread receivingThread;
    private Thread sendingThread;

    private object locker = new object { };

    //Server
    private IPAddress ipAd;
    private TcpListener myList;
    private Socket s;

    //Client
    private TcpClient tcpclnt;
    private Stream stm;

    public TcpStream(bool isServer, string host, int port)
    {
        this.IsServer = isServer;
        this.Host = host;
        this.Port = port;

        new Thread(new ThreadStart(StartingThread)).Start();

       /* try
        {
           
        }
        catch (Exception e)
        {
            OnException.Invoke(e);
        }*/
    }

    public void Dispose()
    {
        receivingThread.Abort();
        sendingThread.Abort();

        if (IsServer)
        {
            stm.Close();
            tcpclnt.Close();
            stm.Dispose();
        }
        else
        {
            s.Close();
            myList.Stop();
        }
    }

    private void StartingThread()
    {
        if (IsServer)
        {
            ipAd = IPAddress.Parse(Host);

            myList = new TcpListener(ipAd, Port);
            myList.Start();

            s = myList.AcceptSocket();

            Connected = true;
        }
        else
        {
            tcpclnt = new TcpClient();
            tcpclnt.Connect(Host, Port);

            stm = tcpclnt.GetStream();

            Connected = true;
        }

        receivingThread = new Thread(new ThreadStart(ReceivingThread));
        sendingThread = new Thread(new ThreadStart(SendingThread));

        receivingThread.Start();
        sendingThread.Start();
    }

    private void ReceivingThread()
    {
        try
        {
            if (IsServer)
            {
                while (true)
                {
                    byte[] bb = new byte[256];
                    int ss = s.Receive(bb);

                    lock (this.locker)
                    {
                        received.Enqueue(Encoding.ASCII.GetString(bb, 0, ss));
                    }
                }
            }
            else
            {
                while (true)
                {
                    byte[] bb = new byte[256];
                    int ss = stm.Read(bb, 0, 256);

                    lock (this.locker)
                    {
                        received.Enqueue(Encoding.ASCII.GetString(bb, 0, ss));
                    }
                }
            }
        }
        catch(Exception e)
        {
            Dispose();
            OnException?.Invoke(e);
        }

    }

    private void SendingThread()
    {
        try
        {
            if (IsServer)
            {
                while (true)
                {
                    while (toSend.Count <= 0) ;

                    byte[] ba = Encoding.ASCII.GetBytes(toSend.Dequeue());
                    s.Send(ba, ba.Length, SocketFlags.None);
                }
            }
            else
            {
                while (true)
                {
                    while (toSend.Count <= 0) ;

                    byte[] ba = Encoding.ASCII.GetBytes(toSend.Dequeue());
                    stm.Write(ba, 0, ba.Length);
                }
            }
        }
        catch (Exception e)
        {
            Dispose();
            OnException?.Invoke(e);
        }
    }
}
