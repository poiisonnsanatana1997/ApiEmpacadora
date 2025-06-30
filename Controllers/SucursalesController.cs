using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SucursalesController : ControllerBase
    {
        private readonly ISucursalService _sucursalService;

        public SucursalesController(ISucursalService sucursalService)
        {
            _sucursalService = sucursalService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SucursalDTO>>> GetSucursales()
        {
            var sucursales = await _sucursalService.GetSucursalesAsync();
            return Ok(sucursales);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SucursalDTO>> GetSucursal(int id)
        {
            var sucursal = await _sucursalService.GetSucursalByIdAsync(id);
            if (sucursal == null)
            {
                return NotFound();
            }
            return Ok(sucursal);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<SucursalDTO>> PostSucursal(CreateSucursalDTO createSucursalDto)
        {
            var usuario = User.Identity.Name;
            if (string.IsNullOrEmpty(usuario))
            {
                return Unauthorized("No se pudo obtener el nombre de usuario del token.");
            }

            var nuevaSucursal = await _sucursalService.CreateSucursalAsync(createSucursalDto, usuario);
            return CreatedAtAction(nameof(GetSucursal), new { id = nuevaSucursal.Id }, nuevaSucursal);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSucursal(int id, UpdateSucursalDTO updateSucursalDto)
        {
            var sucursalActualizada = await _sucursalService.UpdateSucursalAsync(id, updateSucursalDto);
            if (sucursalActualizada == null)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSucursal(int id)
        {
            var result = await _sucursalService.DeleteSucursalAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
} 