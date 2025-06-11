using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPIEmpacadora.Models.DTOs;

namespace AppAPIEmpacadora.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductoResponseDTO>> GetAllProductsAsync();
        Task<ProductoResponseDTO> GetProductByIdAsync(int id);
        Task<ProductoResponseDTO> CreateProductAsync(CrearProductoDTO productoDto, string usuarioRegistro);
        Task<ProductoResponseDTO> UpdateProductAsync(int id, ActualizarProductoDTO productoDto, string usuarioModificacion);
        Task<bool> DeleteProductAsync(int id);
    }
} 