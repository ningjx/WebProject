using Microsoft.VisualStudio.TestTools.UnitTesting;
using JustMySocksProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustMySocksProvider.Tests
{
    [TestClass()]
    public class ConfigProviderTests
    {
        [TestMethod()]
        public void GetServiceInfoTest()
        {
            var zones = TimeZoneInfo.GetSystemTimeZones();
            string service = "";
            string id = "";
            var result = ConfigProvider.Instance.GetServiceInfo(service, id);
            Console.WriteLine(result);
        }
    }
}