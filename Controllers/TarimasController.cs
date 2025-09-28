using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppAPIEmpacadora.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TarimasController : ControllerBase
    {
        private readonly ITarimaService _tarimaService;
        private readonly ITarimaPesoService _tarimaPesoService;

        public TarimasController(ITarimaService tarimaService, ITarimaPesoService tarimaPesoService)
        {
            _tarimaService = tarimaService;
            _tarimaPesoService = tarimaPesoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TarimaDTO>>> GetTarimas()
        {
            var tarimas = await _tarimaService.GetTarimasAsync();
            return Ok(tarimas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TarimaDTO>> GetTarima(int id)
        {
            var tarima = await _tarimaService.GetTarimaByIdAsync(id);
            if (tarima == null)
            {
                return NotFound();
            }
            return Ok(tarima);
        }

        [HttpGet("parciales")]
        public async Task<ActionResult<IEnumerable<TarimaParcialCompletaDTO>>> GetTarimasParciales()
        {
            try
            {
                var tarimasParciales = await _tarimaService.GetTarimasParcialesAsync();
                return Ok(tarimasParciales);
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error al obtener las tarimas parciales: {ex.Message}");
            }
        }

        [HttpGet("parciales-completas")]
        public async Task<ActionResult<IEnumerable<TarimaParcialCompletaDTO>>> GetTarimasParcialesYCompletas()
        {
            try
            {
                var tarimas = await _tarimaService.GetTarimasParcialesYCompletasAsync();
                return Ok(tarimas);
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error al obtener las tarimas parciales y completas: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CreateTarimaResponseDTO>> PostTarima(CreateTarimaDTO createTarimaDto)
        {
            var usuario = User.Identity.Name;
            if (string.IsNullOrEmpty(usuario))
            {
                return Unauthorized("No se pudo obtener el nombre de usuario del token.");
            }

            try
            {
                var resultado = await _tarimaService.CreateTarimaAsync(createTarimaDto, usuario);
                
                // Procesar resumen después de crear tarimas
                await _tarimaPesoService.ProcesarResumenDiarioAsync(DateTime.Today, usuario);
                
                return CreatedAtAction(nameof(GetTarima), new { id = resultado.TarimasCreadas.FirstOrDefault()?.Id }, resultado);
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error al crear las tarimas: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutTarima(int id, UpdateTarimaDTO updateTarimaDto)
        {
            var usuario = User.Identity.Name;
            if (string.IsNullOrEmpty(usuario))
            {
                return Unauthorized("No se pudo obtener el nombre de usuario del token.");
            }

            var tarimaActualizada = await _tarimaService.UpdateTarimaAsync(id, updateTarimaDto, usuario);
            if (tarimaActualizada == null)
            {
                return NotFound();
            }
            return Ok(tarimaActualizada);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTarima(int id)
        {
            var result = await _tarimaService.DeleteTarimaAsync(id);
            if (!result)
            {
                return BadRequest();
            }
            return NoContent();
        }

        // Controllers/TarimasController.cs
        [HttpDelete("{idTarima}/clasificacion/{idClasificacion}")]
        [Authorize]
        public async Task<IActionResult> EliminarTarimaClasificacion(int idTarima, int idClasificacion)
        {
            try
            {
                var result = await _tarimaService.EliminarTarimaClasificacionAsync(idTarima, idClasificacion);
                if (!result)
                {
                    return BadRequest("No se encontró la relación especificada");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("parcial/{idTarima}")]
        [Authorize]
        public async Task<ActionResult<TarimaDTO>> UpdateTarimaParcial(int idTarima, TarimaUpdateParcialDTO dto)
        {
            var usuario = User.Identity.Name;
            if (string.IsNullOrEmpty(usuario))
            {
                return Unauthorized("No se pudo obtener el nombre de usuario del token.");
            }

            // Validar que el ID de la tarima en el DTO coincida con el de la URL
            if (dto.IdTarima != idTarima)
            {
                return BadRequest("El ID de la tarima en el DTO no coincide con el ID de la URL.");
            }

            try
            {
                var tarimaActualizada = await _tarimaService.UpdateTarimaParcialAsync(dto, usuario);
                return Ok(tarimaActualizada);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Error de validación: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("buscarPedidosCompatibles")]
        [Authorize]
        public async Task<ActionResult<List<PedidoClienteResponseDTO>>> BuscarPedidosCompatibles(List<TarimaAsignacionRequestDTO> tarimasAsignacion)
        {
            try
            {
                var pedidosDisponibles = await _tarimaService.BuscarPedidosCompatiblesAsync(tarimasAsignacion);
                return Ok(pedidosDisponibles);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Error de validación: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // Endpoints para Dashboard y Gráficas
        [HttpGet("dashboard")]
        public async Task<ActionResult<TarimaDashboardDTO>> GetDashboard([FromQuery] DateTime fecha)
        {
            try
            {
                var dashboard = await _tarimaPesoService.ObtenerDatosDashboardAsync(fecha);
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener datos del dashboard: {ex.Message}");
            }
        }

        [HttpGet("peso/diario-por-tipo")]
        public async Task<ActionResult<IEnumerable<TarimaPesoDiarioDTO>>> GetPesoDiarioPorTipo([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                var pesoDiario = await _tarimaPesoService.ObtenerPesoDiarioPorTipoAsync(fechaInicio, fechaFin);
                return Ok(pesoDiario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener peso diario por tipo: {ex.Message}");
            }
        }

        [HttpGet("peso/por-tipo-en-mes")]
        public async Task<ActionResult<IEnumerable<TarimaPesoPorTipoDTO>>> GetPesoPorTipoEnMes([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                var pesoPorTipo = await _tarimaPesoService.ObtenerPesoPorTipoEnMesAsync(fechaInicio, fechaFin);
                return Ok(pesoPorTipo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener peso por tipo en mes: {ex.Message}");
            }
        }

        [HttpGet("peso/grafica-diario-por-tipo")]
        public async Task<ActionResult<Dictionary<string, List<PesoDiarioDTO>>>> GetPesoDiarioPorTipoParaGrafica([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                var pesoParaGrafica = await _tarimaPesoService.ObtenerPesoDiarioPorTipoParaGraficaAsync(fechaInicio, fechaFin);
                return Ok(pesoParaGrafica);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener peso diario por tipo para gráfica: {ex.Message}");
            }
        }

        [HttpGet("graficas/evolucion-diaria")]
        public async Task<ActionResult<IEnumerable<TarimaEvolucionDTO>>> GetEvolucionDiaria([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                var evolucion = await _tarimaPesoService.ObtenerEvolucionDiariaAsync(fechaInicio, fechaFin);
                return Ok(evolucion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener evolución diaria: {ex.Message}");
            }
        }

        [HttpGet("graficas/evolucion-semanal")]
        public async Task<ActionResult<IEnumerable<TarimaEvolucionDTO>>> GetEvolucionSemanal([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                var evolucion = await _tarimaPesoService.ObtenerEvolucionSemanalAsync(fechaInicio, fechaFin);
                return Ok(evolucion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener evolución semanal: {ex.Message}");
            }
        }

        [HttpGet("graficas/evolucion-mensual")]
        public async Task<ActionResult<IEnumerable<TarimaEvolucionDTO>>> GetEvolucionMensual([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                var evolucion = await _tarimaPesoService.ObtenerEvolucionMensualAsync(fechaInicio, fechaFin);
                return Ok(evolucion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener evolución mensual: {ex.Message}");
            }
        }

        [HttpGet("graficas/comparativa")]
        public async Task<ActionResult<IEnumerable<TarimaComparativaDTO>>> GetComparativaEntrePeriodos(
            [FromQuery] DateTime fechaInicio1, 
            [FromQuery] DateTime fechaFin1,
            [FromQuery] DateTime fechaInicio2, 
            [FromQuery] DateTime fechaFin2)
        {
            try
            {
                var comparativa = await _tarimaPesoService.ObtenerComparativaEntrePeriodosAsync(fechaInicio1, fechaFin1, fechaInicio2, fechaFin2);
                return Ok(comparativa);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener comparativa: {ex.Message}");
            }
        }

        [HttpGet("graficas/eficiencia-por-tipo")]
        public async Task<ActionResult<Dictionary<string, decimal>>> GetEficienciaPorTipo([FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin)
        {
            try
            {
                var eficiencia = await _tarimaPesoService.ObtenerEficienciaPorTipoAsync(fechaInicio, fechaFin);
                return Ok(eficiencia);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener eficiencia por tipo: {ex.Message}");
            }
        }
    }
} 