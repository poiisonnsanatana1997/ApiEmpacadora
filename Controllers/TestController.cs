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
            return Ok(new { message = "API funcionando correctamente", timestamp = DateTime.Now });
        }

        [HttpGet("swagger-test")]
        public IActionResult SwaggerTest()
        {
            return Ok(new { 
                message = "Endpoint de prueba para Swagger", 
                timestamp = DateTime.Now,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "No configurado"
            });
        }

        [HttpGet("swagger-status")]
        public IActionResult SwaggerStatus()
        {
            var swaggerUrl = $"{Request.Scheme}://{Request.Host}/swagger/v1/swagger.json";
            return Ok(new { 
                message = "Estado de Swagger", 
                swaggerUrl = swaggerUrl,
                timestamp = DateTime.Now,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "No configurado"
            });
        }
    }
} 