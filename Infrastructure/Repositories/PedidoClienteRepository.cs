using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Infrastructure.Data;
using AppAPIEmpacadora.Repositories.Interfaces;

namespace AppAPIEmpacadora.Infrastructure.Repositories
{
    public class PedidoClienteRepository : IPedidoClienteRepository
    {
        private readonly ApplicationDbContext _context;
        public PedidoClienteRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<PedidoCliente>> ObtenerTodosAsync() => await _context.PedidosCliente.ToListAsync();
        public async Task<PedidoCliente> ObtenerPorIdAsync(int id) => await _context.PedidosCliente.FindAsync(id);
        public async Task<PedidoCliente> CrearAsync(PedidoCliente entity)
        {
            _context.PedidosCliente.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> ActualizarAsync(PedidoCliente entity)
        {
            _context.PedidosCliente.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> EliminarAsync(int id)
        {
            var entity = await _context.PedidosCliente.FindAsync(id);
            if (entity == null) return false;
            _context.PedidosCliente.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<IEnumerable<PedidoCliente>> ObtenerTodosConDetallesAsync()
        {
            return await _context.PedidosCliente
                .Include(p => p.Cliente)
                    .ThenInclude(c => c.CajasCliente)
                .ToListAsync();
        }
    }
} 