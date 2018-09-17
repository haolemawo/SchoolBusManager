using System;
using System.Collections.Generic;

using WBPlatform.Config;
using WBPlatform.Database;
using WBPlatform.Database.Connection;
using WBPlatform.Database.IO;
using WBPlatform.Logging;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;

namespace Debug_Tool
{
    public static class Program
    {
        static void Main(string[] args)
        {
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
            L.D(DataBaseOperation.CreateData(ref me).Stringify());
            //L.D(DataBaseOperation.QuerySingle(new DBQuery().WhereEqualTo("UserName", "liuhaoyu"), out UserObject me).ToString());
            L.D(me.Stringify());

            string[] dePartMent = { "小学", "初中", "普高", "中加", "剑桥" };
            string[] namelist = { "钟天泽", "刑从珊", "牟绮南", "陈绮琴", "柯良俊", "伦腾骏", "闪安梦", "浑诗霜", "晁振华", "李易绿", "咎鸿宝", "士芳茵", "隗依晨", "宿德庸", "夏侯清嘉", "乜白亦", "出依波", "邬天青", "惠秋月", "次兴言", "支嘉珍", "枝承嗣", "濮阳亦绿", "革湛英", "韶琼思", "是觅晴", "抄念之", "泉觅翠", "道德元", "貊依丝", "邶芳春", "贺问梅", "蒉晨濡", "鞠德曜", "蔺暄文", "业英悟", "应芳泽", "苦飞双", "欧锦欣", "第五语梦", "悉晓燕", "保鸿畴", "乌孙思懿", "许天蓝", "亥乐水", "邱雅寒", "阿新蕾", "植叶欣", "图门昊伟", "万娅欣", "夕运凯", "高香芹", "夙成周", "狄访风", "无君昊", "温阳焱", "宋合乐", "苑梦蕊", "徭烨磊", "令狐晴岚", "佟佳湛芳", "赫连安珊", "郁阳曦", "迮姝美", "伏觅双", "苍正雅", "冼和硕", "平子珍", "子车晨潍", "危清婉", "九思松", "太叔冬灵", "宏宛亦", "错淑君", "奈明明", "冉盼夏", "嘉水冬", "建永福", "党瑾瑶", "信孤晴", "訾萧曼", "零向真", "风英韶", "后开畅", "凭桃雨", "苏高峯", "让绿蝶", "盛文林", "范绿兰", "施胤雅", "卓安萱", "辜元正", "肖自强", "舒畅然", "公良幻梅", "丹星纬", "堵博易", "虎坚秉", "甲玉泽", "孟竹月", "詹彩萱" };
            List<ClassObject> classList = new List<ClassObject>();
            List<SchoolBusObject> busList = new List<SchoolBusObject>();
            for (int i = 1; i < 16; i++)
            {
                ClassObject @class = new ClassObject()
                {
                    CDepartment = dePartMent[RandomInt(0, 3)],
                    CGrade = RandomInt(1, 14) + "年级",
                    CNumber = RandomInt(1, 8) + "班",
                    TeacherID = me.ObjectId
                };
                L.D(DataBaseOperation.CreateData(ref @class).ToString());
                L.D(@class.Stringify());
                classList.Add(@class);
            }

            string[] places = { "城镇", /*"莆陂", "城阜", "泉州", "上庄", "辰洞" */};
            foreach (var item in places)
            {
                SchoolBusObject bo = new SchoolBusObject()
                {
                    BusName = item,
                    TeacherID = me.ObjectId,
                    BigWeekOnly = RandomBool,
                    AHChecked = false,
                    CSChecked = false,
                    LSChecked = false
                };
                L.D(DataBaseOperation.CreateData(ref bo).ToString());
                L.D(bo.Stringify());
                busList.Add(bo);
            }


            foreach (var item in namelist)
            {
                StudentObject stu = new StudentObject()
                {
                    BusID = busList[RandomInt(0, busList.Count)].ObjectId,
                    ClassID = classList[RandomInt(0, classList.Count)].ObjectId,
                    Sex = RandomBool ? "M" : "F",
                    StudentName = item,
                    AHChecked = false,
                    CSChecked = false,
                    LSChecked = false,
                    TakingBus = true,
                    WeekType = (StudentBigWeekMode)RandomInt(0, 3),
                    DirectGoHome = (DirectGoHomeMode)RandomInt(0, 3)
                };
                L.D(DataBaseOperation.CreateData(ref stu).ToString());

                L.D(stu.Stringify());
                bool sexParent = RandomBool;
                UserObject user = new UserObject()
                {
                    Sex = sexParent ? "M" : "F",
                    ChildList = new List<string>() { stu.ObjectId },
                    PhoneNumber = "00000000000",
                    RealName = stu.StudentName + "的" + (sexParent ? "爸爸" : "妈妈"),
                    UserGroup = new UserGroup(false, false, false, true),
                    UserName = "stu_Parent" + stu.ObjectId,
                    HeadImagePath = ""
                };
                L.D(DataBaseOperation.CreateData(ref user).ToString());
                L.D(user.Stringify());

                if (RandomBool)
                {
                    if (stu != null)
                    {
                        me.ChildList.Add(stu.ObjectId);
                    }
                }
            }

            me.HeadImagePath = "liuhaoyu.gif";
            me.ClassList.Add(classList[RandomInt(0, classList.Count)].ObjectId);
            L.D(DataBaseOperation.UpdateData(ref me).ToString());
            L.D(me.Stringify());


            DatabaseSocketsClient.KillConnection();
            return;
        }
        public static bool RandomBool => new Random().Next(0, 2) == 1;
        public static int RandomInt(int min, int max) => new Random().Next(min, max);
    }
}
