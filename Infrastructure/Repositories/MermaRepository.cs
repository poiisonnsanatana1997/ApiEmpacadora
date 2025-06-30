using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Infrastructure.Data;
using AppAPIEmpacadora.Repositories.Interfaces;

namespace AppAPIEmpacadora.Infrastructure.Repositories
{
    public class MermaRepository : IMermaRepository
    {
        private readonly ApplicationDbContext _context;
        public MermaRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Merma>> ObtenerTodosAsync() => await _context.Mermas.ToListAsync();
        public async Task<Merma> ObtenerPorIdAsync(int id) => await _context.Mermas.FindAsync(id);
        public async Task<Merma> CrearAsync(Merma entity)
        {
            _context.Mermas.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> ActualizarAsync(Merma entity)
        {
            _context.Mermas.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> EliminarAsync(int id)
        {
            var entity = await _context.Mermas.FindAsync(id);
            if (entity == null) return false;
            _context.Mermas.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
} 