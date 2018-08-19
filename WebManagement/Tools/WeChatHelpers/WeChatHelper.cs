using System;
using System.Collections.Generic;

using WBPlatform.Config;
using WBPlatform.Logging;
using WBPlatform.StaticClasses;
using WBPlatform.WebManagement.Tools;

namespace WBPlatform.WebManagement.Tools
{
    public static class WeChatHelper
    {
        public static WeChatXMLHelper WeChatEncryptor { get; set; }
        public static string AccessTicket { get; set; }
        public static string AccessToken { get; set; }
        public static DateTime AvailableTime_Ticket { get; set; }
        public static DateTime AvailableTime_Token { get; set; }
        public static readonly string GetAccessToken_Url = "https://qyapi.weixin.qq.com/cgi-bin/gettoken?" + "corpid=" + XConfig.Current.WeChat.CorpID + "&corpsecret=" + XConfig.Current.WeChat.CorpSecret;


        public static bool InitialiseEncryptor()
        {
            LW.I("Initialising WeChat Data Packet Encryptor.....");
            WeChatEncryptor = new WeChatXMLHelper(XConfig.Current.WeChat.SToken, XConfig.Current.WeChat.AESKey, XConfig.Current.WeChat.CorpID);
            LW.I("WeChat Data Packet Encryptor Initialisation Finished!");
            return true;
        }

        private static bool InitialiseWeChatCodes()
        {
            LW.I("Query New WeChat Keys....");
            Dictionary<string, string> JSON;
            LW.I("\tGetting Access Token....");
            JSON = PublicTools.HTTPGet(GetAccessToken_Url);
            AccessToken = JSON["access_token"];
            AvailableTime_Token = DateTime.Now.AddSeconds(int.Parse(JSON["expires_in"]));
            LW.I("\tGetting Ticket....");
            JSON = PublicTools.HTTPGet("https://qyapi.weixin.qq.com/cgi-bin/get_jsapi_ticket?access_token=" + AccessToken);
            AccessTicket = JSON["ticket"];
            AvailableTime_Ticket = DateTime.Now.AddSeconds(int.Parse(JSON["expires_in"]));
            LW.I("WeChat Keys Initialised Successfully!");
            return true;
        }
        public static bool ReNewWCCodes()
        {
            LW.I("Started Renew WeChat Operation Codes.....");
            LW.I("\tChecking Access Tickets...");
            if (AvailableTime_Ticket.Subtract(DateTime.Now).TotalMilliseconds <= 0) { InitialiseWeChatCodes(); return false; }
            LW.I("\tChecking Tokens...");
            if (AvailableTime_Token.Subtract(DateTime.Now).TotalMilliseconds <= 0) { InitialiseWeChatCodes(); return false; }
            return true;
        }
    }
}