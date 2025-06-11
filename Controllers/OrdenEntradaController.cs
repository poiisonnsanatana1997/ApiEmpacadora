using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppAPIEmpacadora.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdenEntradaController : ControllerBase
    {
        private readonly IOrdenEntradaService _ordenEntradaService;

        public OrdenEntradaController(IOrdenEntradaService ordenEntradaService)
        {
            _ordenEntradaService = ordenEntradaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdenEntradaDTO>>> GetOrdenesEntrada()
        {
            var ordenes = await _ordenEntradaService.ObtenerOrdenesEntradaAsync();
            return Ok(ordenes);
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<OrdenEntradaDTO>> GetOrdenEntrada(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
                return BadRequest("El código de la orden es requerido");

            var orden = await _ordenEntradaService.ObtenerOrdenEntradaPorCodigoAsync(codigo);
            if (orden == null)
                return NotFound($"No se encontró la orden con código {codigo}");

            return Ok(orden);
        }

        [HttpPost]
        public async Task<ActionResult<OrdenEntradaDTO>> CrearOrdenEntrada([FromBody] CrearOrdenEntradaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var usuarioRegistro = User.Identity?.Name ?? "sistema";
                var ordenCreada = await _ordenEntradaService.CrearOrdenEntradaAsync(dto, usuarioRegistro);
                return StatusCode(201, ordenCreada);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor al crear la orden de entrada");
            }
        }

        [HttpPut("{codigo}")]
        public async Task<ActionResult<OrdenEntradaDTO>> ActualizarOrdenEntrada(string codigo, [FromBody] CrearOrdenEntradaDTO dto)
        {
            if (string.IsNullOrEmpty(codigo))
                return BadRequest("El código de la orden es requerido");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var usuarioModificacion = User.Identity?.Name ?? "sistema";
                var ordenActualizada = await _ordenEntradaService.ActualizarOrdenEntradaAsync(codigo, dto, usuarioModificacion);
                if (ordenActualizada == null)
                    return NotFound($"No se encontró la orden con código {codigo}");

                return Ok(ordenActualizada);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor al actualizar la orden de entrada");
            }
        }

        [HttpDelete("{codigo}")]
        public async Task<ActionResult> EliminarOrdenEntrada(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
                return BadRequest("El código de la orden es requerido");

            var resultado = await _ordenEntradaService.EliminarOrdenEntradaAsync(codigo);
            if (!resultado)
                return NotFound($"No se encontró la orden con código {codigo}");

            return NoContent();
        }

        [HttpPost("{codigo}/tarimas")]
        public async Task<ActionResult<TarimaDTO>> CrearTarima(string codigo, [FromBody] TarimaDTO tarima)
        {
            if (string.IsNullOrEmpty(codigo))
                return BadRequest("El código de la orden es requerido");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tarimaCreada = await _ordenEntradaService.CrearTarimaAsync(codigo, tarima);
            if (tarimaCreada == null)
                return StatusCode(500, "No se pudo crear la tarima");

            return StatusCode(201, tarimaCreada);
        }

        [HttpGet("{codigo}/detalle")]
        public async Task<ActionResult<DetalleOrdenEntradaDTO>> GetDetalleOrdenEntrada(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
                return BadRequest("El código de la orden es requerido");

            var detalle = await _ordenEntradaService.ObtenerDetalleOrdenEntradaAsync(codigo);
            if (detalle == null)
                return NotFound($"No se encontró el detalle de la orden con código {codigo}");

            return Ok(detalle);
        }

        [HttpPut("{codigo}/tarimas/{numeroTarima}")]
        public async Task<ActionResult> ActualizarPesajeTarima(string codigo, string numeroTarima, [FromBody] ActualizarPesajeTarimaDTO dto)
        {
            if (string.IsNullOrEmpty(codigo))
                return BadRequest("El código de la orden es requerido");

            if (string.IsNullOrEmpty(numeroTarima))
                return BadRequest("El número de tarima es requerido");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await _ordenEntradaService.ActualizarPesajeTarimaAsync(codigo, numeroTarima, dto);
            if (!resultado)
                return NotFound($"No se encontró la tarima {numeroTarima} en la orden {codigo}");

            return NoContent();
        }

        [HttpDelete("{codigo}/tarimas/{numeroTarima}")]
        public async Task<ActionResult> EliminarTarima(string codigo, string numeroTarima)
        {
            if (string.IsNullOrEmpty(codigo))
                return BadRequest("El código de la orden es requerido");

            if (string.IsNullOrEmpty(numeroTarima))
                return BadRequest("El número de tarima es requerido");

            var resultado = await _ordenEntradaService.EliminarTarimaAsync(codigo, numeroTarima);
            if (!resultado)
                return NotFound($"No se encontró la tarima {numeroTarima} en la orden {codigo}");

            return NoContent();
        }

        [HttpGet("peso-total-hoy")]
        public async Task<ActionResult<PesoTotalDTO>> GetPesoTotalRecibidoHoy()
        {
            try
            {
                var pesoTotal = await _ordenEntradaService.ObtenerPesoTotalRecibidoHoyAsync();
                return Ok(new PesoTotalDTO { PesoTotal = pesoTotal });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor al obtener el peso total");
            }
        }

        [HttpGet("estadisticas/pendientes-hoy")]
        public async Task<ActionResult<CantidadPendientesDTO>> GetCantidadPendientesHoy()
        {
            try
            {
                var cantidadPendientes = await _ordenEntradaService.ObtenerCantidadPendientesHoyAsync();
                return Ok(new CantidadPendientesDTO { CantidadPendientes = cantidadPendientes });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor al obtener la cantidad de pendientes");
            }
        }
    }
} 