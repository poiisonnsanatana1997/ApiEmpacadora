using Microsoft.EntityFrameworkCore;
using AppAPIEmpacadora.Infrastructure.Data;
using AppAPIEmpacadora.Repositories.Interfaces;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Models.DTOs;

namespace AppAPIEmpacadora.Infrastructure.Repositories
{
    public class OrdenEntradaRepository : IOrdenEntradaRepository
    {
        private readonly ApplicationDbContext _context;

        public OrdenEntradaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrdenEntradaDTO>> ObtenerOrdenesEntradaAsync()
        {
            return await _context.PedidosProveedor
                .Include(p => p.Proveedor)
                .Include(p => p.ProductosPedido)
                    .ThenInclude(pp => pp.Producto)
                .SelectMany(p => p.ProductosPedido.Select(pp => new OrdenEntradaDTO
                {
                    Id = p.Id,
                    Codigo = p.Codigo,
                    Proveedor = new ProveedorSimpleDTO
                    {
                        Id = p.Proveedor.Id,
                        Nombre = p.Proveedor.Nombre
                    },
                    Producto = new ProductoSimpleDTO
                    {
                        Id = pp.Producto.Id,
                        Nombre = pp.Producto.Nombre,
                        Codigo = pp.Producto.Codigo,
                        Variedad = pp.Producto.Variedad

                    },
                    FechaEstimada = p.FechaEstimada,
                    FechaRegistro = p.FechaRegistro,
                    FechaRecepcion = p.FechaRecepcion,
                    UsuarioRecepcion = p.UsuarioRecepcion,
                    UsuarioRegistro = p.UsuarioRegistro,
                    Estado = p.Estado,
                    Observaciones = p.Observaciones
                }))
                .ToListAsync();
        }

        public async Task<OrdenEntradaDTO> ObtenerOrdenEntradaPorCodigoAsync(string codigo)
        {
            return await _context.PedidosProveedor
                .Include(p => p.Proveedor)
                .Include(p => p.ProductosPedido)
                    .ThenInclude(pp => pp.Producto)
                .Where(p => p.Codigo == codigo)
                .SelectMany(p => p.ProductosPedido.Select(pp => new OrdenEntradaDTO
                {
                    Id = p.Id,
                    Codigo = p.Codigo,
                    Proveedor = new ProveedorSimpleDTO
                    {
                        Id = p.Proveedor.Id,
                        Nombre = p.Proveedor.Nombre
                    },
                    Producto = new ProductoSimpleDTO
                    {
                        Id = pp.Producto.Id,
                        Nombre = pp.Producto.Nombre,
                        Codigo = pp.Producto.Codigo,
                        Variedad = pp.Producto.Variedad
                    },
                    FechaEstimada = p.FechaEstimada,
                    FechaRegistro = p.FechaRegistro,
                    FechaRecepcion = p.FechaRecepcion,
                    UsuarioRecepcion = p.UsuarioRecepcion,
                    UsuarioRegistro = p.UsuarioRegistro,
                    Estado = p.Estado,
                    Observaciones = p.Observaciones
                }))
                .FirstOrDefaultAsync();
        }

        public async Task<OrdenEntradaDTO> ObtenerUltimaOrdenEntradaAsync()
        {
            return await _context.PedidosProveedor
                .Include(p => p.Proveedor)
                .Include(p => p.ProductosPedido)
                    .ThenInclude(pp => pp.Producto)
                .OrderByDescending(p => p.Id)
                .SelectMany(p => p.ProductosPedido.Select(pp => new OrdenEntradaDTO
                {
                    Id = p.Id,
                    Codigo = p.Codigo,
                    Proveedor = new ProveedorSimpleDTO
                    {
                        Id = p.Proveedor.Id,
                        Nombre = p.Proveedor.Nombre
                    },
                    Producto = new ProductoSimpleDTO
                    {
                        Id = pp.Producto.Id,
                        Nombre = pp.Producto.Nombre,
                        Codigo = pp.Producto.Codigo,
                        Variedad = pp.Producto.Variedad
                    },
                    FechaEstimada = p.FechaEstimada,
                    FechaRegistro = p.FechaRegistro,
                    FechaRecepcion = p.FechaRecepcion,
                    UsuarioRecepcion = p.UsuarioRecepcion,
                    UsuarioRegistro = p.UsuarioRegistro,
                    Estado = p.Estado,
                    Observaciones = p.Observaciones
                }))
                .FirstOrDefaultAsync();
        }

        public async Task<OrdenEntradaDTO> CrearOrdenEntradaAsync(OrdenEntradaDTO ordenEntrada)
        {
            var pedido = new PedidoProveedor
            {
                Codigo = ordenEntrada.Codigo,
                IdProveedor = ordenEntrada.Proveedor.Id,
                Estado = ordenEntrada.Estado,
                Observaciones = ordenEntrada.Observaciones,
                FechaRegistro = ordenEntrada.FechaRegistro,
                FechaEstimada = ordenEntrada.FechaEstimada,
                FechaRecepcion = ordenEntrada.FechaRecepcion,
                UsuarioRegistro = ordenEntrada.UsuarioRegistro,
                ProductosPedido = new List<ProductoPedido>
                {
                    new ProductoPedido
                    {
                        IdProducto = ordenEntrada.Producto.Id
                    }
                }
            };
            _context.PedidosProveedor.Add(pedido);
            await _context.SaveChangesAsync();
            
            // Devolver la orden creada con el ID asignado
            ordenEntrada.Id = pedido.Id;
            return ordenEntrada;
        }

        public async Task<bool> ActualizarOrdenEntradaAsync(OrdenEntradaDTO ordenEntrada)
        {

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var pedido = await _context.PedidosProveedor
                                .Include(p => p.ProductosPedido)
                                .FirstOrDefaultAsync(p => p.Codigo == ordenEntrada.Codigo);

                if (pedido == null)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                // Actualizar datos básicos del pedido
                pedido.IdProveedor = ordenEntrada.Proveedor.Id;
                pedido.Estado = ordenEntrada.Estado;
                pedido.Observaciones = ordenEntrada.Observaciones;
                pedido.FechaEstimada = ordenEntrada.FechaEstimada;
                pedido.FechaRecepcion = ordenEntrada.FechaRecepcion;
                pedido.UsuarioRecepcion = ordenEntrada.UsuarioRecepcion;

                // Manejar la actualización de productos
                if (pedido.ProductosPedido.Any())
                {
                    _context.ProductosPedido.RemoveRange(pedido.ProductosPedido);
                }

                pedido.ProductosPedido.Add(new ProductoPedido 
                { 
                    IdProducto = ordenEntrada.Producto.Id,
                    IdPedidoProveedor = pedido.Id
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("La orden ha sido modificada por otro usuario. Por favor, actualice la página e intente nuevamente.", ex);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Error al actualizar la orden en la base de datos. Por favor, verifique los datos e intente nuevamente.", ex);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Error inesperado al actualizar la orden.", ex);
            }
        }

        public async Task<bool> EliminarOrdenEntradaAsync(string codigo)
        {
            var pedido = await _context.PedidosProveedor
                .Include(p => p.ProductosPedido)
                .FirstOrDefaultAsync(p => p.Codigo == codigo);

            if (pedido == null)
                return false;

            _context.ProductosPedido.RemoveRange(pedido.ProductosPedido);
            _context.PedidosProveedor.Remove(pedido);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DetalleOrdenEntradaDTO> ObtenerDetalleOrdenEntradaAsync(string codigo)
        {
            var pedido = await _context.PedidosProveedor
                .Include(p => p.Proveedor)
                .Include(p => p.ProductosPedido)
                    .ThenInclude(pp => pp.Producto)
                .Include(p => p.CantidadesPedido)
                .FirstOrDefaultAsync(p => p.Codigo == codigo);

            if (pedido == null)
                return null;

            var ordenEntrada = pedido.ProductosPedido.Select(pp => new OrdenEntradaDTO
            {
                Id = pedido.Id,
                Codigo = pedido.Codigo,
                Proveedor = new ProveedorSimpleDTO
                {
                    Id = pedido.Proveedor.Id,
                    Nombre = pedido.Proveedor.Nombre
                },
                Producto = new ProductoSimpleDTO
                {
                    Id = pp.Producto.Id,
                    Nombre = pp.Producto.Nombre,
                    Codigo = pp.Producto.Codigo,
                    Variedad = pp.Producto.Variedad
                },
                FechaEstimada = pedido.FechaEstimada,
                FechaRegistro = pedido.FechaRegistro,
                FechaRecepcion = pedido.FechaRecepcion,
                UsuarioRecepcion = pedido.UsuarioRecepcion,
                UsuarioRegistro = pedido.UsuarioRegistro,
                Estado = pedido.Estado,
                Observaciones = pedido.Observaciones
            }).FirstOrDefault();

            var tarimas = pedido.CantidadesPedido.Select(t => new TarimaDetalleDTO
            {
                Numero = t.Codigo,
                PesoBruto = t.PesoBruto,
                PesoTara = t.PesoTara,
                PesoTarima = t.PesoTarima,
                PesoPatin = t.PesoPatin,
                PesoNeto = t.PesoNeto,
                CantidadCajas = t.CantidadCajas,
                PesoPorCaja = t.PesoPorCaja,
                Observaciones = t.Observaciones
            }).ToList();

            return new DetalleOrdenEntradaDTO
            {
                OrdenEntrada = ordenEntrada,
                Tarimas = tarimas
            };
        }

        public async Task<IEnumerable<TarimaDetalleDTO>> ObtenerTarimasPorOrdenAsync(string codigo)
        {
            return await _context.CantidadesPedido
                .Where(t => t.PedidoProveedor.Codigo == codigo)
                .Select(t => new TarimaDetalleDTO
                {
                    Numero = t.Codigo,
                    PesoBruto = t.PesoBruto,
                    PesoTara = t.PesoTara,
                    PesoTarima = t.PesoTarima,
                    PesoPatin = t.PesoPatin,
                    PesoNeto = t.PesoNeto,
                    CantidadCajas = t.CantidadCajas,
                    PesoPorCaja = t.PesoPorCaja,
                    Observaciones = t.Observaciones
                })
                .ToListAsync();
        }

        public async Task<TarimaDetalleDTO> ObtenerTarimaPorNumeroAsync(string codigo, string numeroTarima)
        {
            return await _context.CantidadesPedido
                .Where(t => t.PedidoProveedor.Codigo == codigo && t.Codigo == numeroTarima)
                .Select(t => new TarimaDetalleDTO
                {
                    Numero = t.Codigo,
                    CodigoOrden = t.PedidoProveedor.Codigo,
                    PesoBruto = t.PesoBruto,
                    PesoTara = t.PesoTara,
                    PesoTarima = t.PesoTarima,
                    PesoPatin = t.PesoPatin,
                    PesoNeto = t.PesoNeto,
                    CantidadCajas = t.CantidadCajas,
                    PesoPorCaja = t.PesoPorCaja,
                    Observaciones = t.Observaciones
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ActualizarTarimaAsync(TarimaDetalleDTO tarima)
        {
            var tarimaEntity = await _context.CantidadesPedido
                .Include(t => t.PedidoProveedor)
                .FirstOrDefaultAsync(t => t.Codigo == tarima.Numero && t.PedidoProveedor.Codigo == tarima.CodigoOrden);

            if (tarimaEntity == null)
                return false;

            tarimaEntity.PesoBruto = tarima.PesoBruto;
            tarimaEntity.PesoTara = tarima.PesoTara;
            tarimaEntity.PesoTarima = tarima.PesoTarima;
            tarimaEntity.PesoPatin = tarima.PesoPatin;
            tarimaEntity.PesoNeto = tarima.PesoNeto;
            tarimaEntity.CantidadCajas = tarima.CantidadCajas;
            tarimaEntity.PesoPorCaja = tarima.PesoPorCaja;
            tarimaEntity.Observaciones = tarima.Observaciones;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarTarimaAsync(TarimaDetalleDTO tarima)
        {
            var tarimaEntity = await _context.CantidadesPedido
                .FirstOrDefaultAsync(t => t.Codigo == tarima.Numero);

            if (tarimaEntity == null)
                return false;

            _context.CantidadesPedido.Remove(tarimaEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> ObtenerPesoTotalRecibidoHoyAsync()
        {
            var pesoTotal = await _context.CantidadesPedido
                .Where(cp => cp.PedidoProveedor.Estado == "Recibida")
                .SumAsync(cp => cp.PesoNeto ?? 0);

            return pesoTotal;
        }

        public async Task<int> ObtenerCantidadPendientesHoyAsync()
        {
            var hoy = DateTime.Now.Date;

            return await _context.PedidosProveedor
                .Where(p => p.FechaEstimada.Date == hoy && p.Estado == "Pendiente")
                .CountAsync();
        }

        public async Task<TarimaDetalleDTO> CrearTarimaAsync(string codigoOrden, TarimaDetalleDTO tarima)
        {
            var pedido = await _context.PedidosProveedor
                .FirstOrDefaultAsync(p => p.Codigo == codigoOrden);
            if (pedido == null)
                return null;

            var nuevaTarima = new CantidadPedido
            {
                Codigo = tarima.Numero,
                IdPedidoProveedor = pedido.Id,
                CantidadCajas = tarima.CantidadCajas,
                PesoPorCaja = tarima.PesoPorCaja,
                PesoBruto = tarima.PesoBruto,
                PesoTara = tarima.PesoTara,
                PesoTarima = tarima.PesoTarima,
                PesoPatin = tarima.PesoPatin,
                PesoNeto = tarima.PesoNeto,
                Observaciones = tarima.Observaciones
            };
            _context.CantidadesPedido.Add(nuevaTarima);
            await _context.SaveChangesAsync();

            return new TarimaDetalleDTO
            {
                Numero = nuevaTarima.Codigo,
                CodigoOrden = codigoOrden,
                PesoBruto = nuevaTarima.PesoBruto,
                PesoTara = nuevaTarima.PesoTara,
                PesoTarima = nuevaTarima.PesoTarima,
                PesoPatin = nuevaTarima.PesoPatin,
                PesoNeto = nuevaTarima.PesoNeto,
                CantidadCajas = nuevaTarima.CantidadCajas,
                PesoPorCaja = nuevaTarima.PesoPorCaja,
                Observaciones = nuevaTarima.Observaciones
            };
        }

        public async Task<PedidoProveedor> GetByIdAsync(int id)
        {
            return await _context.PedidosProveedor
                .Include(p => p.CantidadesPedido)
                .Include(p => p.Proveedor)
                .Include(p => p.ProductosPedido)
                    .ThenInclude(pp => pp.Producto)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdatePedidoAsync(PedidoProveedor pedido)
        {
            _context.Entry(pedido).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<PedidoCompletoDTO> ObtenerPedidoCompletoPorIdAsync(int id)
        {
            var pedido = await _context.PedidosProveedor
                .Include(p => p.Proveedor)
                .Include(p => p.ProductosPedido)
                    .ThenInclude(pp => pp.Producto)
                .Include(p => p.Clasificaciones)
                    .ThenInclude(c => c.Mermas)
                .Include(p => p.Clasificaciones)
                    .ThenInclude(c => c.RetornosDetalle)
                .Include(p => p.Clasificaciones)
                    .ThenInclude(c => c.TarimasClasificaciones)
                        .ThenInclude(tc => tc.Tarima)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
                return null;

            return new PedidoCompletoDTO
            {
                Id = pedido.Id,
                Codigo = pedido.Codigo,
                Estado = pedido.Estado,
                FechaRegistro = pedido.FechaRegistro,
                FechaEstimada = pedido.FechaEstimada,
                FechaRecepcion = pedido.FechaRecepcion,
                UsuarioRegistro = pedido.UsuarioRegistro,
                UsuarioRecepcion = pedido.UsuarioRecepcion,
                Observaciones = pedido.Observaciones,
                Proveedor = new ProveedorSimpleDTO
                {
                    Id = pedido.Proveedor.Id,
                    Nombre = pedido.Proveedor.Nombre
                },
                Producto = pedido.ProductosPedido?.FirstOrDefault()?.Producto != null ? new ProductoSimpleDTO
                {
                    Id = pedido.ProductosPedido.First().Producto.Id,
                    Codigo = pedido.ProductosPedido.First().Producto.Codigo,
                    Nombre = pedido.ProductosPedido.First().Producto.Nombre,
                    Variedad = pedido.ProductosPedido.First().Producto.Variedad
                } : null,
                Clasificaciones = pedido.Clasificaciones?.Select(c => new ClasificacionCompletaDTO
                {
                    Id = c.Id,
                    Lote = c.Lote,
                    PesoTotal = c.PesoTotal,
                    FechaRegistro = c.FechaRegistro,
                    UsuarioRegistro = c.UsuarioRegistro,
                    XL = c.XL,
                    L = c.L,
                    M = c.M,
                    S = c.S,
                    Retornos = c.Retornos,
                    Mermas = c.Mermas?.Select(m => new MermaDetalleDTO
                    {
                        Id = m.Id,
                        Tipo = m.Tipo,
                        Peso = m.Peso,
                        Observaciones = m.Observaciones,
                        FechaRegistro = m.FechaRegistro,
                        UsuarioRegistro = m.UsuarioRegistro
                    }).ToList(),
                    RetornosDetalle = c.RetornosDetalle?.Select(r => new RetornoDetalleDTO
                    {
                        Id = r.Id,
                        Numero = r.Numero,
                        Peso = r.Peso,
                        Observaciones = r.Observaciones,
                        FechaRegistro = r.FechaRegistro,
                        UsuarioRegistro = r.UsuarioRegistro
                    }).ToList(),
                    TarimasClasificaciones = c.TarimasClasificaciones?.Select(tc => new TarimaClasificacionDTO
                    {
                        IdTarima = tc.IdTarima,
                        IdClasificacion = tc.IdClasificacion,
                        Peso = tc.Peso,
                        Tipo = tc.Tipo,
                        Tarima = new TarimaDTO
                        {
                            Id = tc.Tarima.Id,
                            Codigo = tc.Tarima.Codigo,
                            Estatus = tc.Tarima.Estatus,
                            FechaRegistro = tc.Tarima.FechaRegistro,
                            FechaActualizacion = tc.Tarima.FechaActualizacion,
                            UsuarioRegistro = tc.Tarima.UsuarioRegistro,
                            UsuarioModificacion = tc.Tarima.UsuarioModificacion,
                            Cantidad = tc.Tarima.Cantidad,
                            Observaciones = tc.Tarima.Observaciones,
                            UPC = tc.Tarima.UPC,
                            Peso = tc.Tarima.Peso
                        }
                    }).ToList()
                }).ToList()
            };
        }
    }
}