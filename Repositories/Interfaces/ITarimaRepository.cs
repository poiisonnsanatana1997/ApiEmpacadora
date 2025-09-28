using AppAPIEmpacadora.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Repositories.Interfaces
{
    public interface ITarimaRepository
    {
        Task<IEnumerable<Tarima>> GetAllAsync();
        Task<Tarima> GetByIdAsync(int id);
        Task<Tarima> AddAsync(Tarima tarima);
        Task<Tarima> UpdateAsync(Tarima tarima);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Tarima>> GetTarimasParcialesAsync();
        Task<IEnumerable<Tarima>> GetTarimasParcialesYCompletasAsync();
        Task<TarimaClasificacion> GetTarimaClasificacionAsync(int idTarima, int idClasificacion);
        Task<TarimaClasificacion> CreateTarimaClasificacionAsync(TarimaClasificacion tarimaClasificacion);
        Task<TarimaClasificacion> UpdateTarimaClasificacionAsync(TarimaClasificacion tarimaClasificacion);
        Task<IEnumerable<Tarima>> GetAllWithClasificacionesAsync();
        Task<Tarima> GetByIdWithClasificacionesAsync(int id);
        Task<IEnumerable<dynamic>> GetDatosResumenDiarioAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<bool> DeleteTarimaClasificacionAsync(int idTarima, int idClasificacion);
    }
} 