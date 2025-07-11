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

        public async Task<IEnumerable<Tarima>> GetTarimasParcialesAsync()
        {
            return await _context.Tarimas
                .Where(t => t.Estatus.ToLower() == "parcial")
                .Include(t => t.TarimasClasificaciones)
                    .ThenInclude(tc => tc.Clasificacion)
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
    }
} 