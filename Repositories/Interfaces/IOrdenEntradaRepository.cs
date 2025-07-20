using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Models.Entities;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Repositories.Interfaces
{
    public interface IOrdenEntradaRepository
    {
        Task<IEnumerable<OrdenEntradaDTO>> ObtenerOrdenesEntradaAsync();
        Task<OrdenEntradaDTO> ObtenerOrdenEntradaPorCodigoAsync(string codigo);
        Task<OrdenEntradaDTO> ObtenerUltimaOrdenEntradaAsync();
        Task<OrdenEntradaDTO> CrearOrdenEntradaAsync(OrdenEntradaDTO ordenEntrada);
        Task<bool> ActualizarOrdenEntradaAsync(OrdenEntradaDTO ordenEntrada);
        Task<bool> EliminarOrdenEntradaAsync(string codigo);
        Task<DetalleOrdenEntradaDTO> ObtenerDetalleOrdenEntradaAsync(string codigo);
        Task<IEnumerable<TarimaDetalleDTO>> ObtenerTarimasPorOrdenAsync(string codigo);
        Task<TarimaDetalleDTO> ObtenerTarimaPorNumeroAsync(string codigo, string numeroTarima);
        Task<bool> ActualizarTarimaAsync(TarimaDetalleDTO tarima);
        Task<bool> EliminarTarimaAsync(string codigo, string numero);
        Task<decimal> ObtenerPesoTotalRecibidoHoyAsync();
        Task<int> ObtenerCantidadPendientesHoyAsync();
        Task<TarimaDetalleDTO> CrearTarimaAsync(string codigoOrden, TarimaDetalleDTO tarima);
        Task<PedidoProveedor> GetByIdAsync(int id);
        Task UpdatePedidoAsync(PedidoProveedor pedido);
        Task<PedidoCompletoDTO> ObtenerPedidoCompletoPorIdAsync(int id);
    }
} 