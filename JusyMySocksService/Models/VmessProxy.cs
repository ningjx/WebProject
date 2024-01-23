using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using YamlDotNet.Serialization;

namespace JustMySocksService.Models
{
    public class VmessProxy : BaseProxy
    {
        public VmessProxy() : base("vmess") { }

        [YamlMember(Order = 5)]
        public string uuid;
        [YamlMember(Order = 6)]
        public int alterId;
        [YamlMember(Order = 7)]
        public string cipher;
        [YamlMember(Order = 8)]
        public bool udp = false;
        [YamlMember(Order = 9)]
        public bool tls = true;
        [YamlMember(Order = 10, Alias = "skip-cert-verify")]
        public bool skipCertVerify = true;
    }
}
