using JustMySocksService.Providers;

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
            var result = new ConfigService(null).GetServiceStatus(service, id);
            Console.WriteLine(result);
        }

        [TestMethod()]
        public void GetLastestConfigTest()
        {
            string service = "";
            string id = "";
            var result = new ConfigService(null).GetLastestConfig(service, id);
        }
    }
}