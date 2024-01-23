using JustMySocksService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static JustMySocksService.Helpers.HttpHelper;
using static JustMySocksService.Helpers.TextHelper;

namespace JustMySocksService.Services
{
    public class SubscribeConverterService : ISubscribeConverterService
    {
        public async Task<string> ConvertSubscribeAsync(string url)
        {
            var data = await GetDataFromUrlAsync(url);

            var ssSubs = SSSubscribeReg.Matches(data);
            if (ssSubs.Count > 0)
            {

            }
            return string.Empty;
        }
    }
}
