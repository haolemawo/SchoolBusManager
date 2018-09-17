using System;
using System.Collections.Generic;

using WBPlatform.Config;
using WBPlatform.Logging;
using WBPlatform.StaticClasses;

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
            L.I("Initialising WeChat Data Packet Encryptor.....");
            WeChatEncryptor = new WeChatXMLHelper(XConfig.Current.WeChat.SToken, XConfig.Current.WeChat.AESKey, XConfig.Current.WeChat.CorpID);
            return true;
        }

        private static bool RenewWeChatCodes()
        {
            Dictionary<string, string> JSON;

            L.I("Getting Access Token....");
            JSON = PublicTools.HTTPGet(GetAccessToken_Url);
            AccessToken = JSON["access_token"];
            AvailableTime_Token = DateTime.Now.AddSeconds(int.Parse(JSON["expires_in"]));

            L.I("Getting Ticket....");
            JSON = PublicTools.HTTPGet("https://qyapi.weixin.qq.com/cgi-bin/get_jsapi_ticket?access_token=" + AccessToken);
            AccessTicket = JSON["ticket"];
            AvailableTime_Ticket = DateTime.Now.AddSeconds(int.Parse(JSON["expires_in"]));

            L.I("WeChat Keys Initialised Successfully!");
            return true;
        }

        public static void PrepareCodes() { if (IsCodeOutDated) RenewWeChatCodes(); }

        private static bool IsCodeOutDated =>
            AvailableTime_Ticket.Subtract(DateTime.Now).TotalSeconds < 0 ||
            AvailableTime_Token.Subtract(DateTime.Now).TotalSeconds < 0;
    }
}