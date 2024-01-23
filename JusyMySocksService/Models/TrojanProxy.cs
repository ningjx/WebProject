using System.Reflection.Metadata;
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
        [YamlMember(Order = 8, Alias = "allow-insecure")]
        public bool allowInsecure;
        [YamlMember(Order = 9, Alias = "skip-cert-verify")]
        public bool skipCertVerify = true;
    }
}
