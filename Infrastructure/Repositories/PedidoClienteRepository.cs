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
        public async Task<IEnumerable<PedidoCliente>> ObtenerTodosAsync() => await _context.PedidosCliente
            .Include(p => p.Sucursal)
            .Include(p => p.Cliente)
            .ToListAsync();
        public async Task<PedidoCliente> ObtenerPorIdAsync(int id) => await _context.PedidosCliente
            .Include(p => p.Sucursal)
            .Include(p => p.Cliente)
            .Include(p => p.PedidoTarimas)
                .ThenInclude(pt => pt.Tarima)
                    .ThenInclude(t => t.TarimasClasificaciones)
            .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<PedidoCliente> ObtenerPorIdConRelacionesCompletasAsync(int id) => await _context.PedidosCliente
            .Include(p => p.Sucursal)
            .Include(p => p.Cliente)
            .Include(p => p.OrdenesPedidoCliente)
            .Include(p => p.PedidoTarimas)
                .ThenInclude(pt => pt.Tarima)
                    .ThenInclude(t => t.TarimasClasificaciones)
            .FirstOrDefaultAsync(p => p.Id == id);
        public async Task<PedidoCliente> CrearAsync(PedidoCliente entity)
        {
            _context.PedidosCliente.Add(entity);
            await _context.SaveChangesAsync();
            
            // Cargar las relaciones después de guardar
            await _context.Entry(entity)
                .Reference(p => p.Sucursal)
                .LoadAsync();
            await _context.Entry(entity)
                .Reference(p => p.Cliente)
                .LoadAsync();
            
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
                .Include(p => p.Sucursal)
                .Include(p => p.Cliente)
                    .ThenInclude(c => c.CajasCliente)
                .ToListAsync();
        }

        public async Task<IEnumerable<PedidoCliente>> ObtenerPorTipoConTarimasAsync(string tipo)
        {
            return await _context.PedidosCliente
                .Include(p => p.Sucursal)
                .Include(p => p.Cliente)
                    .ThenInclude(c => c.CajasCliente)
                .Include(p => p.OrdenesPedidoCliente.Where(o => o.Tipo == tipo))
                    .ThenInclude(o => o.Producto)
                .Include(p => p.PedidoTarimas)
                    .ThenInclude(pt => pt.Tarima)
                        .ThenInclude(t => t.TarimasClasificaciones.Where(tc => tc.Tipo == tipo))
                .Where(p => p.OrdenesPedidoCliente.Any(o => o.Tipo == tipo))
                .ToListAsync();
        }
    }
} 