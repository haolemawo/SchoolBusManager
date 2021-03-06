﻿using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using WBPlatform.Config;
using WBPlatform.Logging;
using WBPlatform.StaticClasses;

namespace WBPlatform.Database.Connection
{
    public static class DatabaseSocketsClient
    {
        private static Thread ReceiverThread = new Thread(Recv);
        private static Thread DataBaseConnectionMaintainer = new Thread(new ThreadStart(Maintain));
        private static TcpClient socketClient = new TcpClient();
        private static NetworkStream stream;
        private static IPEndPoint remoteEndpoint;
        private static bool IsShutdownStarted { get; set; } = false;

        private static TimeSpan WaitTimeout = new TimeSpan(0, 0, XConfig.Current.Database.ClientTimeout);
        private static bool IsFirstTimeInit { get; set; } = true;

        public static bool Connected => socketClient.Connected;
        private static ConcurrentDictionary<string, string> _messages { get; set; } = new ConcurrentDictionary<string, string>();
        public static void KillConnection()
        {
            IsShutdownStarted = true;
            ReceiverThread = null;
            DataBaseConnectionMaintainer = null;
            socketClient.CloseAndDispose();
            stream.CloseAndDispose();
        }
        public static bool Initialise(IPAddress ServerIP, int Port)
        {
            if (IsShutdownStarted) return false;
            socketClient = new TcpClient();
            remoteEndpoint = new IPEndPoint(ServerIP, Port);
            int FailedRetry = XConfig.Current.Database.FailedRetryTime;
            for (int i = 0; i < FailedRetry; i++)
            {
                try
                {
                    socketClient.Connect(remoteEndpoint);
                    stream = socketClient.GetStream();
                    L.I("Database Connection Estabilished!");
                    if (IsFirstTimeInit)
                    {
                        ReceiverThread.Start();
                        DataBaseConnectionMaintainer.Start();
                        IsFirstTimeInit = false;
                    }
                    SendCommand("openConnection", "00000", out string token);
                    L.I("Database Connected! Identity: " + token);
                    return true;
                }
                catch (Exception ex)
                {
                    L.E("Database connection to server: " + ServerIP + " failed. ");
                    ex.LogException();
                    Thread.Sleep(1000);
                }
            }
            return false;
        }

        // 接收服务端发来信息的方法
        static void Recv()
        {
            while (!IsShutdownStarted)
            {
                while (Connected && !IsShutdownStarted)
                {
                    try
                    {
                        string requestString = PublicTools.DecodeDatabasePacket(stream);
                        _messages.TryAdd(requestString.Substring(0, 5), requestString.Substring(5));
                    }
                    catch (Exception ex) { Thread.Sleep(1000); ex.LogException(); }
                }
                while (!Connected && !IsShutdownStarted)
                {
                    L.E("Message Recieve waiting for connection......");
                    Thread.Sleep(500);
                }
            }
        }

        private static void Maintain()
        {
            while (!IsShutdownStarted)
            {
                try
                {
                    string _mid = Cryptography.RandomString(5, false);
                    byte[] packet = PublicTools.MakeDatabasePacket(_mid, "HeartBeat");

                    if (!CoreSend(packet, _mid, out string reply))
                        throw new Exception("CoreSend Error: Timeout");

                    Thread.Sleep(5000);
                }
                catch (Exception ex)
                {
                    if (ex is ThreadAbortException || ex is NullReferenceException) return;
                    ex.LogException();

                    socketClient.CloseAndDispose();
                    stream.CloseAndDispose();

                    Thread.Sleep(5000);
                    Initialise(remoteEndpoint.Address, remoteEndpoint.Port);
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
                L.E("Message Sent Waiting for connection....");
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
                if (_messages.TryRemove(MessageId, out rcvdMessage)) return true;
                if (_timeoutTime.Subtract(DateTime.Now).TotalMilliseconds <= 0)
                {
                    rcvdMessage = null;
                    return false;
                }
            }
        }
    }
}
