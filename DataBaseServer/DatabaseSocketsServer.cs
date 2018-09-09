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
        public static ConcurrentDictionary<string, string> QueryStrings { get; private set; } = new ConcurrentDictionary<string, string>();
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
                    L.E(ex.Message);
                    continue;
                }

                string remoteEndPoint = connection.RemoteEndPoint.ToString();
                L.I("Estalished a connection with " + remoteEndPoint);

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
                    if (requestString.Length <= 5)
                    {
                        baseSocket.CloseAndDispose();
                        stream.CloseAndDispose();
                        L.W("Recived Data: " + requestString);
                        break;
                    }

                    string _MessageId = requestString.Substring(0, 5);
                    requestString = requestString.Substring(5);

                    if (requestString == "openConnection")
                    {
                        L.W("OC: Recieve an OpenConnection Request, from " + remoteEP);
                        byte[] arrSendMsg = PublicTools.MakeDatabasePacket(_MessageId, remoteEP);
                        stream.Write(arrSendMsg, 0, arrSendMsg.Length);
                        stream.Flush();
                        _connectionOpened = true;
                        L.W("OC: Replied an OpenConnection Request, to " + remoteEP);
                    }
                    else if (_connectionOpened)
                    {
                        if (requestString == "HeartBeat")
                        {
                            L.D("HB: Recieve a HearBeat, from " + remoteEP);
                            DateTime rtime = DateTime.Now;
                            byte[] arrSendMsg = PublicTools.MakeDatabasePacket(_MessageId, rtime.ToNormalString());
                            stream.Write(arrSendMsg, 0, arrSendMsg.Length);
                            stream.Flush();
                            L.D("HB: Replied a HearBeat, to " + remoteEP);
                        }
                        else if (requestString.ToParsedObject(out DataBaseSocketIO request))
                        {
                            QueryStrings[remoteEP] = requestString;
                            L.I("Q: " + remoteEP + " :: " + requestString);
                            //It takes Time.....
                            string returnStr = DatabaseCore.ProcessRequest(request);
                            byte[] arrSendMsg = PublicTools.MakeDatabasePacket(_MessageId, returnStr);
                            stream.Write(arrSendMsg, 0, arrSendMsg.Length);
                            stream.Flush();
                            L.I("Q: " + remoteEP + " :: " + returnStr);
                        }
                        else
                        {
                            //Invalid Connection......
                            baseSocket.CloseAndDispose();
                            QueryStrings.TryRemove(remoteEP, out val);
                            L.W("Recived Data: " + requestString);
                            L.E("E: " + remoteEP + " :: JSON Parse Exception!");
                            break;
                        }
                    }
                    else
                    {
                        baseSocket.CloseAndDispose();
                        stream.CloseAndDispose();
                        QueryStrings.TryRemove(remoteEP, out val);
                        L.W("Recived Data: " + requestString);
                        L.E("Connection to " + remoteEP + " is not marked as 'Opened'");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    stream.CloseAndDispose();
                    baseSocket.CloseAndDispose();
                    L.E("Client " + remoteEP + " drops the connection. ");
                    ex.LogException();
                    QueryStrings.TryRemove(remoteEP, out val);
                    break;
                }
            }
            QueryStrings.TryRemove(remoteEP, out val);
            L.E("Client Connection Socket to " + remoteEP + " gonna Stop!");
            return;
        }
    }
}
