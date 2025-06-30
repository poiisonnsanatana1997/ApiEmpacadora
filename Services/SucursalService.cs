using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Repositories.Interfaces;
using AppAPIEmpacadora.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Services
{
    public class SucursalService : ISucursalService
    {
        private readonly ISucursalRepository _sucursalRepository;

        public SucursalService(ISucursalRepository sucursalRepository)
        {
            _sucursalRepository = sucursalRepository;
        }

        public async Task<IEnumerable<SucursalDTO>> GetSucursalesAsync()
        {
            var sucursales = await _sucursalRepository.GetAllAsync();
            var sucursalDTOs = new List<SucursalDTO>();
            foreach (var sucursal in sucursales)
            {
                sucursalDTOs.Add(new SucursalDTO
                {
                    Id = sucursal.Id,
                    Nombre = sucursal.Nombre,
                    Direccion = sucursal.Direccion,
                    EncargadoAlmacen = sucursal.EncargadoAlmacen,
                    Telefono = sucursal.Telefono,
                    Correo = sucursal.Correo,
                    Activo = sucursal.Activo,
                    FechaRegistro = sucursal.FechaRegistro,
                    UsuarioRegistro = sucursal.UsuarioRegistro,
                    IdCliente = sucursal.IdCliente
                });
            }
            return sucursalDTOs;
        }

        public async Task<SucursalDTO> GetSucursalByIdAsync(int id)
        {
            var sucursal = await _sucursalRepository.GetByIdAsync(id);
            if (sucursal == null) return null;

            return new SucursalDTO
            {
                Id = sucursal.Id,
                Nombre = sucursal.Nombre,
                Direccion = sucursal.Direccion,
                EncargadoAlmacen = sucursal.EncargadoAlmacen,
                Telefono = sucursal.Telefono,
                Correo = sucursal.Correo,
                Activo = sucursal.Activo,
                FechaRegistro = sucursal.FechaRegistro,
                UsuarioRegistro = sucursal.UsuarioRegistro,
                IdCliente = sucursal.IdCliente
            };
        }

        public async Task<SucursalDTO> CreateSucursalAsync(CreateSucursalDTO createSucursalDto, string usuario)
        {
            var sucursal = new Sucursal
            {
                Nombre = createSucursalDto.Nombre,
                Direccion = createSucursalDto.Direccion,
                EncargadoAlmacen = createSucursalDto.EncargadoAlmacen,
                Telefono = createSucursalDto.Telefono,
                Correo = createSucursalDto.Correo,
                Activo = createSucursalDto.Activo,
                FechaRegistro = System.DateTime.Now,
                UsuarioRegistro = usuario,
                IdCliente = createSucursalDto.IdCliente
            };

            var nuevaSucursal = await _sucursalRepository.AddAsync(sucursal);

            return new SucursalDTO
            {
                Id = nuevaSucursal.Id,
                Nombre = nuevaSucursal.Nombre,
                Direccion = nuevaSucursal.Direccion,
                EncargadoAlmacen = nuevaSucursal.EncargadoAlmacen,
                Telefono = nuevaSucursal.Telefono,
                Correo = nuevaSucursal.Correo,
                Activo = nuevaSucursal.Activo,
                FechaRegistro = nuevaSucursal.FechaRegistro,
                UsuarioRegistro = nuevaSucursal.UsuarioRegistro,
                IdCliente = nuevaSucursal.IdCliente
            };
        }

        public async Task<SucursalDTO> UpdateSucursalAsync(int id, UpdateSucursalDTO updateSucursalDto)
        {
            var sucursal = await _sucursalRepository.GetByIdAsync(id);
            if (sucursal == null) return null;

            if (updateSucursalDto.Nombre != null) sucursal.Nombre = updateSucursalDto.Nombre;
            if (updateSucursalDto.Direccion != null) sucursal.Direccion = updateSucursalDto.Direccion;
            if (updateSucursalDto.EncargadoAlmacen != null) sucursal.EncargadoAlmacen = updateSucursalDto.EncargadoAlmacen;
            if (updateSucursalDto.Telefono != null) sucursal.Telefono = updateSucursalDto.Telefono;
            if (updateSucursalDto.Correo != null) sucursal.Correo = updateSucursalDto.Correo;
            if (updateSucursalDto.Activo.HasValue) sucursal.Activo = updateSucursalDto.Activo.Value;
            if (updateSucursalDto.IdCliente.HasValue) sucursal.IdCliente = updateSucursalDto.IdCliente.Value;

            var sucursalActualizada = await _sucursalRepository.UpdateAsync(sucursal);

            return new SucursalDTO
            {
                Id = sucursalActualizada.Id,
                Nombre = sucursalActualizada.Nombre,
                Direccion = sucursalActualizada.Direccion,
                EncargadoAlmacen = sucursalActualizada.EncargadoAlmacen,
                Telefono = sucursalActualizada.Telefono,
                Correo = sucursalActualizada.Correo,
                Activo = sucursalActualizada.Activo,
                FechaRegistro = sucursalActualizada.FechaRegistro,
                UsuarioRegistro = sucursalActualizada.UsuarioRegistro,
                IdCliente = sucursalActualizada.IdCliente
            };
        }

        public async Task<bool> DeleteSucursalAsync(int id)
        {
            return await _sucursalRepository.DeleteAsync(id);
        }
    }
} 