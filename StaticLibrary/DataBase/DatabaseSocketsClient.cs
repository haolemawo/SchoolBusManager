using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using WBPlatform.StaticClasses;
using WBPlatform.Config;
using WBPlatform.Logging;
using System.Collections.Concurrent;

namespace WBPlatform.Database.Connection
{
    public static class DatabaseSocketsClient
    {
        //创建 1个客户端套接字 和1个负责监听服务端请求的线程  
        private static Thread ReceiverThread = new Thread(Recv);
        private static Thread DataBaseConnectionMaintainer = new Thread(new ThreadStart(Maintain));
        private static TcpClient socketclient = new TcpClient();
        private static NetworkStream stream;
        private static IPEndPoint remoteEndpoint;

        private static TimeSpan WaitTimeout = new TimeSpan(0, 0, XConfig.Current.Database.ClientTimeout);
        private static bool IsFirstTimeInit { get; set; } = true;

        public static bool Connected { get { return socketclient.Connected; } }

        private static ConcurrentDictionary<string, string> _messages { get; set; } = new ConcurrentDictionary<string, string>();
        public static void KillConnection()
        {
            ReceiverThread.Abort();
            DataBaseConnectionMaintainer.Abort();
            socketclient.CloseAndDispose();
            stream.CloseAndDispose();
        }
        public static bool Initialise(IPAddress ServerIP, int Port)
        {
            socketclient = new TcpClient();
            remoteEndpoint = new IPEndPoint(ServerIP, Port);
            int FailedRetry = XConfig.Current.Database.FailedRetryTime;
            for (int i = 0; i < FailedRetry; i++)
            {
                try
                {
                    socketclient.Connect(remoteEndpoint);
                    stream = socketclient.GetStream();
                    LW.I("\tDatabase Connection Estabilished!");
                    if (IsFirstTimeInit)
                    {
                        ReceiverThread.Start();
                        DataBaseConnectionMaintainer.Start();
                        IsFirstTimeInit = false;
                    }
                    SendCommand("openConnection", "00000", out string token);
                    LW.I("\tDatabase Connected! Identity: " + token);
                    return true;
                }
                catch (Exception ex)
                {
                    LW.E("\t\tDatabase connection to server: " + ServerIP + " failed. " + ex.Message);
                    Thread.Sleep(1000);
                }
            }
            return false;
        }

        // 接收服务端发来信息的方法    
        static void Recv()
        {
            while (true)
            {
                while (Connected)
                {
                    try
                    {
                        string requestString = PublicTools.DecodeDatabasePacket(stream);
                        _messages.TryAdd(requestString.Substring(0, 5), requestString.Substring(5));
                    }
                    catch (ThreadAbortException e)
                    {
                        return;
                    }
                    catch (Exception ex) { Thread.Sleep(1000); LW.E(ex.ToParsedString()); }
                }
                while (!Connected)
                {
                    LW.E("Message Recieve waiting for connection......");
                    Thread.Sleep(500);
                }
            }
        }

        private static void Maintain()
        {
            while (true)
            {
                try
                {
                    string _mid = Cryptography.RandomString(5, false);
                    byte[] packet = PublicTools.MakeDatabasePacket(_mid, "HeartBeat");
                    if (CoreSend(packet, _mid, out string reply))
                    {
                        LW.I("HeartBeat Succeed! " + reply);
                    }
                    else
                    {
                        throw new Exception("CoreSend Error: Timeout");
                    }
                    Thread.Sleep(5000);
                }
                catch (Exception ex)
                {
                    if (ex is ThreadAbortException) return;
                    LW.E("Heartbeat Error! " + ex.Message);
                    socketclient.CloseAndDispose();
                    stream.CloseAndDispose();
                    Initialise(remoteEndpoint.Address, remoteEndpoint.Port);
                    Thread.Sleep(5000);
                }
            }
        }

        //发送字符信息到服务端的方法
        public static bool SendCommand(string sendMsg, string MessageId, out string rcvdMessage)
        {
            rcvdMessage = "";
            byte[] mergedPackage = PublicTools.MakeDatabasePacket(MessageId, sendMsg);
            while (!Connected)
            {
                LW.E("Message Sent Waiting for connection....");
                Thread.Sleep(500);
            }
            return CoreSend(mergedPackage, MessageId, out rcvdMessage);
        }

        private static bool CoreSend(byte[] packet, string MessageId, out string rcvdMessage)
        {
            stream.Write(packet, 0, packet.Length);
            DateTime _timeoutTime = DateTime.Now.Add(WaitTimeout);
            while (true)
            {
                if (_messages.ContainsKey(MessageId))
                {
                    _messages.TryRemove(MessageId, out rcvdMessage);
                    return true;
                }
                Thread.Sleep(10);
                if (_timeoutTime.Subtract(DateTime.Now).TotalMilliseconds <= 0)
                {
                    rcvdMessage = null;
                    return false;
                }
            }
        }
    }
}
