using AppAPIEmpacadora.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Services.Interfaces
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteSummaryDTO>> GetClientesAsync();
        Task<ClienteDTO> GetClienteByIdAsync(int id);
        Task<ClienteDTO> CreateClienteAsync(CreateClienteDTO createClienteDto, string usuario);
        Task<ClienteDTO> UpdateClienteAsync(int id, UpdateClienteDTO updateClienteDto);
        Task<bool> DeleteClienteAsync(int id);
    }
} 