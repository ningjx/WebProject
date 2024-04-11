using JustMySocksService.Helpers;
using JustMySocksService.Interfaces;
using JustMySocksService.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using YamlDotNet.Serialization;
using static JustMySocksService.Helpers.HttpHelper;
using static JustMySocksService.Helpers.TextHelper;
using ILogger = Serilog.ILogger;

namespace JustMySocksService.Services
{
    public class SubscribeConverterService : ISubscribeConverterService
    {
        private readonly ILogger<SubscribeConverterService> _logger;

        public SubscribeConverterService(ILogger<SubscribeConverterService> logger)
        {
            _logger = logger;
        }

        public async Task<string> ConvertSubscribeAsync(string url)
        {
            var proxies = GetProxiesFromUrlAsync(url);

            var configText = GetDataFromUrlAsync(ConfigTempUrl, _logger);

            await Task.WhenAll(proxies, configText);

            return BuildConfig(configText.Result, proxies.Result);
        }

        private async Task<List<BaseProxy>> GetProxiesFromUrlAsync(string url)
        {
            var result = new List<BaseProxy>();

            var data = await GetDataFromUrlAsync(url, _logger);
            data = data.Base64Decode();

            var ssSubs = SSSubscribeReg.Matches(data);
            if (ssSubs.Count > 0)
            {

            }

            var vmessSubs = VmessSubscribeReg.Matches(data);
            if (vmessSubs.Count > 0)
            {

            }

            var trojanSubs = TrojanSubscribeReg.Matches(data);
            if (trojanSubs.Count > 0)
            {
                foreach (var trojanSub in trojanSubs.Cast<Match>())
                {
                    var trojanProxy = new TrojanProxy
                    {
                        name = TrojanNameReg.Match(trojanSub.Value).Value.UrlDecode().RemoveEnter(),
                        server = TrojanServerReg.Match(trojanSub.Value).Value.UrlDecode(),
                        port = TrojanPortReg.Match(trojanSub.Value).Value.UrlDecode(),

                        password = TrojanPwdReg.Match(trojanSub.Value).Value.UrlDecode(),
                        sni = TrojanSniReg.Match(trojanSub.Value).Value.UrlDecode(),
                        udp = TrojanUdpReg.Match(trojanSub.Value).Value.UrlDecode().ToLower() != "tcp",
                        allowInsecure = TrojanInsecReg.Match(trojanSub.Value).Value.UrlDecode().ConvertToBool()
                    };
                    result.Add(trojanProxy);
                }
            }
            return result;
        }

        private string BuildConfig(string text, List<BaseProxy> proxies)
        {
            var configBuilder = new StringBuilder();
            configBuilder.Append(text);

            configBuilder.Insert(0, $"#配置更新时间：{DateTime.Now:yyyy-MM-dd hh:mm:ss fff}\n");

            var yamlSerializer = new SerializerBuilder().WithNewLine("\n    ").Build();

            var proxyList = string.Empty;
            var proxyNames = string.Empty;

            foreach (var proxy in proxies)
            {
                proxyList += "\n  - ";
                proxyList += $"{yamlSerializer.Serialize(proxy)}";
                proxyNames += $"      - {proxy.name}{(proxies.Last() != proxy ? "\n" : string.Empty)}";
            }

            configBuilder.Replace("{ProxyList}", proxyList);
            configBuilder.Replace("{ProxiesNames}", proxyNames);

            return configBuilder.ToString();
        }
    }
}
