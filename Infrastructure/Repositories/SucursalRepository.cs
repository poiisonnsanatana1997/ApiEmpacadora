using AppAPIEmpacadora.Infrastructure.Data;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Infrastructure.Repositories
{
    public class SucursalRepository : ISucursalRepository
    {
        private readonly ApplicationDbContext _context;

        public SucursalRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Sucursal>> GetAllAsync()
        {
            return await _context.Sucursales.ToListAsync();
        }

        public async Task<Sucursal> GetByIdAsync(int id)
        {
            return await _context.Sucursales.FindAsync(id);
        }

        public async Task<Sucursal> AddAsync(Sucursal sucursal)
        {
            _context.Sucursales.Add(sucursal);
            await _context.SaveChangesAsync();
            return sucursal;
        }

        public async Task<Sucursal> UpdateAsync(Sucursal sucursal)
        {
            _context.Entry(sucursal).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return sucursal;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sucursal = await _context.Sucursales.FindAsync(id);
            if (sucursal == null)
            {
                return false;
            }

            _context.Sucursales.Remove(sucursal);
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 