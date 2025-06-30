using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPIEmpacadora.Models.DTOs;

namespace AppAPIEmpacadora.Services.Interfaces
{
    public interface IProveedorService
    {
        Task<IEnumerable<ProveedorSimpleDTO>> ObtenerTodosAsync();
        Task<ProveedorSimpleDTO> ObtenerPorIdAsync(int id);
        Task<IEnumerable<ProveedorDTO>> GetProveedoresAsync();
        Task<ProveedorDTO> GetProveedorByIdAsync(int id);
        Task<ProveedorDTO> CreateProveedorAsync(CreateProveedorDTO createProveedorDto, string usuario);
        Task<ProveedorDTO> UpdateProveedorAsync(int id, UpdateProveedorDTO updateProveedorDto, string usuario);
        Task<bool> DeleteProveedorAsync(int id);
    }
} 