using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

using WBPlatform.Config;
using WBPlatform.Logging;
using WBPlatform.StaticClasses;

namespace WBPlatform.WebManagement.Tools
{
    public static partial class WeChatMessageSystem
    {
        private static ConcurrentQueue<WeChatSentMessage> SentMessageList { get; set; } = new ConcurrentQueue<WeChatSentMessage>();
        private static Thread ProcessorSENTThread = new Thread(new ThreadStart(_ProcessSENT));

        public static void AddToSendList(WeChatSentMessage message) { SentMessageList.Enqueue(message); }
        private static void _ProcessSENT()
        {
            while (true)
            {
                if (SentMessageList.TryDequeue(out WeChatSentMessage message)) SendMessagePacket(message);
                else Thread.Sleep(500);
            }
        }
        private static Dictionary<string, string> SendMessagePacket(WeChatSentMessage message)
        {
            string users = "";
            foreach (string item in message.toUser)
            {
                users = users + item + "|";
            }
            return SendMessageString(message.type, users, message.Title, message.Content, message.URL_OnClick);
        }

        private static Dictionary<string, string> SendMessageString(WeChatSMsg MessageType, string users, string Title, string Content, string URL = null)
        {
            WeChatMessageBackupService.AddToSendList(users, Title, Content);
            WeChatHelper.ReNewWCCodes();
            string Message = "{\"touser\":\"" + users + "\",\"msgtype\":\"" + MessageType.ToString() + "\",\"agentid\":" + XConfig.Current.WeChat.AgentId + ",\"" + MessageType.ToString() + "\":";
            switch (MessageType)
            {
                case WeChatSMsg.text:
                    Message = Message + $"{{\"content\":\"{Content}\r\n\r\nMST: {DateTime.Now.ToNormalString()}\"}}";
                    break;
                case WeChatSMsg.textcard:
                    Message = Message + $"{{\"title\":\"{Title}\",\"description\":\"{Content}\",\"url\":\"{URL}\"}}";
                    break;
                case WeChatSMsg.file:
                    Message = Message + $"{{\"media_id\":\"{Content}\"}}";
                    break;
            }
            Message = Message + "}";
            L.I("WeChat Message Sent: " + Message);
            return PublicTools.HTTPPost("https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=" + WeChatHelper.AccessToken, Message);
        }
    }
}
