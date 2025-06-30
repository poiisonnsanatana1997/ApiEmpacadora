using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CajaClienteController : ControllerBase
    {
        private readonly ICajaClienteService _cajaClienteService;

        public CajaClienteController(ICajaClienteService cajaClienteService)
        {
            _cajaClienteService = cajaClienteService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var cajas = await _cajaClienteService.GetCajasClienteAsync();
            return Ok(cajas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var caja = await _cajaClienteService.GetCajaClienteByIdAsync(id);
            if (caja == null)
            {
                return NotFound();
            }
            return Ok(caja);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateCajaClienteDTO createCajaClienteDTO)
        {
            var nuevaCaja = await _cajaClienteService.CreateCajaClienteAsync(createCajaClienteDTO);
            return CreatedAtAction(nameof(Get), new { id = nuevaCaja.Id }, nuevaCaja);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateCajaClienteDTO updateCajaClienteDTO)
        {
            var cajaActualizada = await _cajaClienteService.UpdateCajaClienteAsync(id, updateCajaClienteDTO);
            if (cajaActualizada == null)
            {
                return NotFound();
            }
            return Ok(cajaActualizada);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _cajaClienteService.DeleteCajaClienteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
} 