using JustMySocksProvider.Enums;
using JustMySocksProvider.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using System.Data;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace JustMySocksProvider
{
    public class ConfigProvider
    {
        private const string sbLink = "https://jmssub.net/members/getsub.php?service={service}&id={id}";
        private const string infoLink = "https://justmysocks5.net/members/getbwcounter.php?service={service}&id={id}";
        private static Regex DomainRegex = new Regex(@"(?<=@)(.+)(?=\:)");
        private static Regex SSInfoRegex = new Regex(@"(?<=ss://)(.+)(?=#)");

        public static ConfigProvider Instance
        {
            get
            {
                if (configProvider == null)
                {
                    configProvider = new ConfigProvider();
                }
                    
                return configProvider;
            }
        }

        private static ConfigProvider configProvider = null;

        private ConfigProvider() { }

        public string GetLastestConfig(string service, string id, bool useDomain = true)
        {
            var link = sbLink.Replace("{service}", service).Replace("{id}", id);
            if (useDomain)
                link += "&usedomains=1";

            var configText = Encoding.UTF8.GetString(ConfigResource.ClashConfigTemp);
            var subInfos = GetSubInfos(link);
            return ReplaceParamWith(configText, subInfos);
        }

        public string GetServiceStatus(string service, string id)
        {
            var info = GetServiceInfo(service, id);
            return $"upload=0; download={info.Used * 1.073741824d}; total={info.Limit * 1.073741824d}; expire={info.TimeStamp}";
        }

        public ServiceInfo GetServiceInfo(string service, string id)
        {
            var link = infoLink.Replace("{service}", service).Replace("{id}", id);
            //var data = "{\"monthly_bw_limit_b\":500000000000,\"bw_counter_b\":79018881709,\"bw_reset_day_of_month\":16}";
            var data = GetDataFromUrl(link);//"{\"monthly_bw_limit_b\":500000000000,\"bw_counter_b\":79018881709,\"bw_reset_day_of_month\":16}";
            if (string.IsNullOrEmpty(data)) return new ServiceInfo();

            var info = JsonConvert.DeserializeObject<ServiceInfo>(data);
            //Subscription-Userinfo: upload=2375927198; download=12983696043; total=1099511627776; expire=1862111613
            //1024 / 1000 = 1.024
            //1.024 * 1.024 * 1.024 =  1.073741824
            //Convert 1000 to 1024 by * 1.073741824
            DateTime expireTime;
            //Los Angeles time zone : UTC-7
            //Get current Los Angeles time
            TimeZoneInfo laZone = TimeZoneInfo.FindSystemTimeZoneById("US Mountain Standard Time");
            DateTime laTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, laZone);

            if (laTime.Day < info.ResetDay)
            {
                var thisMonth = laTime.AddDays(info.ResetDay - laTime.Day);
                //本月?号
                expireTime = new DateTime(thisMonth.Year, thisMonth.Month, thisMonth.Day);
            }

            else
            {
                var nextMonth = laTime.AddMonths(1);
                //次月?号
                expireTime = new DateTime(nextMonth.Year, nextMonth.Month, 1);
                expireTime = expireTime.AddDays(info.ResetDay - expireTime.Day);
            }

            var utcTime = expireTime - laZone.BaseUtcOffset;//LA time to UTC time
            var timeStamp = Convert.ToInt64((utcTime - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);
            info.TimeStamp = timeStamp;
            return info;
        }

        private static string GetDataFromUrl(string url)
        {
            HttpClient client = new HttpClient();
            var data = client.GetStringAsync(url).Result;

            if (string.IsNullOrEmpty(data))
                return string.Empty;

            return data;
        }

        private static string Base64Decode(string data)
        {
            data = data.Replace("\n", "");
            return Base64UrlEncoder.Encoder.Decode(data);
            //byte[] bytes = Convert.FromBase64String(data);
            //return Encoding.Default.GetString(bytes);
        }

        private static List<SubInfo> GetSubInfos(string url)
        {
            var result = new List<SubInfo>();

            var data = GetDataFromUrl(url);
            if (string.IsNullOrEmpty(data)) return result;

            data = Base64Decode(data);
            var datas = data.Split('\n');
            foreach (var subInfoStr in datas)
            {
                if (string.IsNullOrEmpty(subInfoStr)) continue;

                SubInfo subInfo = new SubInfo();
                if (subInfoStr.StartsWith("ss://"))
                {
                    subInfo.SubType = SubType.Shadowsocks;

                    var ssStr = Base64Decode(SSInfoRegex.Match(subInfoStr).Value);
                    var infos = ssStr.Split(':', '@');

                    subInfo.SSInfo = new SSInfo
                    {
                        cipher = infos[0],
                        password = infos[1],
                        server = infos[2],
                        port = infos[3],
                    };
                }
                if (subInfoStr.StartsWith("vmess://"))
                {
                    subInfo.SubType = SubType.V2ray;

                    var v2rayStr = Base64Decode(subInfoStr.Replace("vmess://", ""));

                    subInfo.v2RayInfo = JsonConvert.DeserializeObject<V2rayInfo>(v2rayStr);
                }
                result.Add(subInfo);
            }

            return result;
        }

        private static string ReplaceParamWith(string text, List<SubInfo> subInfos)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(text);

            int ssIndex = 1;
            int v2rIndex = 1;
            foreach (var info in subInfos)
            {
                if (info.SubType == SubType.Shadowsocks)
                {
                    /***server: {SSDomainReplace1}
                    port: {SSPortReplace1}
                    cipher: {SSCipherReplace1}
                    password: {SSPasswordReplace1}***/
                    string a = $"SSDomainReplace{ssIndex}";
                    string b = $"SSPortReplace{ssIndex}";
                    string c = $"SSCipherReplace{ssIndex}";
                    string d = $"SSPasswordReplace{ssIndex}";

                    builder = builder.Replace(a, info.SSInfo.server);
                    builder = builder.Replace(b, info.SSInfo.port);
                    builder = builder.Replace(c, info.SSInfo.cipher);
                    builder = builder.Replace(d, info.SSInfo.password);
                    ssIndex++;
                }
                if (info.SubType == SubType.V2ray)
                {
                    /**
                      server: {V2rayDomainReplace1}
                      port: {V2rayPortReplace1}
                      uuid: {V2rayUUIDReplace1}
                      alterId: {V2rayAIDReplace1}
                     * **/
                    string a = $"V2rayDomainReplace{v2rIndex}";
                    string b = $"V2rayPortReplace{v2rIndex}";
                    string c = $"V2rayUUIDReplace{v2rIndex}";
                    string d = $"V2rayAIDReplace{v2rIndex}";

                    builder = builder.Replace(a, info.v2RayInfo.add);
                    builder = builder.Replace(b, info.v2RayInfo.port);
                    builder = builder.Replace(c, info.v2RayInfo.id);
                    builder = builder.Replace(d, info.v2RayInfo.aid.ToString());
                    v2rIndex++;
                }
            }
            return builder.ToString();
        }

    }
}