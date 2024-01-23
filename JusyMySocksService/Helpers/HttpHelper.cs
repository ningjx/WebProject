using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustMySocksService.Helpers
{
    public static class HttpHelper
    {
        public static async Task<string> GetDataFromUrlAsync(string url)
        {
            HttpClient client = new HttpClient();
            var data = await client.GetStringAsync(url);

            if (string.IsNullOrEmpty(data))
                throw new Exception($"无法从{url}获取数据");

            return data;
        }
    }
}
