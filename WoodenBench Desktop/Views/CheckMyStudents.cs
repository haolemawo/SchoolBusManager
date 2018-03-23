using cn.bmob.io;
using cn.bmob.response;
using DevComponents.DotNetBar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WBServicePlatform.StaticClasses;
using WBServicePlatform.TableObject;
using static WBServicePlatform.WinClient.StaticClasses.GlobalFunc;

namespace WBServicePlatform.WinClient.Views
{
    public partial class CheckMyStudents : DevComponents.DotNetBar.Metro.MetroForm
    {
        public CheckMyStudents()
        {
            InitializeComponent();
            if (defaultInstance == null) defaultInstance = this;
        }
        #region For us easier to call
        private static CheckMyStudents defaultInstance { get; set; }
        static void DefaultInstance_FormClosed(object sender, FormClosedEventArgs e)
        {
            defaultInstance = null;
        }
        public static CheckMyStudents Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    defaultInstance = new CheckMyStudents();
                    defaultInstance.FormClosed += new FormClosedEventHandler(DefaultInstance_FormClosed);
                }
                return defaultInstance;
            }
        }
        #endregion

        private void CheckMyStudents_Shown(object sender, EventArgs e)
        {
            if (CurrentUser.UserGroup.IsBusManager)
            {
                Text = Text + " - �Ѿ����ñ༭";
                SchoolBusObject busObject = new SchoolBusObject();
                BmobQuery query = new BmobQuery();
                query.WhereEqualTo("TeacherObjectID", CurrentUser.objectId);
                Task<QueryCallbackData<SchoolBusObject>> task;
                task = _BmobWin.FindTaskAsync<SchoolBusObject>(WBConsts.TABLE_N_Mgr_BusData, query);
                task.Wait();
                if (task.Result.results.Count <= 0)
                {
                    MessageBox.Show("�Ҳ����κ��������У��");
                }
                else if (task.Result.results.Count == 1)
                {
                    busObject = task.Result.results[0];
                }
                else
                {
                    MessageBox.Show("�ҵ��˶������󶨵�У��(�ⲻ���ܡ���)��Ŀǰֻ����ʾ���е�һ��");
                    busObject = task.Result.results[0];
                }
                myID.Text = busObject.objectId;
                myDirection.Text = busObject.BusName;
                LeavingChecked.Text = busObject.LSChecked.ToString();
                BackChecked.Text = busObject.CSChecked.ToString();
                ExpNumber.Text = "��δ����";
                BackNumber.Text = "��δ����";
                ExDescription.Text = "�������";
                Application.DoEvents();
                StudentDataGrid.AutoResizeColumns();
            }
            else if (CurrentUser.UserGroup.IsAdmin)
            {
                Text = Text + " - �Ѿ����ñ༭";
            }
            else
            {
                MessageBox.Show("�㲻��У����ʦ��ֻ��У����ʦ�͹���Ա�ܱ༭����", "ֻ��ģʽ", MessageBoxButtons.OK);
                Text = Text + " - ֻ��ģʽ";
            }
        }

        private void CheckMyStudents_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainForm.Default.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void teacherBasicData_Enter(object sender, EventArgs e)
        {

        }

        private void LoadAll_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("����ȫ�����ݿ�����Ҫһ��ʱ�䣬�⽫ȡ������������ٶ�", "����ʱ��������", MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return;
            }
            studentDataObjectBindingSource.Clear();
            BmobQuery query = new BmobQuery();
            query.WhereEqualTo("BusID", myID.Text);
            Task<QueryCallbackData<UserObject>> task;
            task = _BmobWin.FindTaskAsync<UserObject>(WBConsts.TABLE_N_Mgr_StuData, query);
            task.Wait();
            if (task.IsCompleted)
            {
                List<UserObject> list = task.Result.results;
                foreach (UserObject item in list)
                {
                    studentDataObjectBindingSource.Add(item);
                }
                ExpNumber.Text = list.Count.ToString();
                LeaveNumber.Text = CountTicks(4).ToString();
                LeavingChecked.Text = CountTicks(5).ToString();
                BackNumber.Text = CountTicks(6).ToString();
                BackChecked.Text = CountTicks(7).ToString();
            }
        }

        private void CheckMyStudents_Load(object sender, EventArgs e)
        {

        }

        private void StudentDataGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            LeaveNumber.Text = CountTicks(4).ToString();
            LeavingChecked.Text = CountTicks(5).ToString();
            BackNumber.Text = CountTicks(6).ToString();
            BackChecked.Text = CountTicks(7).ToString();
        }

        private int CountTicks(int ColNum)
        {
            int Val = 0;
            foreach (DataGridViewRow row in StudentDataGrid.Rows)
            {
                if (Convert.ToBoolean(row.Cells[ColNum].Value)) Val++;
            }
            return Val;
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            foreach (StudentObject item in studentDataObjectBindingSource)
            {
                Task<UpdateCallbackData> task = _BmobWin.UpdateTaskAsync<StudentObject>(item);
                task.Wait();
                ExDescription.Text = "�ɹ������" + item.StudentName;
                Application.DoEvents();
            }
        }
    }
}