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
        
        // Nuevos métodos para actualización parcial
        Task<TarimaClasificacion> GetTarimaClasificacionAsync(int idTarima, int idClasificacion);
        Task<TarimaClasificacion> CreateTarimaClasificacionAsync(TarimaClasificacion tarimaClasificacion);
        Task<TarimaClasificacion> UpdateTarimaClasificacionAsync(TarimaClasificacion tarimaClasificacion);
        
        // Método para obtener tarimas con clasificaciones incluidas
        Task<IEnumerable<Tarima>> GetAllWithClasificacionesAsync();
        Task<Tarima> GetByIdWithClasificacionesAsync(int id);
    }
} 