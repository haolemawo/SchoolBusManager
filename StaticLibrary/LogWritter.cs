﻿using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using WBPlatform.Config;
using WBPlatform.StaticClasses;

namespace WBPlatform.Logging
{
    public delegate void OnLogWrited(OnLogChangedEventArgs logchange, object sender);
    public class OnLogChangedEventArgs : EventArgs
    {
        public OnLogChangedEventArgs() { }
        public OnLogChangedEventArgs(string logString, LogLevel logLevel)
        {
            LogString = logString;
            LogLevel = logLevel;
        }
        public string LogString { get; set; }
        public LogLevel LogLevel { get; set; }
    }

    public static class L
    {
        public static event OnLogWrited OnLog;
        public static LogLevel _LogLevel { private get; set; }

        private static StreamWriter Fs { get; set; }
        private static string LogFilePath { get; set; }

        //Actually it should be a new instance when used, However, due to the "static" LW class, there's only one instance.
        //To prevent instances takes up your memory, a constant instance is used...
        public static OnLogChangedEventArgs LogEvent = new OnLogChangedEventArgs();

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
            //Message = Message.Replace("\r\n", @" -\r\n ");
            if (level < _LogLevel) return;
            string LogMsg = $"[{DateTime.Now.ToDetailedString()} - {level.ToString()}] {Message}\r\n";
#if DEBUG
            Debug.Write(LogMsg);
#endif
#if TRACE
            Trace.WriteLine(LogMsg);
#endif
            switch (level)
            {
                case LogLevel.DBG:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case LogLevel.INF:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case LogLevel.WRN:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case LogLevel.ERR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
            Console.Write(LogMsg);
            Console.ResetColor();

            Fs.Write(LogMsg);

            LogEvent.LogString = LogMsg;
            LogEvent.LogLevel = level;
            OnLog?.Invoke(LogEvent, null);
        }

        private static string GetCallerClassName => new StackTrace().GetFrame(2).GetMethod().ReflectedType.Name;

        public static void D(string Message, [CallerMemberName] string memberName = "") => WriteLog(LogLevel.DBG, GetCallerClassName + "::" + memberName + "\t" + Message);
        public static void I(string Message, string Operation = "", [CallerMemberName] string memberName = "") => WriteLog(LogLevel.INF, GetCallerClassName + "::" + memberName + "\t" + Message);
        public static void W(string Message, string Operation = "", [CallerMemberName] string memberName = "") => WriteLog(LogLevel.WRN, GetCallerClassName + "::" + memberName + "\t" + Message);
        public static void E(string Message, string Operation = "", [CallerMemberName] string memberName = "") => WriteLog(LogLevel.ERR, GetCallerClassName + "::" + memberName + "\t" + Message);
        public static void LogException(this Exception ex, [CallerMemberName] string memberName = "") => WriteLog(LogLevel.ERR, GetCallerClassName + "::" + memberName + "\r\n" + ex + "\r\n");
    }
    public enum LogLevel { DBG = 0, INF = 1, WRN = 2, ERR = 3, }
}
