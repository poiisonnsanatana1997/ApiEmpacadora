using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            try
            {
                var usuario = User.Identity?.Name ?? "sistema";
                var ok = await _service.ActualizarEstatusAsync(id, dto.Estatus, usuario);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor al actualizar el estatus del pedido");
            }
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

        [HttpGet("{idPedido}/disponibilidad-cajas")]
        public async Task<ActionResult<PedidoClientePorAsignarDTO>> GetDisponibilidadCajasPorPedido(
            int idPedido, 
            [FromQuery] string tipo, 
            [FromQuery] int? idProducto = null)
        {
            if (string.IsNullOrWhiteSpace(tipo))
            {
                return BadRequest("El tipo es requerido");
            }

            var resultado = await _service.ObtenerDisponibilidadCajasPorPedidoAsync(idPedido, tipo, idProducto);
            if (resultado == null)
            {
                return NotFound("No se encontró el pedido o no cumple con los criterios especificados");
            }
            
            return Ok(resultado);
        }

        [HttpPost("{pedidoId}/asignar-tarimas")]
        public async Task<ActionResult<PedidoClienteResponseDTO>> AsignarTarimasYCalcularSurtido(
            int pedidoId, 
            [FromBody] List<int> tarimaIds)
        {
            if (tarimaIds == null || !tarimaIds.Any())
            {
                return BadRequest("Se requiere una lista de tarimas para asignar");
            }

            try
            {
                var usuario = User.Identity?.Name ?? "sistema";
                var resultado = await _service.AsignarTarimasYCalcularSurtidoAsync(pedidoId, tarimaIds, usuario);
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("desasignar-tarimas")]
        public async Task<ActionResult> DesasignarTarimas(
            [FromBody] List<DesasignacionTarimaDTO> desasignaciones)
        {
            if (desasignaciones == null || !desasignaciones.Any())
            {
                return BadRequest("Se requiere una lista de desasignaciones para realizar");
            }

            try
            {
                var usuario = User.Identity?.Name ?? "sistema";
                await _service.DesasignarTarimasAsync(desasignaciones, usuario);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
} 