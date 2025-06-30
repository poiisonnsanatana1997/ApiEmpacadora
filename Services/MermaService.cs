using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Repositories.Interfaces;
using AppAPIEmpacadora.Services.Interfaces;

namespace AppAPIEmpacadora.Services
{
    public class MermaService : IMermaService
    {
        private readonly IMermaRepository _repo;
        public MermaService(IMermaRepository repo)
        {
            _repo = repo;
        }
        public async Task<IEnumerable<MermaResponseDTO>> ObtenerTodosAsync()
        {
            var list = await _repo.ObtenerTodosAsync();
            var result = new List<MermaResponseDTO>();
            foreach (var item in list)
            {
                result.Add(MapToResponseDTO(item));
            }
            return result;
        }
        public async Task<MermaResponseDTO> ObtenerPorIdAsync(int id)
        {
            var entity = await _repo.ObtenerPorIdAsync(id);
            return entity == null ? null : MapToResponseDTO(entity);
        }
        public async Task<MermaResponseDTO> CrearAsync(CreateMermaDTO dto, string usuarioRegistro)
        {
            var entity = new Merma
            {
                Tipo = dto.Tipo,
                Peso = dto.Peso,
                Observaciones = dto.Observaciones,
                FechaRegistro = dto.FechaRegistro,
                UsuarioRegistro = usuarioRegistro,
                IdClasificacion = dto.IdClasificacion
            };
            var created = await _repo.CrearAsync(entity);
            return MapToResponseDTO(created);
        }
        public async Task<bool> ActualizarAsync(int id, UpdateMermaDTO dto)
        {
            var entity = await _repo.ObtenerPorIdAsync(id);
            if (entity == null) return false;
            if (dto.Tipo != null) entity.Tipo = dto.Tipo;
            if (dto.Peso.HasValue) entity.Peso = dto.Peso.Value;
            if (dto.Observaciones != null) entity.Observaciones = dto.Observaciones;
            return await _repo.ActualizarAsync(entity);
        }
        public async Task<bool> EliminarAsync(int id)
        {
            return await _repo.EliminarAsync(id);
        }
        private MermaResponseDTO MapToResponseDTO(Merma entity)
        {
            return new MermaResponseDTO
            {
                Id = entity.Id,
                Tipo = entity.Tipo,
                Peso = entity.Peso,
                Observaciones = entity.Observaciones,
                FechaRegistro = entity.FechaRegistro,
                UsuarioRegistro = entity.UsuarioRegistro,
                IdClasificacion = entity.IdClasificacion
            };
        }
    }
} 