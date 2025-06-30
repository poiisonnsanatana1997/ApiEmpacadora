using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPIEmpacadora.Models.Entities;

namespace AppAPIEmpacadora.Repositories.Interfaces
{
    public interface IMermaRepository
    {
        Task<IEnumerable<Merma>> ObtenerTodosAsync();
        Task<Merma> ObtenerPorIdAsync(int id);
        Task<Merma> CrearAsync(Merma entity);
        Task<bool> ActualizarAsync(Merma entity);
        Task<bool> EliminarAsync(int id);
    }
} 