using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteSummaryDTO>>> GetClientes()
        {
            var clientes = await _clienteService.GetClientesAsync();
            return Ok(clientes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDTO>> GetCliente(int id)
        {
            var cliente = await _clienteService.GetClienteByIdAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return Ok(cliente);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ClienteDTO>> PostCliente([FromForm] CreateClienteDTO createClienteDto)
        {
            var usuario = User.Identity.Name;
            if (string.IsNullOrEmpty(usuario))
            {
                return Unauthorized("No se pudo obtener el nombre de usuario del token.");
            }

            var nuevoCliente = await _clienteService.CreateClienteAsync(createClienteDto, usuario);
            return CreatedAtAction(nameof(GetCliente), new { id = nuevoCliente.Id }, nuevoCliente);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCliente(int id, [FromForm] UpdateClienteDTO updateClienteDto)
        {
            var clienteActualizado = await _clienteService.UpdateClienteAsync(id, updateClienteDto);
            if (clienteActualizado == null)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var result = await _clienteService.DeleteClienteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
} 