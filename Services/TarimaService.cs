using AppAPIEmpacadora.Infrastructure.Data;
using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Repositories.Interfaces;
using AppAPIEmpacadora.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;

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
            var tarimas = await _tarimaRepository.GetAllWithClasificacionesAsync();
            var tarimaDTOs = new List<TarimaDTO>();
            foreach (var tarima in tarimas)
            {
                // Calcular la cantidad total sumando todas las cantidades de las relaciones tarima-clasificación
                decimal? cantidadTotal = null;
                if (tarima.TarimasClasificaciones != null && tarima.TarimasClasificaciones.Any())
                {
                    cantidadTotal = tarima.TarimasClasificaciones.Sum(tc => tc.Cantidad ?? 0);
                }

                tarimaDTOs.Add(new TarimaDTO
                {
                    Id = tarima.Id,
                    Codigo = tarima.Codigo,
                    Estatus = tarima.Estatus,
                    FechaRegistro = tarima.FechaRegistro,
                    FechaActualizacion = tarima.FechaActualizacion,
                    UsuarioRegistro = tarima.UsuarioRegistro,
                    UsuarioModificacion = tarima.UsuarioModificacion,
                    Observaciones = tarima.Observaciones,
                    UPC = tarima.UPC,
                    Peso = tarima.Peso,
                    Cantidad = cantidadTotal
                });
            }
            return tarimaDTOs;
        }

        public async Task<TarimaDTO> GetTarimaByIdAsync(int id)
        {
            var tarima = await _tarimaRepository.GetByIdWithClasificacionesAsync(id);
            if (tarima == null) return null;

            // Calcular la cantidad total sumando todas las cantidades de las relaciones tarima-clasificación
            decimal? cantidadTotal = null;
            if (tarima.TarimasClasificaciones != null && tarima.TarimasClasificaciones.Any())
            {
                cantidadTotal = tarima.TarimasClasificaciones.Sum(tc => tc.Cantidad ?? 0);
            }

            return new TarimaDTO
            {
                Id = tarima.Id,
                Codigo = tarima.Codigo,
                Estatus = tarima.Estatus,
                FechaRegistro = tarima.FechaRegistro,
                FechaActualizacion = tarima.FechaActualizacion,
                UsuarioRegistro = tarima.UsuarioRegistro,
                UsuarioModificacion = tarima.UsuarioModificacion,
                Observaciones = tarima.Observaciones,
                UPC = tarima.UPC,
                Peso = tarima.Peso,
                Cantidad = cantidadTotal
            };
        }

        public async Task<CreateTarimaResponseDTO> CreateTarimaAsync(CreateTarimaDTO createTarimaDto, string usuario)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var tarimasCreadas = new List<TarimaDTO>();
                var fechaHoy = DateTime.Now.ToString("yyyyMMdd");
                
                // Obtener el último número secuencial del día
                var ultimoCodigo = await _context.Tarimas
                    .Where(t => t.Codigo.StartsWith($"TAR-{fechaHoy}-"))
                    .OrderByDescending(t => t.Codigo)
                    .Select(t => t.Codigo)
                    .FirstOrDefaultAsync();

                int siguienteNumero = 1;
                if (!string.IsNullOrEmpty(ultimoCodigo))
                {
                    var partes = ultimoCodigo.Split('-');
                    if (partes.Length == 3 && int.TryParse(partes[2], out int ultimoNumero))
                    {
                        siguienteNumero = ultimoNumero + 1;
                    }
                }

                // Crear múltiples tarimas
                for (int i = 0; i < createTarimaDto.CantidadTarimas; i++)
                {
                    var codigoTarima = $"TAR-{fechaHoy}-{siguienteNumero:D3}";
                    
                    var tarima = new Tarima
                    {
                        Codigo = codigoTarima,
                        Estatus = createTarimaDto.Estatus,
                        FechaRegistro = createTarimaDto.FechaRegistro,
                        UsuarioRegistro = usuario,
                        Observaciones = createTarimaDto.Observaciones,
                        UPC = createTarimaDto.UPC,
                        Peso = createTarimaDto.Peso
                    };
                    
                    var nuevaTarima = await _tarimaRepository.AddAsync(tarima);

                    // Crear la relación PedidoTarima solo si se proporciona IdPedidoCliente
                    if (createTarimaDto.IdPedidoCliente > 0)
                    {
                        var pedidoTarima = new PedidoTarima
                        {
                            IdPedidoCliente = createTarimaDto.IdPedidoCliente.Value,
                            IdTarima = nuevaTarima.Id
                        };
                        _context.PedidoTarimas.Add(pedidoTarima);
                    }

                    // Crear la relación TarimaClasificacion solo si se proporciona IdClasificacion
                    if (createTarimaDto.IdClasificacion > 0)
                    {
                        var tarimaClasificacion = new TarimaClasificacion
                        {
                            IdClasificacion = createTarimaDto.IdClasificacion.Value,
                            IdTarima = nuevaTarima.Id,
                            Peso = createTarimaDto.Peso ?? 0,
                            Tipo = createTarimaDto.Tipo,
                            Cantidad = createTarimaDto.CantidadTarimas
                        };
                        _context.TarimaClasificaciones.Add(tarimaClasificacion);
                    }

                    // Agregar a la lista de tarimas creadas
                    tarimasCreadas.Add(new TarimaDTO
                    {
                        Id = nuevaTarima.Id,
                        Codigo = nuevaTarima.Codigo,
                        Estatus = nuevaTarima.Estatus,
                        FechaRegistro = nuevaTarima.FechaRegistro,
                        UsuarioRegistro = nuevaTarima.UsuarioRegistro,
                        Observaciones = nuevaTarima.Observaciones,
                        UPC = nuevaTarima.UPC,
                        Peso = nuevaTarima.Peso,
                        Cantidad = createTarimaDto.IdClasificacion > 0 ? createTarimaDto.CantidadTarimas : null
                    });

                    siguienteNumero++;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new CreateTarimaResponseDTO
                {
                    TarimasCreadas = tarimasCreadas,
                    CantidadCreada = tarimasCreadas.Count,
                    Mensaje = $"Se crearon exitosamente {tarimasCreadas.Count} tarimas"
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<TarimaDTO> UpdateTarimaAsync(int id, UpdateTarimaDTO updateTarimaDto, string usuario)
        {
            var tarima = await _tarimaRepository.GetByIdWithClasificacionesAsync(id);
            if (tarima == null) return null;

            if (updateTarimaDto.Codigo != null) tarima.Codigo = updateTarimaDto.Codigo;
            if (updateTarimaDto.Estatus != null) tarima.Estatus = updateTarimaDto.Estatus;
            if (updateTarimaDto.Observaciones != null) tarima.Observaciones = updateTarimaDto.Observaciones;
            if (updateTarimaDto.UPC != null) tarima.UPC = updateTarimaDto.UPC;
            if (updateTarimaDto.Peso.HasValue) tarima.Peso = updateTarimaDto.Peso.Value;

            tarima.UsuarioModificacion = usuario;
            tarima.FechaActualizacion = updateTarimaDto.FechaActualizacion;
            
            var tarimaActualizada = await _tarimaRepository.UpdateAsync(tarima);

            // Calcular la cantidad total sumando todas las cantidades de las relaciones tarima-clasificación
            decimal? cantidadTotal = null;
            if (tarima.TarimasClasificaciones != null && tarima.TarimasClasificaciones.Any())
            {
                cantidadTotal = tarima.TarimasClasificaciones.Sum(tc => tc.Cantidad ?? 0);
            }

            return new TarimaDTO
            {
                Id = tarimaActualizada.Id,
                Codigo = tarimaActualizada.Codigo,
                Estatus = tarimaActualizada.Estatus,
                FechaRegistro = tarimaActualizada.FechaRegistro,
                FechaActualizacion = tarimaActualizada.FechaActualizacion,
                UsuarioRegistro = tarimaActualizada.UsuarioRegistro,
                UsuarioModificacion = tarimaActualizada.UsuarioModificacion,
                Observaciones = tarimaActualizada.Observaciones,
                UPC = tarimaActualizada.UPC,
                Peso = tarimaActualizada.Peso,
                Cantidad = cantidadTotal
            };
        }

        public async Task<bool> DeleteTarimaAsync(int id)
        {
            var tarima = await _tarimaRepository.GetByIdAsync(id);
            if (tarima == null) return false;

            return await _tarimaRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<TarimaParcialCompletaDTO>> GetTarimasParcialesAsync()
        {
            var tarimas = await _tarimaRepository.GetTarimasParcialesAsync();
            var tarimasParciales = new List<TarimaParcialCompletaDTO>();

            foreach (var tarima in tarimas)
            {
                var tarimaParcial = new TarimaParcialCompletaDTO
                {
                    Id = tarima.Id,
                    Codigo = tarima.Codigo,
                    Estatus = tarima.Estatus,
                    FechaRegistro = tarima.FechaRegistro,
                    FechaActualizacion = tarima.FechaActualizacion,
                    UsuarioRegistro = tarima.UsuarioRegistro,
                    UsuarioModificacion = tarima.UsuarioModificacion,
                    Observaciones = tarima.Observaciones,
                    UPC = tarima.UPC,
                    Peso = tarima.Peso
                };

                // Mapear TarimasClasificaciones
                if (tarima.TarimasClasificaciones != null)
                {
                    foreach (var tc in tarima.TarimasClasificaciones)
                    {
                        tarimaParcial.TarimasClasificaciones.Add(new TarimaClasificacionParcialDTO
                        {
                            IdClasificacion = tc.IdClasificacion,
                            Lote = tc.Clasificacion?.Lote ?? string.Empty,
                            Peso = tc.Peso,
                            Tipo = tc.Tipo,
                            Cantidad = tc.Cantidad,
                            PesoTotal = tc.Clasificacion?.PesoTotal ?? 0,
                            FechaRegistro = tc.Clasificacion?.FechaRegistro ?? DateTime.MinValue,
                            UsuarioRegistro = tc.Clasificacion?.UsuarioRegistro ?? string.Empty
                        });
                    }
                }

                // Mapear PedidoTarimas
                if (tarima.PedidoTarimas != null)
                {
                    foreach (var pt in tarima.PedidoTarimas)
                    {
                        tarimaParcial.PedidoTarimas.Add(new PedidoTarimaDTO
                        {
                            IdPedidoCliente = pt.IdPedidoCliente,
                            Observaciones = pt.PedidoCliente?.Observaciones ?? string.Empty,
                            Estatus = pt.PedidoCliente?.Estatus ?? string.Empty,
                            FechaEmbarque = pt.PedidoCliente?.FechaEmbarque,
                            FechaRegistro = pt.PedidoCliente?.FechaRegistro ?? DateTime.MinValue,
                            UsuarioRegistro = pt.PedidoCliente?.UsuarioRegistro ?? string.Empty,
                            NombreCliente = pt.PedidoCliente?.Cliente?.Nombre ?? string.Empty,
                            NombreSucursal = pt.PedidoCliente?.Sucursal?.Nombre ?? string.Empty
                        });
                    }
                }

                tarimasParciales.Add(tarimaParcial);
            }

            return tarimasParciales;
        }

        public async Task<TarimaDTO> UpdateTarimaParcialAsync(TarimaUpdateParcialDTO dto, string usuario)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validaciones iniciales
                var tarima = await _tarimaRepository.GetByIdAsync(dto.IdTarima);
                if (tarima == null)
                {
                    throw new ArgumentException($"La tarima con ID {dto.IdTarima} no existe");
                }

                if (dto.Cantidad <= 0)
                {
                    throw new ArgumentException("La cantidad debe ser mayor a 0");
                }

                if (tarima.Peso <= 0)
                {
                    throw new ArgumentException("La tarima debe tener un peso válido para realizar el cálculo");
                }

                // Verificar si existe la relación Tarima-Clasificación
                var tarimaClasificacion = await _tarimaRepository.GetTarimaClasificacionAsync(dto.IdTarima, dto.IdClasificacion);

                if (tarimaClasificacion != null)
                {
                    // Regla 1: La relación EXISTE - Actualizar cantidad y peso
                    tarimaClasificacion.Cantidad = (tarimaClasificacion.Cantidad ?? 0) + dto.Cantidad;
                    var pesoCalculado = dto.Cantidad * tarima.Peso.Value;
                    tarimaClasificacion.Peso += pesoCalculado;
                    
                    await _tarimaRepository.UpdateTarimaClasificacionAsync(tarimaClasificacion);
                }
                else
                {
                    // Regla 2: La relación NO EXISTE - Crear nueva relación
                    var pesoCalculado = dto.Cantidad * tarima.Peso.Value;
                    
                    var nuevaTarimaClasificacion = new TarimaClasificacion
                    {
                        IdTarima = dto.IdTarima,
                        IdClasificacion = dto.IdClasificacion,
                        Peso = pesoCalculado,
                        Tipo = dto.Tipo, // Usar el tipo proporcionado en el DTO
                        Cantidad = dto.Cantidad
                    };
                    
                    await _tarimaRepository.CreateTarimaClasificacionAsync(nuevaTarimaClasificacion);
                }

                // Actualizar el estatus de la tarima
                tarima.Estatus = dto.Estatus;
                tarima.UsuarioModificacion = usuario;
                tarima.FechaActualizacion = DateTime.Now;

                var tarimaActualizada = await _tarimaRepository.UpdateAsync(tarima);

                await transaction.CommitAsync();

                // Obtener la tarima actualizada con clasificaciones para calcular la cantidad total
                var tarimaConClasificaciones = await _tarimaRepository.GetByIdWithClasificacionesAsync(tarimaActualizada.Id);
                
                // Calcular la cantidad total sumando todas las cantidades de las relaciones tarima-clasificación
                decimal? cantidadTotal = null;
                if (tarimaConClasificaciones?.TarimasClasificaciones != null && tarimaConClasificaciones.TarimasClasificaciones.Any())
                {
                    cantidadTotal = tarimaConClasificaciones.TarimasClasificaciones.Sum(tc => tc.Cantidad ?? 0);
                }

                // Retornar la tarima actualizada
                return new TarimaDTO
                {
                    Id = tarimaActualizada.Id,
                    Codigo = tarimaActualizada.Codigo,
                    Estatus = tarimaActualizada.Estatus,
                    FechaRegistro = tarimaActualizada.FechaRegistro,
                    FechaActualizacion = tarimaActualizada.FechaActualizacion,
                    UsuarioRegistro = tarimaActualizada.UsuarioRegistro,
                    UsuarioModificacion = tarimaActualizada.UsuarioModificacion,
                    Observaciones = tarimaActualizada.Observaciones,
                    UPC = tarimaActualizada.UPC,
                    Peso = tarimaActualizada.Peso,
                    Cantidad = cantidadTotal
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
} 