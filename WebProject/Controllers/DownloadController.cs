using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebProject.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    public class DownloadController : ControllerBase
    {

        [HttpGet()]
        public IActionResult GetDownload()
        {
            return Ok();
        }
    }
}
