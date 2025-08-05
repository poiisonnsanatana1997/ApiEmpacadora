using System.ComponentModel.DataAnnotations;

namespace AppAPIEmpacadora.Models.DTOs
{
    public class CreateOrdenPedidoClienteDTO
    {
        [Required]
        [StringLength(20)]
        public string Tipo { get; set; }

        public decimal? Cantidad { get; set; }

        public decimal? Peso { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        public int? IdProducto { get; set; }

        [Required]
        public int IdPedidoCliente { get; set; }
    }

    public class UpdateOrdenPedidoClienteDTO
    {
        [StringLength(20)]
        public string? Tipo { get; set; }

        public decimal? Cantidad { get; set; }

        public decimal? Peso { get; set; }

        public int? IdProducto { get; set; }
    }

    public class OrdenPedidoClienteResponseDTO
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal? Peso { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public ProductoSimpleDTO? Producto { get; set; }
    }

    public class OrdenPedidoClienteConDetallesDTO
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal? Peso { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public ProductoSimpleDTO? Producto { get; set; }
        public PedidoClienteSimpleDTO PedidoCliente { get; set; }
    }

    public class PedidoClienteSimpleDTO
    {
        public int Id { get; set; }
        public string? Observaciones { get; set; }
        public string Estatus { get; set; }
    }
} 