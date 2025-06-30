using AppAPIEmpacadora.Infrastructure.Data;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Infrastructure.Repositories
{
    public class CajaClienteRepository : ICajaClienteRepository
    {
        private readonly ApplicationDbContext _context;

        public CajaClienteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CajaCliente>> GetAllAsync()
        {
            return await _context.CajaClientes.Include(c => c.Cliente).ToListAsync();
        }

        public async Task<CajaCliente> GetByIdAsync(int id)
        {
            return await _context.CajaClientes.Include(c => c.Cliente).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CajaCliente> AddAsync(CajaCliente cajaCliente)
        {
            _context.CajaClientes.Add(cajaCliente);
            await _context.SaveChangesAsync();
            return cajaCliente;
        }

        public async Task<CajaCliente> UpdateAsync(CajaCliente cajaCliente)
        {
            _context.Entry(cajaCliente).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return cajaCliente;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cajaCliente = await _context.CajaClientes.FindAsync(id);
            if (cajaCliente == null)
            {
                return false;
            }

            _context.CajaClientes.Remove(cajaCliente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 