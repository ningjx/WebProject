using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace JustMySocksService.Helpers
{
    public static class HttpHelper
    {
        public static async Task<string> GetDataFromUrlAsync(string url)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36 Edg/120.0.0.0");

            var data = await client.GetStringAsync(url);

            if (string.IsNullOrEmpty(data))
                throw new Exception($"无法从{url}获取数据");

            return data;
        }

        public static string UrlDecode(this string data)
        {
            if(string.IsNullOrEmpty(data))
                return string.Empty;

            return HttpUtility.UrlDecode(data);
        }
    }
}
