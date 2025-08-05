using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPIEmpacadora.Models.DTOs;

namespace AppAPIEmpacadora.Services.Interfaces
{
    public interface IOrdenPedidoClienteService
    {
        Task<IEnumerable<OrdenPedidoClienteResponseDTO>> ObtenerTodosAsync();
        Task<OrdenPedidoClienteResponseDTO> ObtenerPorIdAsync(int id);
        Task<OrdenPedidoClienteResponseDTO> CrearAsync(CreateOrdenPedidoClienteDTO dto, string usuario);
        Task<bool> ActualizarAsync(int id, UpdateOrdenPedidoClienteDTO dto);
        Task<bool> EliminarAsync(int id);
        Task<IEnumerable<OrdenPedidoClienteConDetallesDTO>> ObtenerTodosConDetallesAsync();
        Task<IEnumerable<OrdenPedidoClienteResponseDTO>> ObtenerPorPedidoClienteAsync(int idPedidoCliente);
    }
} 