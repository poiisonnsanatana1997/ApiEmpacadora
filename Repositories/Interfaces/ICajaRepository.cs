using AppAPIEmpacadora.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Repositories.Interfaces
{
    public interface ICajaRepository
    {
        Task<IEnumerable<Caja>> GetAllAsync();
        Task<Caja> GetByIdAsync(int id);
        Task<Caja> AddAsync(Caja caja);
        Task<Caja> UpdateAsync(Caja caja);
        Task<bool> DeleteAsync(int id);
        Task<Caja> GetByTipoAndClasificacionAsync(string tipo, int? idClasificacion);
        Task<IEnumerable<Caja>> GetByClasificacionAsync(int idClasificacion);
    }
} 