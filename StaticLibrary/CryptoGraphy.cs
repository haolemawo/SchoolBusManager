using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace WBPlatform.StaticClasses
{
    public static class Cryptography
    {
        public static string RandomString(int Length, bool Symbols, string CustomStr = "")
        {
            byte[] b = new byte[4];
            new RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, f = CustomStr;
            f += "0123456789";
            f += "abcdefghijklmnopqrstuvwxyz";
            f += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (Symbols) f += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}（）_——-~·、，。；‘“’”《》￥……~";
            for (int i = 0; i < Length; i++)
            {
                s += f.Substring(r.Next(0, f.Length - 1), 1);
            }
            return s;
        }
    }
}