using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppAPIEmpacadora.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdenPedidoClienteController : ControllerBase
    {
        private readonly IOrdenPedidoClienteService _service;
        
        public OrdenPedidoClienteController(IOrdenPedidoClienteService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdenPedidoClienteResponseDTO>>> GetAll()
            => Ok(await _service.ObtenerTodosAsync());

        [HttpGet("con-detalles")]
        public async Task<ActionResult<IEnumerable<OrdenPedidoClienteConDetallesDTO>>> GetAllWithDetails()
            => Ok(await _service.ObtenerTodosConDetallesAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<OrdenPedidoClienteResponseDTO>> GetById(int id)
        {
            var result = await _service.ObtenerPorIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("pedido-cliente/{idPedidoCliente}")]
        public async Task<ActionResult<IEnumerable<OrdenPedidoClienteResponseDTO>>> GetByPedidoCliente(int idPedidoCliente)
            => Ok(await _service.ObtenerPorPedidoClienteAsync(idPedidoCliente));

        [HttpPost]
        public async Task<ActionResult<OrdenPedidoClienteResponseDTO>> Create([FromBody] CreateOrdenPedidoClienteDTO dto)
        {
            var usuario = User.Identity?.Name ?? "sistema";
            var created = await _service.CrearAsync(dto, usuario);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateOrdenPedidoClienteDTO dto)
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