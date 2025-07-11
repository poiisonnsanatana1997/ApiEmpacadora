using Microsoft.EntityFrameworkCore;
using AppAPIEmpacadora.Infrastructure.Data;
using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Repositories.Interfaces;

namespace AppAPIEmpacadora.Infrastructure.Repositories
{
    public class ProveedorRepository : IProveedorRepository
    {
        private readonly ApplicationDbContext _context;

        public ProveedorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProveedorSimpleDTO>> ObtenerTodosAsync()
        {
            return await _context.Proveedores
                .Where(x => x.Activo)
                .Select(p => new ProveedorSimpleDTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre
                })
                .ToListAsync();
        }

        public async Task<ProveedorSimpleDTO> ObtenerPorIdAsync(int id)
        {
            var proveedor = await _context.Proveedores
                .Where(p => p.Id == id)
                .Select(p => new ProveedorSimpleDTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre
                })
                .FirstOrDefaultAsync();

            return proveedor;
        }

        public async Task<IEnumerable<ProveedorDTO>> GetAllAsync()
        {
            return await _context.Proveedores
                .Select(p => new ProveedorDTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    RFC = p.RFC,
                    Activo = p.Activo,
                    Telefono = p.Telefono,
                    Correo = p.Correo,
                    DireccionFiscal = p.DireccionFiscal,
                    SituacionFiscal = p.SituacionFiscal,
                    FechaRegistro = p.FechaRegistro,
                    UsuarioRegistro = p.UsuarioRegistro
                })
                .ToListAsync();
        }

        public async Task<ProveedorDTO> GetByIdAsync(int id)
        {
            var proveedor = await _context.Proveedores
                .Where(p => p.Id == id)
                .Select(p => new ProveedorDTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    RFC = p.RFC,
                    Activo = p.Activo,
                    Telefono = p.Telefono,
                    Correo = p.Correo,
                    DireccionFiscal = p.DireccionFiscal,
                    SituacionFiscal = p.SituacionFiscal,
                    FechaRegistro = p.FechaRegistro,
                    UsuarioRegistro = p.UsuarioRegistro
                })
                .FirstOrDefaultAsync();

            return proveedor;
        }

        public async Task<Proveedor> AddAsync(Proveedor proveedor)
        {
            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();
            return proveedor;
        }

        public async Task<Proveedor> UpdateAsync(Proveedor proveedor)
        {
            _context.Proveedores.Update(proveedor);
            await _context.SaveChangesAsync();
            return proveedor;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null)
                return false;

            _context.Proveedores.Remove(proveedor);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}