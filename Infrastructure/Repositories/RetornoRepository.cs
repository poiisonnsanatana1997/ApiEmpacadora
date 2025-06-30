using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Infrastructure.Data;
using AppAPIEmpacadora.Repositories.Interfaces;

namespace AppAPIEmpacadora.Infrastructure.Repositories
{
    public class RetornoRepository : IRetornoRepository
    {
        private readonly ApplicationDbContext _context;
        public RetornoRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Retorno>> ObtenerTodosAsync() => await _context.Retornos.ToListAsync();
        public async Task<Retorno> ObtenerPorIdAsync(int id) => await _context.Retornos.FindAsync(id);
        public async Task<Retorno> CrearAsync(Retorno entity)
        {
            _context.Retornos.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> ActualizarAsync(Retorno entity)
        {
            _context.Retornos.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> EliminarAsync(int id)
        {
            var entity = await _context.Retornos.FindAsync(id);
            if (entity == null) return false;
            _context.Retornos.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
} 