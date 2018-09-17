using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using WBPlatform.Config;
using WBPlatform.Database;
using WBPlatform.Logging;
using WBPlatform.TableObject;
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
            StartUpTime = DateTime.Now;
            L.InitLog();
            L.I("WoodenBench WebServer Starting....");
            L.I($"Startup Time {StartUpTime.ToString()}.");

            Version = new FileInfo(new string(Assembly.GetExecutingAssembly().CodeBase.Skip(8).ToArray())).LastWriteTime.ToString();
            L.I($"Version: {Version}");

            var v = XConfig.LoadAll();
            if (!(v.Item1 && v.Item2))
            {
                L.E("XConfig Load error... Quiting");
                return;
            }

            DataBaseOperation.InitialiseClient();

            XConfig.ServerConfig.GetConfig();

            StatusMonitor.StartMonitorThread();
            WeChatHelper.PrepareCodes();
            WeChatHelper.InitialiseEncryptor();

            L.I("Initialising Core Messaging Systems.....");
            WeChatMessageSystem.StartProcessThreads();
            WeChatMessageBackupService.StartBackupThread();
            MessagingSystem.StartProcessThread();

            var webHost = BuildWebHost(args, XConfig.Current.ApplicationInsightInstrumentationKey);
            L.I("Starting WebHost....");
            WebServerTask = webHost.RunAsync(ServerStopToken.Token);
            WebServerTask.Wait();
            L.E("WebServer Stoped! Cancellation Token = " + ServerStopToken.IsCancellationRequested);
        }

        public static IWebHost BuildWebHost(string[] args, string instrumentationKey)
        {
            L.I("Building WebHost....");
            var host = WebHost.CreateDefaultBuilder(args)
                 .UseKestrel()
                 .UseApplicationInsights(instrumentationKey)
                 .UseStartup<Startup>()
                 .Build();
            return host;
        }
    }
}
