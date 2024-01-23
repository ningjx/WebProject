using YamlDotNet.Serialization;

namespace JustMySocksService.Models
{
    public class SSProxy:BaseProxy
    {
        public SSProxy():base("ss") { }

        [YamlMember(Order = 5)]
        public string cipher;
        [YamlMember(Order = 6)]
        public string password;
        [YamlMember(Order = 7)]
        public bool udp = true;
    }
}
