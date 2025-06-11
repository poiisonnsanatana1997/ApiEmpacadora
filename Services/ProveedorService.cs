using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPIEmpacadora.Models.DTOs;

public class ProveedorService : IProveedorService
{
    private readonly IProveedorRepository _proveedorRepository;

    public ProveedorService(IProveedorRepository proveedorRepository)
    {
        _proveedorRepository = proveedorRepository;
    }

    public async Task<IEnumerable<ProveedorSimpleDTO>> ObtenerTodosAsync()
    {
        return await _proveedorRepository.ObtenerTodosAsync();
    }

    public async Task<ProveedorSimpleDTO> ObtenerPorIdAsync(int id)
    {
        return await _proveedorRepository.ObtenerPorIdAsync(id);
    }
} 