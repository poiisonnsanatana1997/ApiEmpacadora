using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppAPIEmpacadora.Infrastructure.Data;
using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Repositories.Interfaces;

namespace AppAPIEmpacadora.Infrastructure.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductoSimpleDTO>> ObtenerTodosAsync()
        {
            return await _context.Productos
                .Select(p => new ProductoSimpleDTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre
                })
                .ToListAsync();
        }

        public async Task<ProductoSimpleDTO> ObtenerPorIdAsync(int id)
        {
            var producto = await _context.Productos
                .Where(p => p.Id == id)
                .Select(p => new ProductoSimpleDTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre
                })
                .FirstOrDefaultAsync();

            return producto;
        }
    }
} 