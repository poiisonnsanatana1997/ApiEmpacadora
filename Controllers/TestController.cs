using Microsoft.AspNetCore.Mvc;

namespace AppAPIEmpacadora.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "GET funciona correctamente", timestamp = DateTime.Now });
        }

        [HttpPost]
        public IActionResult Post([FromBody] object data)
        {
            return Ok(new { message = "POST funciona correctamente", data, timestamp = DateTime.Now });
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] object data)
        {
            return Ok(new { message = "PUT funciona correctamente", id, data, timestamp = DateTime.Now });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok(new { message = "DELETE funciona correctamente", id, timestamp = DateTime.Now });
        }

        [HttpOptions]
        public IActionResult Options()
        {
            return Ok(new { message = "OPTIONS funciona correctamente", timestamp = DateTime.Now });
        }
    }
} 