using System.IO;
using System.Linq;
using System.Reflection;

namespace WBPlatform.StaticClasses
{

    public static partial class WBConsts
    {
        public static string CurrentCoreVersion => new FileInfo(new string(Assembly.GetExecutingAssembly().CodeBase.Skip(8).ToArray())).LastWriteTime.ToString();

        public const string TABLE_Mgr_StuData = "StudentsData";
        public const string TABLE_Mgr_Classes = "Classes";
        public const string TABLE_Mgr_BusData = "SchoolBuses";
        public const string TABLE_Mgr_WeekIssue = "WeeklyIssues";

        public const string TABLE_Gen_UserTable = "AllUsersTable";
        public const string TABLE_Gen_General = "GeneralData";
        public const string TABLE_Gen_Bugreport = "UserQuestions";

        public const string TABLE_Gen_Notification = "Notifications";
        public const string TABLE_Gen_UserRequest = "UserRequest";

    }

    public enum UserChangeRequestTypes
    {
        真实姓名 = 0,
        手机号码 = 1,
        班级 = 2,
        孩子 = 3,
        校车 = 4
    }
    

    public enum NotificationType
    {
        WindowsClient = 1,
        WeChatC2C = 2,
        WeChatMultiCast = 3,
        WeChatBroadCast = 4
    }

    public enum BusReportTypeE
    {
        堵车 = 0,
        事故 = 1,
        学生迟到 = 2,
        到校 = 3,
        到家 = 4,
        其他 = 9,
    }

    public enum UCRProcessStatus
    {
        NotSolved = -1, Accepted = 0, Refused = 1
    }

    public enum UCRRefusedReasons
    {
        理由不充分 = 0, 格式有误_请重新填写 = 1, 其他原因 = -1
    }
}
