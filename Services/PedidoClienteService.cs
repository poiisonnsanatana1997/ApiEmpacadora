using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Repositories.Interfaces;
using AppAPIEmpacadora.Services.Interfaces;
using System.Linq;
using System;

namespace AppAPIEmpacadora.Services
{
    public class PedidoClienteService : IPedidoClienteService
    {
        private readonly IPedidoClienteRepository _repo;
        private readonly IOrdenPedidoClienteService _ordenService;
        
        public PedidoClienteService(IPedidoClienteRepository repo, IOrdenPedidoClienteService ordenService)
        {
            _repo = repo;
            _ordenService = ordenService;
        }
        
        public async Task<IEnumerable<PedidoClienteResponseDTO>> ObtenerTodosAsync()
        {
            var list = await _repo.ObtenerTodosAsync();
            var result = new List<PedidoClienteResponseDTO>();
            foreach (var item in list)
            {
                result.Add(MapToResponseDTO(item));
            }
            return result;
        }
        
        public async Task<PedidoClienteResponseDTO> ObtenerPorIdAsync(int id)
        {
            var entity = await _repo.ObtenerPorIdAsync(id);
            return entity == null ? null : MapToResponseDTO(entity);
        }
        
        public async Task<PedidoClienteConOrdenesResponseDTO> ObtenerPorIdConOrdenesAsync(int id)
        {
            var entity = await _repo.ObtenerPorIdAsync(id);
            if (entity == null) return null;
            
            // Obtener las órdenes asociadas al pedido
            var ordenes = await _ordenService.ObtenerPorPedidoClienteAsync(id);
            
            // Construir la respuesta con órdenes
            var response = new PedidoClienteConOrdenesResponseDTO
            {
                Id = entity.Id,
                Observaciones = entity.Observaciones,
                Estatus = entity.Estatus,
                FechaEmbarque = entity.FechaEmbarque,
                FechaModificacion = entity.FechaModificacion,
                FechaRegistro = entity.FechaRegistro,
                UsuarioRegistro = entity.UsuarioRegistro,
                Activo = entity.Activo,
                Sucursal = entity.Sucursal?.Nombre ?? string.Empty,
                Cliente = entity.Cliente?.RazonSocial ?? string.Empty,
                Ordenes = ordenes.ToList()
            };
            
            return response;
        }
        
        public async Task<PedidoClienteResponseDTO> CrearAsync(CreatePedidoClienteDTO dto, string usuarioRegistro)
        {
            var entity = new PedidoCliente
            {
                Observaciones = dto.Observaciones,
                Estatus = dto.Estatus,
                FechaEmbarque = dto.FechaEmbarque,
                IdSucursal = dto.IdSucursal,
                IdCliente = dto.IdCliente,
                FechaRegistro = dto.FechaRegistro,
                UsuarioRegistro = usuarioRegistro,
                Activo = dto.Activo,
                PorcentajeSurtido = dto.PorcentajeSurtido
            };
            var created = await _repo.CrearAsync(entity);
            return MapToResponseDTO(created);
        }

        public async Task<PedidoClienteConOrdenesResponseDTO> CrearConOrdenesAsync(CreatePedidoClienteConOrdenesDTO dto, string usuarioRegistro)
        {
            // Crear el pedido cliente
            var pedidoEntity = new PedidoCliente
            {
                Observaciones = dto.Observaciones,
                Estatus = dto.Estatus,
                FechaEmbarque = dto.FechaEmbarque,
                IdSucursal = dto.IdSucursal,
                IdCliente = dto.IdCliente,
                FechaRegistro = dto.FechaRegistro,
                UsuarioRegistro = usuarioRegistro,
                Activo = dto.Activo
            };
            
            var pedidoCreado = await _repo.CrearAsync(pedidoEntity);
            
            // Crear las órdenes asociadas
            var ordenesCreadas = new List<OrdenPedidoClienteResponseDTO>();
            
            foreach (var ordenDto in dto.Ordenes)
            {
                var createOrdenDto = new CreateOrdenPedidoClienteDTO
                {
                    Tipo = ordenDto.Tipo,
                    Cantidad = ordenDto.Cantidad,
                    Peso = ordenDto.Peso,
                    FechaRegistro = dto.FechaRegistro,
                    IdProducto = ordenDto.IdProducto,
                    IdPedidoCliente = pedidoCreado.Id
                };
                
                var ordenCreada = await _ordenService.CrearAsync(createOrdenDto, usuarioRegistro);
                ordenesCreadas.Add(ordenCreada);
            }
            
            // Construir la respuesta
            var response = new PedidoClienteConOrdenesResponseDTO
            {
                Id = pedidoCreado.Id,
                Observaciones = pedidoCreado.Observaciones,
                Estatus = pedidoCreado.Estatus,
                FechaEmbarque = pedidoCreado.FechaEmbarque,
                FechaModificacion = pedidoCreado.FechaModificacion,
                FechaRegistro = pedidoCreado.FechaRegistro,
                UsuarioRegistro = pedidoCreado.UsuarioRegistro,
                Activo = pedidoCreado.Activo,
                Sucursal = pedidoCreado.Sucursal?.Nombre ?? string.Empty,
                Cliente = pedidoCreado.Cliente?.RazonSocial ?? string.Empty,
                Ordenes = ordenesCreadas
            };
            
            return response;
        }
        
        public async Task<bool> ActualizarAsync(int id, UpdatePedidoClienteDTO dto)
        {
            var entity = await _repo.ObtenerPorIdAsync(id);
            if (entity == null) return false;
            
            if (dto.Observaciones != null) entity.Observaciones = dto.Observaciones;
            if (dto.Estatus != null) entity.Estatus = dto.Estatus;
            if (dto.FechaEmbarque.HasValue) entity.FechaEmbarque = dto.FechaEmbarque;
            if (dto.FechaModificacion.HasValue) entity.FechaModificacion = dto.FechaModificacion;
            if (dto.Activo.HasValue) entity.Activo = dto.Activo.Value;
            if (dto.PorcentajeSurtido.HasValue) entity.PorcentajeSurtido = dto.PorcentajeSurtido.Value;
            
            return await _repo.ActualizarAsync(entity);
        }
        
        public async Task<bool> ActualizarEstatusAsync(int id, string estatus, string usuarioModificacion)
        {
            var entity = await _repo.ObtenerPorIdAsync(id);
            if (entity == null) return false;
            
            entity.Estatus = estatus;
            entity.FechaModificacion = DateTime.Now;
            entity.UsuarioModificacion = usuarioModificacion;
            
            // Regla de negocio: Si el estatus es "Cancelado" o "Devuelto", desasignar tarimas y actualizar porcentaje
            if (estatus.Equals("Cancelado", StringComparison.OrdinalIgnoreCase) || 
                estatus.Equals("Devuelto", StringComparison.OrdinalIgnoreCase))
            {
                await DesasignarTarimasYActualizarSurtidoAsync(id, usuarioModificacion);
                entity.PorcentajeSurtido = 0;
            }
            
            return await _repo.ActualizarAsync(entity);
        }
        
        private async Task DesasignarTarimasYActualizarSurtidoAsync(int pedidoId, string usuarioModificacion)
        {
            try
            {
                // Desasignar todas las tarimas del pedido
                await _repo.DesasignarTodasLasTarimasAsync(pedidoId);
                
                // Log de auditoría (opcional - se puede implementar un sistema de logging)
                // _logger.LogInformation($"Tarimas desasignadas del pedido {pedidoId} por usuario {usuarioModificacion} debido a cambio de estatus");
            }
            catch (Exception ex)
            {
                // Log del error (opcional)
                // _logger.LogError(ex, $"Error al desasignar tarimas del pedido {pedidoId}");
                throw new InvalidOperationException($"Error al desasignar tarimas del pedido {pedidoId}: {ex.Message}");
            }
        }
        
        public async Task<bool> EliminarAsync(int id)
        {
            return await _repo.EliminarAsync(id);
        }
        
        public async Task<IEnumerable<PedidoClienteConDetallesDTO>> ObtenerTodosConDetallesAsync()
        {
            var list = await _repo.ObtenerTodosConDetallesAsync();
            var result = new List<PedidoClienteConDetallesDTO>();
            foreach (var item in list)
            {
                result.Add(MapToConDetallesDTO(item));
            }
            return result;
        }
        
        private PedidoClienteResponseDTO MapToResponseDTO(PedidoCliente entity)
        {
            return new PedidoClienteResponseDTO
            {
                Id = entity.Id,
                Observaciones = entity.Observaciones,
                Estatus = entity.Estatus,
                FechaEmbarque = entity.FechaEmbarque,
                FechaModificacion = entity.FechaModificacion,
                FechaRegistro = entity.FechaRegistro,
                UsuarioRegistro = entity.UsuarioRegistro,
                Activo = entity.Activo,
                Sucursal = entity.Sucursal?.Nombre ?? string.Empty,
                Cliente = entity.Cliente?.RazonSocial ?? string.Empty,
                PorcentajeSurtido = entity.PorcentajeSurtido
            };
        }
        
        private PedidoClienteConDetallesDTO MapToConDetallesDTO(PedidoCliente entity)
        {
            return new PedidoClienteConDetallesDTO
            {
                Id = entity.Id,
                RazonSocialCliente = entity.Cliente?.RazonSocial ?? string.Empty,
                PesoCajaCliente = entity.Cliente?.CajasCliente?.FirstOrDefault()?.Peso ?? 0
            };
        }

        public async Task<IEnumerable<PedidoClientePorAsignarDTO>> ObtenerDisponiblesPorTipoAsync(string tipo, int? idProducto = null)
        {
            var pedidos = await _repo.ObtenerPorTipoConTarimasAsync(tipo);
            var resultado = new List<PedidoClientePorAsignarDTO>();

            foreach (var pedido in pedidos)
            {
                // Validación: El pedido no debe estar cancelado
                if (pedido.Estatus?.ToLower() == "cancelado")
                {
                    continue;
                }

                // Obtener la orden del tipo específico
                var orden = pedido.OrdenesPedidoCliente.FirstOrDefault(o => o.Tipo == tipo);
                if (orden == null) continue;

                // Validación: Filtrar por ID de producto si se especifica
                if (idProducto.HasValue && orden.IdProducto != idProducto.Value)
                {
                    continue;
                }

                // Calcular cantidad asignada desde tarimas
                var cantidadAsignada = pedido.PedidoTarimas
                    .SelectMany(pt => pt.Tarima.TarimasClasificaciones)
                    .Where(tc => tc.Tipo == tipo)
                    .Sum(tc => tc.Cantidad ?? 0);

                // Calcular cantidad disponible
                var cantidadDisponible = (orden.Cantidad ?? 0) - cantidadAsignada;

                // Solo incluir si hay disponibilidad
                if (cantidadDisponible > 0)
                {
                    var dto = new PedidoClientePorAsignarDTO
                    {
                        Id = pedido.Id,
                        Tipo = orden.Tipo,
                        Cantidad = (int)cantidadDisponible, // Cantidad disponible
                        Peso = orden.Peso ?? 0,
                        PesoCajaCliente = pedido.Cliente?.CajasCliente?.FirstOrDefault()?.Peso ?? 0,
                                                 Producto = new ProductoSimpleDTO
                         {
                             Id = orden.Producto?.Id ?? 0,
                             Nombre = orden.Producto?.Nombre ?? string.Empty,
                             Codigo = orden.Producto?.Codigo ?? string.Empty,
                             Variedad = orden.Producto?.Variedad ?? string.Empty
                         },
                        Cliente = new ClienteSummaryDTO
                        {
                            Id = pedido.Cliente?.Id ?? 0,
                            RazonSocial = pedido.Cliente?.RazonSocial ?? string.Empty
                        },
                        Sucursal = new SucursalSummaryDTO
                        {
                            Id = pedido.Sucursal?.Id ?? 0,
                            Nombre = pedido.Sucursal?.Nombre ?? string.Empty
                        }
                    };

                    resultado.Add(dto);
                }
            }

            return resultado;
        }

        public async Task<PedidoClientePorAsignarDTO> ObtenerDisponibilidadCajasPorPedidoAsync(int idPedido, string tipo, int? idProducto = null)
        {
            // Obtener el pedido específico con todas sus relaciones
            var pedido = await _repo.ObtenerPorIdConRelacionesCompletasAsync(idPedido);
            if (pedido == null) return null;

            // Validación: El pedido no debe estar cancelado
            if (pedido.Estatus?.ToLower() == "Cancelado")
            {
                return null;
            }

            // Obtener la orden del tipo específico
            var orden = pedido.OrdenesPedidoCliente?.FirstOrDefault(o => o.Tipo == tipo);
            if (orden == null) return null;

            // Validación: Filtrar por ID de producto si se especifica
            if (idProducto.HasValue && orden.IdProducto != idProducto.Value)
            {
                return null;
            }

            // Calcular cantidad asignada desde tarimas
            var cantidadAsignada = pedido.PedidoTarimas?
                .SelectMany(pt => pt.Tarima.TarimasClasificaciones)
                .Where(tc => tc.Tipo == tipo)
                .Sum(tc => tc.Cantidad ?? 0) ?? 0;

            // Calcular cantidad disponible
            var cantidadDisponible = (orden.Cantidad ?? 0) - cantidadAsignada;

            // Devolver el DTO con la información de disponibilidad
            return new PedidoClientePorAsignarDTO
            {
                Id = pedido.Id,
                Tipo = orden.Tipo,
                Cantidad = (int)cantidadDisponible, // Cantidad disponible (puede ser 0)
                Peso = orden.Peso ?? 0,
                PesoCajaCliente = pedido.Cliente?.CajasCliente?.FirstOrDefault()?.Peso ?? 0,
                Producto = new ProductoSimpleDTO
                {
                    Id = orden.Producto?.Id ?? 0,
                    Nombre = orden.Producto?.Nombre ?? string.Empty,
                    Codigo = orden.Producto?.Codigo ?? string.Empty,
                    Variedad = orden.Producto?.Variedad ?? string.Empty
                },
                Cliente = new ClienteSummaryDTO
                {
                    Id = pedido.Cliente?.Id ?? 0,
                    RazonSocial = pedido.Cliente?.RazonSocial ?? string.Empty
                },
                Sucursal = new SucursalSummaryDTO
                {
                    Id = pedido.Sucursal?.Id ?? 0,
                    Nombre = pedido.Sucursal?.Nombre ?? string.Empty
                }
            };
        }

        public async Task<PedidoClienteProgresoDTO> ObtenerProgresoAsync(int id)
        {
            var entity = await _repo.ObtenerPorIdAsync(id);
            if (entity == null) return null;
            
            // Obtener las órdenes asociadas al pedido
            var ordenes = await _ordenService.ObtenerPorPedidoClienteAsync(id);
            
            // Mapear las tarimas a DTOs para evitar referencias circulares
            var tarimasDelPedido = entity.PedidoTarimas
                .Select(pt => MapToTarimaProgresoDTO(pt.Tarima))
                .ToList();
            
            // Construir la respuesta con progreso
            var response = new PedidoClienteProgresoDTO
            {
                Id = entity.Id,
                Estatus = entity.Estatus,
                PorcentajeSurtido = entity.PorcentajeSurtido,
                Observaciones = entity.Observaciones,
                Ordenes = ordenes.ToList(),
                Tarimas = tarimasDelPedido
            };
            
            return response;
        }

        private TarimaProgresoDTO MapToTarimaProgresoDTO(Tarima tarima)
        {
            return new TarimaProgresoDTO
            {
                Id = tarima.Id,
                Codigo = tarima.Codigo,
                Estatus = tarima.Estatus,
                Observaciones = tarima.Observaciones,
                UPC = tarima.UPC,
                Peso = tarima.Peso,
                TarimasClasificaciones = tarima.TarimasClasificaciones?.Select(tc => new TarimaClasificacionProgresoDTO
                {
                    Tipo = tc.Tipo,
                    Peso = tc.Peso,
                    Cantidad = tc.Cantidad,
                    Producto = tc.Clasificacion?.PedidoProveedor?.ProductosPedido?.FirstOrDefault()?.Producto != null 
                        ? new ProductoSimpleDTO
                        {
                            Id = tc.Clasificacion.PedidoProveedor.ProductosPedido.First().Producto.Id,
                            Codigo = tc.Clasificacion.PedidoProveedor.ProductosPedido.First().Producto.Codigo,
                            Nombre = tc.Clasificacion.PedidoProveedor.ProductosPedido.First().Producto.Nombre,
                            Variedad = tc.Clasificacion.PedidoProveedor.ProductosPedido.First().Producto.Variedad
                        }
                        : null
                }).ToList() ?? new List<TarimaClasificacionProgresoDTO>()
            };
        }

        /// <summary>
        /// Calcula el porcentaje de surtido de un pedido cliente
        /// </summary>
        /// <param name="idPedidoCliente">ID del pedido cliente</param>
        /// <returns>Porcentaje de surtido calculado</returns>
        public async Task<decimal> CalcularPorcentajeSurtidoAsync(int idPedidoCliente)
        {
            // Obtener el pedido con todas sus relaciones
            var pedido = await _repo.ObtenerPorIdConRelacionesCompletasAsync(idPedidoCliente);
            if (pedido == null) return 0;

            // Calcular cantidad total solicitada
            var cantidadSolicitada = pedido.OrdenesPedidoCliente?.Sum(o => o.Cantidad ?? 0) ?? 0;
            if (cantidadSolicitada == 0) return 0;

            // Calcular cantidad total surtida
            var cantidadSurtida = pedido.PedidoTarimas?
                .SelectMany(pt => pt.Tarima.TarimasClasificaciones ?? new List<TarimaClasificacion>())
                .Sum(tc => tc.Cantidad ?? 0) ?? 0;

            // Calcular porcentaje
            var porcentaje = (cantidadSurtida / cantidadSolicitada) * 100;
            return Math.Round(porcentaje, 2);
        }

        /// <summary>
        /// Actualiza el porcentaje de surtido de un pedido cliente
        /// </summary>
        /// <param name="idPedidoCliente">ID del pedido cliente</param>
        /// <returns>True si se actualizó correctamente</returns>
        public async Task<bool> ActualizarPorcentajeSurtidoAsync(int idPedidoCliente)
        {
            try
            {
                var porcentaje = await CalcularPorcentajeSurtidoAsync(idPedidoCliente);
                var pedido = await _repo.ObtenerPorIdConRelacionesCompletasAsync(idPedidoCliente);
                
                if (pedido != null)
                {
                    // Actualizar porcentaje
                    pedido.PorcentajeSurtido = porcentaje;
                    
                    // Determinar y actualizar estatus automáticamente
                    var nuevoEstatus = DeterminarEstatusPorPorcentaje(porcentaje);
                    if (pedido.Estatus != nuevoEstatus)
                    {
                        pedido.Estatus = nuevoEstatus;
                        pedido.FechaModificacion = DateTime.Now;
                    }
                    
                    return await _repo.ActualizarAsync(pedido);
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Determina el estatus del pedido basado en el porcentaje de surtido
        /// </summary>
        /// <param name="porcentaje">Porcentaje de surtido calculado</param>
        /// <returns>Estatus correspondiente al porcentaje</returns>
        private string DeterminarEstatusPorPorcentaje(decimal porcentaje)
        {
            return porcentaje switch
            {
                0 => "Pendiente",
                100 => "Surtido",
                _ => "Surtiendo"  // Cualquier valor entre 0 y 100
            };
        }

        /// <summary>
        /// Asigna múltiples tarimas a un pedido y calcula el porcentaje de surtido
        /// </summary>
        /// <param name="pedidoId">ID del pedido</param>
        /// <param name="tarimaIds">Lista de IDs de las tarimas a asignar</param>
        /// <param name="usuario">Usuario que realiza la asignación</param>
        /// <returns>Pedido actualizado con el nuevo porcentaje de surtido</returns>
        public async Task<PedidoClienteResponseDTO> AsignarTarimasYCalcularSurtidoAsync(int pedidoId, List<int> tarimaIds, string usuario)
        {
            // Validar que el pedido existe
            var pedido = await _repo.ObtenerPorIdAsync(pedidoId);
            if (pedido == null)
            {
                throw new ArgumentException("El pedido no existe");
            }

            // Validar que se proporcionaron tarimas
            if (tarimaIds == null || !tarimaIds.Any())
            {
                throw new ArgumentException("No se proporcionaron tarimas para asignar");
            }

            // Asignar las tarimas al pedido
            var asignacionExitosa = await _repo.AsignarTarimasAPedidoAsync(pedidoId, tarimaIds);
            if (!asignacionExitosa)
            {
                throw new InvalidOperationException("Error al asignar las tarimas. Verifique que las tarimas existan y estén disponibles.");
            }

            // Calcular el nuevo porcentaje de surtido
            var nuevoPorcentaje = await _repo.CalcularPorcentajeSurtidoAsync(pedidoId);

            // Actualizar el pedido con el nuevo porcentaje
            pedido.PorcentajeSurtido = nuevoPorcentaje;
            pedido.FechaModificacion = DateTime.Now;
            pedido.UsuarioModificacion = usuario;

            // Determinar el nuevo estatus basado en el porcentaje
            var nuevoEstatus = DeterminarEstatusPorPorcentaje(nuevoPorcentaje);
            if (pedido.Estatus != nuevoEstatus)
            {
                pedido.Estatus = nuevoEstatus;
            }

            // Guardar los cambios
            await _repo.ActualizarAsync(pedido);

            // Retornar el pedido actualizado
            return MapToResponseDTO(pedido);
        }

        /// <summary>
        /// Desasigna múltiples tarimas de pedidos y recalcula el porcentaje de surtido
        /// </summary>
        /// <param name="desasignaciones">Lista de desasignaciones a realizar</param>
        /// <param name="usuario">Usuario que realiza la desasignación</param>
        public async Task DesasignarTarimasAsync(List<DesasignacionTarimaDTO> desasignaciones, string usuario)
        {
            // Validar que se proporcionaron desasignaciones
            if (desasignaciones == null || !desasignaciones.Any())
            {
                throw new ArgumentException("No se proporcionaron desasignaciones para realizar");
            }

            // Eliminar las relaciones PedidoTarima
            var eliminacionExitosa = await _repo.EliminarPedidoTarimasAsync(desasignaciones);
            if (!eliminacionExitosa)
            {
                throw new InvalidOperationException("Error al eliminar las relaciones. Verifique que todas las asignaciones existan.");
            }

            // Obtener los IDs únicos de pedidos afectados
            var pedidosAfectados = desasignaciones.Select(d => d.IdPedido).Distinct().ToList();

            // Actualizar cada pedido afectado
            foreach (var pedidoId in pedidosAfectados)
            {
                var pedido = await _repo.ObtenerPorIdAsync(pedidoId);
                if (pedido != null)
                {
                    // Calcular el nuevo porcentaje de surtido
                    var nuevoPorcentaje = await _repo.CalcularPorcentajeSurtidoAsync(pedidoId);

                    // Actualizar el pedido
                    pedido.PorcentajeSurtido = nuevoPorcentaje;
                    pedido.FechaModificacion = DateTime.Now;
                    pedido.UsuarioModificacion = usuario;

                    // Determinar el nuevo estatus basado en el porcentaje
                    var nuevoEstatus = DeterminarEstatusPorPorcentaje(nuevoPorcentaje);
                    if (pedido.Estatus != nuevoEstatus)
                    {
                        pedido.Estatus = nuevoEstatus;
                    }

                    // Guardar los cambios
                    await _repo.ActualizarAsync(pedido);
                }
            }
        }
    }
} 