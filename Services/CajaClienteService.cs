using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Repositories.Interfaces;
using AppAPIEmpacadora.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Services
{
    public class CajaClienteService : ICajaClienteService
    {
        private readonly ICajaClienteRepository _cajaClienteRepository;

        public CajaClienteService(ICajaClienteRepository cajaClienteRepository)
        {
            _cajaClienteRepository = cajaClienteRepository;
        }

        public async Task<IEnumerable<CajaClienteDTO>> GetCajasClienteAsync()
        {
            var cajasCliente = await _cajaClienteRepository.GetAllAsync();
            return cajasCliente.Select(c => new CajaClienteDTO
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Peso = c.Peso,
                Precio = c.Precio,
                IdCliente = c.IdCliente,
                NombreCliente = c.Cliente?.Nombre
            });
        }

        public async Task<CajaClienteDTO> GetCajaClienteByIdAsync(int id)
        {
            var c = await _cajaClienteRepository.GetByIdAsync(id);
            if (c == null) return null;
            return new CajaClienteDTO
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Peso = c.Peso,
                Precio = c.Precio,
                IdCliente = c.IdCliente,
                NombreCliente = c.Cliente?.Nombre
            };
        }

        public async Task<CajaClienteDTO> CreateCajaClienteAsync(CreateCajaClienteDTO createCajaClienteDTO)
        {
            var cajaCliente = new CajaCliente
            {
                Nombre = createCajaClienteDTO.Nombre,
                Peso = createCajaClienteDTO.Peso,
                Precio = createCajaClienteDTO.Precio ?? 0,
                IdCliente = createCajaClienteDTO.IdCliente
            };

            var newCajaCliente = await _cajaClienteRepository.AddAsync(cajaCliente);
            return await GetCajaClienteByIdAsync(newCajaCliente.Id);
        }

        public async Task<CajaClienteDTO> UpdateCajaClienteAsync(int id, UpdateCajaClienteDTO updateCajaClienteDTO)
        {
            var cajaCliente = await _cajaClienteRepository.GetByIdAsync(id);
            if (cajaCliente == null) return null;

            if (updateCajaClienteDTO.Nombre != null)
                cajaCliente.Nombre = updateCajaClienteDTO.Nombre;
            if (updateCajaClienteDTO.Peso.HasValue)
                cajaCliente.Peso = updateCajaClienteDTO.Peso.Value;
            if (updateCajaClienteDTO.Precio.HasValue)
                cajaCliente.Precio = updateCajaClienteDTO.Precio.Value;
            if (updateCajaClienteDTO.IdCliente.HasValue)
                cajaCliente.IdCliente = updateCajaClienteDTO.IdCliente.Value;

            await _cajaClienteRepository.UpdateAsync(cajaCliente);
            return await GetCajaClienteByIdAsync(id);
        }

        public async Task<bool> DeleteCajaClienteAsync(int id)
        {
            return await _cajaClienteRepository.DeleteAsync(id);
        }
    }
} 