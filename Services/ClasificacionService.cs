using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Repositories.Interfaces;
using AppAPIEmpacadora.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Services
{
    public class ClasificacionService : IClasificacionService
    {
        private readonly IClasificacionRepository _clasificacionRepository;
        private readonly IOrdenEntradaRepository _ordenEntradaRepository;

        public ClasificacionService(IClasificacionRepository clasificacionRepository, IOrdenEntradaRepository ordenEntradaRepository)
        {
            _clasificacionRepository = clasificacionRepository;
            _ordenEntradaRepository = ordenEntradaRepository;
        }

        public async Task<IEnumerable<ClasificacionDTO>> GetClasificacionesAsync()
        {
            var clasificaciones = await _clasificacionRepository.GetAllAsync();
            var clasificacionDTOs = new List<ClasificacionDTO>();
            foreach (var clasificacion in clasificaciones)
            {
                clasificacionDTOs.Add(new ClasificacionDTO
                {
                    Id = clasificacion.Id,
                    Lote = clasificacion.Lote,
                    PesoTotal = clasificacion.PesoTotal,
                    FechaRegistro = clasificacion.FechaRegistro,
                    UsuarioRegistro = clasificacion.UsuarioRegistro,
                    IdPedidoProveedor = clasificacion.IdPedidoProveedor
                });
            }
            return clasificacionDTOs;
        }

        public async Task<ClasificacionDTO> GetClasificacionByIdAsync(int id)
        {
            var clasificacion = await _clasificacionRepository.GetByIdAsync(id);
            if (clasificacion == null) return null;

            return new ClasificacionDTO
            {
                Id = clasificacion.Id,
                Lote = clasificacion.Lote,
                PesoTotal = clasificacion.PesoTotal,
                FechaRegistro = clasificacion.FechaRegistro,
                UsuarioRegistro = clasificacion.UsuarioRegistro,
                IdPedidoProveedor = clasificacion.IdPedidoProveedor
            };
        }

        public async Task<ClasificacionDTO> CreateClasificacionAsync(CreateClasificacionDTO createClasificacionDto, string usuario)
        {
            var pedido = await _ordenEntradaRepository.GetByIdAsync(createClasificacionDto.IdPedidoProveedor);
            if (pedido == null || !pedido.CantidadesPedido.Any() || !pedido.ProductosPedido.Any())
            {
                // No se puede crear la clasificación si el pedido no existe, no tiene pesajes o no tiene productos.
                return null;
            }

            // 1. Calcular el peso total
            var pesoTotal = pedido.CantidadesPedido.Sum(cp => cp.PesoNeto ?? 0);

            // 2. Generar el lote
            var producto = pedido.ProductosPedido.FirstOrDefault()?.Producto;
            var proveedor = pedido.Proveedor;
            if (producto == null || proveedor == null) return null; // Aún más validación

            var fecha = DateTime.Now;
            var clasificacionesHoy = await _clasificacionRepository.GetByDateAndProductAsync(fecha, producto.Id);
            var consecutivo = (clasificacionesHoy.Count() + 1).ToString("D3");

            var lote = $"{producto.Codigo}-{proveedor.RFC.Substring(0, 3)}-{fecha:yyyyMMdd}-{consecutivo}";

            // 3. Crear la entidad
            var clasificacion = new Clasificacion
            {
                Lote = lote,
                PesoTotal = pesoTotal,
                FechaRegistro = fecha,
                UsuarioRegistro = usuario,
                IdPedidoProveedor = createClasificacionDto.IdPedidoProveedor
            };

            var nuevaClasificacion = await _clasificacionRepository.AddAsync(clasificacion);

            // Actualizar el estado del pedido
            pedido.Estado = "Clasificada";
            await _ordenEntradaRepository.UpdatePedidoAsync(pedido);

            return new ClasificacionDTO
            {
                Id = nuevaClasificacion.Id,
                Lote = nuevaClasificacion.Lote,
                PesoTotal = nuevaClasificacion.PesoTotal,
                FechaRegistro = nuevaClasificacion.FechaRegistro,
                UsuarioRegistro = nuevaClasificacion.UsuarioRegistro,
                IdPedidoProveedor = nuevaClasificacion.IdPedidoProveedor
            };
        }

        public async Task<ClasificacionDTO> UpdateClasificacionAsync(int id, UpdateClasificacionDTO updateClasificacionDto)
        {
            var clasificacion = await _clasificacionRepository.GetByIdAsync(id);
            if (clasificacion == null) return null;

            if (updateClasificacionDto.Lote != null) clasificacion.Lote = updateClasificacionDto.Lote;
            if (updateClasificacionDto.PesoTotal.HasValue) clasificacion.PesoTotal = updateClasificacionDto.PesoTotal.Value;
            if (updateClasificacionDto.IdPedidoProveedor.HasValue) clasificacion.IdPedidoProveedor = updateClasificacionDto.IdPedidoProveedor.Value;

            var clasificacionActualizada = await _clasificacionRepository.UpdateAsync(clasificacion);

            return new ClasificacionDTO
            {
                Id = clasificacionActualizada.Id,
                Lote = clasificacionActualizada.Lote,
                PesoTotal = clasificacionActualizada.PesoTotal,
                FechaRegistro = clasificacionActualizada.FechaRegistro,
                UsuarioRegistro = clasificacionActualizada.UsuarioRegistro,
                IdPedidoProveedor = clasificacionActualizada.IdPedidoProveedor
            };
        }

        public async Task<bool> DeleteClasificacionAsync(int id)
        {
            return await _clasificacionRepository.DeleteAsync(id);
        }
    }
} 