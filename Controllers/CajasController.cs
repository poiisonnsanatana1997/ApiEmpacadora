using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace AppAPIEmpacadora.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CajasController : ControllerBase
    {
        private readonly ICajaService _cajaService;

        public CajasController(ICajaService cajaService)
        {
            _cajaService = cajaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CajaSummaryDTO>>> GetCajas()
        {
            var cajas = await _cajaService.GetCajasAsync();
            return Ok(cajas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CajaDTO>> GetCaja(int id)
        {
            var caja = await _cajaService.GetCajaByIdAsync(id);
            if (caja == null)
            {
                return NotFound();
            }
            return Ok(caja);
        }

        [HttpGet("por-clasificacion/{idClasificacion}")]
        public async Task<ActionResult<IEnumerable<CajaDTO>>> GetCajasPorClasificacion(int idClasificacion)
        {
            var cajas = await _cajaService.GetCajasByClasificacionAsync(idClasificacion);
            return Ok(cajas);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CajaDTO>> PostCaja([FromBody] CreateCajaDTO createCajaDto)
        {
            var usuario = User.Identity.Name;
            if (string.IsNullOrEmpty(usuario))
            {
                return Unauthorized("No se pudo obtener el nombre de usuario del token.");
            }

            var nuevaCaja = await _cajaService.CreateCajaAsync(createCajaDto, usuario);
            return CreatedAtAction(nameof(GetCaja), new { id = nuevaCaja.Id }, nuevaCaja);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCaja(int id, [FromBody] UpdateCajaDTO updateCajaDto)
        {
            var cajaActualizada = await _cajaService.UpdateCajaAsync(id, updateCajaDto);
            if (cajaActualizada == null)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCaja(int id)
        {
            var result = await _cajaService.DeleteCajaAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost("ajustar-cantidad")]
        [Authorize]
        public async Task<ActionResult<CajaDTO>> AjustarCantidadCaja([FromBody] AjustarCantidadCajaDTO ajusteDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = User.Identity.Name;
            if (string.IsNullOrEmpty(usuario))
            {
                return Unauthorized("No se pudo obtener el nombre de usuario del token.");
            }

            try
            {
                var cajaAjustada = await _cajaService.AjustarCantidadCajaAsync(ajusteDto, usuario);
                return Ok(cajaAjustada);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
} 