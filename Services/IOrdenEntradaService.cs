using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPIEmpacadora.Models.DTOs;

namespace AppAPIEmpacadora.Services
{
    public interface IOrdenEntradaService
    {
        Task<IEnumerable<OrdenEntradaDTO>> ObtenerOrdenesEntradaAsync();
        Task<OrdenEntradaDTO> ObtenerOrdenEntradaPorCodigoAsync(string codigo);
        Task<OrdenEntradaDTO> CrearOrdenEntradaAsync(CrearOrdenEntradaDTO dto, string usuarioRegistro);
        Task<OrdenEntradaDTO> ActualizarOrdenEntradaAsync(string codigo, ActualizarOrdenEntradaDTO dto, string usuarioRegistro);
        Task<bool> EliminarOrdenEntradaAsync(string codigo);
        Task<DetalleOrdenEntradaDTO> ObtenerDetalleOrdenEntradaAsync(string codigo);
        Task<bool> ActualizarPesajeTarimaAsync(string codigo, string numeroTarima, ActualizarPesajeTarimaDTO dto);
        Task<bool> EliminarTarimaAsync(string codigo, string numeroTarima);
        Task<decimal> ObtenerPesoTotalRecibidoHoyAsync();
        Task<int> ObtenerCantidadPendientesHoyAsync();
        Task<TarimaDetalleDTO> CrearTarimaAsync(string codigoOrden, TarimaDetalleDTO tarima);
    }
} 