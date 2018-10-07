using System.Collections.Generic;
using System.Linq;
using System.Threading;

using WBPlatform.Database;
using WBPlatform.Logging;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;

namespace WBPlatform.WebManagement.Tools
{
    public class WeChatMessageBackupService
    {
        public static int GetCount { get => NotificationList.Count; }
        public static bool GetStatus { get => NotificationBackupThread.IsAlive; }
        private static Thread NotificationBackupThread = new Thread(new ThreadStart(_Process));
        private static List<NotificationObject> NotificationList { get; set; } = new List<NotificationObject>();
        public static void StartBackupThread()
        {
            NotificationBackupThread.Start();
            L.I("\tNotificationBackupThread Started!");
        }

        public static void AddToSendList(string users, string Title, string Content)
        {
            // If is @all defaultly set it to Broadcast, else ClientToClient
            NotificationType _type = users == "@all" ? NotificationType.WeChatBroadCast : NotificationType.WeChatC2C;

            // If is broadcast, set "@all" into the list., else, convert users into a list.
            List<string> targetUsers = _type == NotificationType.WeChatBroadCast ? new List<string>() { "@all" } : users.Split(';').ToList();

            // If reciver is larger than 1; set tp multicast....
            _type = targetUsers.Count > 1 ? NotificationType.WeChatMultiCast : _type;

            NotificationObject notification = new NotificationObject()
            {
                Content = Content ?? "",
                ReceiversList = targetUsers,
                Sender = null,
                Title = Title ?? "",
                Type = _type
            };
            lock (NotificationList) { NotificationList.Add(notification); }
        }

        private static void _Process()
        {
            NotificationObject message;
            while (true)
            {
                lock (NotificationList)
                {
                    if (NotificationList.Count != 0)
                    {
                        message = NotificationList[NotificationList.Count - 1];
                        NotificationList.Remove(NotificationList.Last());
                    }
                    else message = null;
                }
                if (message != null) DataBaseOperation.CreateData(ref message);
                else Thread.Sleep(1000);
            }
        }
    }
}
