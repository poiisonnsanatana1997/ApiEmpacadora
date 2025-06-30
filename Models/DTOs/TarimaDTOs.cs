using System.ComponentModel.DataAnnotations;

namespace AppAPIEmpacadora.Models.DTOs
{
    public class TarimaDTO
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Estatus { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string UsuarioRegistro { get; set; }
        public string? UsuarioModificacion { get; set; }
        public string Tipo { get; set; }
        public decimal Cantidad { get; set; }
    }

    public class CreateTarimaDTO
    {
        [Required]
        [StringLength(50)]
        public string Codigo { get; set; }

        [Required]
        [StringLength(50)]
        public string Estatus { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        [Required]
        [StringLength(20)]
        public string Tipo { get; set; }

        [Required]
        public decimal Cantidad { get; set; }

        [Required]
        public int IdClasificacion { get; set; }

        [Required]
        public int IdPedidoCliente { get; set; }
    }

    public class UpdateTarimaDTO
    {
        [StringLength(50)]
        public string? Codigo { get; set; }

        [StringLength(50)]
        public string? Estatus { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        [StringLength(20)]
        public string? Tipo { get; set; }

        public decimal? Cantidad { get; set; }
    }
} 