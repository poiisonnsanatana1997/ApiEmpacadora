using AppAPIEmpacadora.Models.Entities;

namespace AppAPIEmpacadora.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Producto>> GetAllAsync();
        Task<Producto> GetByIdAsync(int id);
        Task<Producto> CreateAsync(Producto producto);
        Task<Producto> UpdateAsync(Producto producto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
} 