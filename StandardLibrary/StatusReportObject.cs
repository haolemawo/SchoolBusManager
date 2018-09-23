using Newtonsoft.Json;
using System;
using System.ComponentModel;
using WBPlatform.StaticClasses;

namespace WBPlatform.ServiceStatus
{
    public class StatusReportObject
    {
        [JsonIgnore]
        [DisplayName("服务器整体状态")]
        public bool IsAlive => SessionThread &&
            WeChatRCVDThreadStatus &&
            WeChatSENTThreadStatus &&
            CoreMessageSystemThread &&
            MessageBackupThread &&
            Math.Abs(ReportTime.Subtract(DateTime.Now).TotalMinutes) < 1;

        [DisplayName("上次报告时间")]
        public DateTime ReportTime { get; set; } = DateTime.MinValue;

        [DisplayName("Session 计数")]
        public int SessionsCount { get; set; } = 0;

        [DisplayName("Session 线程状态")]
        public bool SessionThread { get; set; } = false;

        [DisplayName("Token 计数")]
        public int Tokens { get; set; } = 0;

        [DisplayName("微信消息接收线程状态")]
        public bool WeChatRCVDThreadStatus { get; set; } = false;

        [DisplayName("微信消息发送线程状态")]
        public bool WeChatSENTThreadStatus { get; set; } = false;

        [DisplayName("微信消息接收队列")]
        public int WeChatRCVDListCount { get; set; } = 0;

        [DisplayName("微信消息发送队列")]
        public int WeChatSENTListCount { get; set; } = 0;

        [DisplayName("主数据库连接")]
        public bool Database { get; set; } = false;

        [DisplayName("内部通信系统状态")]
        public bool CoreMessageSystemThread { get; set; }

        [DisplayName("内部通信队列")]
        public int CoreMessageSystemCount { get; set; } = 0;

        [DisplayName("消息副本提供程序")]
        public bool MessageBackupThread { get; set; } = false;

        [DisplayName("消息副本提供程序队列")]
        public int MessageBackupCount { get; set; } = 0;

        [DisplayName("服务启动时间")]
        public string StartupTime { get; set; } = DateTime.MinValue.ToDetailedString();

        [DisplayName("服务程序版本号")]
        public string ServerVer { get; set; } = "V0.0.0.0";

        [DisplayName("核心库版本号")]
        public string CoreLibVer { get; set; } = "V0.0.0.0";

        [DisplayName("运行时版本号")]
        public string NetCoreCLRVer { get; set; } = "V0.0.0.0";
    }

    public class DescriptedField<TValue> where TValue : IConvertible
    {
        public TValue Value { get; set; }
        public string Description { get; private set; }
        public DescriptedField(TValue value, string Description)
        {
            Value = value;
            this.Description = Description;
        }
        public static bool operator ==(object value, DescriptedField<TValue> field)
            => value is DescriptedField<TValue>
                ? (value as DescriptedField<TValue>).Value.Equals(field.Value)
                : false;
        public static bool operator !=(object value, DescriptedField<TValue> field) => !(value == field);
        public override bool Equals(object obj) => Value.Equals(obj);
        public override int GetHashCode() => Value.GetHashCode();

    }
}