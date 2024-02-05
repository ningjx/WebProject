using Newtonsoft.Json;

namespace JustMySocksService.Models
{
    public class ServiceInfo
    {
        [JsonProperty("monthly_bw_limit_b")]
        public long Limit { get; set; } = 0;
        [JsonProperty("bw_reset_day_of_month")]
        public int ResetDay { get; set; } = 0;
        [JsonProperty("bw_counter_b")]
        public long Used { get; set; } = 0;

        public long TimeStamp { get; set; } = 0;

        public override string ToString() => $"upload=0; download={Used}; total={Limit}; expire={TimeStamp}";
    }
}
