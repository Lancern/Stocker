using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Stocker.WebAPI.Controllers
{
    [ApiController]
    [Route("echo")]
    public class EchoController : ControllerBase
    {
        [HttpGet]
        public ActionResult<object> Get([FromQuery][BindRequired] string message)
        {
            return new { Message = DateTime.Now.ToString("s") + message };
        }
    }
}
