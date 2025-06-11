using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPIEmpacadora.Models.DTOs;

public interface IProveedorService
{
    Task<IEnumerable<ProveedorSimpleDTO>> ObtenerTodosAsync();
    Task<ProveedorSimpleDTO> ObtenerPorIdAsync(int id);
} 