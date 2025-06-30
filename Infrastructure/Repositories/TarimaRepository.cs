using AppAPIEmpacadora.Infrastructure.Data;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Infrastructure.Repositories
{
    public class TarimaRepository : ITarimaRepository
    {
        private readonly ApplicationDbContext _context;

        public TarimaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tarima>> GetAllAsync()
        {
            return await _context.Tarimas.ToListAsync();
        }

        public async Task<Tarima> GetByIdAsync(int id)
        {
            return await _context.Tarimas.FindAsync(id);
        }

        public async Task<Tarima> AddAsync(Tarima tarima)
        {
            _context.Tarimas.Add(tarima);
            await _context.SaveChangesAsync();
            return tarima;
        }

        public async Task<Tarima> UpdateAsync(Tarima tarima)
        {
            _context.Entry(tarima).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return tarima;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var tarima = await _context.Tarimas.FindAsync(id);
            if (tarima == null)
            {
                return false;
            }

            _context.Tarimas.Remove(tarima);
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 