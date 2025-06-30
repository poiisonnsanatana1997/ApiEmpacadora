using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPIEmpacadora.Models.DTOs;

namespace AppAPIEmpacadora.Services.Interfaces
{
    public interface IMermaService
    {
        Task<IEnumerable<MermaResponseDTO>> ObtenerTodosAsync();
        Task<MermaResponseDTO> ObtenerPorIdAsync(int id);
        Task<MermaResponseDTO> CrearAsync(CreateMermaDTO dto, string usuarioRegistro);
        Task<bool> ActualizarAsync(int id, UpdateMermaDTO dto);
        Task<bool> EliminarAsync(int id);
    }
} 