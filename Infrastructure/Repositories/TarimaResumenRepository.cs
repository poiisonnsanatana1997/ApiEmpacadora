using Microsoft.EntityFrameworkCore;
using AppAPIEmpacadora.Infrastructure.Data;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Repositories.Interfaces;

namespace AppAPIEmpacadora.Infrastructure.Repositories
{
    public class TarimaResumenRepository : ITarimaResumenRepository
    {
        private readonly ApplicationDbContext _context;

        public TarimaResumenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TarimaResumenDiario?> GetByFechaYTipoAsync(DateTime fecha, string tipo)
        {
            return await _context.TarimaResumenDiarios
                .FirstOrDefaultAsync(r => r.Fecha == fecha.Date && r.Tipo == tipo);
        }

        public async Task<IEnumerable<TarimaResumenDiario>> GetByFechaAsync(DateTime fecha)
        {
            return await _context.TarimaResumenDiarios
                .Where(r => r.Fecha == fecha.Date)
                .OrderBy(r => r.Tipo)
                .ToListAsync();
        }

        public async Task<IEnumerable<TarimaResumenDiario>> GetByRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.TarimaResumenDiarios
                .Where(r => r.Fecha >= fechaInicio.Date && r.Fecha <= fechaFin.Date)
                .OrderBy(r => r.Fecha)
                .ThenBy(r => r.Tipo)
                .ToListAsync();
        }

        public async Task<IEnumerable<TarimaResumenDiario>> GetByTipoAsync(string tipo, DateTime? fechaInicio, DateTime? fechaFin)
        {
            var query = _context.TarimaResumenDiarios
                .Where(r => r.Tipo == tipo);

            if (fechaInicio.HasValue)
                query = query.Where(r => r.Fecha >= fechaInicio.Value.Date);

            if (fechaFin.HasValue)
                query = query.Where(r => r.Fecha <= fechaFin.Value.Date);

            return await query
                .OrderBy(r => r.Fecha)
                .ToListAsync();
        }

        public async Task<TarimaResumenDiario> AddAsync(TarimaResumenDiario tarimaResumen)
        {
            _context.TarimaResumenDiarios.Add(tarimaResumen);
            await _context.SaveChangesAsync();
            return tarimaResumen;
        }

        public async Task<TarimaResumenDiario> UpdateAsync(TarimaResumenDiario tarimaResumen)
        {
            _context.TarimaResumenDiarios.Update(tarimaResumen);
            await _context.SaveChangesAsync();
            return tarimaResumen;
        }

        public async Task<bool> ExistsAsync(DateTime fecha, string tipo)
        {
            return await _context.TarimaResumenDiarios
                .AnyAsync(r => r.Fecha == fecha.Date && r.Tipo == tipo);
        }

        public async Task<IEnumerable<TarimaResumenDiario>> GetAllAsync()
        {
            return await _context.TarimaResumenDiarios
                .OrderBy(r => r.Fecha)
                .ThenBy(r => r.Tipo)
                .ToListAsync();
        }

        public async Task AddBatchAsync(IEnumerable<TarimaResumenDiario> resumenes)
        {
            _context.TarimaResumenDiarios.AddRange(resumenes);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBatchAsync(IEnumerable<TarimaResumenDiario> resumenes)
        {
            _context.TarimaResumenDiarios.UpdateRange(resumenes);
            await _context.SaveChangesAsync();
        }

        public async Task ExecuteInTransactionAsync(Func<Task> operation)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await operation();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
