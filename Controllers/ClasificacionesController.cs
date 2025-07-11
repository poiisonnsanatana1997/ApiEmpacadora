using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClasificacionesController : ControllerBase
    {
        private readonly IClasificacionService _clasificacionService;

        public ClasificacionesController(IClasificacionService clasificacionService)
        {
            _clasificacionService = clasificacionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClasificacionDTO>>> GetClasificaciones()
        {
            var clasificaciones = await _clasificacionService.GetClasificacionesAsync();
            return Ok(clasificaciones);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClasificacionDTO>> GetClasificacion(int id)
        {
            var clasificacion = await _clasificacionService.GetClasificacionByIdAsync(id);
            if (clasificacion == null)
            {
                return NotFound();
            }
            return Ok(clasificacion);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ClasificacionDTO>> PostClasificacion(CreateClasificacionDTO createClasificacionDto)
        {
            var usuario = User.Identity.Name;
            if (string.IsNullOrEmpty(usuario))
            {
                return Unauthorized("No se pudo obtener el nombre de usuario del token.");
            }

            var nuevaClasificacion = await _clasificacionService.CreateClasificacionAsync(createClasificacionDto, usuario);
            if (nuevaClasificacion == null)
            {
                return BadRequest("No se pudo crear la clasificación. Verifique que la orden de entrada exista y tenga productos y pesajes asociados.");
            }
            return CreatedAtAction(nameof(GetClasificacion), new { id = nuevaClasificacion.Id }, nuevaClasificacion);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutClasificacion(int id, UpdateClasificacionDTO updateClasificacionDto)
        {
            var clasificacionActualizada = await _clasificacionService.UpdateClasificacionAsync(id, updateClasificacionDto);
            if (clasificacionActualizada == null)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClasificacion(int id)
        {
            var result = await _clasificacionService.DeleteClasificacionAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost("{id}/ajustar-peso")]
        [Authorize]
        public async Task<ActionResult<AjustePesoClasificacionResponseDTO>> AjustarPesoClasificacion(int id, AjustePesoClasificacionDTO ajusteDto)
        {
            var usuario = User.Identity.Name;
            if (string.IsNullOrEmpty(usuario))
            {
                return Unauthorized("No se pudo obtener el nombre de usuario del token.");
            }

            var resultado = await _clasificacionService.AjustarPesoClasificacionAsync(id, ajusteDto, usuario);
            
            if (!resultado.AjusteRealizado)
            {
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }
    }
} 