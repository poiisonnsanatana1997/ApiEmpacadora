using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Repositories.Interfaces;
using AppAPIEmpacadora.Services.Interfaces;
using System.Linq;

namespace AppAPIEmpacadora.Services
{
    public class PedidoClienteService : IPedidoClienteService
    {
        private readonly IPedidoClienteRepository _repo;
        public PedidoClienteService(IPedidoClienteRepository repo)
        {
            _repo = repo;
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
                Activo = dto.Activo
            };
            var created = await _repo.CrearAsync(entity);
            return MapToResponseDTO(created);
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
            return await _repo.ActualizarAsync(entity);
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
                IdSucursal = entity.IdSucursal,
                IdCliente = entity.IdCliente
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
    }
} 