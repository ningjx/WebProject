using Microsoft.VisualStudio.TestTools.UnitTesting;
using JustMySocksService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JustMySocksService.Helpers.TextHelper;
using static JustMySocksService.Helpers.HttpHelper;

namespace JustMySocksService.Services.Tests
{
    [TestClass()]
    public class SubscribeConverterServiceTests
    {
        [TestMethod()]
        public void ConvertSubscribeAsyncTest()
        {
            var configService = new SubscribeConverterService(null);
            var test1 = configService.ConvertSubscribeAsync("").Result;
        }
    }
}