using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPIEmpacadora.Models.DTOs;

namespace AppAPIEmpacadora.Services
{
    public interface IProductoService
    {
        Task<IEnumerable<ProductoSimpleDTO>> ObtenerTodosAsync();
        Task<ProductoSimpleDTO> ObtenerPorIdAsync(int id);
    }
} 