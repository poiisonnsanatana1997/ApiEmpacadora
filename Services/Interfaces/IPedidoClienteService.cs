using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPIEmpacadora.Models.DTOs;

namespace AppAPIEmpacadora.Services.Interfaces
{
    public interface IPedidoClienteService
    {
        Task<IEnumerable<PedidoClienteResponseDTO>> ObtenerTodosAsync();
        Task<PedidoClienteResponseDTO> ObtenerPorIdAsync(int id);
        Task<PedidoClienteConOrdenesResponseDTO> ObtenerPorIdConOrdenesAsync(int id);
        Task<PedidoClienteResponseDTO> CrearAsync(CreatePedidoClienteDTO dto, string usuarioRegistro);
        Task<PedidoClienteConOrdenesResponseDTO> CrearConOrdenesAsync(CreatePedidoClienteConOrdenesDTO dto, string usuarioRegistro);
        Task<bool> ActualizarAsync(int id, UpdatePedidoClienteDTO dto);
        Task<bool> EliminarAsync(int id);
        Task<IEnumerable<PedidoClienteConDetallesDTO>> ObtenerTodosConDetallesAsync();
        Task<bool> ActualizarEstatusAsync(int id, string estatus, string usuarioModificacion);
        Task<IEnumerable<PedidoClientePorAsignarDTO>> ObtenerDisponiblesPorTipoAsync(string tipo, int? idProducto = null);
        Task<PedidoClientePorAsignarDTO> ObtenerDisponibilidadCajasPorPedidoAsync(int idPedido, string tipo, int? idProducto = null);
        Task<PedidoClienteProgresoDTO> ObtenerProgresoAsync(int id);
        Task<decimal> CalcularPorcentajeSurtidoAsync(int idPedidoCliente);
        Task<bool> ActualizarPorcentajeSurtidoAsync(int idPedidoCliente);
        Task<PedidoClienteResponseDTO> AsignarTarimasYCalcularSurtidoAsync(int pedidoId, List<int> tarimaIds, string usuario);
        Task DesasignarTarimasAsync(List<DesasignacionTarimaDTO> desasignaciones, string usuario);
    }
} 