using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppAPIEmpacadora.Infrastructure.Data;
using AppAPIEmpacadora.Models.DTOs;

public class ProveedorRepository : IProveedorRepository
{
    private readonly ApplicationDbContext _context;

    public ProveedorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProveedorSimpleDTO>> ObtenerTodosAsync()
    {
        return await _context.Proveedores
            .Select(p => new ProveedorSimpleDTO
            {
                Id = p.Id,
                Nombre = p.Nombre
            })
            .ToListAsync();
    }

    public async Task<ProveedorSimpleDTO> ObtenerPorIdAsync(int id)
    {
        var proveedor = await _context.Proveedores
            .Where(p => p.Id == id)
            .Select(p => new ProveedorSimpleDTO
            {
                Id = p.Id,
                Nombre = p.Nombre
            })
            .FirstOrDefaultAsync();

        return proveedor;
    }
} 