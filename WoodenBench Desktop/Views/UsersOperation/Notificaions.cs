using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Linq;
using DevComponents.DotNetBar.Metro;

using WBPlatform.Database;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;
using WBPlatform.DesktopClient.Users;

using static WBPlatform.DesktopClient.StaticClasses.CurrentInstance;

namespace WBPlatform.DesktopClient.Views
{
    public partial class NotificationsForm : MetroForm
    {
        private List<NotificationObject> NotificationLists { get; set; } = new List<NotificationObject>();
        public NotificationsForm()
        {
            InitializeComponent();
            if (defaultInstance == null) defaultInstance = this;
        }
        #region For us easier to call
        private static NotificationsForm defaultInstance { get; set; }
        static void DefaultInstance_FormClosed(object sender, FormClosedEventArgs e)
        {
            defaultInstance = null;
        }
        public static NotificationsForm Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    defaultInstance = new NotificationsForm();
                    defaultInstance.FormClosed += new FormClosedEventHandler(DefaultInstance_FormClosed);
                }
                return defaultInstance;
            }
        }
        #endregion

        private void BusesManager_Load(object sender, EventArgs e)
        {

        }

        private void BusesManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainForm.Default.Show();
        }

        private void loadMessage_Click(object sender, EventArgs e)
        {
            if (Database.DataBaseOperation.QueryMultipleData(new DBQuery(), out List<NotificationObject> list) >= 0)
            {
                listView1.Items.Clear();
                NotificationLists.Clear();
                foreach (NotificationObject item in list)
                {
                    NotificationLists.Add(item);
                    listView1.Items.Add(new ListViewItem(new string[] { item.Sender, item.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"), new string(item.Content.Take(20).ToArray()) + "...." }));
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                NotificationObject @object = NotificationLists[listView1.SelectedItems[0].Index];
                msgTitle.Text = @object.Title;
                msgTime.Text = @object.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                msgType.Text = @object.Type.ToString();
                msgSendID.Text = @object.Sender;
                msgRecvID.Text = @object.GetStringRecivers();
                msgContent.Text = @object.Content;
            }
        }

        private void copyMessage_Click(object sender, EventArgs e)
        {
            string clip = $"" +
                $"信息标题：{msgTitle.Text} \r\n" +
                $"发送者：{msgSendID.Text} \r\n" +
                $"接收者：{msgRecvID.Text}\r\n" +
                $"发送时间：{msgTime.Text}\r\n" +
                $"消息内容：\r\n" +
                $"{msgContent.Text}";
            //Clipboard.SetText(clip);
            copyFrame.Text = clip;
            copyFrame.Copy();
            Clipboard.SetDataObject(clip, false, 10, 500);
        }
    }
}