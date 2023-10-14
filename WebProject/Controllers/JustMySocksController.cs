using JustMySocksProvider;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace WebProject.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    public class JustMySocksController : Controller
    {
        private readonly ILogger<JustMySocksController> _logger;

        public JustMySocksController(ILogger<JustMySocksController> logger)
        {
            _logger = logger;
        }

        [HttpGet()]
        //下面的标签不能动，很多地方用到了这个链接
        [Route("/JustMySocks")]
        public ActionResult GetConfig([FromQuery] string service, [FromQuery] string id, [FromQuery] bool useDomain = true)
        {
            _logger.LogInformation($"GetConfig，service:{service},id:{id},useDomain:{useDomain}");

            if (string.IsNullOrEmpty(service) || string.IsNullOrEmpty(id))
                return BadRequest();

            var contentType = "APPLICATION/octet-stream";

            var text = ConfigProvider.Instance.GetLastestConfig(service, id, useDomain);
            var info = ConfigProvider.Instance.GetServiceStatus(service,id);

            HttpContext.Response.Headers.Add("Subscription-Userinfo", info);

            return File(Encoding.UTF8.GetBytes(text), contentType, "ClashConfig.yaml");
        }

        [HttpGet]
        [Route("/JustMySocks/GetServiceStatus")]
        public ActionResult GetServiceStatus([FromQuery] string service, [FromQuery] string id)
        {
            _logger.LogInformation($"GetServiceStatus，service:{service},id:{id}");

            if(string.IsNullOrEmpty(service) || string.IsNullOrEmpty(id))
                return BadRequest();

            var info = ConfigProvider.Instance.GetServiceInfo(service, id);
            HttpContext.Response.Headers.Add("Subscription-Userinfo", $"upload=0; download={info.Used * 1.073741824d}; total={info.Limit * 1.073741824d}; expire={info.TimeStamp}");
            return Ok(JsonConvert.SerializeObject(info));
        }
    }
}
