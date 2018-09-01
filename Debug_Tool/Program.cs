using System;
using System.Diagnostics;
using System.Net;
using WBPlatform.Database;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;
using WBPlatform.Logging;
using WBPlatform.Config;

namespace Debug_Tool
{
    class Program
    {
        static void Main(string[] args)
        {
            LW.SetLogLevel(LogLevel.D);
            LW.InitLog();
            XConfig.LoadConfig("XConfig.conf");
            DataBaseOperation.InitialiseClient();
            UserObject me = new UserObject()
            {
                HeadImagePath = "liuhaoyu.gif",
                Password = "",
                PhoneNumber = "18632738306",
                Sex = "M",
                UserGroup = new UserGroup(true, true, true, true),
                UserName = "liuhaoyu",
                Precision = 3,
                RealName = "刘浩宇",
                CurrentPoint = new System.Drawing.PointF(0, 0)
            };
            LW.D(DataBaseOperation.CreateData(ref me).ToParsedString());
            //LW.D(DataBaseOperation.QuerySingleData(new DBQuery().WhereEqualTo("realname", "刘浩宇"), out UserObject me).ToString());
            LW.D(me.ToParsedString());
            ClassObject co = new ClassObject()
            {
                CDepartment = "学部",
                CGrade = "1年级",
                CNumber = "5班",
                TeacherID = me.ObjectId
            };
            LW.D(DataBaseOperation.CreateData(ref co).ToString());
            me.ClassList.Add(co.ObjectId);
            LW.D(co.ToParsedString());


            SchoolBusObject bo = new SchoolBusObject()
            {
                BusName = "班车方向1",
                TeacherID = me.ObjectId
            };
            LW.D(DataBaseOperation.CreateData(ref bo).ToString());

            LW.D(bo.ToParsedString());

            for (int cn = 1; cn < 40; cn++)
            {
                StudentObject stu = new StudentObject()
                {
                    BusID = bo.ObjectId,
                    ClassID = co.ObjectId,
                    Sex = "M",
                    StudentName = "学生-" + cn.ToString("000"),
                    AHChecked = false,
                    CSChecked = false,
                    LSChecked = false
                };
                LW.D(DataBaseOperation.CreateData(ref stu).ToString());

                LW.D(stu.ToParsedString());
                if (RandomBool)
                {
                    me.ChildList.Add(stu.ObjectId);
                }
            }

            me.HeadImagePath = "liuhaoyu.gif";
            LW.D(DataBaseOperation.UpdateData(ref me).ToString());
            LW.D(me.ToParsedString());
        }
        public static bool RandomBool => new Random().Next(0, 2) == 1;
    }
}
