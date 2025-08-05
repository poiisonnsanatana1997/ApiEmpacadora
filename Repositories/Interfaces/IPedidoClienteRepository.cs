using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPIEmpacadora.Models.Entities;

namespace AppAPIEmpacadora.Repositories.Interfaces
{
    public interface IPedidoClienteRepository
    {
        Task<IEnumerable<PedidoCliente>> ObtenerTodosAsync();
        Task<PedidoCliente> ObtenerPorIdAsync(int id);
        Task<PedidoCliente> ObtenerPorIdConRelacionesCompletasAsync(int id);
        Task<PedidoCliente> CrearAsync(PedidoCliente entity);
        Task<bool> ActualizarAsync(PedidoCliente entity);
        Task<bool> EliminarAsync(int id);
        Task<IEnumerable<PedidoCliente>> ObtenerTodosConDetallesAsync();
        Task<IEnumerable<PedidoCliente>> ObtenerPorTipoConTarimasAsync(string tipo);
    }
} 