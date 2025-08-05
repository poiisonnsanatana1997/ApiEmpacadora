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
                    IdPedidoProveedor = clasificacion.IdPedidoProveedor,
                    XL = clasificacion.XL,
                    L = clasificacion.L,
                    M = clasificacion.M,
                    S = clasificacion.S,
                    Retornos = clasificacion.Retornos,
                    PorcentajeClasificado = clasificacion.PorcentajeClasificado
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
                IdPedidoProveedor = clasificacion.IdPedidoProveedor,
                XL = clasificacion.XL,
                L = clasificacion.L,
                M = clasificacion.M,
                S = clasificacion.S,
                Retornos = clasificacion.Retornos,
                PorcentajeClasificado = clasificacion.PorcentajeClasificado
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

            // Generar el lote de forma segura, manejando RFCs cortos
            var rfcPrefix = !string.IsNullOrEmpty(proveedor.RFC) && proveedor.RFC.Length >= 3 
                ? proveedor.RFC.Substring(0, 3) 
                : proveedor.RFC ?? "XXX";
            var lote = $"{producto.Codigo}-{rfcPrefix}-{fecha:yyyyMMdd}-{consecutivo}";

            // 3. Crear la entidad
            var clasificacion = new Clasificacion
            {
                Lote = lote,
                PesoTotal = pesoTotal,
                FechaRegistro = fecha,
                UsuarioRegistro = usuario,
                IdPedidoProveedor = createClasificacionDto.IdPedidoProveedor,
                XL = 0,
                L = 0,
                M = 0,
                S = 0,
                Retornos = 0
            };

            var nuevaClasificacion = await _clasificacionRepository.AddAsync(clasificacion);

            // Actualizar el estado del pedido
            pedido.Estado = "Clasificando";
            await _ordenEntradaRepository.UpdatePedidoAsync(pedido);

            return new ClasificacionDTO
            {
                Id = nuevaClasificacion.Id,
                Lote = nuevaClasificacion.Lote,
                PesoTotal = nuevaClasificacion.PesoTotal,
                FechaRegistro = nuevaClasificacion.FechaRegistro,
                UsuarioRegistro = nuevaClasificacion.UsuarioRegistro,
                IdPedidoProveedor = nuevaClasificacion.IdPedidoProveedor,
                XL = nuevaClasificacion.XL,
                L = nuevaClasificacion.L,
                M = nuevaClasificacion.M,
                S = nuevaClasificacion.S,
                Retornos = nuevaClasificacion.Retornos,
                PorcentajeClasificado = nuevaClasificacion.PorcentajeClasificado
            };
        }

        public async Task<ClasificacionDTO> UpdateClasificacionAsync(int id, UpdateClasificacionDTO updateClasificacionDto)
        {
            var clasificacion = await _clasificacionRepository.GetByIdAsync(id);
            if (clasificacion == null) return null;

            if (updateClasificacionDto.Lote != null) clasificacion.Lote = updateClasificacionDto.Lote;
            if (updateClasificacionDto.PesoTotal.HasValue) clasificacion.PesoTotal = updateClasificacionDto.PesoTotal.Value;
            if (updateClasificacionDto.IdPedidoProveedor.HasValue) clasificacion.IdPedidoProveedor = updateClasificacionDto.IdPedidoProveedor.Value;
            if (updateClasificacionDto.XL.HasValue) clasificacion.XL = updateClasificacionDto.XL.Value;
            if (updateClasificacionDto.L.HasValue) clasificacion.L = updateClasificacionDto.L.Value;
            if (updateClasificacionDto.M.HasValue) clasificacion.M = updateClasificacionDto.M.Value;
            if (updateClasificacionDto.S.HasValue) clasificacion.S = updateClasificacionDto.S.Value;
            if (updateClasificacionDto.Retornos.HasValue) clasificacion.Retornos = updateClasificacionDto.Retornos.Value;
            if (updateClasificacionDto.PorcentajeClasificado.HasValue) clasificacion.PorcentajeClasificado = updateClasificacionDto.PorcentajeClasificado.Value;

            var clasificacionActualizada = await _clasificacionRepository.UpdateAsync(clasificacion);

            return new ClasificacionDTO
            {
                Id = clasificacionActualizada.Id,
                Lote = clasificacionActualizada.Lote,
                PesoTotal = clasificacionActualizada.PesoTotal,
                FechaRegistro = clasificacionActualizada.FechaRegistro,
                UsuarioRegistro = clasificacionActualizada.UsuarioRegistro,
                IdPedidoProveedor = clasificacionActualizada.IdPedidoProveedor,
                XL = clasificacionActualizada.XL,
                L = clasificacionActualizada.L,
                M = clasificacionActualizada.M,
                S = clasificacionActualizada.S,
                Retornos = clasificacionActualizada.Retornos,
                PorcentajeClasificado = clasificacionActualizada.PorcentajeClasificado
            };
        }

        public async Task<bool> DeleteClasificacionAsync(int id)
        {
            return await _clasificacionRepository.DeleteAsync(id);
        }

        public async Task<AjustePesoClasificacionResponseDTO> AjustarPesoClasificacionAsync(int idClasificacion, AjustePesoClasificacionDTO ajusteDto, string usuario)
        {
            var response = new AjustePesoClasificacionResponseDTO
            {
                IdClasificacion = idClasificacion,
                AjusteRealizado = false,
                Mensaje = "Iniciando ajuste de peso de clasificación"
            };

            // Verificar que la clasificación existe
            var clasificacion = await _clasificacionRepository.GetByIdAsync(idClasificacion);
            if (clasificacion == null)
            {
                response.Mensaje = "La clasificación especificada no existe";
                return response;
            }

            response.Lote = clasificacion.Lote;
            var tarimasAjustadas = new List<TarimaClasificacion>();
            var totalTarimasAjustadas = 0;

            // Procesar cada tipo de clasificación que tenga valor
            var tiposAjuste = new Dictionary<string, decimal?>
            {
                { "XL", ajusteDto.XL },
                { "L", ajusteDto.L },
                { "M", ajusteDto.M },
                { "S", ajusteDto.S }
            };

            foreach (var tipoAjuste in tiposAjuste)
            {
                if (!tipoAjuste.Value.HasValue || tipoAjuste.Value.Value <= 0)
                {
                    continue;
                }

                // Buscar tarimas con este tipo
                var tarimasTipo = await _clasificacionRepository.GetTarimasClasificacionByTipoAsync(idClasificacion, tipoAjuste.Key);
                var tarimasList = tarimasTipo.ToList();

                if (tarimasList.Count > 0)
                {
                    // Calcular peso por tarima (distribución equitativa)
                    var pesoPorTarima = tipoAjuste.Value.Value / tarimasList.Count;

                    // Aplicar el ajuste a todas las tarimas (sumar al peso existente)
                    foreach (var tarima in tarimasList)
                    {
                        tarima.Peso += pesoPorTarima;
                        tarimasAjustadas.Add(tarima);
                    }

                    totalTarimasAjustadas += tarimasList.Count;
                }
            }

            // Guardar los cambios si hay tarimas para ajustar
            if (tarimasAjustadas.Count > 0)
            {
                var resultado = await _clasificacionRepository.UpdateTarimasClasificacionAsync(tarimasAjustadas);
                if (resultado)
                {
                    response.AjusteRealizado = true;
                    response.Mensaje = $"Ajuste completado exitosamente. Se modificaron {totalTarimasAjustadas} tarimas.";
                }
                else
                {
                    response.Mensaje = "Error al guardar los ajustes en la base de datos";
                }
            }
            else
            {
                response.Mensaje = "No se encontraron tarimas para ajustar con los tipos especificados";
            }

            return response;
        }
    }
} 