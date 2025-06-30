using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Services.Interfaces;

namespace AppAPIEmpacadora.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidoClienteController : ControllerBase
    {
        private readonly IPedidoClienteService _service;
        public PedidoClienteController(IPedidoClienteService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PedidoClienteResponseDTO>>> GetAll()
            => Ok(await _service.ObtenerTodosAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<PedidoClienteResponseDTO>> GetById(int id)
        {
            var result = await _service.ObtenerPorIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult<PedidoClienteResponseDTO>> Create([FromBody] CreatePedidoClienteDTO dto)
        {
            var usuario = User.Identity?.Name ?? "sistema";
            var dtoConUsuario = new CreatePedidoClienteDTO {
                Observaciones = dto.Observaciones,
                Estatus = dto.Estatus,
                FechaEmbarque = dto.FechaEmbarque,
                IdSucursal = dto.IdSucursal,
                IdCliente = dto.IdCliente,
                FechaRegistro = dto.FechaRegistro,
                Activo = dto.Activo
            };
            var created = await _service.CrearAsync(dtoConUsuario, usuario);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdatePedidoClienteDTO dto)
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