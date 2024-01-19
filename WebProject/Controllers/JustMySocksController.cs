using JustMySocksService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace WebProject.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    public class JustMySocksController : Controller
    {
        private readonly ILogger<JustMySocksController> _logger;
        private readonly IConfigService _configService;

        public JustMySocksController(ILogger<JustMySocksController> logger, IConfigService configService)
        {
            _logger = logger;
            _configService = configService;
        }

        [HttpGet()]
        //下面的标签不能动，很多地方用到了这个链接
        [Route("/JustMySocks")]
        public ActionResult GetConfig([FromQuery] string service, [FromQuery] string id, [FromQuery] bool useDomain = true)
        {
            try
            {
                _logger.LogInformation($"获取服务器配置，service:{service},id:{id},useDomain:{useDomain},IP:{HttpContext.Request.Headers["X-Real-Ip"]}");

                if (string.IsNullOrEmpty(service) || string.IsNullOrEmpty(id))
                    return BadRequest();

                var info = _configService.GetServiceInfo(service, id);
                var config = _configService.GetLastestConfig(service, id, useDomain);
                info.Start();
                config.Start();

                HttpContext.Response.Headers.Add("Subscription-Userinfo", info.Result);
                return File(Encoding.UTF8.GetBytes(config.Result), "APPLICATION/octet-stream", "ClashConfig.yaml");
            }
            catch (Exception ex)
            {
                _logger.LogError(500, ex, ex.Message);
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("/JustMySocks/GetServiceStatus")]
        public ActionResult GetServiceStatus([FromQuery] string service, [FromQuery] string id)
        {
            try
            {
                //var test = HttpContext.Request.Headers["X-Real-Ip"].FirstOrDefault();
                _logger.LogInformation($"获取剩余流量，service:{service},id:{id},IP:{HttpContext.Request.Headers["X-Real-Ip"]}");

                if (string.IsNullOrEmpty(service) || string.IsNullOrEmpty(id))
                    return BadRequest();

                var info = _configService.GetServiceInfo(service, id);
                info.Start();
                HttpContext.Response.Headers.Add("Subscription-Userinfo", info.Result);
                return Ok(JsonConvert.SerializeObject(info));
            }
            catch (Exception ex)
            {
                _logger.LogError(500, ex, ex.Message);
                return BadRequest();
            }
        }
    }
}
