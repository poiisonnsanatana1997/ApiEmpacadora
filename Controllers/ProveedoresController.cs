using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Services.Interfaces;

namespace AppAPIEmpacadora.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProveedoresController : ControllerBase
    {
        private readonly IProveedorService _proveedorService;

        public ProveedoresController(IProveedorService proveedorService)
        {
            _proveedorService = proveedorService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProveedorSimpleDTO>>> Get()
        {
            var proveedores = await _proveedorService.ObtenerTodosAsync();
            return Ok(proveedores);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProveedorSimpleDTO>> GetById(int id)
        {
            var proveedor = await _proveedorService.ObtenerPorIdAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }
            return Ok(proveedor);
        }

        [HttpGet("completos")]
        public async Task<ActionResult<IEnumerable<ProveedorDTO>>> GetProveedoresCompletos()
        {
            var proveedores = await _proveedorService.GetProveedoresAsync();
            return Ok(proveedores);
        }

        [HttpGet("completo/{id}")]
        public async Task<ActionResult<ProveedorDTO>> GetProveedorCompleto(int id)
        {
            var proveedor = await _proveedorService.GetProveedorByIdAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }
            return Ok(proveedor);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ProveedorDTO>> PostProveedor([FromForm] CreateProveedorDTO createProveedorDto)
        {
            var usuario = User.Identity.Name;
            if (string.IsNullOrEmpty(usuario))
            {
                return Unauthorized("No se pudo obtener el nombre de usuario del token.");
            }

            try
            {
                var nuevoProveedor = await _proveedorService.CreateProveedorAsync(createProveedorDto, usuario);
                return CreatedAtAction(nameof(GetProveedorCompleto), new { id = nuevoProveedor.Id }, nuevoProveedor);
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error al crear el proveedor: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutProveedor(int id, [FromForm] UpdateProveedorDTO updateProveedorDto)
        {
            var usuario = User.Identity.Name;
            if (string.IsNullOrEmpty(usuario))
            {
                return Unauthorized("No se pudo obtener el nombre de usuario del token.");
            }

            var proveedorActualizado = await _proveedorService.UpdateProveedorAsync(id, updateProveedorDto, usuario);
            if (proveedorActualizado == null)
            {
                return NotFound();
            }
            return Ok(proveedorActualizado);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProveedor(int id)
        {
            var result = await _proveedorService.DeleteProveedorAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
} 