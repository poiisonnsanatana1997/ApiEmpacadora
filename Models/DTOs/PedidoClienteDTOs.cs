using System.ComponentModel.DataAnnotations;
using AppAPIEmpacadora.Models.Entities;

namespace AppAPIEmpacadora.Models.DTOs
{
    public class CreatePedidoClienteDTO
    {
        public string? Observaciones { get; set; }
        [Required]
        public string Estatus { get; set; }
        public DateTime? FechaEmbarque { get; set; }
        public int IdSucursal { get; set; }
        public int IdCliente { get; set; }
        [Required]
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; } = true;
        public decimal PorcentajeSurtido { get; set; } = 0;
    }

    public class UpdatePedidoClienteDTO
    {
        public string? Observaciones { get; set; }
        public string? Estatus { get; set; }
        public DateTime? FechaEmbarque { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool? Activo { get; set; }
        public decimal? PorcentajeSurtido { get; set; }
    }

    public class PedidoClienteResponseDTO
    {
        public int Id { get; set; }
        public string? Observaciones { get; set; }
        public string Estatus { get; set; }
        public DateTime? FechaEmbarque { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public bool Activo { get; set; }
        public string Sucursal { get; set; }
        public string Cliente { get; set; }
        public decimal PorcentajeSurtido { get; set; }
    }

    public class PedidoClienteConDetallesDTO
    {
        public int Id { get; set; }
        public string RazonSocialCliente { get; set; }
        public decimal PesoCajaCliente { get; set; }
    }

    // Nuevos DTOs para crear pedido con múltiples órdenes
    public class CreatePedidoClienteConOrdenesDTO
    {
        // Campos de CreatePedidoClienteDTO
        public string? Observaciones { get; set; }
        
        [Required] 
        public string Estatus { get; set; }
        
        public DateTime? FechaEmbarque { get; set; }
        
        public int IdSucursal { get; set; }
        
        public int IdCliente { get; set; }
        
        [Required] 
        public DateTime FechaRegistro { get; set; }
        
        public bool Activo { get; set; } = true;
        
        // Lista de órdenes (sin IdPedidoCliente ya que se asignará automáticamente)
        public List<CreateOrdenItemDTO> Ordenes { get; set; } = new List<CreateOrdenItemDTO>();
    }

    public class CreateOrdenItemDTO
    {
        [Required] 
        [StringLength(20)] 
        public string Tipo { get; set; }
        
        public decimal? Cantidad { get; set; }
        
        public decimal? Peso { get; set; }
        
        public int? IdProducto { get; set; }
    }

    public class PedidoClienteConOrdenesResponseDTO
    {
        public int Id { get; set; }
        public string? Observaciones { get; set; }
        public string Estatus { get; set; }
        public DateTime? FechaEmbarque { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public bool Activo { get; set; }
        public string Sucursal { get; set; }
        public string Cliente { get; set; }
        
        // Listado de órdenes creadas
        public List<OrdenPedidoClienteResponseDTO> Ordenes { get; set; } = new List<OrdenPedidoClienteResponseDTO>();
    }

    public class UpdateEstatusPedidoClienteDTO
    {
        [Required]
        public string Estatus { get; set; }
    }

    public class PedidoClientePorAsignarDTO
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public int Cantidad { get; set; }
        public decimal Peso { get; set; }
        public decimal PesoCajaCliente { get; set; }
        public ProductoSimpleDTO Producto { get; set; }
        public ClienteSummaryDTO Cliente { get; set; }
        public SucursalSummaryDTO Sucursal { get; set; }
    }

    public class PedidoClienteProgresoDTO{
        public int Id { get; set; }
        public string Estatus { get; set; }
        public decimal PorcentajeSurtido { get; set; }
        public string? Observaciones { get; set; }
        public List<OrdenPedidoClienteResponseDTO> Ordenes { get; set; } = new List<OrdenPedidoClienteResponseDTO>();
        public List<TarimaProgresoDTO> Tarimas { get; set; } = new List<TarimaProgresoDTO>();
    }

    public class TarimaProgresoDTO
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Estatus { get; set; }
        public string? Observaciones { get; set; }
        public string? UPC { get; set; }
        public decimal? Peso { get; set; }
        public List<TarimaClasificacionProgresoDTO> TarimasClasificaciones { get; set; } = new List<TarimaClasificacionProgresoDTO>();
    }

    public class TarimaClasificacionProgresoDTO
    {
        public string Tipo { get; set; }
        public decimal? Peso { get; set; }
        public decimal? Cantidad { get; set; }
        public ProductoSimpleDTO? Producto { get; set; }
    }

    public class DesasignacionTarimaDTO
    {
        public int IdPedido { get; set; }
        public int IdTarima { get; set; }
    }


} 