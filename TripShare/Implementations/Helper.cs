using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace TripShare.Implementations
{
    public static class Helper
    {
        private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static uint ToTimestamp(this DateTime time)
        {
            return (uint)(time.ToUniversalTime() - unixEpoch).TotalSeconds;
        }

        public static string ReverseHex(string hex)
        {
            if (hex.Length % 2 != 0)
                return "";
            string m = "";
            for (int i = hex.Length - 2; i >= 0; i -= 2)
            {
                m += hex.Substring(i, 2);
            }
            return m;
        }

        public static string Str2Hex(string str)
        {
            byte[] ba = Encoding.UTF8.GetBytes(str);
            var hexString = BitConverter.ToString(ba);
            hexString = hexString.Replace("-", "");
            return hexString;
        }

        public static string Hex2Str(string hex)
        {
            byte[] da = Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();

            var str = Encoding.UTF8.GetString(da);
            return str;
        }

        public static string Num2Hex(int num)
        {
            var hex = num.ToString("X2");
            return hex;
        }

        public static int Hex2Num(string hex)
        {
            int num = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return num;
        }

        public static string GetKeyWithPostfix(string info, string postfix)
        {
            return "";
        }
    }
}
