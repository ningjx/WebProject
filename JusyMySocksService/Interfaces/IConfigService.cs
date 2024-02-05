using JustMySocksService.Models;

namespace JustMySocksService.Interfaces
{
    public interface IConfigService
    {
        public Task<string> GetLastestConfigAsync(string service, string id, bool useDomain = true);

        public Task<ServiceInfo> GetServiceInfoAsync(string service, string id, bool convertValue = false);
    }
}
