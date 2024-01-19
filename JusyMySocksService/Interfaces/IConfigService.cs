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
        public Task<string> GetLastestConfigAsync(string service, string id, bool useDomain = true);

        public Task<string> GetServiceInfoAsync(string service, string id, bool convertValue = false);
    }
}
