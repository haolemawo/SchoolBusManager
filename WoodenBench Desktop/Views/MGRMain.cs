﻿using cn.bmob.io;
using DevComponents.DotNetBar.Metro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WoodenBench.StaticClasses;
using WoodenBench.Users;
using WoodenBench.Views;
using static WoodenBench.StaticClasses.GlobalFunc;

namespace WoodenBench.Views
{
    public partial class ManagementWindow : MetroForm
    {
        int UsrLevel;
        public ManagementWindow(int UserLevel) : base()
        {
            InitializeComponent();
            UsrLevel = UserLevel;
        }

        private void Management_Load(object sender, EventArgs e)
        {
            tableLayoutPanel1.BackColor = Color.White;
        }

        private void Btn_UpdateWinNoti_Click(object sender, EventArgs e)
        {

        }

        private void Btn_UpdateWebNoti_Click(object sender, EventArgs e)
        {

        }

        private void allUserObjectBindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {

        }

        private void SearchBtn_Click(object sender, EventArgs e)
        {
            bool IsGoing = true;
            int CurrNum = 0;
            allUserObjectBindingSource.Clear();
            BmobQuery query = new BmobQuery();


            if (ContentTxBox.Text.ToLower() == "true")
            {
                query.WhereContainedIn<bool>(ColNameTx.Text, true);

            }
            else if (ContentTxBox.Text.ToLower() == "false")
            {
                query.WhereContainedIn<bool>(ColNameTx.Text, false);
            }
            else
            {
                query.WhereContainedIn<string>(ColNameTx.Text, ContentTxBox.Text);
            }
            //query.Skip(CurrNum);
            var future = _BmobWin.FindTaskAsync<AllUserObject>(Consts.TABLE_N_Gen_UsrTable, query);
            future.Wait();
            if (future.IsFaulted)
            {
                MessageBox.Show("Failed getting data");
            }
            if (future.IsCompleted)
            {
                List<AllUserObject> list = future.Result.results;
                foreach (AllUserObject item in list)
                {
                    allUserObjectBindingSource.Add(item);
                }
            }
        }

        private void radialMenu2_ItemClick(object sender, EventArgs e)
        {

        }

        private void dataGridViewX1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }
    }
}
