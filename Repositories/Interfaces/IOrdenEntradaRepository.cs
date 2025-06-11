using AppAPIEmpacadora.Models.DTOs;

namespace AppAPIEmpacadora.Repositories.Interfaces
{
    public interface IOrdenEntradaRepository
    {
        Task<IEnumerable<OrdenEntradaDTO>> ObtenerOrdenesEntradaAsync();
        Task<OrdenEntradaDTO> ObtenerOrdenEntradaPorCodigoAsync(string codigo);
        Task<OrdenEntradaDTO> ObtenerUltimaOrdenEntradaAsync();
        Task CrearOrdenEntradaAsync(OrdenEntradaDTO ordenEntrada, string usuarioRegistro);
        Task<bool> ActualizarOrdenEntradaAsync(OrdenEntradaDTO ordenEntrada);
        Task<bool> EliminarOrdenEntradaAsync(string codigo);
        Task<DetalleOrdenEntradaDTO> ObtenerDetalleOrdenEntradaAsync(string codigo);
        Task<IEnumerable<TarimaDTO>> ObtenerTarimasPorOrdenAsync(string codigo);
        Task<TarimaDTO> ObtenerTarimaPorNumeroAsync(string codigo, string numeroTarima);
        Task<bool> ActualizarTarimaAsync(TarimaDTO tarima);
        Task<bool> EliminarTarimaAsync(TarimaDTO tarima);
        Task<decimal> ObtenerPesoTotalRecibidoHoyAsync();
        Task<int> ObtenerCantidadPendientesHoyAsync();
        Task<TarimaDTO> CrearTarimaAsync(string codigoOrden, TarimaDTO tarima);
    }
} 