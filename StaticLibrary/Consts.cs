using System.IO;
using System.Linq;
using System.Reflection;

namespace WBPlatform.StaticClasses
{
    public static partial class WBConsts
    {
        public static string CoreVersion => new FileInfo(new string(Assembly.GetExecutingAssembly().CodeBase.Skip(8).ToArray())).LastWriteTime.ToString();
    }

    public enum UserChangeRequestTypes
    {
        真实姓名 = 0,
        手机号码 = 1,
        班级 = 2,
        孩子 = 3,
        班车 = 4
    }

    public enum DirectGoHomeMode
    {
        NotSet = 0,
        DirectlyGoHome = 1,
        NeedParentsSign = 2
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
