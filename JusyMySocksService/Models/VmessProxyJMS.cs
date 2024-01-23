using Microsoft.AspNetCore.SignalR.Protocol;

namespace JustMySocksService.Models
{
    public class VmessProxyJMS : VmessProxy
    {
        public VmessProxyJMS()
        {
            base.cipher = "chacha20-poly1305";
        }
        public string ps { get => name; set { name = value; } }
        public string id { get => uuid; set { uuid = value; } }
        public int aid { get => alterId; set { alterId = value; } }
        public string net
        {
            get { return udp.ToString(); }
            set { udp = value != "tcp"; }
        }
        public string sni;
        public string add { get => server; set { server = value; } }

        public new string tls
        {
            get { return base.tls.ToString(); }
            set { base.tls = value == "tls"; }
        }

        public new bool skipCertVerify
        {
            get => base.skipCertVerify;
            set { base.skipCertVerify = value; }
        }
    }
}
