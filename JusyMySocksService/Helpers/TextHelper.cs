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
        public static Regex DomainReg = new Regex(@"(?<=@)(.+)(?=\:)");

        public static Regex SSSubscribeReg = new Regex(@"((?<=^ss://).+?$)", RegexOptions.Multiline);
        public static Regex VmessSubscribeReg = new Regex(@"((?<=^vmess://).+?$)", RegexOptions.Multiline);
        public static Regex TrojanSubscribeReg = new Regex(@"((?<=^trojan://).+?$)", RegexOptions.Multiline);

        public static Regex SSInfoReg = new Regex(@"^(.+?):(.+?)@(.+?):(\d+)$");
        public static Regex TrojanInfoReg = new Regex(@"^(.+?):(.+?)@(.+?):(\d+)$");

        public static string Base64Decode(this string data)
        {
            data = data.Replace("\n", "");
            return Base64UrlEncoder.Encoder.Decode(data);
            //byte[] bytes = Convert.FromBase64String(data);
            //return Encoding.Default.GetString(bytes);
        }
    }
}
