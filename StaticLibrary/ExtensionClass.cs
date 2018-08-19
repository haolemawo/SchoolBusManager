using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using WBPlatform.Logging;
using System.Text;
using WBPlatform.Database;
using WBPlatform.Database.IO;
using System.Security.Cryptography;

namespace WBPlatform.StaticClasses
{
    public static class ExtensionClass
    {
        public static string ToNormalString(this DateTime dateTime) => dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        public static string ToDetailedString(this DateTime dateTime) => dateTime.ToNormalString() + DateTime.Now.Millisecond.ToString("000");

        public static void CloseAndDispose(this Socket socket)
        {
            socket?.Disconnect(true);
            socket?.Close();
            socket?.Dispose();
            socket = null;
        }

        public static void CloseAndDispose(this TcpClient client)
        {
            client?.Close();
            client = null;
        }

        public static void CloseAndDispose(this NetworkStream stream)
        {
            stream?.Close();
            stream?.Dispose();
            stream = null;
        }


        public static byte[] ToBytesArray(this int n)
        {
            byte[] b = new byte[4];
            for (int i = 0; i < 4; i++) { b[i] = (byte)(n >> (24 - (i * 8))); }
            return b;
        }


        public static int ToInt32(this byte[] b)
        {
            int mask = 0xff;
            int temp = 0;
            int n = 0;
            for (int i = 0; i < b.Length; i++)
            {
                n <<= 8;
                temp = b[i] & mask;
                n |= temp;
            }
            return n;
        }

        public static T[] MoveToArray<T>(this T thing) => new T[] { thing };

        public static string ToParsedString<T>(this T value) => JsonConvert.SerializeObject(value, typeof(T), new JsonSerializerSettings());

        public static bool ToParsedObject<T>(this string JSON, out T data)
        {
            try
            {
                data = JsonConvert.DeserializeObject<T>(JSON);
                return data != null;
            }
            catch (Exception ex)
            {
                LW.E(ex.ToParsedString());
                data = default(T);
                return false;
            }
        }

        public static string GetString(this DataBaseIO io, string Key) => io.GetT<string>(Key);

        public static List<string> GetList(this DataBaseIO io, string Key)
        {
            string _listString = GetString(io, Key);
            return string.IsNullOrWhiteSpace(_listString) ? new List<string>() : _listString.Split(',').ToList();
        }

        public static bool GetBool(this DataBaseIO io, string Key) => io.GetT<bool>(Key);
        public static int GetInt(this DataBaseIO io, string Key) => io.GetT<int>(Key);
        public static DateTime GetDateTime(this DataBaseIO io, string Key) => io.GetT<DateTime>(Key);
        public static string SHA256Encrypt(this string strIN)
        {
            byte[] bytValue = Encoding.UTF8.GetBytes(strIN);
            try
            {
                SHA256 sha256 = new SHA256CryptoServiceProvider();
                byte[] retVal = sha256.ComputeHash(bytValue);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetSHA256HashFromString() fail, error: " + ex.Message);
            }
        }

        public static string SHA512Encrypt(this string strIN)
        {
            byte[] bytValue = Encoding.UTF8.GetBytes(strIN);
            try
            {
                SHA512 sha512 = new SHA512CryptoServiceProvider();
                byte[] retVal = sha512.ComputeHash(bytValue);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetSHA256HashFromString() fail, error: " + ex.Message);
            }
        }

    }
}
