using YamlDotNet.Serialization;

namespace JustMySocksService.Models
{
    public class TrojanProxy : BaseProxy
    {
        public TrojanProxy() : base("trojan") { }

        [YamlMember(Order = 5)]
        public string password;
        [YamlMember(Order = 6)]
        public bool udp = false;
        [YamlMember(Order = 7)]
        public string sni;
        [YamlMember(Order = 8, Alias = "skip-cert-verify")]
        public bool skipCertVerify = true;
    }
}
