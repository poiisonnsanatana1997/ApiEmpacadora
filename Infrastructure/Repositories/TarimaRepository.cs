using AppAPIEmpacadora.Infrastructure.Data;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

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
            return await _context.Tarimas
                .Include(t => t.TarimasClasificaciones)
                .Include(t => t.PedidoTarimas)
                .FirstOrDefaultAsync(t => t.Id == id);
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
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Obtener la tarima con todas sus relaciones
                var tarima = await _context.Tarimas
                    .Include(t => t.TarimasClasificaciones)
                    .Include(t => t.PedidoTarimas)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (tarima == null)
                {
                    return false;
                }

                // Eliminar relaciones TarimaClasificacion en cascada
                if (tarima.TarimasClasificaciones?.Any() == true)
                {
                    _context.TarimaClasificaciones.RemoveRange(tarima.TarimasClasificaciones);
                }

                // Eliminar relaciones PedidoTarima en cascada
                if (tarima.PedidoTarimas?.Any() == true)
                {
                    _context.PedidoTarimas.RemoveRange(tarima.PedidoTarimas);
                }

                // Eliminar la tarima
                _context.Tarimas.Remove(tarima);
                
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

        public async Task<IEnumerable<Tarima>> GetTarimasParcialesAsync()
        {
            return await _context.Tarimas
                .Where(t => t.Estatus.ToLower() == "parcial")
                .Include(t => t.TarimasClasificaciones)
                    .ThenInclude(tc => tc.Clasificacion)
                        .ThenInclude(c => c.PedidoProveedor)
                            .ThenInclude(pp => pp.ProductosPedido)
                                .ThenInclude(pp => pp.Producto)
                .Include(t => t.PedidoTarimas)
                    .ThenInclude(pt => pt.PedidoCliente)
                        .ThenInclude(pc => pc.Cliente)
                .Include(t => t.PedidoTarimas)
                    .ThenInclude(pt => pt.PedidoCliente)
                        .ThenInclude(pc => pc.Sucursal)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Tarima>> GetTarimasParcialesYCompletasAsync()
        {
            return await _context.Tarimas
                .Where(t => t.Estatus.ToLower() == "parcial" || t.Estatus.ToLower() == "completa")
                .Include(t => t.TarimasClasificaciones)
                    .ThenInclude(tc => tc.Clasificacion)
                        .ThenInclude(c => c.PedidoProveedor)
                            .ThenInclude(pp => pp.ProductosPedido)
                                .ThenInclude(pp => pp.Producto)
                .Include(t => t.PedidoTarimas)
                    .ThenInclude(pt => pt.PedidoCliente)
                        .ThenInclude(pc => pc.Cliente)
                .Include(t => t.PedidoTarimas)
                    .ThenInclude(pt => pt.PedidoCliente)
                        .ThenInclude(pc => pc.Sucursal)
                .AsNoTracking()
                .ToListAsync();
        }

        // Nuevos métodos para actualización parcial
        public async Task<TarimaClasificacion> GetTarimaClasificacionAsync(int idTarima, int idClasificacion)
        {
            return await _context.TarimaClasificaciones
                .FirstOrDefaultAsync(tc => tc.IdTarima == idTarima && tc.IdClasificacion == idClasificacion);
        }

        public async Task<TarimaClasificacion> CreateTarimaClasificacionAsync(TarimaClasificacion tarimaClasificacion)
        {
            _context.TarimaClasificaciones.Add(tarimaClasificacion);
            await _context.SaveChangesAsync();
            return tarimaClasificacion;
        }

        public async Task<TarimaClasificacion> UpdateTarimaClasificacionAsync(TarimaClasificacion tarimaClasificacion)
        {
            _context.Entry(tarimaClasificacion).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return tarimaClasificacion;
        }

        // Método para obtener tarimas con clasificaciones incluidas
        public async Task<IEnumerable<Tarima>> GetAllWithClasificacionesAsync()
        {
            return await _context.Tarimas
                .Include(t => t.TarimasClasificaciones)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Tarima> GetByIdWithClasificacionesAsync(int id)
        {
            return await _context.Tarimas
                .Include(t => t.TarimasClasificaciones)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<dynamic>> GetDatosResumenDiarioAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Tarimas
                .Include(t => t.TarimasClasificaciones)
                .Where(t => t.FechaRegistro >= fechaInicio && 
                           t.FechaRegistro < fechaFin)
                .SelectMany(t => t.TarimasClasificaciones)
                .GroupBy(tc => tc.Tipo)
                .Select(g => new
                {
                    Tipo = g.Key,
                    CantidadTarimas = g.Count(),
                    CantidadTotal = g.Sum(tc => tc.Cantidad ?? 0),
                    PesoTotal = g.Sum(tc => tc.Peso),
                    TarimasCompletas = g.Count(tc => tc.Tarima.Estatus == "Completa"),
                    TarimasParciales = g.Count(tc => tc.Tarima.Estatus == "Parcial")
                })
                .ToListAsync();
        }

        public async Task<bool> DeleteTarimaClasificacionAsync(int idTarima, int idClasificacion)
        {
            try
            {
                var tarimaClasificacion = await _context.TarimaClasificaciones
                    .FirstOrDefaultAsync(tc => tc.IdTarima == idTarima && tc.IdClasificacion == idClasificacion);
                
                if (tarimaClasificacion == null) 
                    return false;
                
                _context.TarimaClasificaciones.Remove(tarimaClasificacion);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                // Log the specific database error
                // _logger.LogError(ex, "Error al eliminar TarimaClasificacion. IdTarima: {IdTarima}, IdClasificacion: {IdClasificacion}", idTarima, idClasificacion);
                return false;
            }
            catch (Exception ex)
            {
                // Log unexpected errors
                // _logger.LogError(ex, "Error inesperado al eliminar TarimaClasificacion. IdTarima: {IdTarima}, IdClasificacion: {IdClasificacion}", idTarima, idClasificacion);
                return false;
            }
        }

    }
} 