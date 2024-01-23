using Microsoft.VisualStudio.TestTools.UnitTesting;
using JustMySocksService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace JustMySocksService.Services.Tests
{
    [TestClass()]
    public class ConfigServiceTests
    {
        [TestMethod()]
        public void GetServiceInfoTest()
        {
            var zones = TimeZoneInfo.GetSystemTimeZones();
            string service = "";
            string id = "";
            var result = new ConfigService(null).GetServiceInfoAsync(service, id).Result;
            Console.WriteLine(result);
        }

        [TestMethod()]
        public void GetLastestConfigTest()
        {
            string service = "";
            string id = "";
            var task1 = new ConfigService(null).GetLastestConfigAsync(service, id);
            var res = task1.Result;
        }
    }
}