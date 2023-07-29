﻿using JustMySocksProvider.Enums;
using JustMySocksProvider.Models;
using Newtonsoft.Json;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace JustMySocksProvider
{
    public class ConfigProvider
    {
        private const string sbLink = "https://jmssub.net/members/getsub.php?service={service}&id={id}";
        private const string infoLink = "https://justmysocks5.net/members/getbwcounter.php?service={service}&id={id}";
        private static Regex DomainRegex = new Regex(@"(?<=@)(.+)\.(.+).(.+)(?=\:)");
        private static Regex SSInfoRegex = new Regex(@"(?<=ss://)(.+)(?=#)");

        public static ConfigProvider Instance
        {
            get
            {
                if (configProvider == null)
                    configProvider = new ConfigProvider();
                return configProvider;
            }
        }

        private static ConfigProvider configProvider = null;

        private ConfigProvider() { }

        public string GetLastestConfig(string service, string id, bool useDomain = true)
        {
            var link = sbLink.Replace("{service}", service).Replace("{id}", id);

            var configText = Encoding.UTF8.GetString(ConfigResource.ClashConfigTemp);
            var subInfos = GetSubInfos(link);
            return ReplaceParamWith(configText, subInfos, useDomain);
        }

        public string GetServiceInfo(string service, string id)
        {
            var link = infoLink.Replace("{service}", service).Replace("{id}", id);
            var data = GetDataFromUrl(link);
            if (string.IsNullOrEmpty(data)) return string.Empty;

            var info = JsonConvert.DeserializeObject<ServiceInfo>(data);
            //Subscription-Userinfo: upload=2375927198; download=12983696043; total=1099511627776; expire=1862111613
            //1024 / 1000 = 1.024
            //1.024 * 1.024 * 1.024 =  1.073741824
            //Convert 1000 to 1024 by * 1.073741824
            DateTime expireTime;
            var startTime  = new DateTime(1970, 1, 1, 0, 0, 0);
            //Los Angeles time zone : UTC-7
            //Get current Los Angeles time
            TimeZoneInfo pstZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            DateTime pdtTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pstZone);
            if (pdtTime.Day < info.ResetDay)
                expireTime = pdtTime.AddDays(info.ResetDay - pdtTime.Day);//本月?号
            else
            {
                var nextMonth = pdtTime.AddMonths(1);
                //次月?号
                expireTime = new DateTime(nextMonth.Year, nextMonth.Month, 1, nextMonth.Hour, nextMonth.Minute, nextMonth.Second).AddDays(info.ResetDay);
            }

            var timeStamp = Convert.ToInt64((expireTime.ToUniversalTime() - startTime).TotalSeconds);
            return $"upload=0; download={info.Used * 1.073741824d}; total={info.Limit * 1.073741824d}; expire={timeStamp}";
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
                    var domain = DomainRegex.Match(subInfoStr).Value;

                    var ssStr = Base64Decode(SSInfoRegex.Match(subInfoStr).Value);

                    var infos = ssStr.Split(':', '@');
                    subInfo.SSInfo = new SSInfo
                    {
                        cipher = infos[0],
                        password = infos[1],
                        ip = infos[2],
                        port = infos[3],
                        server = domain
                    };
                }
                if (subInfoStr.StartsWith("vmess://"))
                {
                    subInfo.SubType = SubType.V2ray;

                    var v2rayStr = Base64Decode(subInfoStr.Replace("vmess://", ""));

                    subInfo.v2RayInfo = JsonConvert.DeserializeObject<V2rayInfo>(v2rayStr);
                    subInfo.v2RayInfo.server = DomainRegex.Match(subInfo.v2RayInfo.ps).Value;
                }
                result.Add(subInfo);
            }

            return result;
        }

        private static string ReplaceParamWith(string text, List<SubInfo> subInfos, bool useDomain = true)
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

                    builder = builder.Replace(a, useDomain == true ? info.SSInfo.server : info.SSInfo.ip);
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

                    builder = builder.Replace(a, useDomain == true ? info.v2RayInfo.server : info.v2RayInfo.add);
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