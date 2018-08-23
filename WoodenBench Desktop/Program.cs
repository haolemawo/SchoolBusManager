using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using WBPlatform.Config;
using WBPlatform.Database;
using WBPlatform.Database.Connection;
using WBPlatform.DesktopClient.Views;
using WBPlatform.Logging;

namespace WBPlatform.DesktopClient
{
    public static class Program
    {
        [STAThread]
        public static int Main(string[] args)
        {
            LW.SetLogLevel(LogLevel.D);
            LW.InitLog();
            LW.I("========= = Start WoodenBench for Schoolbus Windows Client = =========");
            if (!XConfig.LoadConfig("XConfig.conf"))
            {
                LW.E("Config Loading Failed! Check file...");
                return 0;
            }

            DataBaseOperation.InitialiseClient();
            Application.EnableVisualStyles();
            Application.Run(LoginWindow.Default);

            DatabaseSocketsClient.KillConnection();
            return 0;
        }
    }
}
