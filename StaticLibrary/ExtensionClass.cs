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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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

        public static T DeepCloneObject<T>(this object _object)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, _object);
            stream.Position = 0;
            return (T)formatter.Deserialize(stream);
        }


        public static string Middle(this string str, string After, string Before)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return "";
            }
            string result = "";
            if (str.Contains(After))
            {
                string s1 = str.Split(After)[1];
                if (Before == null || "" == Before)
                {
                    result = s1;
                }
                else
                {
                    string[] s2 = s1.Split(Before);
                    if (s2 != null)
                    {
                        result = s2[0];
                    }
                }
            }
            return result;
        }
        public static string[] Split(this string str, string Saperator) => str.Split(new string[] { Saperator }, StringSplitOptions.None);
        public static List<T[]> BreakDown<T>(this T[] data, int size)
        {
            List<T[]> list = new List<T[]>();
            for (int i = 0; i < data.Length / size; i++)
            {
                T[] r = new T[size];
                Array.Copy(data, i * size, r, 0, size);
                list.Add(r);
            }
            if (data.Length % size != 0)
            {
                T[] r = new T[data.Length % size];
                Array.Copy(data, data.Length - (data.Length % size), r, 0, data.Length % size);
                list.Add(r);
            }
            return list;
        }

        //合并任意类型数据
        public static T[] MergeArray<T>(this T[][] arraies)
        {
            List<T> list = new List<T>();
            foreach (T[] item in arraies)
            {
                for (int i = 0; i < item.Length; i++) list.Add(item[i]);
            }
            T[] r = new T[list.Count];
            int n = 0;
            foreach (T x in list)
            {
                r[n++] = x;
            }
            return r;
        }

        public static string ToUnicodeString(this string str)
        {
            StringBuilder strResult = new StringBuilder();
            if (!string.IsNullOrEmpty(str))
            {
                for (int i = 0; i < str.Length; i++)
                {
                    strResult.Append("\\u");
                    strResult.Append(((int)str[i]).ToString("x"));
                }
            }
            return strResult.ToString();
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
            data = JsonConvert.DeserializeObject<T>(JSON);
            return data != null;
        }
        public static string ToBase64(this string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
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

        public static string SHAHashEncrypt<TProvider>(this string strIN) where TProvider : HashAlgorithm, new()
        {
            byte[] bytValue = Encoding.UTF8.GetBytes(strIN);
            try
            {
                HashAlgorithm hashAlgo = new TProvider();
                byte[] retVal = hashAlgo.ComputeHash(bytValue);
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

        public static string SHA256Encrypt(this string strIN) => SHAHashEncrypt<SHA256CryptoServiceProvider>(strIN);
        public static string SHA384Encrypt(this string strIN) => SHAHashEncrypt<SHA384CryptoServiceProvider>(strIN);
        public static string SHA512Encrypt(this string strIN) => SHAHashEncrypt<SHA512CryptoServiceProvider>(strIN);
    }
}
