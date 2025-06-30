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
    }
} 