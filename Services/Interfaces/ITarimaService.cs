using AppAPIEmpacadora.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Services.Interfaces
{
    public interface ITarimaService
    {
        Task<IEnumerable<TarimaDTO>> GetTarimasAsync();
        Task<TarimaDTO> GetTarimaByIdAsync(int id);
        Task<TarimaDTO> CreateTarimaAsync(CreateTarimaDTO createTarimaDto, string usuario);
        Task<TarimaDTO> UpdateTarimaAsync(int id, UpdateTarimaDTO updateTarimaDto, string usuario);
        Task<bool> DeleteTarimaAsync(int id);
    }
} 