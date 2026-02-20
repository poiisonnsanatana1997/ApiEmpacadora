using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppAPIEmpacadora.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportesController : ControllerBase
    {
        private readonly IReporteServices _reporteServices;
        public ReportesController(IReporteServices reporteServices)
        {
            _reporteServices = reporteServices;
        }
        [HttpPost("ordenes-clasificadas")]
        public async Task<ActionResult<IEnumerable<ReporteClasificacionesDTO>>> GetReporteClientes(int[] ids)
        {
            try{
                var reportes = await _reporteServices.ObtenerReporteClasificacionesAsync(ids);
                return Ok(reportes);
            }catch(Exception ex){
                return BadRequest(ex.Message);
            }
        }
    }
}
