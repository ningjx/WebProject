using Newtonsoft.Json;

namespace JustMySocksProvider.Models
{
    public class ServiceInfo
    {
        [JsonProperty("monthly_bw_limit_b")]
        public long Limit { get; set; }
        [JsonProperty("bw_reset_day_of_month")]
        public int ResetDay { get; set; }
        [JsonProperty("bw_counter_b")]
        public long Used { get; set; }
    }
}
