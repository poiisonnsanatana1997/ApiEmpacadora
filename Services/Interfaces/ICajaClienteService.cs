using AppAPIEmpacadora.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Services.Interfaces
{
    public interface ICajaClienteService
    {
        Task<IEnumerable<CajaClienteDTO>> GetCajasClienteAsync();
        Task<CajaClienteDTO> GetCajaClienteByIdAsync(int id);
        Task<CajaClienteDTO> CreateCajaClienteAsync(CreateCajaClienteDTO createCajaClienteDTO);
        Task<CajaClienteDTO> UpdateCajaClienteAsync(int id, UpdateCajaClienteDTO updateCajaClienteDTO);
        Task<bool> DeleteCajaClienteAsync(int id);
    }
} 