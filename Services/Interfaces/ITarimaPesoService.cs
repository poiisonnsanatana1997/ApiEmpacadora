using AppAPIEmpacadora.Models.DTOs;

namespace AppAPIEmpacadora.Services.Interfaces
{
    public interface ITarimaPesoService
    {
        // Procesamiento de resúmenes
        Task ProcesarResumenDiarioAsync(DateTime fecha, string usuario);
        
        // Consultas de peso
        Task<IEnumerable<TarimaPesoDiarioDTO>> ObtenerPesoDiarioPorTipoAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<TarimaPesoPorTipoDTO>> ObtenerPesoPorTipoEnMesAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<Dictionary<string, List<PesoDiarioDTO>>> ObtenerPesoDiarioPorTipoParaGraficaAsync(DateTime fechaInicio, DateTime fechaFin);
        
        // Información para dashboard
        Task<TarimaDashboardDTO> ObtenerDatosDashboardAsync(DateTime fecha);
        
        // Datos para gráficas
        Task<IEnumerable<TarimaEvolucionDTO>> ObtenerEvolucionDiariaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<TarimaEvolucionDTO>> ObtenerEvolucionSemanalAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<TarimaEvolucionDTO>> ObtenerEvolucionMensualAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<TarimaComparativaDTO>> ObtenerComparativaEntrePeriodosAsync(DateTime fechaInicio1, DateTime fechaFin1, DateTime fechaInicio2, DateTime fechaFin2);
        Task<Dictionary<string, decimal>> ObtenerEficienciaPorTipoAsync(DateTime? fechaInicio, DateTime? fechaFin);
    }
}
