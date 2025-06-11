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
                    Codigo = p.Codigo,
                    Proveedor = new ProveedorSimpleDTO
                    {
                        Id = p.Proveedor.Id,
                        Nombre = p.Proveedor.Nombre
                    },
                    Producto = new ProductoSimpleDTO
                    {
                        Id = pp.Producto.Id,
                        Nombre = pp.Producto.Nombre
                    },
                    FechaEstimada = p.FechaEstimada,
                    FechaRegistro = p.FechaRegistro,
                    FechaRecepcion = p.FechaRecepcion,
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
                    Codigo = p.Codigo,
                    Proveedor = new ProveedorSimpleDTO
                    {
                        Id = p.Proveedor.Id,
                        Nombre = p.Proveedor.Nombre
                    },
                    Producto = new ProductoSimpleDTO
                    {
                        Id = pp.Producto.Id,
                        Nombre = pp.Producto.Nombre
                    },
                    FechaEstimada = p.FechaEstimada,
                    FechaRegistro = p.FechaRegistro,
                    FechaRecepcion = p.FechaRecepcion,
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
                    Codigo = p.Codigo,
                    Proveedor = new ProveedorSimpleDTO
                    {
                        Id = p.Proveedor.Id,
                        Nombre = p.Proveedor.Nombre
                    },
                    Producto = new ProductoSimpleDTO
                    {
                        Id = pp.Producto.Id,
                        Nombre = pp.Producto.Nombre
                    },
                    FechaEstimada = p.FechaEstimada,
                    FechaRegistro = p.FechaRegistro,
                    FechaRecepcion = p.FechaRecepcion,
                    Estado = p.Estado,
                    Observaciones = p.Observaciones
                }))
                .FirstOrDefaultAsync();
        }

        public async Task CrearOrdenEntradaAsync(OrdenEntradaDTO ordenEntrada, string usuarioRegistro)
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
                UsuarioRegistro = usuarioRegistro,
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
        }

        public async Task<bool> ActualizarOrdenEntradaAsync(OrdenEntradaDTO ordenEntrada)
        {
            var pedido = await _context.PedidosProveedor
                .Include(p => p.ProductosPedido)
                .FirstOrDefaultAsync(p => p.Codigo == ordenEntrada.Codigo);

            if (pedido == null)
                return false;

            pedido.IdProveedor = ordenEntrada.Proveedor.Id;
            pedido.Estado = ordenEntrada.Estado;
            pedido.Observaciones = ordenEntrada.Observaciones;
            pedido.FechaEstimada = ordenEntrada.FechaEstimada;
            pedido.FechaRecepcion = ordenEntrada.FechaRecepcion;

            // Actualizar producto (asumiendo solo uno por orden)
            if (pedido.ProductosPedido.Any())
            {
                pedido.ProductosPedido.First().IdProducto = ordenEntrada.Producto.Id;
            }
            else
            {
                pedido.ProductosPedido.Add(new ProductoPedido { IdProducto = ordenEntrada.Producto.Id });
            }

            await _context.SaveChangesAsync();
            return true;
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
                Codigo = pedido.Codigo,
                Proveedor = new ProveedorSimpleDTO
                {
                    Id = pedido.Proveedor.Id,
                    Nombre = pedido.Proveedor.Nombre
                },
                Producto = new ProductoSimpleDTO
                {
                    Id = pp.Producto.Id,
                    Nombre = pp.Producto.Nombre
                },
                FechaEstimada = pedido.FechaEstimada,
                FechaRegistro = pedido.FechaRegistro,
                FechaRecepcion = pedido.FechaRecepcion,
                Estado = pedido.Estado,
                Observaciones = pedido.Observaciones
            }).FirstOrDefault();

            var tarimas = pedido.CantidadesPedido.Select(t => new TarimaDTO
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

        public async Task<IEnumerable<TarimaDTO>> ObtenerTarimasPorOrdenAsync(string codigo)
        {
            return await _context.CantidadesPedido
                .Where(t => t.PedidoProveedor.Codigo == codigo)
                .Select(t => new TarimaDTO
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

        public async Task<TarimaDTO> ObtenerTarimaPorNumeroAsync(string codigo, string numeroTarima)
        {
            return await _context.CantidadesPedido
                .Where(t => t.PedidoProveedor.Codigo == codigo && t.Codigo == numeroTarima)
                .Select(t => new TarimaDTO
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
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ActualizarTarimaAsync(TarimaDTO tarima)
        {
            var tarimaEntity = await _context.CantidadesPedido
                .FirstOrDefaultAsync(t => t.Codigo == tarima.Numero);

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

        public async Task<bool> EliminarTarimaAsync(TarimaDTO tarima)
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
            var hoy = DateTime.UtcNow.Date;
            return await _context.PedidosProveedor
                .Where(p => p.FechaEstimada.Date == hoy && p.Estado == "Pendiente")
                .CountAsync();
        }

        public async Task<TarimaDTO> CrearTarimaAsync(string codigoOrden, TarimaDTO tarima)
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
            
            return new TarimaDTO
            {
                Numero = nuevaTarima.Codigo,
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
    }
} 