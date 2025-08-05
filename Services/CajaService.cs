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
    public class CajaService : ICajaService
    {
        private readonly ICajaRepository _cajaRepository;

        public CajaService(ICajaRepository cajaRepository)
        {
            _cajaRepository = cajaRepository;
        }

        public async Task<IEnumerable<CajaSummaryDTO>> GetCajasAsync()
        {
            var cajas = await _cajaRepository.GetAllAsync();
            var cajaDTOs = new List<CajaSummaryDTO>();
            
            foreach (var caja in cajas)
            {
                cajaDTOs.Add(new CajaSummaryDTO
                {
                    Id = caja.Id,
                    Tipo = caja.Tipo,
                    Cantidad = caja.Cantidad,
                    Peso = caja.Peso,
                    FechaRegistro = caja.FechaRegistro
                });
            }
            return cajaDTOs;
        }

        public async Task<CajaDTO> GetCajaByIdAsync(int id)
        {
            var caja = await _cajaRepository.GetByIdAsync(id);
            if (caja == null) return null;

            return new CajaDTO
            {
                Id = caja.Id,
                Tipo = caja.Tipo,
                Cantidad = caja.Cantidad,
                Peso = caja.Peso,
                FechaRegistro = caja.FechaRegistro,
                UsuarioRegistro = caja.UsuarioRegistro,
                IdClasificacion = caja.IdClasificacion
            };
        }

        public async Task<CajaDTO> CreateCajaAsync(CreateCajaDTO createCajaDto, string usuario)
        {
            var caja = new Caja
            {
                Tipo = createCajaDto.Tipo,
                Cantidad = createCajaDto.Cantidad,
                Peso = createCajaDto.Peso,
                IdClasificacion = createCajaDto.IdClasificacion,
                FechaRegistro = DateTime.Now,
                UsuarioRegistro = usuario
            };

            var nuevaCaja = await _cajaRepository.AddAsync(caja);
            
            return new CajaDTO
            {
                Id = nuevaCaja.Id,
                Tipo = nuevaCaja.Tipo,
                Cantidad = nuevaCaja.Cantidad,
                Peso = nuevaCaja.Peso,
                FechaRegistro = nuevaCaja.FechaRegistro,
                UsuarioRegistro = nuevaCaja.UsuarioRegistro,
                IdClasificacion = nuevaCaja.IdClasificacion
            };
        }

        public async Task<CajaDTO> UpdateCajaAsync(int id, UpdateCajaDTO updateCajaDto)
        {
            var caja = await _cajaRepository.GetByIdAsync(id);
            if (caja == null) return null;

            // Actualizar solo los campos proporcionados
            if (!string.IsNullOrEmpty(updateCajaDto.Tipo))
                caja.Tipo = updateCajaDto.Tipo;
            
            if (updateCajaDto.Cantidad.HasValue)
                caja.Cantidad = updateCajaDto.Cantidad;
            
            if (updateCajaDto.Peso.HasValue)
                caja.Peso = updateCajaDto.Peso;
            
            if (updateCajaDto.IdClasificacion.HasValue)
                caja.IdClasificacion = updateCajaDto.IdClasificacion;

            var cajaActualizada = await _cajaRepository.UpdateAsync(caja);
            
            return new CajaDTO
            {
                Id = cajaActualizada.Id,
                Tipo = cajaActualizada.Tipo,
                Cantidad = cajaActualizada.Cantidad,
                Peso = cajaActualizada.Peso,
                FechaRegistro = cajaActualizada.FechaRegistro,
                UsuarioRegistro = cajaActualizada.UsuarioRegistro,
                IdClasificacion = cajaActualizada.IdClasificacion
            };
        }

        public async Task<bool> DeleteCajaAsync(int id)
        {
            return await _cajaRepository.DeleteAsync(id);
        }

        public async Task<CajaDTO> AjustarCantidadCajaAsync(AjustarCantidadCajaDTO ajusteDto, string usuario)
        {
            // Buscar caja existente por tipo y clasificación
            var cajaExistente = await _cajaRepository.GetByTipoAndClasificacionAsync(ajusteDto.Tipo, ajusteDto.IdClasificacion);
            
            Caja cajaActualizada;
            
            if (cajaExistente != null)
            {
                // Caja existe, ajustar cantidad y peso
                var nuevaCantidad = (cajaExistente.Cantidad ?? 0) + ajusteDto.CantidadAjuste;
                
                // Validar que la cantidad no quede negativa
                if (nuevaCantidad < 0)
                {
                    throw new InvalidOperationException($"La cantidad no puede quedar negativa. Cantidad actual: {cajaExistente.Cantidad}, Ajuste: {ajusteDto.CantidadAjuste}");
                }
                
                cajaExistente.Cantidad = nuevaCantidad;
                
                // Ajustar peso si se proporciona
                if (ajusteDto.PesoAjuste.HasValue)
                {
                    cajaExistente.Peso = (cajaExistente.Peso ?? 0) + ajusteDto.PesoAjuste.Value;
                }
                
                cajaActualizada = await _cajaRepository.UpdateAsync(cajaExistente);
            }
            else
            {
                // Caja no existe, crear nueva
                if (ajusteDto.CantidadAjuste < 0)
                {
                    throw new InvalidOperationException("No se puede crear una caja con cantidad negativa");
                }
                
                var nuevaCaja = new Caja
                {
                    Tipo = ajusteDto.Tipo,
                    Cantidad = ajusteDto.CantidadAjuste,
                    Peso = ajusteDto.PesoAjuste,
                    IdClasificacion = ajusteDto.IdClasificacion,
                    FechaRegistro = DateTime.Now,
                    UsuarioRegistro = usuario
                };
                
                cajaActualizada = await _cajaRepository.AddAsync(nuevaCaja);
            }
            
            return new CajaDTO
            {
                Id = cajaActualizada.Id,
                Tipo = cajaActualizada.Tipo,
                Cantidad = cajaActualizada.Cantidad,
                Peso = cajaActualizada.Peso,
                FechaRegistro = cajaActualizada.FechaRegistro,
                UsuarioRegistro = cajaActualizada.UsuarioRegistro,
                IdClasificacion = cajaActualizada.IdClasificacion
            };
        }

        public async Task<IEnumerable<CajaDTO>> GetCajasByClasificacionAsync(int idClasificacion)
        {
            var cajas = await _cajaRepository.GetByClasificacionAsync(idClasificacion);
            var cajaDTOs = new List<CajaDTO>();
            
            foreach (var caja in cajas)
            {
                cajaDTOs.Add(new CajaDTO
                {
                    Id = caja.Id,
                    Tipo = caja.Tipo,
                    Cantidad = caja.Cantidad,
                    Peso = caja.Peso,
                    FechaRegistro = caja.FechaRegistro,
                    UsuarioRegistro = caja.UsuarioRegistro,
                    IdClasificacion = caja.IdClasificacion
                });
            }
            return cajaDTOs;
        }
    }
} 