using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Repositories.Interfaces;
using AppAPIEmpacadora.Services.Interfaces;
using System.Linq;

namespace AppAPIEmpacadora.Services
{
    public class OrdenPedidoClienteService : IOrdenPedidoClienteService
    {
        private readonly IOrdenPedidoClienteRepository _repo;
        
        public OrdenPedidoClienteService(IOrdenPedidoClienteRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<OrdenPedidoClienteResponseDTO>> ObtenerTodosAsync()
        {
            var list = await _repo.ObtenerTodosAsync();
            var result = new List<OrdenPedidoClienteResponseDTO>();
            foreach (var item in list)
            {
                result.Add(MapToResponseDTO(item));
            }
            return result;
        }

        public async Task<OrdenPedidoClienteResponseDTO> ObtenerPorIdAsync(int id)
        {
            var entity = await _repo.ObtenerPorIdAsync(id);
            return entity == null ? null : MapToResponseDTO(entity);
        }

        public async Task<OrdenPedidoClienteResponseDTO> CrearAsync(CreateOrdenPedidoClienteDTO dto, string usuario)
        {
            var entity = new OrdenPedidoCliente
            {
                Tipo = dto.Tipo,
                Cantidad = dto.Cantidad,
                Peso = dto.Peso,
                FechaRegistro = dto.FechaRegistro,
                UsuarioRegistro = usuario,
                IdProducto = dto.IdProducto,
                IdPedidoCliente = dto.IdPedidoCliente
            };
            var created = await _repo.CrearAsync(entity);
            return MapToResponseDTO(created);
        }

        public async Task<bool> ActualizarAsync(int id, UpdateOrdenPedidoClienteDTO dto)
        {
            var entity = await _repo.ObtenerPorIdAsync(id);
            if (entity == null) return false;
            
            if (dto.Tipo != null) entity.Tipo = dto.Tipo;
            if (dto.Cantidad.HasValue) entity.Cantidad = dto.Cantidad;
            if (dto.Peso.HasValue) entity.Peso = dto.Peso;
            if (dto.IdProducto.HasValue) entity.IdProducto = dto.IdProducto;
            
            return await _repo.ActualizarAsync(entity);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            return await _repo.EliminarAsync(id);
        }

        public async Task<IEnumerable<OrdenPedidoClienteConDetallesDTO>> ObtenerTodosConDetallesAsync()
        {
            var list = await _repo.ObtenerTodosConDetallesAsync();
            var result = new List<OrdenPedidoClienteConDetallesDTO>();
            foreach (var item in list)
            {
                result.Add(MapToConDetallesDTO(item));
            }
            return result;
        }

        public async Task<IEnumerable<OrdenPedidoClienteResponseDTO>> ObtenerPorPedidoClienteAsync(int idPedidoCliente)
        {
            var list = await _repo.ObtenerPorPedidoClienteAsync(idPedidoCliente);
            var result = new List<OrdenPedidoClienteResponseDTO>();
            foreach (var item in list)
            {
                result.Add(MapToResponseDTO(item));
            }
            return result;
        }

        private OrdenPedidoClienteResponseDTO MapToResponseDTO(OrdenPedidoCliente entity)
        {
            return new OrdenPedidoClienteResponseDTO
            {
                Id = entity.Id,
                Tipo = entity.Tipo,
                Cantidad = entity.Cantidad,
                Peso = entity.Peso,
                FechaRegistro = entity.FechaRegistro,
                UsuarioRegistro = entity.UsuarioRegistro,
                Producto = entity.Producto != null ? new ProductoSimpleDTO
                {
                    Id = entity.Producto.Id,
                    Codigo = entity.Producto.Codigo,
                    Nombre = entity.Producto.Nombre,
                    Variedad = entity.Producto.Variedad
                } : null
            };
        }

        private OrdenPedidoClienteConDetallesDTO MapToConDetallesDTO(OrdenPedidoCliente entity)
        {
            return new OrdenPedidoClienteConDetallesDTO
            {
                Id = entity.Id,
                Tipo = entity.Tipo,
                Cantidad = entity.Cantidad,
                Peso = entity.Peso,
                FechaRegistro = entity.FechaRegistro,
                UsuarioRegistro = entity.UsuarioRegistro,
                Producto = entity.Producto != null ? new ProductoSimpleDTO
                {
                    Id = entity.Producto.Id,
                    Nombre = entity.Producto.Nombre,
                    Codigo = entity.Producto.Codigo
                } : null,
                PedidoCliente = new PedidoClienteSimpleDTO
                {
                    Id = entity.PedidoCliente.Id,
                    Observaciones = entity.PedidoCliente.Observaciones,
                    Estatus = entity.PedidoCliente.Estatus
                }
            };
        }
    }
} 