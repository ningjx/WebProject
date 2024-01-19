using JustMySocksService.Services;

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
            var result = new ConfigService(null).GetServiceInfo(service, id).Result;
            Console.WriteLine(result);
        }

        [TestMethod()]
        public void GetLastestConfigTest()
        {
            string service = "";
            string id = "";
            var task1 = new ConfigService(null).GetLastestConfig(service, id);
            var res = task1.Result;
        }
    }
}