﻿using Newtonsoft.Json;

using System.Collections.Generic;
using System.IO;

using WBPlatform.Logging;

namespace WBPlatform.Config
{
    public class ConfigCollection
    {
        private readonly string https = "https://";
        public string ApplicationInsightInstrumentationKey { get; set; }
        public string StatusReportNamedPipe { get; set; }
        public bool DevelopmentVersion { get; set; }
        public string WebSiteAddress => DevelopmentVersion ? https + "dev.schoolbus.lhy0403.top" : https + "schoolbus.lhy0403.top";
        public string StatusPageAddress => DevelopmentVersion ? https + "dev-status.schoolbus.lhy0403.top" : https + "status.schoolbus.lhy0403.top";

        public DatabaseConfig Database { get; set; } = new DatabaseConfig();
        public WeChatConfig WeChat { get; set; } = new WeChatConfig();
    }

    /// <summary>
    /// DO NOT CHANGE ANY PROPERTIES' NAME 
    /// </summary>
    public class DatabaseConfig
    {
        //Used by Database Server...
        public string SQLServerIP { get; set; }
        public int SQLServerPort { get; set; }
        public string DatabaseName { get; set; }
        public string DatabaseUserName { get; set; }
        public string DatabasePassword { get; set; }


        public string DBServerIP { get; set; }
        public int DBServerPort { get; set; }
        public int ClientTimeout { get; set; }
        public int FailedRetryTime { get; set; }
    }
    public class WeChatConfig
    {
        public string AgentId { get; set; }
        public string CorpID { get; set; }
        public string CorpSecret { get; set; }
        public string SToken { get; set; }
        public string AESKey { get; set; }
    }

    public class LocalisedMessages
    {
        public Dictionary<string, string> messages = new Dictionary<string, string>();

        public string this[string mID] => GetMsg(mID);

        private string GetMsg(string mID) => messages.ContainsKey(mID) ? messages[mID] : $"{{{mID}}}";

        public string NotFound => GetMsg("NotFound");
        public string UserPermissionDenied => GetMsg("UserPermissionDenied");
        public string DataBaseError => GetMsg("DataBaseError");
        public string RequestIllegal => GetMsg("RequestIllegal");
        public string NotSupported => GetMsg("NotSupported");
        public string UnknownInternalException => GetMsg("UnknownInternalException");
        public string InternalDataBaseError => GetMsg("InternalDataBaseError");
        public string ParameterUnexpected => GetMsg("ParameterUnexpected");
    }

    public static class XConfig
    {
        public static ConfigCollection Current { get; set; } = new ConfigCollection();
        public static LocalisedMessages Messages { get; set; } = new LocalisedMessages();

        public static bool LoadMessages(string MessageFile)
        {
            LW.I("Loading Messages....");
            if (!File.Exists(MessageFile)) return false;
            string ConfigString = File.ReadAllText(MessageFile);
            var msg = JsonConvert.DeserializeObject<LocalisedMessages>(ConfigString);
            if (msg == null)
            {
                LW.E("Failed Load Messages.... Exiting...");
                return false;
            }
            Messages = msg;
            LW.I("Finished Loading Messages....");
            return true;
        }

        public static bool LoadConfig(string ConfigFile)
        {
            LW.I("Loading Config....");
            if (!File.Exists(ConfigFile)) return false;
            string ConfigString = File.ReadAllText(ConfigFile);
            var config = JsonConvert.DeserializeObject<ConfigCollection>(ConfigString);
            if (config == null)
            {
                LW.E("Failed Load Config.... Exiting...");
                return false;
            }
            Current = config;
            LW.I("Finished Loading Config....");
            return true;
        }

        public static (bool, bool) LoadAll() => LoadAll("XConfig.conf", "Messages.conf");
        public static (bool, bool) LoadAll(string ConfigFilePath, string MessageFilePath)
        {
            bool resultA = LoadConfig(ConfigFilePath);
            bool resultB = LoadMessages(MessageFilePath);
            return (resultA, resultB);
        }
    }
}
