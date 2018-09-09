using Newtonsoft.Json;

using System;
using System.IO.Pipes;
using System.Reflection;
using System.Text;
using System.Threading;

using WBPlatform.Config;
using WBPlatform.Database.Connection;
using WBPlatform.Logging;
using WBPlatform.StaticClasses;
using WBPlatform.StatusReport;
using WBPlatform.WebManagement.Controllers;

namespace WBPlatform.WebManagement.Tools
{
    public static class StatusMonitor
    {
        private static Thread _MonitorThread = new Thread(new ThreadStart(ThreadWork));
        public static StatusReportObject ReportObject { get; private set; } = new StatusReportObject();
        private static NamedPipeServerStream pipe = new NamedPipeServerStream(XConfig.Current.StatusReportNamedPipe, PipeDirection.Out);

        public static void StartMonitorThread()
        {
            L.I("Starting Monitor Thread");
            _MonitorThread.Start();
            L.I("Monitor Thread: Active");
        }
        private static void ThreadWork()
        {
            while (true)
            {
                var wcStatus = WeChatMessageSystem.Status();

                ReportObject = new StatusReportObject()
                {
                    ReportTime = DateTime.Now,
                    SessionsCount = BaseController.GetCount,
                    SessionThread = true,
                    Tokens = OnePassTicket.GetCount,
                    WeChatRCVDThreadStatus = wcStatus.Item1,
                    WeChatSENTThreadStatus = wcStatus.Item2,
                    WeChatRCVDListCount = wcStatus.Item3,
                    WeChatSENTListCount = wcStatus.Item4,
                    Database = DatabaseSocketsClient.Connected,
                    CoreMessageSystemThread = MessagingSystem.GetStatus,
                    CoreMessageSystemCount = MessagingSystem.GetCount,
                    MessageBackupThread = WeChatMessageBackupService.GetStatus,
                    MessageBackupCount = WeChatMessageBackupService.GetCount,
                    StartupTime = Program.StartUpTime.ToDetailedString(),
                    ServerVer = Program.Version,
                    CoreLibVer = WBConsts.CoreVersion,
                    NetCoreCLRVer = Assembly.GetCallingAssembly().ImageRuntimeVersion
                };
                string data = JsonConvert.SerializeObject(ReportObject);

                byte[] ipByte = Encoding.UTF8.GetBytes(data);
                //client.Send(ipByte, ipByte.Length, endpoint);
                if (!pipe.IsConnected)
                {
                    pipe.WaitForConnection();
                }
                pipe.Write(ipByte, 0, ipByte.Length);
                pipe.Flush();
                pipe.WaitForPipeDrain();
                Thread.Sleep(5000);
            }
        }
    }
}