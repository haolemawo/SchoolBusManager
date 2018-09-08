using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;
using WBPlatform.Logging;

namespace WBPlatform.StaticClasses
{
    public static class PublicTools
    {
        /// <summary>
        /// Anti-Injection for Database...
        /// </summary>
        public static object EncodeAsString(this object item) => item is string ? HttpUtility.UrlPathEncode(item as string) : item;
        public static object DecodeAsObject(this object item) => item is string ? HttpUtility.UrlDecode(item as string) : item;

        //public static object[] EncodeStringArray(this IEnumerable<object> items) => items.EncodeStringArray<object[]>();
        //public static TOut EncodeStringArray<TOut>(this IEnumerable<object> items) where TOut : IEnumerable<object> => (TOut)(IEnumerable<object>)(from _ in items select _.EncodeAsString()).ToArray();

        public static Dictionary<string, string> HTTPGet(string URL)
        {
            L.I("HTTP - GET-rqst: " + URL);
            HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string resp = reader.ReadToEnd();
            L.I("HTTP - GET-rply: " + resp);
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(resp);
        }

        public static Dictionary<string, string> HTTPPost(string postUrl, string paramData)
        {
            L.I("HTTP - POST-rqst: " + postUrl + " WITH DATA : " + paramData);
            byte[] byteArray = Encoding.UTF8.GetBytes(paramData);
            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
            webReq.Method = "POST";
            webReq.ContentType = "application/x-www-form-urlencoded";

            webReq.ContentLength = byteArray.Length;
            Stream newStream = webReq.GetRequestStream();
            newStream.Write(byteArray, 0, byteArray.Length);
            newStream.Flush();
            newStream.Close();
            newStream.Dispose();
            HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string ret = sr.ReadToEnd();
            sr.Close();
            response.Close();

            L.I("HTTP - POST-rply: " + ret);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (KeyValuePair<string, object> item in JsonConvert.DeserializeObject<Dictionary<string, object>>(ret))
            {
                dict.Add(item.Key, item.Value == null ? "" : item.Value.ToString());
            }
            return dict;
        }

        public static string DecodeDatabasePacket(NetworkStream stream)
        {
            byte[] fsBytes;
            int ContentLenth;
            byte[] arrServerRecMsg = new byte[1];
            stream.Read(arrServerRecMsg, 0, 1);
            int HeaderLenth = arrServerRecMsg.ToInt32();

            arrServerRecMsg = new byte[HeaderLenth];
            stream.Read(arrServerRecMsg, 0, HeaderLenth);
            ContentLenth = arrServerRecMsg.ToInt32();

            int total = 0;
            int dataleft = ContentLenth;
            fsBytes = new byte[ContentLenth];
            int recv;
            while (total < ContentLenth)
            {
                recv = stream.Read(fsBytes, total, dataleft);
                if (recv == 0) break;
                total += recv;
                dataleft -= recv;
            }
            return Encoding.UTF8.GetString(fsBytes, 0, ContentLenth);
        }
        public static byte[] MakeDatabasePacket(string MessageId, string sendMsg)
        {
            List<byte> mergedPackage = new List<byte>();
            byte[] arrClientSendMsg = Encoding.UTF8.GetBytes(MessageId + sendMsg);
            byte[] Header = arrClientSendMsg.Length.ToBytesArray();
            byte HeaderSize = Convert.ToByte(Header.Length);
            mergedPackage.Add(HeaderSize);
            mergedPackage.AddRange(Header);
            mergedPackage.AddRange(arrClientSendMsg);
            return mergedPackage.ToArray();
        }
    }
}
