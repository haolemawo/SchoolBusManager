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
using WBPlatform.Database.Connection;
using WBPlatform.Logging;
using WBPlatform.WebManagement.Controllers;
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

            L.E("Current Directory: " + Directory.GetCurrentDirectory());

            Version = new FileInfo(new string(Assembly.GetExecutingAssembly().CodeBase.Skip(8).ToArray())).LastWriteTime.ToString();
            L.I($"Version: {Version}");

            (bool, bool) v = args.Length >= 2 && !string.IsNullOrEmpty(args[0]) && !string.IsNullOrEmpty(args[1])
                ? XConfig.LoadAll(args[0], args[1])
                : XConfig.LoadAll();

            if (!(v.Item1 && v.Item2))
            {
                L.E("XConfig Load error... Quiting");
                return;
            }

            DBConnectionBuilder.InitialiseDBConnection();
            DataBaseOperation.Initialise(new DataBase_ng.DataBaseContext());

            L.I("Starting Job Watcher");
            TimedJob.StartJobWatcher();
            TimedJob.AddToJobList("Get Config", ServerConfig.Current.GetConfig, 10, 0);
            TimedJob.AddToJobList("Session Checker", BaseController.CheckSessions, 10, 0);
            TimedJob.AddToJobList("Status Monitor", StatusMonitor.SendStatus, 10);

            
            var webHostBuilder = CreateWebHostBuilder(args);
            webHostBuilder.UseApplicationInsights(XConfig.Current.ApplicationInsightInstrumentationKey);
            L.I("Building WebHost....");
            var host = webHostBuilder.Build();

            WeChatHelper.PrepareCodes();
            WeChatHelper.InitialiseEncryptor();

            L.I("Initialising Core Messaging Systems.....");
            WeChatMessageSystem.StartProcessThreads();
            WeChatMessageBackupService.StartBackupThread();
            MessagingSystem.StartProcessThread();

            //XConfig.ServerConfig.GetConfig();
            L.I("Starting WebHost....");
            WebServerTask = host.RunAsync(ServerStopToken.Token);

            WebServerTask.Wait();
            L.E("WebServer Stoped! Cancellation Token = " + ServerStopToken.IsCancellationRequested);
            //DatabaseSocketsClient.KillConnection();
            L.E("Now Exit!");
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                 .UseKestrel()
                 .UseStartup<Startup>();
            return host;
        }
    }
}
