using Microsoft.VisualStudio.TestTools.UnitTesting;
using JustMySocksService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustMySocksService.Helpers.Tests
{
    [TestClass()]
    public class HttpHelperTests
    {
        [TestMethod()]
        public void GetDataFromUrlAsyncTest()
        {
            string ConfigTempUrl = "http://gh.con.sh/https://raw.githubusercontent.com/ningjx/Clash-Rules/master/ClashConfigTemp.yaml";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36 Edg/120.0.0.0");
            var data = client.GetStringAsync(ConfigTempUrl).Result;

        }
    }
}