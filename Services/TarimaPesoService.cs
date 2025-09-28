using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Repositories.Interfaces;
using AppAPIEmpacadora.Services.Interfaces;

namespace AppAPIEmpacadora.Services
{
    public class TarimaPesoService : ITarimaPesoService
    {
        private readonly ITarimaRepository _tarimaRepository;
        private readonly ITarimaResumenRepository _tarimaResumenRepository;
        private readonly ILoggingService _loggingService;
        
        public TarimaPesoService(
            ITarimaRepository tarimaRepository,
            ITarimaResumenRepository tarimaResumenRepository,
            ILoggingService loggingService)
        {
            _tarimaRepository = tarimaRepository;
            _tarimaResumenRepository = tarimaResumenRepository;
            _loggingService = loggingService;
        }
        
        public async Task ProcesarResumenDiarioAsync(DateTime fecha, string usuario)
        {
            // Validaciones de parámetros de entrada
            if (string.IsNullOrWhiteSpace(usuario))
                throw new ArgumentException("El usuario no puede estar vacío", nameof(usuario));
            
            if (usuario.Length > 50)
                throw new ArgumentException("El usuario no puede exceder 50 caracteres", nameof(usuario));
            
            if (fecha > DateTime.Today)
                throw new ArgumentException("No se puede procesar fechas futuras", nameof(fecha));
            
            if (fecha < DateTime.Today.AddYears(-2))
                throw new ArgumentException("No se puede procesar fechas anteriores a 2 años", nameof(fecha));

            try
            {
                var fechaInicio = fecha.Date;
                var fechaFin = fechaInicio.AddDays(1);
                
                // Obtener datos de tarimas del día
                var datosTarimas = await _tarimaRepository.GetDatosResumenDiarioAsync(fechaInicio, fechaFin);
                
                // Obtener todos los resúmenes existentes de una vez (optimización: 1 query en lugar de N)
                var resumenesExistentes = await _tarimaResumenRepository.GetByFechaAsync(fechaInicio);
                var resumenesExistentesDict = resumenesExistentes.ToDictionary(r => r.Tipo, r => r);
                
                // Preparar listas para operaciones en lote
                var resumenesParaActualizar = new List<TarimaResumenDiario>();
                var resumenesParaCrear = new List<TarimaResumenDiario>();
                var fechaActual = DateTime.Now;
                
                // Procesar cada tipo en memoria
                foreach (var dato in datosTarimas)
                {
                    if (resumenesExistentesDict.TryGetValue(dato.Tipo, out TarimaResumenDiario resumenExistente))
                    {
                        // Actualizar resumen existente
                        resumenExistente.CantidadTarimas = dato.CantidadTarimas;
                        resumenExistente.CantidadTotal = dato.CantidadTotal;
                        resumenExistente.PesoTotal = dato.PesoTotal;
                        resumenExistente.TarimasCompletas = dato.TarimasCompletas;
                        resumenExistente.TarimasParciales = dato.TarimasParciales;
                        resumenExistente.UltimaActualizacion = fechaActual;
                        resumenExistente.UsuarioUltimaActualizacion = usuario;
                        
                        resumenesParaActualizar.Add(resumenExistente);
                    }
                    else
                    {
                        // Crear nuevo resumen
                        var nuevoResumen = new TarimaResumenDiario
                        {
                            Fecha = fechaInicio,
                            Tipo = dato.Tipo,
                            CantidadTarimas = dato.CantidadTarimas,
                            CantidadTotal = dato.CantidadTotal,
                            PesoTotal = dato.PesoTotal,
                            TarimasCompletas = dato.TarimasCompletas,
                            TarimasParciales = dato.TarimasParciales,
                            FechaRegistro = fechaActual,
                            UsuarioRegistro = usuario,
                            UltimaActualizacion = fechaActual
                        };
                        
                        resumenesParaCrear.Add(nuevoResumen);
                    }
                }
                
                // Ejecutar operaciones en transacción para garantizar consistencia
                await _tarimaResumenRepository.ExecuteInTransactionAsync(async () =>
                {
                    if (resumenesParaActualizar.Any())
                    {
                        await _tarimaResumenRepository.UpdateBatchAsync(resumenesParaActualizar);
                    }
                    
                    if (resumenesParaCrear.Any())
                    {
                        await _tarimaResumenRepository.AddBatchAsync(resumenesParaCrear);
                    }
                });
                
                // Logging de éxito con información detallada
                _loggingService.LogInformation("Resumen diario procesado exitosamente para la fecha: {Fecha}. " +
                    "Tipos procesados: {Tipos}, Actualizados: {Actualizados}, Creados: {Creados}", 
                    fechaInicio,
                    string.Join(", ", datosTarimas.Select(d => d.Tipo)),
                    resumenesParaActualizar.Count, 
                    resumenesParaCrear.Count);
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error al procesar resumen diario para la fecha: {Fecha}. " +
                    "Operación revertida por transacción. Usuario: {Usuario}", ex, fecha, usuario);
                throw;
            }
        }
        
        public async Task<IEnumerable<TarimaPesoDiarioDTO>> ObtenerPesoDiarioPorTipoAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var resumenes = await _tarimaResumenRepository.GetByRangoFechasAsync(fechaInicio, fechaFin);
            
            var pesoDiario = resumenes
                .GroupBy(r => r.Fecha)
                .Select(g => new TarimaPesoDiarioDTO
                {
                    Fecha = g.Key,
                    PesoPorTipo = g.ToDictionary(r => r.Tipo, r => r.PesoTotal),
                    PesoTotal = g.Sum(r => r.PesoTotal)
                })
                .OrderBy(p => p.Fecha)
                .ToList();
            
            return pesoDiario;
        }
        
        public async Task<IEnumerable<TarimaPesoPorTipoDTO>> ObtenerPesoPorTipoEnMesAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var resumenes = await _tarimaResumenRepository.GetByRangoFechasAsync(fechaInicio, fechaFin);
            
            var pesoPorTipo = resumenes
                .GroupBy(r => r.Tipo)
                .Select(g => new TarimaPesoPorTipoDTO
                {
                    Tipo = g.Key,
                    PesosDiarios = g.OrderBy(r => r.Fecha)
                        .Select(r => new PesoDiarioDTO
                        {
                            Fecha = r.Fecha,
                            Peso = r.PesoTotal,
                            Etiqueta = r.Fecha.ToString("dd/MM")
                        })
                        .ToList(),
                    PesoTotalMes = g.Sum(r => r.PesoTotal),
                    PesoPromedioDiario = g.Average(r => r.PesoTotal)
                })
                .ToList();
            
            return pesoPorTipo;
        }
        
        public async Task<Dictionary<string, List<PesoDiarioDTO>>> ObtenerPesoDiarioPorTipoParaGraficaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var resumenes = await _tarimaResumenRepository.GetByRangoFechasAsync(fechaInicio, fechaFin);
            
            var pesoParaGrafica = resumenes
                .GroupBy(r => r.Tipo)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(r => r.Fecha)
                        .Select(r => new PesoDiarioDTO
                        {
                            Fecha = r.Fecha,
                            Peso = r.PesoTotal,
                            Etiqueta = r.Fecha.ToString("dd/MM")
                        })
                        .ToList()
                );
            
            return pesoParaGrafica;
        }
        
        public async Task<TarimaDashboardDTO> ObtenerDatosDashboardAsync(DateTime fecha)
        {
            var resumenes = await _tarimaResumenRepository.GetByFechaAsync(fecha);
            
            var totalTarimas = resumenes.Sum(r => r.CantidadTarimas);
            var totalCompletas = resumenes.Sum(r => r.TarimasCompletas);
            var eficienciaGeneral = totalTarimas > 0 ? (decimal)totalCompletas / totalTarimas * 100 : 0;
            
            var distribucionPorTipos = resumenes.ToDictionary(r => r.Tipo, r => r.CantidadTarimas);
            var eficienciaPorTipo = resumenes.ToDictionary(r => r.Tipo, r => 
                r.CantidadTarimas > 0 ? (decimal)r.TarimasCompletas / r.CantidadTarimas * 100 : 0);
            
            return new TarimaDashboardDTO
            {
                TotalTarimasHoy = totalTarimas,
                EficienciaGeneral = eficienciaGeneral,
                DistribucionPorTipos = distribucionPorTipos,
                EficienciaPorTipo = eficienciaPorTipo
            };
        }
        
        public async Task<IEnumerable<TarimaEvolucionDTO>> ObtenerEvolucionDiariaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var resumenes = await _tarimaResumenRepository.GetByRangoFechasAsync(fechaInicio, fechaFin);
            
            var evolucion = resumenes
                .GroupBy(r => r.Fecha)
                .Select(g => new TarimaEvolucionDTO
                {
                    Fecha = g.Key,
                    TotalTarimas = g.Sum(r => r.CantidadTarimas),
                    EficienciaGeneral = g.Sum(r => r.CantidadTarimas) > 0 ? 
                        (decimal)g.Sum(r => r.TarimasCompletas) / g.Sum(r => r.CantidadTarimas) * 100 : 0,
                    TarimasPorTipo = g.ToDictionary(r => r.Tipo, r => r.CantidadTarimas),
                    PesoPorTipo = g.ToDictionary(r => r.Tipo, r => r.PesoTotal),
                    PesoTotal = g.Sum(r => r.PesoTotal)
                })
                .OrderBy(e => e.Fecha)
                .ToList();
            
            return evolucion;
        }
        
        public async Task<IEnumerable<TarimaEvolucionDTO>> ObtenerEvolucionSemanalAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var resumenes = await _tarimaResumenRepository.GetByRangoFechasAsync(fechaInicio, fechaFin);
            
            var evolucion = resumenes
                .GroupBy(r => new { 
                    Año = r.Fecha.Year, 
                    Semana = GetWeekOfYear(r.Fecha) 
                })
                .Select(g => new TarimaEvolucionDTO
                {
                    Fecha = g.Min(r => r.Fecha),
                    TotalTarimas = g.Sum(r => r.CantidadTarimas),
                    EficienciaGeneral = g.Sum(r => r.CantidadTarimas) > 0 ? 
                        (decimal)g.Sum(r => r.TarimasCompletas) / g.Sum(r => r.CantidadTarimas) * 100 : 0,
                    TarimasPorTipo = g.GroupBy(r => r.Tipo)
                        .ToDictionary(tg => tg.Key, tg => tg.Sum(r => r.CantidadTarimas)),
                    PesoPorTipo = g.GroupBy(r => r.Tipo)
                        .ToDictionary(tg => tg.Key, tg => tg.Sum(r => r.PesoTotal)),
                    PesoTotal = g.Sum(r => r.PesoTotal)
                })
                .OrderBy(e => e.Fecha)
                .ToList();
            
            return evolucion;
        }
        
        public async Task<IEnumerable<TarimaEvolucionDTO>> ObtenerEvolucionMensualAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var resumenes = await _tarimaResumenRepository.GetByRangoFechasAsync(fechaInicio, fechaFin);
            
            var evolucion = resumenes
                .GroupBy(r => new { r.Fecha.Year, r.Fecha.Month })
                .Select(g => new TarimaEvolucionDTO
                {
                    Fecha = new DateTime(g.Key.Year, g.Key.Month, 1),
                    TotalTarimas = g.Sum(r => r.CantidadTarimas),
                    EficienciaGeneral = g.Sum(r => r.CantidadTarimas) > 0 ? 
                        (decimal)g.Sum(r => r.TarimasCompletas) / g.Sum(r => r.CantidadTarimas) * 100 : 0,
                    TarimasPorTipo = g.GroupBy(r => r.Tipo)
                        .ToDictionary(tg => tg.Key, tg => tg.Sum(r => r.CantidadTarimas)),
                    PesoPorTipo = g.GroupBy(r => r.Tipo)
                        .ToDictionary(tg => tg.Key, tg => tg.Sum(r => r.PesoTotal)),
                    PesoTotal = g.Sum(r => r.PesoTotal)
                })
                .OrderBy(e => e.Fecha)
                .ToList();
            
            return evolucion;
        }
        
        public async Task<IEnumerable<TarimaComparativaDTO>> ObtenerComparativaEntrePeriodosAsync(DateTime fechaInicio1, DateTime fechaFin1, DateTime fechaInicio2, DateTime fechaFin2)
        {
            var periodo1 = await _tarimaResumenRepository.GetByRangoFechasAsync(fechaInicio1, fechaFin1);
            var periodo2 = await _tarimaResumenRepository.GetByRangoFechasAsync(fechaInicio2, fechaFin2);
            
            var comparativa = new List<TarimaComparativaDTO>();
            
            // Período 1
            var totalTarimas1 = periodo1.Sum(r => r.CantidadTarimas);
            var totalCompletas1 = periodo1.Sum(r => r.TarimasCompletas);
            var eficiencia1 = totalTarimas1 > 0 ? (decimal)totalCompletas1 / totalTarimas1 * 100 : 0;
            
            comparativa.Add(new TarimaComparativaDTO
            {
                Periodo = $"Período 1 ({fechaInicio1:dd/MM/yyyy} - {fechaFin1:dd/MM/yyyy})",
                TotalTarimas = totalTarimas1,
                EficienciaGeneral = eficiencia1,
                DistribucionPorTipos = periodo1.GroupBy(r => r.Tipo).ToDictionary(g => g.Key, g => g.Sum(r => r.CantidadTarimas)),
                EficienciaPorTipo = periodo1.GroupBy(r => r.Tipo).ToDictionary(g => g.Key, g => 
                    g.Sum(r => r.CantidadTarimas) > 0 ? (decimal)g.Sum(r => r.TarimasCompletas) / g.Sum(r => r.CantidadTarimas) * 100 : 0)
            });
            
            // Período 2
            var totalTarimas2 = periodo2.Sum(r => r.CantidadTarimas);
            var totalCompletas2 = periodo2.Sum(r => r.TarimasCompletas);
            var eficiencia2 = totalTarimas2 > 0 ? (decimal)totalCompletas2 / totalTarimas2 * 100 : 0;
            
            comparativa.Add(new TarimaComparativaDTO
            {
                Periodo = $"Período 2 ({fechaInicio2:dd/MM/yyyy} - {fechaFin2:dd/MM/yyyy})",
                TotalTarimas = totalTarimas2,
                EficienciaGeneral = eficiencia2,
                DistribucionPorTipos = periodo2.GroupBy(r => r.Tipo).ToDictionary(g => g.Key, g => g.Sum(r => r.CantidadTarimas)),
                EficienciaPorTipo = periodo2.GroupBy(r => r.Tipo).ToDictionary(g => g.Key, g => 
                    g.Sum(r => r.CantidadTarimas) > 0 ? (decimal)g.Sum(r => r.TarimasCompletas) / g.Sum(r => r.CantidadTarimas) * 100 : 0)
            });
            
            return comparativa;
        }
        
        public async Task<Dictionary<string, decimal>> ObtenerEficienciaPorTipoAsync(DateTime? fechaInicio, DateTime? fechaFin)
        {
            IEnumerable<TarimaResumenDiario> resumenes;
            
            if (fechaInicio.HasValue && fechaFin.HasValue)
            {
                resumenes = await _tarimaResumenRepository.GetByRangoFechasAsync(fechaInicio.Value, fechaFin.Value);
            }
            else
            {
                // Obtener todos los resúmenes si no se especifica rango
                resumenes = await _tarimaResumenRepository.GetAllAsync();
            }
            
            var eficiencia = resumenes
                .GroupBy(r => r.Tipo)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(r => r.CantidadTarimas) > 0 ? 
                        (decimal)g.Sum(r => r.TarimasCompletas) / g.Sum(r => r.CantidadTarimas) * 100 : 0
                );
            
            return eficiencia;
        }
        
        private static int GetWeekOfYear(DateTime date)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            var calendar = culture.Calendar;
            return calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }
    }
}
