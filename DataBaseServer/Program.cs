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
            L.InitLog();
            if (!XConfig.LoadConfig("XConfig.conf"))
            {
                L.E("Config File Not Loaded!");
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
                L.E(ex.Message);
                L.E("MainForm disappeared caused by exception!");
                Thread.Sleep(1000);
                L.E("DBServer is to be restarted to keep stability!");
            }
            Application.Restart();
        }
    }
}
