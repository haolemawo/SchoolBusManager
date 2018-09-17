using Newtonsoft.Json;

using System.Collections.Generic;
using System.IO;

using WBPlatform.Logging;
using WBPlatform.TableObject;

namespace WBPlatform.Config
{
    /// <summary>
    /// Config Collection, DO NOT CHANGE PROPERTIES' NAME
    /// </summary>
    public class ConfigCollection
    {
        private const string https = "https://";
        private const string http = "http://";

        public int LogLevel { get; set; }
        public string ApplicationInsightInstrumentationKey { get; set; }
        public string StatusReportNamedPipe { get; set; }
        public bool DevelopmentVersion { get; set; }
        public string WebSiteAddress => https + (DevelopmentVersion ? "dev.schoolbus.lhy0403.top" : "schoolbus.lhy0403.top");
        public string StatusPageAddress => https + (DevelopmentVersion ? "dev-status.schoolbus.lhy0403.top" : "status.schoolbus.lhy0403.top");

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
        public static ServerConfigCollection ServerConfig { get; } = new ServerConfigCollection();
        public static ConfigCollection Current { get; set; } = new ConfigCollection();
        public static LocalisedMessages Messages { get; set; } = new LocalisedMessages();

        public static bool LoadMessages(string MessageFile)
        {
            L.I("Loading Messages....");
            if (!File.Exists(MessageFile)) return false;
            string ConfigString = File.ReadAllText(MessageFile, System.Text.Encoding.UTF8);
            var m = JsonConvert.SerializeObject(Messages);
            var msg = JsonConvert.DeserializeObject<LocalisedMessages>(ConfigString);
            if (msg == null)
            {
                L.E("Failed Load Messages.... Exiting...");
                return false;
            }
            Messages = msg;
            L.I("Finished Loading Messages....");
            return true;
        }

        public static bool LoadConfig(string ConfigFile)
        {
            L.I("Loading Config....");
            if (!File.Exists(ConfigFile)) return false;
            string ConfigString = File.ReadAllText(ConfigFile);
            var config = JsonConvert.DeserializeObject<ConfigCollection>(ConfigString);
            if (config == null)
            {
                L.E("Failed Load Config.... Exiting...");
                return false;
            }
            Current = config;
            L.I("Finished Loading Config....");
            L._LogLevel = (LogLevel)Current.LogLevel;
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
