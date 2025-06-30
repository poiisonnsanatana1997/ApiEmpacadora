using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TarimasController : ControllerBase
    {
        private readonly ITarimaService _tarimaService;

        public TarimasController(ITarimaService tarimaService)
        {
            _tarimaService = tarimaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TarimaDTO>>> GetTarimas()
        {
            var tarimas = await _tarimaService.GetTarimasAsync();
            return Ok(tarimas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TarimaDTO>> GetTarima(int id)
        {
            var tarima = await _tarimaService.GetTarimaByIdAsync(id);
            if (tarima == null)
            {
                return NotFound();
            }
            return Ok(tarima);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<TarimaDTO>> PostTarima(CreateTarimaDTO createTarimaDto)
        {
            var usuario = User.Identity.Name;
            if (string.IsNullOrEmpty(usuario))
            {
                return Unauthorized("No se pudo obtener el nombre de usuario del token.");
            }

            try
            {
                var nuevaTarima = await _tarimaService.CreateTarimaAsync(createTarimaDto, usuario);
                return CreatedAtAction(nameof(GetTarima), new { id = nuevaTarima.Id }, nuevaTarima);
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error al crear la tarima: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutTarima(int id, UpdateTarimaDTO updateTarimaDto)
        {
            var usuario = User.Identity.Name;
            if (string.IsNullOrEmpty(usuario))
            {
                return Unauthorized("No se pudo obtener el nombre de usuario del token.");
            }

            var tarimaActualizada = await _tarimaService.UpdateTarimaAsync(id, updateTarimaDto, usuario);
            if (tarimaActualizada == null)
            {
                return NotFound();
            }
            return Ok(tarimaActualizada);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTarima(int id)
        {
            var result = await _tarimaService.DeleteTarimaAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
} 