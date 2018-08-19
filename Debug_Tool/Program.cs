using System;
using System.Diagnostics;
using System.Net;
using WBPlatform.Database;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;
using WBPlatform.Logging;

namespace Debug_Tool
{
    class Program
    {
        static void Main(string[] args)
        {
            LW.InitLog();
            LW.SetLogLevel(LogLevel.I);
            DataBaseOperation.InitialiseClient();

            LW.D(DataBaseOperation.QuerySingleData(new DBQuery().WhereEqualTo("realname", "刘浩宇"), out UserObject me).ToString());
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
                BusName = "校车方向1",
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
                if (cn < 21)
                {
                    me.ChildList.Add(stu.ObjectId);
                }
            }

            me.HeadImagePath = "liuhaoyu.gif";
            LW.D(DataBaseOperation.UpdateData(ref me).ToString());
            LW.D(me.ToParsedString());
        }
    }
}
