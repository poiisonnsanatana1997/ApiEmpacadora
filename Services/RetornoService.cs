using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Repositories.Interfaces;
using AppAPIEmpacadora.Services.Interfaces;

namespace AppAPIEmpacadora.Services
{
    public class RetornoService : IRetornoService
    {
        private readonly IRetornoRepository _repo;
        public RetornoService(IRetornoRepository repo)
        {
            _repo = repo;
        }
        public async Task<IEnumerable<RetornoResponseDTO>> ObtenerTodosAsync()
        {
            var list = await _repo.ObtenerTodosAsync();
            var result = new List<RetornoResponseDTO>();
            foreach (var item in list)
            {
                result.Add(MapToResponseDTO(item));
            }
            return result;
        }
        public async Task<RetornoResponseDTO> ObtenerPorIdAsync(int id)
        {
            var entity = await _repo.ObtenerPorIdAsync(id);
            return entity == null ? null : MapToResponseDTO(entity);
        }
        public async Task<RetornoResponseDTO> CrearAsync(CreateRetornoDTO dto, string usuarioRegistro)
        {
            var entity = new Retorno
            {
                Numero = dto.Numero,
                Peso = dto.Peso,
                Observaciones = dto.Observaciones,
                FechaRegistro = dto.FechaRegistro,
                UsuarioRegistro = usuarioRegistro,
                IdClasificacion = dto.IdClasificacion
            };
            var created = await _repo.CrearAsync(entity);
            return MapToResponseDTO(created);
        }
        public async Task<bool> ActualizarAsync(int id, UpdateRetornoDTO dto)
        {
            var entity = await _repo.ObtenerPorIdAsync(id);
            if (entity == null) return false;
            if (dto.Numero != null) entity.Numero = dto.Numero;
            if (dto.Peso.HasValue) entity.Peso = dto.Peso.Value;
            if (dto.Observaciones != null) entity.Observaciones = dto.Observaciones;
            return await _repo.ActualizarAsync(entity);
        }
        public async Task<bool> EliminarAsync(int id)
        {
            return await _repo.EliminarAsync(id);
        }
        private RetornoResponseDTO MapToResponseDTO(Retorno entity)
        {
            return new RetornoResponseDTO
            {
                Id = entity.Id,
                Numero = entity.Numero,
                Peso = entity.Peso,
                Observaciones = entity.Observaciones,
                FechaRegistro = entity.FechaRegistro,
                UsuarioRegistro = entity.UsuarioRegistro,
                IdClasificacion = entity.IdClasificacion
            };
        }
    }
} 