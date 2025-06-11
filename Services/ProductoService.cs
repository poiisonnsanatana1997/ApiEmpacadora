using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Repositories.Interfaces;

namespace AppAPIEmpacadora.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;

        public ProductoService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task<IEnumerable<ProductoSimpleDTO>> ObtenerTodosAsync()
        {
            return await _productoRepository.ObtenerTodosAsync();
        }

        public async Task<ProductoSimpleDTO> ObtenerPorIdAsync(int id)
        {
            return await _productoRepository.ObtenerPorIdAsync(id);
        }
    }
} 