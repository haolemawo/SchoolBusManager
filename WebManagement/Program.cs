using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using WBPlatform.Config;
using WBPlatform.Database;
using WBPlatform.Logging;
using WBPlatform.WebManagement.Tools;

namespace WBPlatform.WebManagement
{
    public static class Program
    {
        public static string Version { get; private set; }
        public static DateTime StartUpTime { get; private set; }
        public static Task WebServerTask { get; private set; }
        public static CancellationTokenSource ServerStopToken { get; private set; } = new CancellationTokenSource();
        public static void Main(string[] args)
        {
            L.InitLog();
            StartUpTime = DateTime.Now;
            L.I("WoodenBench WebServer Starting....");
            L.I($"\t Startup Time {StartUpTime.ToString()}.");
            Version = new FileInfo(new string(Assembly.GetExecutingAssembly().CodeBase.Skip(8).ToArray())).LastWriteTime.ToString();
            L.I($"\t Version {Version}");

            var v = XConfig.LoadAll();
            if (!(v.Item1 && v.Item2)) return;

            StatusMonitor.StartMonitorThread();
            WeChatHelper.ReNewWCCodes();

            DataBaseOperation.InitialiseClient();
            //DataBaseOperation.InitialiseClient(IPAddress.Loopback);

            WeChatHelper.InitialiseEncryptor();

            L.I("Initialising Core Messaging Systems.....");
            WeChatMessageSystem.StartProcessThreads();
            WeChatMessageBackupService.StartBackupThread();
            MessagingSystem.StartProcessThread();

            var webHost = BuildWebHost(XConfig.Current.ApplicationInsightInstrumentationKey, args);
            L.I("Starting WebHost....");
            WebServerTask = webHost.RunAsync(ServerStopToken.Token);
            WebServerTask.Wait();
            L.E("WebServer Stoped! Cancellation Token = " + ServerStopToken.IsCancellationRequested);
        }

        public static IWebHost BuildWebHost(string instrumentationKey, string[] args)
        {
            L.I("Building WebHost....");
            var host = WebHost.CreateDefaultBuilder(args)
                 .UseIISIntegration()
                 .UseKestrel()
                 .UseApplicationInsights(instrumentationKey)
                 .UseStartup<Startup>()
                 .Build();
            return host;
        }
    }
}
