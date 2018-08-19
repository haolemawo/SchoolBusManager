using System;
using System.Threading;
using System.Windows.Forms;
using WBPlatform.StaticClasses;
using WBPlatform.Config;
using WBPlatform.Logging;
namespace WBPlatform.Database.DBServer
{
    static class Program
    {
        public static MainForm mainForm { get; set; }
        [STAThread]
        static void Main()
        {
            LW.InitLog();
            if (!XConfig.LoadConfig("XConfig.conf"))
            {
                LW.E("Config File Not Loaded!");
                return;
            }
            DatabaseCore.InitialiseDBConnection();
            DatabaseSocketsServer.InitialiseSockets();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            mainForm = new MainForm();
            try
            {
                Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                LW.E(ex.Message);
                LW.E("MainForm disappeared caused by exception!");
                Thread.Sleep(1000);
                LW.E("DBServer is to be restarted to keep stability!");
            }
            Application.Restart();
        }
    }
}
