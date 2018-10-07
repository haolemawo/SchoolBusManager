using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace WBPlatform.StaticClasses
{
    public static class Cryptography
    {
        private const long tsMagicNumber = 621355968000000000;

        public static DateTime FromUnixTimestamp(this DateTime dt, double d)
        {
            DateTime time = DateTime.MinValue;
            DateTime startTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.GetSystemTimeZones()[0]);
            time = startTime.AddMilliseconds(d);
            return time;
        }
        public static string AsUnixTimeStamp(this DateTime dt) => ((dt.ToUniversalTime().Ticks - tsMagicNumber) / 10000000).ToString();
        public static string CurrentTimeStamp => ((DateTime.Now.ToUniversalTime().Ticks - tsMagicNumber) / 10000000).ToString();
        public static string RandomString(int Length, bool Symbols, string CustomStr = "")
        {
            byte[] b = new byte[4];
            new RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, f = CustomStr;
            f += "0123456789";
            f += "abcdefghijklmnopqrstuvwxyz";
            f += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (Symbols) f += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}";
            for (int i = 0; i < Length; i++)
            {
                s += f.Substring(r.Next(0, f.Length - 1), 1);
            }
            return s;
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}