using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WBPlatform.StaticClasses;

namespace WBPlatform.Logging
{
    public class OnLogChangedEventArgs : EventArgs
    {
        public OnLogChangedEventArgs(string logString, LogLevel logLevel)
        {
            LogString = logString;
            LogLevel = logLevel;
        }
        public string LogString { get; set; }
        public LogLevel LogLevel { get; set; }
    }

    public static class LW
    {

        public delegate void OnLogWrited(OnLogChangedEventArgs logchange, object sender);
        public static event OnLogWrited OnLog;


        private static LogLevel _LogLevel { get; set; } = LogLevel.W;
        public static void SetLogLevel(LogLevel level) { _LogLevel = level; }

        private static StreamWriter Fs { get; set; }
        private static string LogFilePath { get; set; }

        //Actually it should be a new instance when used, However, due to the "static" LW class, there's only one instance.
        //To prevent instances takes up your memory, a constant instance is used...
        public static OnLogChangedEventArgs LogEvent = new OnLogChangedEventArgs("", LogLevel.D);

        public static void InitLog()
        {
            LogFilePath = Environment.CurrentDirectory + "\\Logs\\" + DateTime.Now.ToFileNameString() + ".log";
            Directory.CreateDirectory(Environment.CurrentDirectory + "\\Logs\\");
            Fs = File.CreateText(LogFilePath);
            Fs.AutoFlush = true;
            WriteLog(_LogLevel, "Log is Now Initialised!");
        }

        private static void WriteLog(LogLevel level, string Message)
        {
            if (level < _LogLevel) return;
            string LogMsg = $"[{DateTime.Now.ToDetailedString()} - {level.ToString()}] {Message}\r\n";

            switch (level)
            {
                case LogLevel.E:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.I:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case LogLevel.D:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case LogLevel.W:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
            }
            Debug.Write(LogMsg);
            Console.Write(LogMsg);
            Console.ResetColor();

            Fs.Write(LogMsg);

            LogEvent.LogString = LogMsg;
            LogEvent.LogLevel = level;
            OnLog?.Invoke(LogEvent, null);
        }

        private static string GetCallerClassName => new StackTrace().GetFrame(2).GetMethod().ReflectedType.Name;
        public static void D(string Message, [CallerMemberName] string memberName = "")
        {
            WriteLog(LogLevel.D, GetCallerClassName + "::" + memberName + "\t" + Message);
        }

        public static void I(string Message, string Operation = "", [CallerMemberName] string memberName = "")
        {
            WriteLog(LogLevel.I, GetCallerClassName + "::" + memberName + "\t" + Message);
        }

        public static void W(string Message, string Operation = "", [CallerMemberName] string memberName = "")
        {
            WriteLog(LogLevel.W, GetCallerClassName + "::" + memberName + "\t" + Message);
        }

        public static void E(string Message, string Operation = "", [CallerMemberName] string memberName = "")
        {
            WriteLog(LogLevel.E, GetCallerClassName + "::" + memberName + "\t" + Message);
        }

        public static void LogException(this Exception ex, [CallerMemberName] string memberName = "")
        {
            WriteLog(LogLevel.E, GetCallerClassName + "::" + memberName + "\t" + ex);
        }
    }
    #region Log Level
    /// <summary>
    /// LogLevel Enumerate used for <see cref="LW"/>
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Debug Log Level
        /// </summary>
        D = 0,
        /// <summary>
        /// Info Log Level
        /// </summary>
        I = 1,
        /// <summary>
        /// Warning Log Level
        /// </summary>
        W = 2,
        /// <summary>
        /// Error Log Level
        /// </summary>
        E = 3,
    }
    #endregion
}
