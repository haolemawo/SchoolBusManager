using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using WBPlatform.Config;
using WBPlatform.Database.IO;
using WBPlatform.Logging;
using WBPlatform.StaticClasses;

namespace WBPlatform.Database.DBServer
{
    public class DatabaseSocketsServer
    {
        static Socket socketwatch = null;
        //定义一个集合，存储客户端信息
        //static Dictionary<string, Socket> clientConnectionItems { get; set; } = new Dictionary<string, Socket>();
        public static ConcurrentDictionary<string, string> QueryStrings { get; set; } = new ConcurrentDictionary<string, string>();
        public static void InitialiseSockets()
        {
            socketwatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint point = new IPEndPoint(IPAddress.Any, XConfig.Current.Database.DBServerPort);
            socketwatch.Bind(point);
            socketwatch.Listen(20);

            //负责监听客户端的线程:创建一个监听线程  
            Thread threadwatch = new Thread(WatchConnecting) { IsBackground = true };
            threadwatch.Name = "DBServer Connection Watching Thread";
            threadwatch.Start();
        }

        //监听客户端发来的请求  
        private static void WatchConnecting()
        {
            while (true)
            {
                Socket connection = null;
                try
                {
                    connection = socketwatch.Accept();
                }
                catch (Exception ex)
                {
                    LW.E(ex.Message);
                    continue;
                }

                string remoteEndPoint = connection.RemoteEndPoint.ToString();
                LW.I("Estalished a connection with " + remoteEndPoint);

                Thread thread = new Thread(new ParameterizedThreadStart(Recv))
                {
                    IsBackground = true,
                    Name = remoteEndPoint + " Worker Thread"
                };
                thread.Start(connection);
            }
        }

        private static void Recv(object socketclientpara)
        {
            string val;
            Socket baseSocket = socketclientpara as Socket;
            NetworkStream stream = new NetworkStream(baseSocket);
            string remoteEP = baseSocket.RemoteEndPoint.ToString();
            bool _connectionOpened = false;
            while (true)
            {
                try
                {
                    string requestString = PublicTools.DecodeDatabasePacket(stream);
                    LW.I("Recived Data: " + requestString);
                    if (requestString.Length <= 5)
                    {
                        baseSocket.CloseAndDispose();
                        break;
                    }

                    string _MessageId = requestString.Substring(0, 5);
                    requestString = requestString.Substring(5);

                    if (requestString == "openConnection")
                    {
                        LW.I("C: Recieve an OpenConnection Request, from " + remoteEP);
                        byte[] arrSendMsg = PublicTools.MakeDatabasePacket(_MessageId, remoteEP);
                        stream.Write(arrSendMsg, 0, arrSendMsg.Length);
                        stream.Flush();
                        LW.I("C: Replied an OpenConnection Request, to " + remoteEP);
                        _connectionOpened = true;
                    }
                    else if (_connectionOpened)
                    {
                        if (requestString == "HeartBeat")
                        {
                            LW.I("B: Recieve a HearBeat, from " + remoteEP);
                            DateTime rtime = DateTime.Now;
                            byte[] arrSendMsg = PublicTools.MakeDatabasePacket(_MessageId, rtime.ToNormalString());
                            stream.Write(arrSendMsg, 0, arrSendMsg.Length);
                            stream.Flush();
                            LW.I("C: Replied a HearBeat, to " + remoteEP);
                        }
                        else if (requestString.ToParsedObject(out DataBaseSocketIO request))
                        {
                            QueryStrings[remoteEP] = requestString;
                            LW.I("Q: " + remoteEP + " :: " + requestString);
                            //It takes Time.....
                            string returnStr = DatabaseCore.ProcessRequest(request);
                            byte[] arrSendMsg = PublicTools.MakeDatabasePacket(_MessageId, returnStr);
                            stream.Write(arrSendMsg, 0, arrSendMsg.Length);
                            stream.Flush();
                            LW.I("P: " + remoteEP + " :: " + returnStr);
                        }
                        else
                        {
                            //Invalid Connection......
                            LW.E("E: " + remoteEP + " :: JSON Parse Exception!");
                            baseSocket.CloseAndDispose();
                            QueryStrings.TryRemove(remoteEP, out val);
                            break;
                        }
                    }
                    else
                    {
                        LW.E("Connection to " + remoteEP + " is not marked as 'Opened'");
                        baseSocket.CloseAndDispose();
                        stream.CloseAndDispose();
                        QueryStrings.TryRemove(remoteEP, out val);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    LW.E("Client " + remoteEP + " drops the connection. " + "\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n");
                    QueryStrings.TryRemove(remoteEP, out val);
                    stream.CloseAndDispose();
                    baseSocket.CloseAndDispose();
                    break;
                }
            }
            QueryStrings.TryRemove(remoteEP, out val);
            LW.E("Client Connection Socket to " + remoteEP + " gonna Stop!");
            Thread.CurrentThread.Abort();
            return;
        }
    }
}
