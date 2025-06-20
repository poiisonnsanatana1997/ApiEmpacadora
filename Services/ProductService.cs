using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Repositories.Interfaces;
using AppAPIEmpacadora.Models.DTOs;

namespace AppAPIEmpacadora.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IWebHostEnvironment _environment;

        public ProductService(IProductRepository productRepository, IWebHostEnvironment environment)
        {
            _productRepository = productRepository;
            _environment = environment;
        }

        public async Task<IEnumerable<ProductoResponseDTO>> GetAllProductsAsync()
        {
            var productos = await _productRepository.GetAllAsync();
            return productos.Select(MapToProductoResponseDTO);
        }

        public async Task<ProductoResponseDTO> GetProductByIdAsync(int id)
        {
            var producto = await _productRepository.GetByIdAsync(id);
            if (producto == null)
                throw new KeyNotFoundException($"Producto con ID {id} no encontrado.");
            return MapToProductoResponseDTO(producto);
        }

        public async Task<ProductoResponseDTO> CreateProductAsync(CrearProductoDTO productoDto, string usuarioRegistro)
        {
            var producto = new Producto
            {
                Codigo = productoDto.Codigo,
                Nombre = productoDto.Nombre,
                Variedad = productoDto.Variedad,
                UnidadMedida = productoDto.UnidadMedida,
                Precio = productoDto.Precio,
                FechaRegistro = productoDto.Fecha,
                Activo = true,
                Imagen = null, // Se asigna después si hay imagen
                UsuarioRegistro = usuarioRegistro
            };

            if (productoDto.Imagen != null)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + productoDto.Imagen.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await productoDto.Imagen.CopyToAsync(fileStream);
                }

                producto.Imagen = "/uploads/" + uniqueFileName;
            }

            var createdProducto = await _productRepository.CreateAsync(producto);
            return MapToProductoResponseDTO(createdProducto);
        }

        public async Task<ProductoResponseDTO> UpdateProductAsync(int id, ActualizarProductoDTO productoDto, string usuarioModificacion)
        {
            if (!await _productRepository.ExistsAsync(id))
                throw new KeyNotFoundException($"Producto con ID {id} no encontrado.");

            var productoExistente = await _productRepository.GetByIdAsync(id);
            if (productoExistente == null)
                throw new KeyNotFoundException($"Producto con ID {id} no encontrado.");

            productoExistente.Codigo = productoDto.Codigo;
            productoExistente.Nombre = productoDto.Nombre;
            productoExistente.Variedad = productoDto.Variedad;
            productoExistente.UnidadMedida = productoDto.UnidadMedida;
            productoExistente.Precio = productoDto.Precio;
            productoExistente.Activo = productoDto.Activo;
            productoExistente.FechaActualizacion = DateTime.UtcNow;
            productoExistente.UsuarioModificacion = usuarioModificacion;

            if (productoDto.Imagen != null)
            {
                if (!string.IsNullOrEmpty(productoExistente.Imagen))
                {
                    string oldFilePath = Path.Combine(_environment.WebRootPath, productoExistente.Imagen.TrimStart('/'));
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }

                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + productoDto.Imagen.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await productoDto.Imagen.CopyToAsync(fileStream);
                }

                productoExistente.Imagen = "/uploads/" + uniqueFileName;
            }

            var updatedProducto = await _productRepository.UpdateAsync(productoExistente);
            return MapToProductoResponseDTO(updatedProducto);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            if (!await _productRepository.ExistsAsync(id))
                throw new KeyNotFoundException($"Producto con ID {id} no encontrado.");

            var producto = await _productRepository.GetByIdAsync(id);
            if (!string.IsNullOrEmpty(producto.Imagen))
            {
                string filePath = Path.Combine(_environment.WebRootPath, producto.Imagen.TrimStart('/'));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            return await _productRepository.DeleteAsync(id);
        }

        private ProductoResponseDTO MapToProductoResponseDTO(Producto producto)
        {
            string imagenBase64 = null;
            if (!string.IsNullOrEmpty(producto.Imagen))
            {
                string filePath = Path.Combine(_environment.WebRootPath, producto.Imagen.TrimStart('/'));
                if (File.Exists(filePath))
                {
                    byte[] imageBytes = File.ReadAllBytes(filePath);
                    imagenBase64 = Convert.ToBase64String(imageBytes);
                }
            }

            return new ProductoResponseDTO
            {
                Id = producto.Id,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                Variedad = producto.Variedad,
                UnidadMedida = producto.UnidadMedida,
                Precio = producto.Precio,
                Activo = producto.Activo,
                Imagen = imagenBase64,
                FechaRegistro = producto.FechaRegistro,
                FechaActualizacion = producto.FechaActualizacion,
                UsuarioRegistro = producto.UsuarioRegistro,
                UsuarioModificacion = producto.UsuarioModificacion
            };
        }
    }
} 