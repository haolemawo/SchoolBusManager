using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

using Newtonsoft.Json;
using System;
using System.IO.Pipes;
using System.Text;
using System.Threading;

using WBPlatform.Config;
using WBPlatform.Logging;

namespace WBPlatform.ServiceStatus
{
    public class Program
    {
        public static void Main(string[] args)
        {
            L.InitLog();
            var v = XConfig.LoadAll();
            if (!(v.Item1 && v.Item2)) return;

            new Thread(new ThreadStart(GetState)) { Name = "Platform State Obtainer" }.Start();
            BuildWebHost(args).Run();
        }
        public static void GetState()
        {
            NamedPipeClientStream client = new NamedPipeClientStream("localhost", XConfig.Current.StatusReportNamedPipe, PipeDirection.In);
            while (true)
            {
                client.Connect();
                while (client.IsConnected)
                {
                    var data = new byte[65535];
                    try
                    {
                        var count = client.Read(data, 0, data.Length);
                        if (count > 0)
                        {
                            HomeController.ServerStatus = JsonConvert.DeserializeObject<StatusReportObject>(Encoding.UTF8.GetString(data, 0, count));
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.LogException();
                    }
                    finally
                    {
                        GC.Collect();
                    }
                }
                L.E("DisConnected from the WBWebServer....");
                Thread.Sleep(1000);
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseIISIntegration()
                .UseKestrel()
                .UseApplicationInsights(XConfig.Current.ApplicationInsightInstrumentationKey)
                .UseStartup<Startup>()
                .Build();
    }
}
