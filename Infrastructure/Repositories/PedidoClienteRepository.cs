using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Infrastructure.Data;
using AppAPIEmpacadora.Repositories.Interfaces;
using AppAPIEmpacadora.Models.DTOs;
using System.Linq;

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
                .ThenInclude(c => c.CajasCliente)
            .Include(p => p.PedidoTarimas)
                .ThenInclude(pt => pt.Tarima)
                    .ThenInclude(t => t.TarimasClasificaciones)
                        .ThenInclude(tc => tc.Clasificacion)
                            .ThenInclude(c => c.PedidoProveedor)
                                .ThenInclude(pp => pp.ProductosPedido)
                                    .ThenInclude(pp => pp.Producto)
            .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<PedidoCliente> ObtenerPorIdConRelacionesCompletasAsync(int id) => await _context.PedidosCliente
            .Include(p => p.Sucursal)
            .Include(p => p.Cliente)
                .ThenInclude(c => c.CajasCliente)
            .Include(p => p.OrdenesPedidoCliente)
                .ThenInclude(o => o.Producto)
            .Include(p => p.PedidoTarimas)
                .ThenInclude(pt => pt.Tarima)
                    .ThenInclude(t => t.TarimasClasificaciones)
                        .ThenInclude(tc => tc.Clasificacion)
                            .ThenInclude(c => c.PedidoProveedor)
                                .ThenInclude(pp => pp.ProductosPedido)
                                    .ThenInclude(pp => pp.Producto)
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

        public async Task<bool> AsignarTarimasAPedidoAsync(int pedidoId, List<int> tarimaIds)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Verificar que el pedido existe
                var pedido = await _context.PedidosCliente.FindAsync(pedidoId);
                if (pedido == null)
                    return false;

                // Verificar que las tarimas existen y están disponibles
                var tarimas = await _context.Tarimas
                    .Where(t => tarimaIds.Contains(t.Id))
                    .ToListAsync();

                if (tarimas.Count != tarimaIds.Count)
                    return false;

                // Crear las relaciones PedidoTarima
                var pedidoTarimas = tarimaIds.Select(tarimaId => new PedidoTarima
                {
                    IdPedidoCliente = pedidoId,
                    IdTarima = tarimaId
                }).ToList();

                _context.PedidoTarimas.AddRange(pedidoTarimas);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<decimal> CalcularPorcentajeSurtidoAsync(int pedidoId)
        {
            var pedido = await _context.PedidosCliente
                .Include(p => p.OrdenesPedidoCliente)
                .Include(p => p.PedidoTarimas)
                    .ThenInclude(pt => pt.Tarima)
                        .ThenInclude(t => t.TarimasClasificaciones)
                .FirstOrDefaultAsync(p => p.Id == pedidoId);

            if (pedido == null)
                return 0;

            decimal cantidadSolicitada = 0;
            decimal cantidadAsignada = 0;

            // Calcular cantidad solicitada por tipo
            var cantidadesPorTipo = pedido.OrdenesPedidoCliente
                .GroupBy(o => o.Tipo)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(o => o.Cantidad ?? 0)
                );

            // Calcular cantidad asignada por tipo
            var cantidadesAsignadasPorTipo = pedido.PedidoTarimas
                .SelectMany(pt => pt.Tarima.TarimasClasificaciones)
                .GroupBy(tc => tc.Tipo)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(tc => tc.Cantidad ?? 0)
                );

            // Calcular porcentaje por tipo
            var porcentajesPorTipo = new List<decimal>();

            foreach (var tipo in cantidadesPorTipo.Keys)
            {
                var solicitada = cantidadesPorTipo[tipo];
                var asignada = cantidadesAsignadasPorTipo.ContainsKey(tipo) ? cantidadesAsignadasPorTipo[tipo] : 0;

                if (solicitada > 0)
                {
                    var porcentaje = (asignada / solicitada) * 100;
                    porcentajesPorTipo.Add(porcentaje);
                }
            }

            // Retornar el promedio de los porcentajes por tipo
            return porcentajesPorTipo.Any() ? porcentajesPorTipo.Average() : 0;
        }

        public async Task<bool> EliminarPedidoTarimasAsync(List<DesasignacionTarimaDTO> desasignaciones)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Obtener todos los registros y filtrar en memoria
                var todosLosRegistros = await _context.PedidoTarimas.ToListAsync();
                
                var registrosExistentes = todosLosRegistros
                    .Where(pt => desasignaciones.Any(d => d.IdPedido == pt.IdPedidoCliente && d.IdTarima == pt.IdTarima))
                    .ToList();

                if (registrosExistentes.Count != desasignaciones.Count)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                // Eliminar los registros
                _context.PedidoTarimas.RemoveRange(registrosExistentes);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DesasignarTodasLasTarimasAsync(int pedidoId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var pedidoTarimas = await _context.PedidoTarimas
                    .Where(pt => pt.IdPedidoCliente == pedidoId)
                    .ToListAsync();

                if (pedidoTarimas.Any())
                {
                    _context.PedidoTarimas.RemoveRange(pedidoTarimas);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
} 