using JustMySocksService.Helpers;
using JustMySocksService.Interfaces;
using JustMySocksService.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
using static JustMySocksService.Helpers.HttpHelper;
using static JustMySocksService.Helpers.TextHelper;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace JustMySocksService.Services
{
    public class ConfigService : IConfigService
    {
        private const string sbLink = "https://jmssub.net/members/getsub.php?service={0}&id={1}&usedomains={2}";
        private const string infoLink = "https://justmysocks3.net/members/getbwcounter.php?service={0}&id={1}";

        private static Regex SSInfoBase64Reg = new Regex(@"^(.+)(?=#)");
        private static Regex SSInfoReg = new Regex(@"^(.+?):(.+?)@(.+?):(\d+)$");

        private readonly ILogger _logger;

        public ConfigService(ILogger<ConfigService> logger)
        {
            _logger = logger;
        }

        public async Task<string> GetLastestConfigAsync(string service, string id, bool useDomain = true)
        {
            var link = string.Format(sbLink, service, id, useDomain ? 1 : 0);

            var configText = GetDataFromUrlAsync(ConfigTempUrl, _logger);
            var subInfos = GetProxiesFromUrlAsync(link);

            await Task.WhenAll(configText, subInfos);

            return BuildConfig(configText.Result, subInfos.Result);
        }

        public async Task<ServiceInfo> GetServiceInfoAsync(string service, string id, bool convertValue = false)
        {
            var info = await GetServiceInfo(service, id);
            //将数据从1000转换1024
            if (convertValue)
            {
                info.Used = (long)(info.Used * 1.073741824d);
                info.Limit = (long)(info.Limit * 1.073741824d);
            } 
            return info;
        }


        private async Task<ServiceInfo> GetServiceInfo(string service, string id)
        {
            var link = string.Format(infoLink, service, id);
            //var data = "{\"monthly_bw_limit_b\":500000000000,\"bw_counter_b\":79018881709,\"bw_reset_day_of_month\":16}";
            var data = await GetDataFromUrlAsync(link, _logger);//"{\"monthly_bw_limit_b\":500000000000,\"bw_counter_b\":79018881709,\"bw_reset_day_of_month\":16}";

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

        private static string[] ProxyNames = new string[] { "美国1", "美国2", "美国3", "日本", "荷兰", "美国0.1倍流量" };

        private async Task<List<BaseProxy>> GetProxiesFromUrlAsync(string url)
        {
            var result = new List<BaseProxy>();

            var data = await GetDataFromUrlAsync(url, _logger);
            data = data.Base64Decode();

            var ssSubs = SSSubscribeReg.Matches(data);
            if (ssSubs.Count > 0)
            {
                foreach (Match ssSub in ssSubs)
                {
                    var ssStr = SSInfoBase64Reg.Match(ssSub.Value).Value.Base64Decode();
                    var ssInfo = SSInfoReg.Match(ssStr);

                    var ssProxy = new SSProxy()
                    {
                        cipher = ssInfo.Groups[1].Value,
                        password = ssInfo.Groups[2].Value,
                        server = ssInfo.Groups[3].Value,
                        port = ssInfo.Groups[4].Value,
                    };

                    result.Add(ssProxy);
                }
            }

            var vmessSubs = VmessSubscribeReg.Matches(data);
            if (vmessSubs.Count > 0)
            {
                foreach (Match vmessSub in vmessSubs)
                {
                    var v2rayStr = vmessSub.Value.Base64Decode();

                    var vmessProxyJMS = JsonConvert.DeserializeObject<VmessProxyJMS>(v2rayStr);

                    result.Add(vmessProxyJMS);
                }
            }

            return result;
        }

        private string BuildConfig(string text, List<BaseProxy> proxies)
        {
            var configBuilder = new StringBuilder();
            configBuilder.Append(text);
            
            configBuilder.Insert(0, $"#配置更新时间：{DateTime.Now:yyyy-MM-dd hh:mm:ss fff}");

            var yamlSerializer = new SerializerBuilder().WithNewLine("\n    ").Build();

            var proxyNameStack = new Stack<string>(ProxyNames.Reverse());

            var proxyList = string.Empty;

            foreach (var proxy in proxies)
            {
                if (proxy == null)
                    continue;

                proxy.name = proxyNameStack.Pop();
                proxyList += "  - ";
                proxyList += $"{yamlSerializer.Serialize(proxy)}\n";
            }
            configBuilder.Replace("{ProxyList}", proxyList);

            var proxyNames = string.Empty;
            foreach (var name in ProxyNames)
            {
                proxyNames += $"      - {name}{(ProxyNames.Last() != name ? "\n" : string.Empty)}";
            }
            configBuilder.Replace("{ProxiesNames}", proxyNames);

            return configBuilder.ToString();
        }
    }
}