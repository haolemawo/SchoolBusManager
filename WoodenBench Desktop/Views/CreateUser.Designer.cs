﻿namespace WoodenBench.Views
{
    partial class CreateUserWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateUserWindow));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.CheckT = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.UserNameT = new System.Windows.Forms.TextBox();
            this.PasswordT = new System.Windows.Forms.TextBox();
            this.PasswordT2 = new System.Windows.Forms.TextBox();
            this.GroupT = new System.Windows.Forms.ComboBox();
            this.ResultLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.RealNameT = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(48, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "用户名";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(60, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "密码";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(12, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "重复你的密码";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(14, 124);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 17);
            this.label4.TabIndex = 3;
            this.label4.Text = "用户所属学部";
            // 
            // CheckT
            // 
            this.CheckT.AutoSize = true;
            this.CheckT.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CheckT.Location = new System.Drawing.Point(38, 175);
            this.CheckT.Name = "CheckT";
            this.CheckT.Size = new System.Drawing.Size(207, 55);
            this.CheckT.TabIndex = 7;
            this.CheckT.Text = "我已经同意使用小板凳，\r\n任何使用中的误操作所造成的后果\r\n均由用户（我）承担";
            this.CheckT.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.ForeColor = System.Drawing.Color.Red;
            this.button1.Location = new System.Drawing.Point(62, 333);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(159, 35);
            this.button1.TabIndex = 8;
            this.button1.Text = "确认，创建帐户";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // UserNameT
            // 
            this.UserNameT.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.UserNameT.Location = new System.Drawing.Point(100, 9);
            this.UserNameT.Name = "UserNameT";
            this.UserNameT.Size = new System.Drawing.Size(145, 23);
            this.UserNameT.TabIndex = 0;
            // 
            // PasswordT
            // 
            this.PasswordT.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.PasswordT.Location = new System.Drawing.Point(100, 63);
            this.PasswordT.Name = "PasswordT";
            this.PasswordT.PasswordChar = '●';
            this.PasswordT.Size = new System.Drawing.Size(145, 23);
            this.PasswordT.TabIndex = 2;
            // 
            // PasswordT2
            // 
            this.PasswordT2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.PasswordT2.Location = new System.Drawing.Point(100, 90);
            this.PasswordT2.Name = "PasswordT2";
            this.PasswordT2.PasswordChar = '●';
            this.PasswordT2.Size = new System.Drawing.Size(145, 23);
            this.PasswordT2.TabIndex = 3;
            // 
            // GroupT
            // 
            this.GroupT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GroupT.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GroupT.FormattingEnabled = true;
            this.GroupT.Items.AddRange(new object[] {
            "老师",
            "高层管理",
            "家长"});
            this.GroupT.Location = new System.Drawing.Point(100, 119);
            this.GroupT.Name = "GroupT";
            this.GroupT.Size = new System.Drawing.Size(145, 25);
            this.GroupT.TabIndex = 4;
            // 
            // ResultLabel
            // 
            this.ResultLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ResultLabel.Location = new System.Drawing.Point(12, 249);
            this.ResultLabel.Name = "ResultLabel";
            this.ResultLabel.Size = new System.Drawing.Size(247, 46);
            this.ResultLabel.TabIndex = 15;
            this.ResultLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(35, 39);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 17);
            this.label5.TabIndex = 0;
            this.label5.Text = "真实姓名";
            // 
            // RealNameT
            // 
            this.RealNameT.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.RealNameT.Location = new System.Drawing.Point(100, 36);
            this.RealNameT.Name = "RealNameT";
            this.RealNameT.Size = new System.Drawing.Size(145, 23);
            this.RealNameT.TabIndex = 1;
            // 
            // CreateUser
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(271, 380);
            this.Controls.Add(this.ResultLabel);
            this.Controls.Add(this.GroupT);
            this.Controls.Add(this.PasswordT2);
            this.Controls.Add(this.PasswordT);
            this.Controls.Add(this.RealNameT);
            this.Controls.Add(this.UserNameT);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.CheckT);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(287, 419);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(287, 419);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(287, 384);
            this.Name = "CreateUser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "创建用户";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CreateUser_FormClosed);
            this.Load += new System.EventHandler(this.CreateUser_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox CheckT;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox UserNameT;
        private System.Windows.Forms.TextBox PasswordT;
        private System.Windows.Forms.TextBox PasswordT2;
        private System.Windows.Forms.ComboBox GroupT;
        private System.Windows.Forms.Label ResultLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox RealNameT;
    }
}