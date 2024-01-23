using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JustMySocksService.Helpers
{
    public static class TextHelper
    {
        public const string ConfigTempUrl = "http://gh.con.sh/https://raw.githubusercontent.com/ningjx/Clash-Rules/master/ClashConfigTemp.yaml";

        public static Regex VmessSubscribeReg = new Regex(@"((?<=^vmess://).+?$)", RegexOptions.Multiline);

        public static Regex SSSubscribeReg = new Regex(@"((?<=^ss://).+?$)", RegexOptions.Multiline);

        public static Regex TrojanSubscribeReg = new Regex(@"((?<=^trojan://).+?$)", RegexOptions.Multiline);
        public static Regex TrojanPwdReg = new Regex(@"((.+?)(?=@))");
        public static Regex TrojanServerReg = new Regex(@"((?<=@)(.+?)(?=:))");
        public static Regex TrojanPortReg = new Regex(@"(?<=:)(\d+?)(?=\?)");
        public static Regex TrojanArgReg = new Regex(@"(?<=\?)(.+)(?=#)");
        public static Regex TrojanNameReg = new Regex(@"(?<=#)(.+)$");
        public static Regex TrojanSniReg = new Regex(@"(?<=sni=)(.+?)(?=&|#)");
        public static Regex TrojanUdpReg = new Regex(@"(?<=type=)(.+?)(?=&|#)");
        public static Regex TrojanInsecReg = new Regex(@"(?<=allowInsecure=)(.+?)(?=&|#)");

        public static string Base64Decode(this string data)
        {
            data = data.Replace("\n", "");
            return Base64UrlEncoder.Encoder.Decode(data);
            //byte[] bytes = Convert.FromBase64String(data);
            //return Encoding.Default.GetString(bytes);
        }

        public static bool ConvertToBool(this object data)
        {
            if (data == null)
                return false;

            var str = data.ToString();

            if (string.IsNullOrEmpty(str))
                return false;

            if (str == "1" || str.ToLower().Replace(" ", "") == "true" || str == "是")
                return true;

            return false;
        }

        public static string RemoveEnter(this string data)
        {
            if (string.IsNullOrEmpty(data))
                return string.Empty;

            return data.Replace("\n", "").Replace("\r", "");
        }
    }
}
