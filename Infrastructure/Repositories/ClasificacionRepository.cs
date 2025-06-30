using AppAPIEmpacadora.Infrastructure.Data;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Infrastructure.Repositories
{
    public class ClasificacionRepository : IClasificacionRepository
    {
        private readonly ApplicationDbContext _context;

        public ClasificacionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Clasificacion>> GetAllAsync()
        {
            return await _context.Clasificaciones.ToListAsync();
        }

        public async Task<Clasificacion> GetByIdAsync(int id)
        {
            return await _context.Clasificaciones.FindAsync(id);
        }

        public async Task<Clasificacion> AddAsync(Clasificacion clasificacion)
        {
            _context.Clasificaciones.Add(clasificacion);
            await _context.SaveChangesAsync();
            return clasificacion;
        }

        public async Task<Clasificacion> UpdateAsync(Clasificacion clasificacion)
        {
            _context.Entry(clasificacion).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return clasificacion;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var clasificacion = await _context.Clasificaciones.FindAsync(id);
            if (clasificacion == null)
            {
                return false;
            }

            _context.Clasificaciones.Remove(clasificacion);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Clasificacion>> GetByDateAndProductAsync(DateTime date, int idProducto)
        {
            return await _context.Clasificaciones
                .Include(c => c.PedidoProveedor)
                    .ThenInclude(pp => pp.ProductosPedido)
                .Where(c => c.FechaRegistro.Date == date.Date &&
                            c.PedidoProveedor.ProductosPedido.Any(pp => pp.IdProducto == idProducto))
                .ToListAsync();
        }
    }
} 