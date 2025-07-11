using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

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

        [HttpGet("parciales")]
        public async Task<ActionResult<IEnumerable<TarimaParcialCompletaDTO>>> GetTarimasParciales()
        {
            try
            {
                var tarimasParciales = await _tarimaService.GetTarimasParcialesAsync();
                return Ok(tarimasParciales);
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error al obtener las tarimas parciales: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CreateTarimaResponseDTO>> PostTarima(CreateTarimaDTO createTarimaDto)
        {
            var usuario = User.Identity.Name;
            if (string.IsNullOrEmpty(usuario))
            {
                return Unauthorized("No se pudo obtener el nombre de usuario del token.");
            }

            try
            {
                var resultado = await _tarimaService.CreateTarimaAsync(createTarimaDto, usuario);
                return CreatedAtAction(nameof(GetTarima), new { id = resultado.TarimasCreadas.FirstOrDefault()?.Id }, resultado);
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error al crear las tarimas: {ex.Message}");
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

        [HttpPut("parcial/{idTarima}")]
        [Authorize]
        public async Task<ActionResult<TarimaDTO>> UpdateTarimaParcial(int idTarima, TarimaUpdateParcialDTO dto)
        {
            var usuario = User.Identity.Name;
            if (string.IsNullOrEmpty(usuario))
            {
                return Unauthorized("No se pudo obtener el nombre de usuario del token.");
            }

            // Validar que el ID de la tarima en el DTO coincida con el de la URL
            if (dto.IdTarima != idTarima)
            {
                return BadRequest("El ID de la tarima en el DTO no coincide con el ID de la URL.");
            }

            try
            {
                var tarimaActualizada = await _tarimaService.UpdateTarimaParcialAsync(dto, usuario);
                return Ok(tarimaActualizada);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Error de validación: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
} 