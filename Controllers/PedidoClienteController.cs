using System;
using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

namespace AppAPIEmpacadora.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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

        [HttpGet("con-detalles")]
        public async Task<ActionResult<IEnumerable<PedidoClienteConDetallesDTO>>> GetAllWithDetails()
            => Ok(await _service.ObtenerTodosConDetallesAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<PedidoClienteConOrdenesResponseDTO>> GetById(int id)
        {
            var result = await _service.ObtenerPorIdConOrdenesAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("{id}/progreso")]
        public async Task<ActionResult<PedidoClienteProgresoDTO>> GetProgreso(int id)
        {
            var result = await _service.ObtenerProgresoAsync(id);
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

        [HttpPost("con-ordenes")]
        public async Task<ActionResult<PedidoClienteConOrdenesResponseDTO>> CreateWithOrders([FromBody] CreatePedidoClienteConOrdenesDTO dto)
        {
            var usuario = User.Identity?.Name ?? "sistema";
            var created = await _service.CrearConOrdenesAsync(dto, usuario);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdatePedidoClienteDTO dto)
        {
            var ok = await _service.ActualizarAsync(id, dto);
            if (!ok) return NotFound();
            return NoContent();
        }
        
        [HttpPatch("{id}/estatus")]
        public async Task<ActionResult> UpdateEstatus(int id, [FromBody] UpdateEstatusPedidoClienteDTO dto)
        {
            var usuario = User.Identity?.Name ?? "sistema";
            var ok = await _service.ActualizarEstatusAsync(id, dto.Estatus, usuario);
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

        [HttpGet("disponibles/{tipo}")]
        public async Task<ActionResult<IEnumerable<PedidoClientePorAsignarDTO>>> GetDisponiblesPorTipo(string tipo, [FromQuery] int? idProducto = null)
        {
            if (string.IsNullOrWhiteSpace(tipo))
            {
                return BadRequest("El tipo es requerido");
            }

            var resultado = await _service.ObtenerDisponiblesPorTipoAsync(tipo, idProducto);
            return Ok(resultado);
        }
    }
} 