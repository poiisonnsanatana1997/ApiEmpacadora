using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPIEmpacadora.Models.Entities;

namespace AppAPIEmpacadora.Repositories.Interfaces
{
    public interface IOrdenPedidoClienteRepository
    {
        Task<IEnumerable<OrdenPedidoCliente>> ObtenerTodosAsync();
        Task<OrdenPedidoCliente> ObtenerPorIdAsync(int id);
        Task<OrdenPedidoCliente> CrearAsync(OrdenPedidoCliente entity);
        Task<bool> ActualizarAsync(OrdenPedidoCliente entity);
        Task<bool> EliminarAsync(int id);
        Task<IEnumerable<OrdenPedidoCliente>> ObtenerTodosConDetallesAsync();
        Task<IEnumerable<OrdenPedidoCliente>> ObtenerPorPedidoClienteAsync(int idPedidoCliente);
    }
} 