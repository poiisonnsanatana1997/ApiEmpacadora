using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Models.Entities;

namespace AppAPIEmpacadora.Repositories.Interfaces
{
    public interface IProveedorRepository
    {
        Task<IEnumerable<ProveedorSimpleDTO>> ObtenerTodosAsync();
        Task<ProveedorSimpleDTO> ObtenerPorIdAsync(int id);
        Task<IEnumerable<ProveedorDTO>> GetAllAsync();
        Task<ProveedorDTO> GetByIdAsync(int id);
        Task<Proveedor> AddAsync(Proveedor proveedor);
        Task<Proveedor> UpdateAsync(Proveedor proveedor);
        Task<bool> DeleteAsync(int id);
    }
} 