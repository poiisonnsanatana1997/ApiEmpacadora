using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Infrastructure.Data;
using AppAPIEmpacadora.Repositories.Interfaces;

namespace AppAPIEmpacadora.Infrastructure.Repositories
{
    public class OrdenPedidoClienteRepository : IOrdenPedidoClienteRepository
    {
        private readonly ApplicationDbContext _context;
        
        public OrdenPedidoClienteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrdenPedidoCliente>> ObtenerTodosAsync() => await _context.OrdenesPedidoCliente
            .Include(o => o.Producto)
            .Include(o => o.PedidoCliente)
            .ToListAsync();

        public async Task<OrdenPedidoCliente> ObtenerPorIdAsync(int id) => await _context.OrdenesPedidoCliente
            .Include(o => o.Producto)
            .Include(o => o.PedidoCliente)
            .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<OrdenPedidoCliente> CrearAsync(OrdenPedidoCliente entity)
        {
            _context.OrdenesPedidoCliente.Add(entity);
            await _context.SaveChangesAsync();
            
            // Recargar la entidad con las relaciones
            return await _context.OrdenesPedidoCliente
                .Include(o => o.Producto)
                .Include(o => o.PedidoCliente)
                .FirstOrDefaultAsync(o => o.Id == entity.Id);
        }

        public async Task<bool> ActualizarAsync(OrdenPedidoCliente entity)
        {
            _context.OrdenesPedidoCliente.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var entity = await _context.OrdenesPedidoCliente.FindAsync(id);
            if (entity == null) return false;
            _context.OrdenesPedidoCliente.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<OrdenPedidoCliente>> ObtenerTodosConDetallesAsync()
        {
            return await _context.OrdenesPedidoCliente
                .Include(o => o.Producto)
                .Include(o => o.PedidoCliente)
                    .ThenInclude(pc => pc.Cliente)
                .Include(o => o.PedidoCliente)
                    .ThenInclude(pc => pc.Sucursal)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrdenPedidoCliente>> ObtenerPorPedidoClienteAsync(int idPedidoCliente)
        {
            return await _context.OrdenesPedidoCliente
                .Include(o => o.Producto)
                .Include(o => o.PedidoCliente)
                .Where(o => o.IdPedidoCliente == idPedidoCliente)
                .ToListAsync();
        }
    }
} 