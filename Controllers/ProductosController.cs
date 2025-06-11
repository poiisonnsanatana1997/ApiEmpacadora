using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AppAPIEmpacadora.Controllers
{
    [ApiController]
    [Route("api/productos")]
    [Authorize]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;
        private readonly IProductService _productService;

        public ProductosController(IProductoService productoService, IProductService productService)
        {
            _productoService = productoService;
            _productService = productService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductoSimpleDTO>>> Get()
        {
            var productos = await _productoService.ObtenerTodosAsync();
            return Ok(productos);
        }

        [HttpGet("detalle")]
        public async Task<ActionResult<IEnumerable<ProductoResponseDTO>>> GetDetalle()
        {
            var productos = await _productService.GetAllProductsAsync();
            return Ok(productos);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductoSimpleDTO>> GetById(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            return Ok(producto);
        }

        [HttpGet("{id}/detalle")]
        public async Task<ActionResult<ProductoResponseDTO>> GetDetalleById(int id)
        {
            try
            {
                var producto = await _productService.GetProductByIdAsync(id);
                return Ok(producto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProductoResponseDTO>> Crear([FromForm] CrearProductoDTO productoDto)
        {
            try
            {
                var usuarioRegistro = User.FindFirst(ClaimTypes.Name)?.Value ?? "sistema";
                var productoCreado = await _productService.CreateProductAsync(productoDto, usuarioRegistro);
                return CreatedAtAction(nameof(GetDetalleById), new { id = productoCreado.Id }, productoCreado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Supervisor")]
        public async Task<ActionResult<ProductoResponseDTO>> Actualizar(int id, [FromForm] ActualizarProductoDTO productoDto)
        {
            try
            {
                var usuarioModificacion = User.FindFirst(ClaimTypes.Name)?.Value ?? "sistema";
                var productoActualizado = await _productService.UpdateProductAsync(id, productoDto, usuarioModificacion);
                return Ok(productoActualizado);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var resultado = await _productService.DeleteProductAsync(id);
                if (resultado)
                    return NoContent();
                return NotFound($"Producto con ID {id} no encontrado.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
} 