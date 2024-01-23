using JustMySocksService.Models;
using JustMySocksService.Services;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace JustMySocksProvider.Tests
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