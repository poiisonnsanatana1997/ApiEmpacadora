using AppAPIEmpacadora.Infrastructure.Data;
using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Repositories.Interfaces;
using AppAPIEmpacadora.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppAPIEmpacadora.Services
{
    public class TarimaService : ITarimaService
    {
        private readonly ITarimaRepository _tarimaRepository;
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _serviceProvider;

        public TarimaService(ITarimaRepository tarimaRepository, ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            _tarimaRepository = tarimaRepository;
            _context = context;
            _serviceProvider = serviceProvider;
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

                        var pesoTotal = (createTarimaDto.Peso ?? 0) * createTarimaDto.Cantidad;

                        var tarimaClasificacion = new TarimaClasificacion
                        {
                            IdClasificacion = createTarimaDto.IdClasificacion.Value,
                            IdTarima = nuevaTarima.Id,
                            Peso = pesoTotal,
                            Tipo = createTarimaDto.Tipo,
                            Cantidad = createTarimaDto.Cantidad
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
                        Cantidad = createTarimaDto.IdClasificacion > 0 ? createTarimaDto.Cantidad : null
                    });

                    siguienteNumero++;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Calcular y actualizar porcentaje de surtido si se asignó a un pedido cliente
                if (createTarimaDto.IdPedidoCliente > 0)
                {
                    var pedidoClienteService = _serviceProvider.GetRequiredService<IPedidoClienteService>();
                    await pedidoClienteService.ActualizarPorcentajeSurtidoAsync(createTarimaDto.IdPedidoCliente.Value);
                }

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
                            UsuarioRegistro = tc.Clasificacion?.UsuarioRegistro ?? string.Empty,
                            Productos = tc.Clasificacion?.PedidoProveedor?.ProductosPedido?.Select(pp => new ProductoSimpleDTO
                            {
                                Id = pp.Producto.Id,
                                Codigo = pp.Producto.Codigo,
                                Nombre = pp.Producto.Nombre,
                                Variedad = pp.Producto.Variedad
                            }).ToList() ?? new List<ProductoSimpleDTO>()
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

        public async Task<IEnumerable<TarimaParcialCompletaDTO>> GetTarimasParcialesYCompletasAsync()
        {
            var tarimas = await _tarimaRepository.GetTarimasParcialesYCompletasAsync();
            var tarimasResult = new List<TarimaParcialCompletaDTO>();

            foreach (var tarima in tarimas)
            {
                var tarimaDto = new TarimaParcialCompletaDTO
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
                        tarimaDto.TarimasClasificaciones.Add(new TarimaClasificacionParcialDTO
                        {
                            IdClasificacion = tc.IdClasificacion,
                            Lote = tc.Clasificacion?.Lote ?? string.Empty,
                            Peso = tc.Peso,
                            Tipo = tc.Tipo,
                            Cantidad = tc.Cantidad,
                            PesoTotal = tc.Clasificacion?.PesoTotal ?? 0,
                            FechaRegistro = tc.Clasificacion?.FechaRegistro ?? DateTime.MinValue,
                            UsuarioRegistro = tc.Clasificacion?.UsuarioRegistro ?? string.Empty,
                            Productos = tc.Clasificacion?.PedidoProveedor?.ProductosPedido?.Select(pp => new ProductoSimpleDTO
                            {
                                Id = pp.Producto.Id,
                                Codigo = pp.Producto.Codigo,
                                Nombre = pp.Producto.Nombre,
                                Variedad = pp.Producto.Variedad
                            }).ToList() ?? new List<ProductoSimpleDTO>()
                        });
                    }
                }

                // Mapear PedidoTarimas
                if (tarima.PedidoTarimas != null)
                {
                    foreach (var pt in tarima.PedidoTarimas)
                    {
                        tarimaDto.PedidoTarimas.Add(new PedidoTarimaDTO
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

                tarimasResult.Add(tarimaDto);
            }

            return tarimasResult;
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

                // Calcular y actualizar porcentaje de surtido si la tarima está asignada a un pedido cliente
                var pedidoTarima = await _context.PedidoTarimas
                    .Where(pt => pt.IdTarima == tarimaActualizada.Id)
                    .FirstOrDefaultAsync();
                
                if (pedidoTarima != null)
                {
                    var pedidoClienteService = _serviceProvider.GetRequiredService<IPedidoClienteService>();
                    await pedidoClienteService.ActualizarPorcentajeSurtidoAsync(pedidoTarima.IdPedidoCliente);
                }

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

        public async Task<List<PedidoClienteResponseDTO>> BuscarPedidosCompatiblesAsync(List<TarimaAsignacionRequestDTO> tarimasAsignacion)
        {
            if (tarimasAsignacion == null || !tarimasAsignacion.Any())
            {
                throw new ArgumentException("La lista de tarimas para asignación no puede estar vacía");
            }

            // 1. Agrupar tarimas por tipo y producto, sumando cantidades
            var tarimasAgrupadas = tarimasAsignacion
                .GroupBy(t => new { t.Tipo, t.IdProducto })
                .Select(g => new
                {
                    Tipo = g.Key.Tipo,
                    IdProducto = g.Key.IdProducto,
                    CantidadTotal = g.Sum(t => t.Cantidad)
                })
                .ToList();

            // 2. Obtener pedidos con estatus "Pendiente" o "Surtiendo"
            var pedidosValidos = await _context.PedidosCliente
                .Include(pc => pc.Sucursal)
                .Include(pc => pc.Cliente)
                .Include(pc => pc.OrdenesPedidoCliente)
                .Where(pc => (pc.Estatus == "Pendiente" || pc.Estatus == "Surtiendo") && pc.Activo)
                .ToListAsync();

            var pedidosCompletos = new List<PedidoClienteResponseDTO>();

            // 3. Para cada pedido, verificar si es compatible con TODOS los grupos de tarimas
            foreach (var pedido in pedidosValidos)
            {
                bool esCompatibleConTodosLosGrupos = true;

                // Verificar compatibilidad con cada grupo de tarimas
                foreach (var grupoTarimas in tarimasAgrupadas)
                {
                    // 4. Filtrar órdenes que coincidan con el tipo y ID de producto
                    var ordenesCoincidentes = pedido.OrdenesPedidoCliente
                        .Where(o => o.Tipo == grupoTarimas.Tipo && o.IdProducto == grupoTarimas.IdProducto)
                        .ToList();

                    if (!ordenesCoincidentes.Any())
                    {
                        esCompatibleConTodosLosGrupos = false;
                        break;
                    }

                    // 5. Calcular cantidad disponible por surtir en las órdenes
                    var cantidadDisponible = ordenesCoincidentes.Sum(o => o.Cantidad ?? 0);

                    // 6. Validar que la cantidad disponible sea mayor o igual a la cantidad de tarimas
                    if (cantidadDisponible < grupoTarimas.CantidadTotal)
                    {
                        esCompatibleConTodosLosGrupos = false;
                        break;
                    }
                }

                // 7. Solo agregar el pedido si es compatible con TODOS los grupos de tarimas
                if (esCompatibleConTodosLosGrupos)
                {
                    var pedidoDTO = new PedidoClienteResponseDTO
                    {
                        Id = pedido.Id,
                        Observaciones = pedido.Observaciones,
                        Estatus = pedido.Estatus,
                        FechaEmbarque = pedido.FechaEmbarque,
                        FechaModificacion = pedido.FechaModificacion,
                        FechaRegistro = pedido.FechaRegistro,
                        UsuarioRegistro = pedido.UsuarioRegistro,
                        Activo = pedido.Activo,
                        Sucursal = pedido.Sucursal?.Nombre ?? "N/A",
                        Cliente = pedido.Cliente?.RazonSocial ?? "N/A",
                        PorcentajeSurtido = pedido.PorcentajeSurtido
                    };

                    pedidosCompletos.Add(pedidoDTO);
                }
            }

            // 8. Retornar la lista de pedidos compatibles
            return pedidosCompletos;
        }

        public async Task<bool> EliminarTarimaClasificacionAsync(int idTarima, int idClasificacion)
        {
            try
            {
                // Consultar la tarima con sus relaciones
                var tarima = await _tarimaRepository.GetByIdAsync(idTarima);
                if (tarima == null) return false;
                // Validar colecciones del repositorio
                if (tarima.TarimasClasificaciones?.Count == 1)
                {
                    // Eliminar la tarima y todas sus relaciones
                    var resultado = await _tarimaRepository.DeleteAsync(idTarima);
                    if(!resultado) return false;
                    
                }else{
                    // Eliminar la relación tarima-clasificación especifica
                    var resultado = await _tarimaRepository.DeleteTarimaClasificacionAsync(idTarima, idClasificacion);
                    if(!resultado) return false;
                }

                // Validar si existia la relación PedidoTarima
                if(tarima.PedidoTarimas?.Any() == true)
                {
                    // Actualizar el porcentaje de surtido del pedido
                    var pedidoClienteService = _serviceProvider.GetRequiredService<IPedidoClienteService>();
                    await pedidoClienteService.ActualizarPorcentajeSurtidoAsync(tarima.PedidoTarimas.First().IdPedidoCliente);
                }    
                
                return true;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
} 