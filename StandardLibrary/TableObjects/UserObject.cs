﻿
using System.Collections.Generic;
using System.Drawing;

using WBPlatform.Database.IO;
using WBPlatform.StaticClasses;

namespace WBPlatform.TableObject
{
    public class UserObject : DataTableObject<UserObject>
    {
        public override string Table => TABLE_Gen_UserTable;
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RealName { get; set; }
        public string Sex { get; set; }

        private const string passwordHolder = "#######################";
        public UserGroup UserGroup;

        public string HeadImagePath { get; set; }
        public string PhoneNumber { get; set; }

        public List<string> ChildList { get; set; } = new List<string>();
        public List<string> ClassList { get; set; } = new List<string>();

        public PointF CurrentPoint { get; set; }
        public decimal Precision { get; set; }

        public override void ReadFields(DataBaseIO input)
        {
            base.ReadFields(input);
            UserName = input.GetString("Username");
            Password = input.GetString("Password");
            if (string.IsNullOrWhiteSpace(Password))
            {
                Password = passwordHolder;
            }
            Sex = input.GetString("Sex");

            UserGroup = new UserGroup(
                isAdmin: input.GetBool("isAdmin"),
                isTeacher: input.GetBool("isClassTeacher"),
                isBusManager: input.GetBool("isBusTeacher"),
                isParent: input.GetBool("isParent"));


            RealName = input.GetString("RealName");
            HeadImagePath = input.GetString("HeadImage");
            PhoneNumber = input.GetString("PhoneNumber");

            ClassList = input.GetList("ClassIDs");
            ChildList = input.GetList("ChildIDs");

            if (string.IsNullOrWhiteSpace(HeadImagePath))
            {
                HeadImagePath = "default.png";
            }
            CurrentPoint = new PointF(input.Get<float>("longitude"), input.Get<float>("latitude"));
            Precision = input.Get<decimal>("precision");
        }

        public override void WriteObject(DataBaseIO output, bool all)
        {
            base.WriteObject(output, all);
            output.Put("Username", UserName);
            output.Put("Password", Password == passwordHolder ? null : Password);
            output.Put("Sex", Sex);

            //output.Put("isAdmin", UserGroup.IsAdmin); //DISABLED DUE TO SECURTY ISSUE....
            output.Put("isClassTeacher", UserGroup.IsClassTeacher);
            output.Put("isBusTeacher", UserGroup.IsBusManager);
            output.Put("isParent", UserGroup.IsParent);

            output.Put("RealName", RealName);
            output.Put("HeadImage", HeadImagePath);
            output.Put("PhoneNumber", PhoneNumber);

            output.Put("ClassIDs", ClassList);
            output.Put("ChildIDs", ChildList);

            output.Put("longitude", CurrentPoint.X);
            output.Put("latitude", CurrentPoint.Y);
            output.Put("precision", Precision);
        }

        public UserObject SetDefault()
        {
            ObjectId = "0000000000";
            UserName = "UnknownUser";
            Password = "";
            RealName = "UnknownName";
            HeadImagePath = "default.png";
            UserGroup = new UserGroup(false, false, false, false);
            CurrentPoint = new PointF();
            Precision = 0;
            Sex = "M";
            PhoneNumber = "-----------";
            ClassList = new List<string>();
            ChildList = new List<string>();
            return this;
        }
        public string GetIdentifiableCode()
        {
            return string.Join("_", UserName, ObjectId);
        }

        public string GetFullIdentity() => string.Join("-", GetIdentifiableCode(), RealName);
        public static UserObject Default => new UserObject().SetDefault();
        public string GetClassIdString(string Splitter) => string.Join(Splitter, ClassList.ToArray());
        public string GetChildIdString(string Splitter) => string.Join(Splitter, ChildList.ToArray());
    }
}