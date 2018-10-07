using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using WBPlatform.StaticClasses;

namespace WBPlatform.TableObject
{
    public class NotificationObject : DataTableObject<NotificationObject>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        [ForeignKey("SenderID")]
        public UserObject Sender { get; set; }
        public NotificationType Type { get; set; }
        public string Receivers
        {
            get { return string.Join(",", ReceiversList); }
            set { ReceiversList = new List<string>(value.Split(",")); }
        }
        [NotMapped]
        public List<string> ReceiversList { get; set; }

        public string GetStringRecivers()
        {
            string recv = "";
            if (ReceiversList[0].StartsWith("@all")) recv = "@all;";
            else foreach (string item in ReceiversList) recv = recv + item + ";";
            return recv.EndsWith(";;") ? recv.Substring(0, recv.Length - 1) : recv;
        }
    }
}