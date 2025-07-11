using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Repositories.Interfaces;
using AppAPIEmpacadora.Services.Interfaces;

namespace AppAPIEmpacadora.Services
{
    public class ProveedorService : IProveedorService
    {
        private readonly IProveedorRepository _proveedorRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProveedorService(IProveedorRepository proveedorRepository, IWebHostEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _proveedorRepository = proveedorRepository;
            _hostingEnvironment = hostingEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<ProveedorSimpleDTO>> ObtenerTodosAsync()
        {
            return await _proveedorRepository.ObtenerTodosAsync();
        }

        public async Task<ProveedorSimpleDTO> ObtenerPorIdAsync(int id)
        {
            return await _proveedorRepository.ObtenerPorIdAsync(id);
        }

        public async Task<IEnumerable<ProveedorDTO>> GetProveedoresAsync()
        {
            return await _proveedorRepository.GetAllAsync();
        }

        public async Task<ProveedorDTO> GetProveedorByIdAsync(int id)
        {
            var proveedor = await _proveedorRepository.GetByIdAsync(id);
            if (proveedor == null) return null;

            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var situacionFiscalUrl = string.IsNullOrEmpty(proveedor.SituacionFiscal) 
                ? null 
                : $"{baseUrl}/{proveedor.SituacionFiscal.Replace("\\", "/")}";

            return new ProveedorDTO
            {
                Id = proveedor.Id,
                Nombre = proveedor.Nombre,
                RFC = proveedor.RFC,
                Activo = proveedor.Activo,
                Telefono = proveedor.Telefono,
                Correo = proveedor.Correo,
                DireccionFiscal = proveedor.DireccionFiscal,
                SituacionFiscal = situacionFiscalUrl,
                FechaRegistro = proveedor.FechaRegistro,
                UsuarioRegistro = proveedor.UsuarioRegistro
            };
        }

        public async Task<ProveedorDTO> CreateProveedorAsync(CreateProveedorDTO createProveedorDto, string usuario)
        {
            string pathSituacionFiscal = null;
            if (createProveedorDto.SituacionFiscal != null)
            {
                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "situaciones-fiscales");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + createProveedorDto.SituacionFiscal.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await createProveedorDto.SituacionFiscal.CopyToAsync(stream);
                }
                pathSituacionFiscal = Path.Combine("uploads", "situaciones-fiscales", uniqueFileName).Replace('\\', '/');
            }

            var proveedor = new Proveedor
            {
                Nombre = createProveedorDto.Nombre,
                RFC = createProveedorDto.RFC,
                Activo = createProveedorDto.Activo,
                Telefono = createProveedorDto.Telefono,
                Correo = createProveedorDto.Correo,
                DireccionFiscal = createProveedorDto.DireccionFiscal,
                SituacionFiscal = pathSituacionFiscal,
                FechaRegistro = createProveedorDto.FechaRegistro,
                UsuarioRegistro = usuario
            };

            var nuevoProveedor = await _proveedorRepository.AddAsync(proveedor);

            return new ProveedorDTO
            {
                Id = nuevoProveedor.Id,
                Nombre = nuevoProveedor.Nombre,
                RFC = nuevoProveedor.RFC,
                Activo = nuevoProveedor.Activo,
                Telefono = nuevoProveedor.Telefono,
                Correo = nuevoProveedor.Correo,
                DireccionFiscal = nuevoProveedor.DireccionFiscal,
                SituacionFiscal = nuevoProveedor.SituacionFiscal,
                FechaRegistro = nuevoProveedor.FechaRegistro,
                UsuarioRegistro = nuevoProveedor.UsuarioRegistro
            };
        }

        public async Task<ProveedorDTO> UpdateProveedorAsync(int id, UpdateProveedorDTO updateProveedorDto, string usuario)
        {
            var proveedorExistente = await _proveedorRepository.GetByIdAsync(id);
            if (proveedorExistente == null)
                return null;

            // Manejar el archivo de situación fiscal si se proporciona uno nuevo
            if (updateProveedorDto.SituacionFiscal != null)
            {
                // Eliminar el archivo antiguo si existe
                if (!string.IsNullOrEmpty(proveedorExistente.SituacionFiscal))
                {
                    var oldFilePath = Path.Combine(_hostingEnvironment.WebRootPath, proveedorExistente.SituacionFiscal);
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }

                // Guardar el nuevo archivo
                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "situaciones-fiscales");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + updateProveedorDto.SituacionFiscal.FileName;
                var newFilePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    await updateProveedorDto.SituacionFiscal.CopyToAsync(stream);
                }
                proveedorExistente.SituacionFiscal = Path.Combine("uploads", "situaciones-fiscales", uniqueFileName).Replace('\\', '/');
            }

            var proveedorActualizado = new Proveedor
            {
                Id = id,
                Nombre = updateProveedorDto.Nombre ?? proveedorExistente.Nombre,
                RFC = updateProveedorDto.RFC ?? proveedorExistente.RFC,
                Activo = updateProveedorDto.Activo ?? proveedorExistente.Activo,
                Telefono = updateProveedorDto.Telefono ?? proveedorExistente.Telefono,
                Correo = updateProveedorDto.Correo ?? proveedorExistente.Correo,
                DireccionFiscal = updateProveedorDto.DireccionFiscal ?? proveedorExistente.DireccionFiscal,
                SituacionFiscal = proveedorExistente.SituacionFiscal,
                FechaRegistro = proveedorExistente.FechaRegistro,
                UsuarioRegistro = proveedorExistente.UsuarioRegistro
            };

            var resultado = await _proveedorRepository.UpdateAsync(proveedorActualizado);

            return new ProveedorDTO
            {
                Id = resultado.Id,
                Nombre = resultado.Nombre,
                RFC = resultado.RFC,
                Activo = resultado.Activo,
                Telefono = resultado.Telefono,
                Correo = resultado.Correo,
                DireccionFiscal = resultado.DireccionFiscal,
                SituacionFiscal = resultado.SituacionFiscal,
                FechaRegistro = resultado.FechaRegistro,
                UsuarioRegistro = resultado.UsuarioRegistro
            };
        }

        public async Task<bool> DeleteProveedorAsync(int id)
        {
            var proveedor = await _proveedorRepository.GetByIdAsync(id);
            if (proveedor != null && !string.IsNullOrEmpty(proveedor.SituacionFiscal))
            {
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, proveedor.SituacionFiscal);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            return await _proveedorRepository.DeleteAsync(id);
        }
    }
} 