using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Repositories.Interfaces;
using AppAPIEmpacadora.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClienteService(
            IClienteRepository clienteRepository, 
            IWebHostEnvironment hostingEnvironment, 
            IHttpContextAccessor httpContextAccessor)
        {
            _clienteRepository = clienteRepository;
            _hostingEnvironment = hostingEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<ClienteSummaryDTO>> GetClientesAsync()
        {
            var clientes = await _clienteRepository.GetAllAsync();
            var clienteDTOs = new List<ClienteSummaryDTO>();
            foreach (var cliente in clientes)
            {
                clienteDTOs.Add(new ClienteSummaryDTO
                {
                    Id = cliente.Id,
                    Nombre = cliente.Nombre,
                    RazonSocial = cliente.RazonSocial,
                    Rfc = cliente.Rfc,
                    Activo = cliente.Activo,
                    FechaRegistro = cliente.FechaRegistro
                });
            }
            return clienteDTOs;
        }

        public async Task<ClienteDTO> GetClienteByIdAsync(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null) return null;

            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var constanciaUrl = string.IsNullOrEmpty(cliente.ConstanciaFiscal) 
                ? null 
                : $"{baseUrl}/{cliente.ConstanciaFiscal.Replace("\\", "/")}";

            return new ClienteDTO
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                RazonSocial = cliente.RazonSocial,
                Rfc = cliente.Rfc,
                ConstanciaFiscal = constanciaUrl,
                RepresentanteComercial = cliente.RepresentanteComercial,
                TipoCliente = cliente.TipoCliente,
                Activo = cliente.Activo,
                FechaRegistro = cliente.FechaRegistro,
                UsuarioRegistro = cliente.UsuarioRegistro,
                Sucursales = cliente.Sucursales?.Select(s => new SucursalDTO
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    Direccion = s.Direccion,
                    EncargadoAlmacen = s.EncargadoAlmacen,
                    Telefono = s.Telefono,
                    Correo = s.Correo,
                    Activo = s.Activo,
                    IdCliente = s.IdCliente
                }).ToList(),
                CajasCliente = cliente.CajasCliente?.Select(c => new CajaClienteDTO
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Peso = c.Peso,
                    Precio = c.Precio,
                    IdCliente = c.IdCliente,
                    NombreCliente = cliente.Nombre 
                }).ToList()
            };
        }

        public async Task<ClienteDTO> CreateClienteAsync(CreateClienteDTO createClienteDto, string usuario)
        {
            string pathConstancia = null;
            if (createClienteDto.ConstanciaFiscal != null)
            {
                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "constancias");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + createClienteDto.ConstanciaFiscal.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await createClienteDto.ConstanciaFiscal.CopyToAsync(stream);
                }
                pathConstancia = Path.Combine("uploads", "constancias", uniqueFileName).Replace('\\', '/');
            }

            var cliente = new Cliente
            {
                Nombre = createClienteDto.Nombre,
                RazonSocial = createClienteDto.RazonSocial,
                Rfc = createClienteDto.Rfc,
                ConstanciaFiscal = pathConstancia,
                RepresentanteComercial = createClienteDto.RepresentanteComercial,
                TipoCliente = createClienteDto.TipoCliente,
                Activo = createClienteDto.Activo,
                FechaRegistro = createClienteDto.FechaRegistro,
                UsuarioRegistro = usuario
            };

            var nuevoCliente = await _clienteRepository.AddAsync(cliente);

            return new ClienteDTO
            {
                Id = nuevoCliente.Id,
                Nombre = nuevoCliente.Nombre,
                RazonSocial = nuevoCliente.RazonSocial,
                Rfc = nuevoCliente.Rfc,
                ConstanciaFiscal = nuevoCliente.ConstanciaFiscal,
                RepresentanteComercial = nuevoCliente.RepresentanteComercial,
                TipoCliente = nuevoCliente.TipoCliente,
                Activo = nuevoCliente.Activo,
                FechaRegistro = nuevoCliente.FechaRegistro,
                UsuarioRegistro = nuevoCliente.UsuarioRegistro
            };
        }

        public async Task<ClienteDTO> UpdateClienteAsync(int id, UpdateClienteDTO updateClienteDto)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null) return null;

            if (updateClienteDto.ConstanciaFiscal != null)
            {
                // Opcional: Eliminar el archivo antiguo si existe
                if (!string.IsNullOrEmpty(cliente.ConstanciaFiscal))
                {
                    var oldFilePath = Path.Combine(_hostingEnvironment.WebRootPath, cliente.ConstanciaFiscal);
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }

                // Guardar el nuevo archivo
                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "constancias");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + updateClienteDto.ConstanciaFiscal.FileName;
                var newFilePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    await updateClienteDto.ConstanciaFiscal.CopyToAsync(stream);
                }
                cliente.ConstanciaFiscal = Path.Combine("uploads", "constancias", uniqueFileName).Replace('\\', '/');
            }

            if (updateClienteDto.Nombre != null) cliente.Nombre = updateClienteDto.Nombre;
            if (updateClienteDto.RazonSocial != null) cliente.RazonSocial = updateClienteDto.RazonSocial;
            if (updateClienteDto.Rfc != null) cliente.Rfc = updateClienteDto.Rfc;
            if (updateClienteDto.RepresentanteComercial != null) cliente.RepresentanteComercial = updateClienteDto.RepresentanteComercial;
            if (updateClienteDto.TipoCliente != null) cliente.TipoCliente = updateClienteDto.TipoCliente;
            if (updateClienteDto.Activo.HasValue) cliente.Activo = updateClienteDto.Activo.Value;

            var clienteActualizado = await _clienteRepository.UpdateAsync(cliente);

            return new ClienteDTO
            {
                Id = clienteActualizado.Id,
                Nombre = clienteActualizado.Nombre,
                RazonSocial = clienteActualizado.RazonSocial,
                Rfc = clienteActualizado.Rfc,
                ConstanciaFiscal = clienteActualizado.ConstanciaFiscal,
                RepresentanteComercial = clienteActualizado.RepresentanteComercial,
                TipoCliente = clienteActualizado.TipoCliente,
                Activo = clienteActualizado.Activo,
                FechaRegistro = clienteActualizado.FechaRegistro,
                UsuarioRegistro = clienteActualizado.UsuarioRegistro
            };
        }

        public async Task<bool> DeleteClienteAsync(int id)
        {
            return await _clienteRepository.DeleteAsync(id);
        }
    }
} 