using Microsoft.AspNetCore.Mvc;
using AppAPIEmpacadora.Models.DTOs;

[ApiController]
[Route("api/proveedores")]
public class ProveedoresController : ControllerBase
{
    private readonly IProveedorService _proveedorService;

    public ProveedoresController(IProveedorService proveedorService)
    {
        _proveedorService = proveedorService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProveedorSimpleDTO>>> Get()
    {
        var proveedores = await _proveedorService.ObtenerTodosAsync();
        return Ok(proveedores);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProveedorSimpleDTO>> GetById(int id)
    {
        var proveedor = await _proveedorService.ObtenerPorIdAsync(id);
        if (proveedor == null)
        {
            return NotFound();
        }
        return Ok(proveedor);
    }
} 