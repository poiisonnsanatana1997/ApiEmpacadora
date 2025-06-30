using AppAPIEmpacadora.Infrastructure.Data;
using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Repositories.Interfaces;
using AppAPIEmpacadora.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Services
{
    public class TarimaService : ITarimaService
    {
        private readonly ITarimaRepository _tarimaRepository;
        private readonly ApplicationDbContext _context;

        public TarimaService(ITarimaRepository tarimaRepository, ApplicationDbContext context)
        {
            _tarimaRepository = tarimaRepository;
            _context = context;
        }

        public async Task<IEnumerable<TarimaDTO>> GetTarimasAsync()
        {
            var tarimas = await _tarimaRepository.GetAllAsync();
            var tarimaDTOs = new List<TarimaDTO>();
            foreach (var tarima in tarimas)
            {
                tarimaDTOs.Add(new TarimaDTO
                {
                    Id = tarima.Id,
                    Codigo = tarima.Codigo,
                    Estatus = tarima.Estatus,
                    FechaRegistro = tarima.FechaRegistro,
                    FechaActualizacion = tarima.FechaActualizacion,
                    UsuarioRegistro = tarima.UsuarioRegistro,
                    UsuarioModificacion = tarima.UsuarioModificacion,
                    Tipo = tarima.Tipo,
                    Cantidad = tarima.Cantidad
                });
            }
            return tarimaDTOs;
        }

        public async Task<TarimaDTO> GetTarimaByIdAsync(int id)
        {
            var tarima = await _tarimaRepository.GetByIdAsync(id);
            if (tarima == null) return null;

            return new TarimaDTO
            {
                Id = tarima.Id,
                Codigo = tarima.Codigo,
                Estatus = tarima.Estatus,
                FechaRegistro = tarima.FechaRegistro,
                FechaActualizacion = tarima.FechaActualizacion,
                UsuarioRegistro = tarima.UsuarioRegistro,
                UsuarioModificacion = tarima.UsuarioModificacion,
                Tipo = tarima.Tipo,
                Cantidad = tarima.Cantidad
            };
        }

        public async Task<TarimaDTO> CreateTarimaAsync(CreateTarimaDTO createTarimaDto, string usuario)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var tarima = new Tarima
                {
                    Codigo = createTarimaDto.Codigo,
                    Estatus = createTarimaDto.Estatus,
                    FechaRegistro = createTarimaDto.FechaRegistro,
                    UsuarioRegistro = usuario,
                    Tipo = createTarimaDto.Tipo,
                    Cantidad = createTarimaDto.Cantidad
                };
                
                var nuevaTarima = await _tarimaRepository.AddAsync(tarima);

                // Crear la relación PedidoTarima
                var pedidoTarima = new PedidoTarima
                {
                    IdPedidoCliente = createTarimaDto.IdPedidoCliente,
                    IdTarima = nuevaTarima.Id
                };
                _context.PedidoTarimas.Add(pedidoTarima);

                var tarimaClasificacion = new TarimaClasificacion
                {
                    IdClasificacion = createTarimaDto.IdClasificacion,
                    IdTarima = nuevaTarima.Id,
                    Peso = 0 // Opcional: definir un valor por defecto si es necesario
                };
                _context.TarimaClasificaciones.Add(tarimaClasificacion);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new TarimaDTO
                {
                    Id = nuevaTarima.Id,
                    Codigo = nuevaTarima.Codigo,
                    Estatus = nuevaTarima.Estatus,
                    FechaRegistro = nuevaTarima.FechaRegistro,
                    UsuarioRegistro = nuevaTarima.UsuarioRegistro,
                    Tipo = nuevaTarima.Tipo,
                    Cantidad = nuevaTarima.Cantidad
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw; // Relanza la excepción para que sea manejada por el middleware de errores
            }
        }

        public async Task<TarimaDTO> UpdateTarimaAsync(int id, UpdateTarimaDTO updateTarimaDto, string usuario)
        {
            var tarima = await _tarimaRepository.GetByIdAsync(id);
            if (tarima == null) return null;

            if (updateTarimaDto.Codigo != null) tarima.Codigo = updateTarimaDto.Codigo;
            if (updateTarimaDto.Estatus != null) tarima.Estatus = updateTarimaDto.Estatus;
            if (updateTarimaDto.Tipo != null) tarima.Tipo = updateTarimaDto.Tipo;
            if (updateTarimaDto.Cantidad.HasValue) tarima.Cantidad = updateTarimaDto.Cantidad.Value;

            tarima.UsuarioModificacion = usuario;
            tarima.FechaActualizacion = updateTarimaDto.FechaActualizacion;
            
            var tarimaActualizada = await _tarimaRepository.UpdateAsync(tarima);

            return new TarimaDTO
            {
                Id = tarimaActualizada.Id,
                Codigo = tarimaActualizada.Codigo,
                Estatus = tarimaActualizada.Estatus,
                FechaRegistro = tarimaActualizada.FechaRegistro,
                FechaActualizacion = tarimaActualizada.FechaActualizacion,
                UsuarioRegistro = tarimaActualizada.UsuarioRegistro,
                UsuarioModificacion = tarimaActualizada.UsuarioModificacion,
                Tipo = tarimaActualizada.Tipo,
                Cantidad = tarimaActualizada.Cantidad
            };
        }

        public async Task<bool> DeleteTarimaAsync(int id)
        {
            return await _tarimaRepository.DeleteAsync(id);
        }
    }
} 