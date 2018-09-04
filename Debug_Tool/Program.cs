using System;
using System.Collections.Generic;

using WBPlatform.Config;
using WBPlatform.Database;
using WBPlatform.Logging;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;

namespace Debug_Tool
{
    public static class Program
    {
        static void Main(string[] args)
        {
            L.SetLogLevel(LogLevel.DBG);
            L.InitLog();
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
            L.D(DataBaseOperation.CreateData(ref me).ToParsedString());
            //L.D(DataBaseOperation.QuerySingle(new DBQuery().WhereEqualTo("UserName", "liuhaoyu"), out UserObject me).ToString());
            L.D(me.ToParsedString());

            string[] dePartMent = { "小学", "初中", "普高", "中加", "剑桥" };

            List<ClassObject> classList = new List<ClassObject>();
            List<SchoolBusObject> busList = new List<SchoolBusObject>();
            for (int i = 1; i < (8 * 200) + 1; i++)
            {
                ClassObject @class = new ClassObject()
                {
                    CDepartment = dePartMent[RandomInt(0, 3)],
                    CGrade = RandomInt(1, 14) + "年级",
                    CNumber = RandomInt(1, 8) + "班-" + DateTime.Now.AsUnixTimeStamp()
                };
                L.D(DataBaseOperation.CreateData(ref @class).ToString());
                L.D(@class.ToParsedString());
                classList.Add(@class);
            }


            for (int i = 1; i < 161; i++)
            {
                SchoolBusObject bo = new SchoolBusObject()
                {
                    BusName = "班车方向" + i,
                    TeacherID = me.ObjectId
                };
                L.D(DataBaseOperation.CreateData(ref bo).ToString());
                L.D(bo.ToParsedString());
                busList.Add(bo);
            }



            for (int cn = 1; cn < 3000; cn++)
            {
                StudentObject stu = new StudentObject()
                {
                    BusID = busList[RandomInt(0, busList.Count)].ObjectId,
                    ClassID = classList[RandomInt(0, classList.Count)].ObjectId,
                    Sex = RandomBool ? "M" : "F",
                    StudentName = "Stu-" + cn.ToString("000"),
                    AHChecked = false,
                    CSChecked = false,
                    LSChecked = false
                };
                L.D(DataBaseOperation.CreateData(ref stu).ToString());

                L.D(stu.ToParsedString());
                if (RandomBool)
                {
                    if (stu != null)
                    {
                        me.ChildList.Add(stu.ObjectId);
                    }
                }
                L.I(cn.ToParsedString());
            }

            me.HeadImagePath = "liuhaoyu.gif";
            L.D(DataBaseOperation.UpdateData(ref me).ToString());
            L.D(me.ToParsedString());
        }
        public static bool RandomBool => new Random().Next(0, 2) == 1;
        public static int RandomInt(int min, int max) => new Random().Next(min, max);
    }
}
