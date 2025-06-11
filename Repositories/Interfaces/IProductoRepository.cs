using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPIEmpacadora.Models.DTOs;

namespace AppAPIEmpacadora.Repositories.Interfaces
{
    public interface IProductoRepository
    {
        Task<IEnumerable<ProductoSimpleDTO>> ObtenerTodosAsync();
        Task<ProductoSimpleDTO> ObtenerPorIdAsync(int id);
    }
} 