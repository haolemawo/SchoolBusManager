﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using WBPlatform.Logging;
using System.Threading;
using WBPlatform.Database;
using WBPlatform.StaticClasses;
using WBPlatform.Config;
using WBPlatform.TableObject;
using System.Collections.Concurrent;

namespace WBPlatform.WebManagement.Tools
{
    public static partial class WeChatMessageSystem
    {
        private static ConcurrentQueue<WeChatRcvdMessage> RcvdMessageList { get; set; } = new ConcurrentQueue<WeChatRcvdMessage>();
        private static Thread ProcessorRCVDThread = new Thread(new ThreadStart(_ProcessRCVD));
        public static (bool, bool, int, int) Status()
        {
            return (ProcessorRCVDThread.IsAlive, ProcessorSENTThread.IsAlive, RcvdMessageList.Count, SentMessageList.Count);
        }

        public static void StartProcessThreads()
        {
            ProcessorSENTThread.Start();
            L.I("WeChatSendThread Started!");
            ProcessorRCVDThread.Start();
            L.I("WeChatRcvdThread Started!");
        }

        public static void AddToRecvList(WeChatRcvdMessage _Message)
        {
            RcvdMessageList.Enqueue(_Message);
        }

        private static Dictionary<string, string> ResponceToMessage(WeChatRcvdMessage Message)
        {
            switch (Message.MessageType)
            {
                case WeChatRMsg.text:
                    return Message.TextContent.Contains("love") || Message.TextContent.Contains("爱")
                        ? SendMessageString(WeChatSMsg.text, Message.FromUser, null, "I Love you!")
                        : SendMessageString(WeChatSMsg.text, Message.FromUser, null, XConfig.Messages["DefaultReply_Text"] + Message.TextContent + "??");
                case WeChatRMsg.image:
                    return SendMessageString(WeChatSMsg.text, Message.FromUser, null, XConfig.Messages["DefaultReply_Image"]);
                case WeChatRMsg.voice:
                    return SendMessageString(WeChatSMsg.text, Message.FromUser, null, XConfig.Messages["DefaultReply_Voice"]);
                case WeChatRMsg.location:
                    return SendMessageString(WeChatSMsg.text, Message.FromUser, null, XConfig.Messages["DefaultReply_Location"]);
                case WeChatRMsg.video:
                    return SendMessageString(WeChatSMsg.text, Message.FromUser, null, XConfig.Messages["DefaultReply_Video"]);
                case WeChatRMsg.link:
                    return SendMessageString(WeChatSMsg.text, Message.FromUser, null, XConfig.Messages["DefaultReply_WebLink"]);
                case WeChatRMsg.EVENT:
                    switch (Message.Event)
                    {
                        case WeChatEvent.click:
                            switch (Message.EventKey)
                            {
                                //case "ADD_PASSWORD":
                                //    string token = OnePassTicket.CreateTicket();
                                //    if (OnePassTicket.TryAdd(token, new TicketInfo(TicketUsage.AddPassword, "JumpToken_FreeLogin", Message.FromUser)))
                                //    {
                                //        //"要是想使用Windows 客户端登陆的话\r\n就点击<a href='" + XConfig.CurrentConfig.WebSiteAddress + "/Account/Register/?token={0}&_action=AddPassword&user={1}'>这里</a>给自己加一个密码吧!"
                                //        string content = string.Format(XConfig.Messages["AddPasswordMessage"], token, Message.FromUser);
                                //        var p = SendMessageString(WeChatSMsg.text, Message.FromUser, null, content);
                                //        return p;
                                //    }
                                //    else
                                //    {
                                //        return SendMessageString(WeChatSMsg.text, Message.FromUser, null, XConfig.Messages["CreateTokenError"]);
                                //    }
                                case "WEB_SERV_VER":
                                    return SendMessageString(WeChatSMsg.textcard, Message.FromUser,
                                        "小板凳平台版本信息",
                                        "这是当前版本信息: \r\n" +
                                        "启动の时间: " + Program.StartUpTime.ToString() + "\r\n\r\n" +
                                        "服务端版本: " + Program.Version + "\r\n" +
                                        "核心库版本: " + WBConsts.CoreVersion + "\r\n" +
                                        "运行时版本: " + Assembly.GetCallingAssembly().ImageRuntimeVersion + "\r\n\r\n=点击查看系统状态>>", XConfig.Current.StatusPageAddress);
                                default:
                                    L.E("Recieved Not Supported :::Wechat Event Click::: Key " + Message.EventKey);
                                    return null;
                            }
                        case WeChatEvent.LOCATION:
                            var Latitude = Message.Location.X;
                            var Longitude = Message.Location.Y;
                            var Precision = Message.Precision;
                            var time = Message.RecievedTime;
                            var toUser = Message.FromUser;
                            L.I($"Recieved Location: {Latitude}:{Longitude}%{Precision}@{time} form {toUser}");
                            if (DataBaseOperation.QuerySingle(new DBQuery().WhereEqualTo("Username", toUser), out UserObject _user) == DBQueryStatus.ONE_RESULT)
                            {
                                _user.CurrentPoint = Message.Location;
                                _user.Precision = Precision;
                                if (DataBaseOperation.UpdateData(ref _user) != DBQueryStatus.ONE_RESULT)
                                    L.E("Cannot save User with new Position...");
                                else L.I("Succeed Save User with New Position...");
                            }
                            else L.E("Cannot find user in Database....");

                            return null;
                    }
                    return null;
            }
            return SendMessageString(WeChatSMsg.text, Message.FromUser, null, "不支持的消息类型");
        }

        private static void _ProcessRCVD()
        {
            while (true)
            {
                if (RcvdMessageList.TryDequeue(out WeChatRcvdMessage message)) ResponceToMessage(message);
                else Thread.Sleep(500);
            }
        }
    }
}
