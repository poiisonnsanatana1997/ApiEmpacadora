using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPIEmpacadora.Models.DTOs;

namespace AppAPIEmpacadora.Services.Interfaces
{
    public interface IRetornoService
    {
        Task<IEnumerable<RetornoResponseDTO>> ObtenerTodosAsync();
        Task<RetornoResponseDTO> ObtenerPorIdAsync(int id);
        Task<RetornoResponseDTO> CrearAsync(CreateRetornoDTO dto, string usuarioRegistro);
        Task<bool> ActualizarAsync(int id, UpdateRetornoDTO dto);
        Task<bool> EliminarAsync(int id);
    }
} 