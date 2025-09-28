using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Models.DTOs;

namespace AppAPIEmpacadora.Repositories.Interfaces
{
    public interface ITarimaResumenRepository
    {
        Task<TarimaResumenDiario?> GetByFechaYTipoAsync(DateTime fecha, string tipo);
        Task<IEnumerable<TarimaResumenDiario>> GetByFechaAsync(DateTime fecha);
        Task<IEnumerable<TarimaResumenDiario>> GetByRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<TarimaResumenDiario>> GetByTipoAsync(string tipo, DateTime? fechaInicio, DateTime? fechaFin);
        Task<TarimaResumenDiario> AddAsync(TarimaResumenDiario tarimaResumen);
        Task<TarimaResumenDiario> UpdateAsync(TarimaResumenDiario tarimaResumen);
        Task<bool> ExistsAsync(DateTime fecha, string tipo);
        Task<IEnumerable<TarimaResumenDiario>> GetAllAsync();
        
        // Métodos de lote para optimización
        Task AddBatchAsync(IEnumerable<TarimaResumenDiario> resumenes);
        Task UpdateBatchAsync(IEnumerable<TarimaResumenDiario> resumenes);
        
        // Método para ejecutar operaciones en transacción
        Task ExecuteInTransactionAsync(Func<Task> operation);
    }
}
