using AppAPIEmpacadora.Infrastructure.Data;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Infrastructure.Repositories
{
    public class CajaRepository : ICajaRepository
    {
        private readonly ApplicationDbContext _context;

        public CajaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Caja>> GetAllAsync()
        {
            return await _context.Cajas
                .Include(c => c.Clasificacion)
                .ToListAsync();
        }

        public async Task<Caja> GetByIdAsync(int id)
        {
            return await _context.Cajas
                .Include(c => c.Clasificacion)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Caja> AddAsync(Caja caja)
        {
            _context.Cajas.Add(caja);
            await _context.SaveChangesAsync();
            return caja;
        }

        public async Task<Caja> UpdateAsync(Caja caja)
        {
            _context.Entry(caja).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return caja;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var caja = await _context.Cajas.FindAsync(id);
            if (caja == null)
                return false;

            _context.Cajas.Remove(caja);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Caja> GetByTipoAndClasificacionAsync(string tipo, int? idClasificacion)
        {
            return await _context.Cajas
                .Include(c => c.Clasificacion)
                .FirstOrDefaultAsync(c => c.Tipo == tipo && c.IdClasificacion == idClasificacion);
        }

        public async Task<IEnumerable<Caja>> GetByClasificacionAsync(int idClasificacion)
        {
            return await _context.Cajas
                .Include(c => c.Clasificacion)
                .Where(c => c.IdClasificacion == idClasificacion)
                .ToListAsync();
        }
    }
} 