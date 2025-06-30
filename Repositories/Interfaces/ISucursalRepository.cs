using AppAPIEmpacadora.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Repositories.Interfaces
{
    public interface ISucursalRepository
    {
        Task<IEnumerable<Sucursal>> GetAllAsync();
        Task<Sucursal> GetByIdAsync(int id);
        Task<Sucursal> AddAsync(Sucursal sucursal);
        Task<Sucursal> UpdateAsync(Sucursal sucursal);
        Task<bool> DeleteAsync(int id);
    }
} 