using Microsoft.AspNetCore.Mvc;

namespace AppAPIEmpacadora.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Get()
        {
            return Ok("API funcionando correctamente");
        }

        [HttpGet("cors")]
        public ActionResult<object> TestCors()
        {
            return Ok(new { 
                message = "CORS está funcionando correctamente",
                timestamp = DateTime.Now,
                origin = Request.Headers["Origin"].ToString()
            });
        }

        [HttpPost("test")]
        public ActionResult<object> TestPost([FromBody] object data)
        {
            return Ok(new { 
                message = "POST funcionando correctamente",
                receivedData = data,
                timestamp = DateTime.Now
            });
        }
    }
} 