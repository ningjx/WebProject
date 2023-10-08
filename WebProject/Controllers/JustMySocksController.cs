using JustMySocksProvider;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace WebProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JustMySocksController : ControllerBase
    {
        private readonly ILogger<JustMySocksController> _logger;

        public JustMySocksController(ILogger<JustMySocksController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetConfigByName")]
        public ActionResult GetConfig(string service, string id, bool useDomain = true)
        {
            _logger.LogInformation($"获取配置，service:{service},id:{id},useDomain:{useDomain}");
            
            var contentType = "APPLICATION/octet-stream";

            var text = ConfigProvider.Instance.GetLastestConfig(service, id, useDomain);
            var info = ConfigProvider.Instance.GetServiceInfo(service,id);

            HttpContext.Response.Headers.Add("Subscription-Userinfo", info);

            return File(Encoding.UTF8.GetBytes(text), contentType, "ClashConfig.yaml");
        }
    }
}
