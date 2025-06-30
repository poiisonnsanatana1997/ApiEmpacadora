using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPIEmpacadora.Models.DTOs;

namespace AppAPIEmpacadora.Services.Interfaces
{
    public interface IPedidoClienteService
    {
        Task<IEnumerable<PedidoClienteResponseDTO>> ObtenerTodosAsync();
        Task<PedidoClienteResponseDTO> ObtenerPorIdAsync(int id);
        Task<PedidoClienteResponseDTO> CrearAsync(CreatePedidoClienteDTO dto, string usuarioRegistro);
        Task<bool> ActualizarAsync(int id, UpdatePedidoClienteDTO dto);
        Task<bool> EliminarAsync(int id);
    }
} 