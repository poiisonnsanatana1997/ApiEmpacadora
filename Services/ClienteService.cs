using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Models.Exceptions;
using AppAPIEmpacadora.Repositories.Interfaces;
using AppAPIEmpacadora.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace AppAPIEmpacadora.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggingService _loggingService;

        public ClienteService(
            IClienteRepository clienteRepository,
            IWebHostEnvironment hostingEnvironment,
            IHttpContextAccessor httpContextAccessor,
            ILoggingService loggingService)
        {
            _clienteRepository = clienteRepository;
            _hostingEnvironment = hostingEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _loggingService = loggingService;
        }

        public async Task<IEnumerable<ClienteSummaryDTO>> GetClientesAsync()
        {
            try
            {
                _loggingService.LogInformation("Iniciando obtención de lista de clientes");
                
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
                        Telefono = cliente.Telefono,
                        Correo = cliente.Correo,
                        Activo = cliente.Activo,
                        FechaRegistro = cliente.FechaRegistro
                    });
                }
                
                _loggingService.LogInformation("Lista de clientes obtenida exitosamente. Total: {Count}", clienteDTOs.Count);
                return clienteDTOs;
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error en GetClientesAsync", ex);
                throw;
            }
        }

        public async Task<IEnumerable<ClienteDTO>> GetClientesDetalladosAsync()
        {
            var clientes = await _clienteRepository.GetAllAsync();
            var clienteDTOs = new List<ClienteDTO>();

            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            foreach (var cliente in clientes)
            {
                var constanciaUrl = string.IsNullOrEmpty(cliente.ConstanciaFiscal)
                    ? null
                    : $"{baseUrl}/{cliente.ConstanciaFiscal.Replace("\\", "/")}";

                clienteDTOs.Add(new ClienteDTO
                {
                    Id = cliente.Id,
                    Nombre = cliente.Nombre,
                    RazonSocial = cliente.RazonSocial,
                    Rfc = cliente.Rfc,
                    ConstanciaFiscal = constanciaUrl,
                    RepresentanteComercial = cliente.RepresentanteComercial,
                    TipoCliente = cliente.TipoCliente,
                    Telefono = cliente.Telefono,
                    Correo = cliente.Correo,
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
                Telefono = cliente.Telefono,
                Correo = cliente.Correo,
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
            try
            {
                _loggingService.LogUserAction("Crear cliente", usuario, $"Nombre: {createClienteDto.Nombre}");
                
                string pathConstancia = null;
                if (createClienteDto.ConstanciaFiscal != null)
                {
                    try
                    {
                        var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "constancias");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                            _loggingService.LogInformation("Directorio de constancias creado: {Path}", uploadsFolder);
                        }

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + createClienteDto.ConstanciaFiscal.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await createClienteDto.ConstanciaFiscal.CopyToAsync(stream);
                        }
                        pathConstancia = Path.Combine("uploads", "constancias", uniqueFileName).Replace('\\', '/');
                        
                        _loggingService.LogFileOperation("Crear", uniqueFileName, usuario);
                    }
                    catch (Exception ex)
                    {
                        _loggingService.LogError("Error al procesar archivo de constancia fiscal", ex);
                        throw new FileOperationException("Error al procesar el archivo de constancia fiscal", ex);
                    }
                }

                var cliente = new Cliente
                {
                    Nombre = createClienteDto.Nombre,
                    RazonSocial = createClienteDto.RazonSocial,
                    Rfc = createClienteDto.Rfc,
                    ConstanciaFiscal = pathConstancia,
                    RepresentanteComercial = createClienteDto.RepresentanteComercial,
                    TipoCliente = createClienteDto.TipoCliente,
                    Telefono = createClienteDto.Telefono,
                    Correo = createClienteDto.Correo,
                    Activo = createClienteDto.Activo,
                    FechaRegistro = createClienteDto.FechaRegistro,
                    UsuarioRegistro = usuario
                };

                var nuevoCliente = await _clienteRepository.AddAsync(cliente);

                _loggingService.LogInformation("Cliente creado exitosamente con ID {Id} por usuario {Usuario}", nuevoCliente.Id, usuario);

                return new ClienteDTO
                {
                    Id = nuevoCliente.Id,
                    Nombre = nuevoCliente.Nombre,
                    RazonSocial = nuevoCliente.RazonSocial,
                    Rfc = nuevoCliente.Rfc,
                    ConstanciaFiscal = nuevoCliente.ConstanciaFiscal,
                    RepresentanteComercial = nuevoCliente.RepresentanteComercial,
                    TipoCliente = nuevoCliente.TipoCliente,
                    Telefono = nuevoCliente.Telefono,
                    Correo = nuevoCliente.Correo,
                    Activo = nuevoCliente.Activo,
                    FechaRegistro = nuevoCliente.FechaRegistro,
                    UsuarioRegistro = nuevoCliente.UsuarioRegistro
                };
            }
            catch (FileOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error al crear cliente", ex);
                throw;
            }
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
            if (updateClienteDto.Telefono != null) cliente.Telefono = updateClienteDto.Telefono;
            if (updateClienteDto.Correo != null) cliente.Correo = updateClienteDto.Correo;
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
                Telefono = clienteActualizado.Telefono,
                Correo = clienteActualizado.Correo,
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