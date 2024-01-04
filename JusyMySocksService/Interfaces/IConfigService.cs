using JustMySocksService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustMySocksService.Interfaces
{
    public interface IConfigService
    {
        public string GetLastestConfig(string service, string id, bool useDomain = true);

        public string GetServiceStatus(string service, string id);

        public ServiceInfo GetServiceInfo(string service, string id);
    }
}
