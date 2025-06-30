using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPIEmpacadora.Models.Entities;

namespace AppAPIEmpacadora.Repositories.Interfaces
{
    public interface IRetornoRepository
    {
        Task<IEnumerable<Retorno>> ObtenerTodosAsync();
        Task<Retorno> ObtenerPorIdAsync(int id);
        Task<Retorno> CrearAsync(Retorno entity);
        Task<bool> ActualizarAsync(Retorno entity);
        Task<bool> EliminarAsync(int id);
    }
} 