using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace JustMySocksService.Models
{
    public class VmessProxyJMS : VmessProxy
    {
        public VmessProxyJMS()
        {
            base.cipher = "chacha20-poly1305";
        }
        [YamlIgnore]
        public string ps { get => name; set { name = value; } }
        [YamlIgnore]
        public string id { get => uuid; set { uuid = value; } }
        [YamlIgnore]
        public int aid { get => alterId; set { alterId = value; } }
        [YamlIgnore]
        public string net
        {
            get { return udp.ToString(); }
            set { udp = value != "tcp"; }
        }
        [YamlIgnore]
        public string sni;
        [YamlIgnore]
        public string add { get => server; set { server = value; } }

        [YamlIgnore]
        [JsonProperty("tls")]
        public string Tls_json
        {
            get { return tls.ToString(); }
            set { tls = value == "tls"; }
        }

        [YamlIgnore]
        [JsonProperty("skipCertVerify")]
        public bool SkipCertVerify_json
        {
            get => skipCertVerify;
            set { skipCertVerify = value; }
        }
    }
}
