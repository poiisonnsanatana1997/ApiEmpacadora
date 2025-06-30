using AppAPIEmpacadora.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Services.Interfaces
{
    public interface ISucursalService
    {
        Task<IEnumerable<SucursalDTO>> GetSucursalesAsync();
        Task<SucursalDTO> GetSucursalByIdAsync(int id);
        Task<SucursalDTO> CreateSucursalAsync(CreateSucursalDTO createSucursalDto, string usuario);
        Task<SucursalDTO> UpdateSucursalAsync(int id, UpdateSucursalDTO updateSucursalDto);
        Task<bool> DeleteSucursalAsync(int id);
    }
} 