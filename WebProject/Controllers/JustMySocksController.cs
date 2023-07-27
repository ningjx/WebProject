using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace WebProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JustMySocksController : ControllerBase
    {
        [HttpGet(Name = "GetConfigByName")]
        public ActionResult GetConfig(string service, string id, bool useDomain = true)
        {
            var contentType = "APPLICATION/octet-stream";
            var text = JustMySocksProvider.ConfigProvider.Instance.GetLastestConfig(service, id, useDomain);
            return File(Encoding.UTF8.GetBytes(text), contentType, "ClashConfig.yaml");
        }
    }
}
