using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Services.Interfaces;

namespace AppAPIEmpacadora.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MermaController : ControllerBase
    {
        private readonly IMermaService _service;
        public MermaController(IMermaService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MermaResponseDTO>>> GetAll()
            => Ok(await _service.ObtenerTodosAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<MermaResponseDTO>> GetById(int id)
        {
            var result = await _service.ObtenerPorIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult<MermaResponseDTO>> Create([FromBody] CreateMermaDTO dto)
        {
            var usuario = User.Identity?.Name ?? "sistema";
            var dtoConUsuario = new CreateMermaDTO {
                Tipo = dto.Tipo,
                Peso = dto.Peso,
                Observaciones = dto.Observaciones,
                FechaRegistro = dto.FechaRegistro,
                IdClasificacion = dto.IdClasificacion
            };
            var created = await _service.CrearAsync(dtoConUsuario, usuario);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateMermaDTO dto)
        {
            var ok = await _service.ActualizarAsync(id, dto);
            if (!ok) return NotFound();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var ok = await _service.EliminarAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
} 