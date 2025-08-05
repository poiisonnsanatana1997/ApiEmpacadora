using System.ComponentModel.DataAnnotations;

namespace AppAPIEmpacadora.Models.DTOs
{
    public class CajaDTO
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal? Peso { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public int? IdClasificacion { get; set; }
    }

    public class CajaSummaryDTO
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal? Peso { get; set; }
        public DateTime FechaRegistro { get; set; }
    }

    public class CreateCajaDTO
    {
        [Required]
        [StringLength(20)]
        public string Tipo { get; set; }

        public decimal? Cantidad { get; set; }

        public decimal? Peso { get; set; }

        public int? IdClasificacion { get; set; }
    }

    public class UpdateCajaDTO
    {
        [StringLength(20)]
        public string? Tipo { get; set; }

        public decimal? Cantidad { get; set; }

        public decimal? Peso { get; set; }

        public int? IdClasificacion { get; set; }
    }

    public class AjustarCantidadCajaDTO
    {
        [Required]
        [StringLength(20)]
        public string Tipo { get; set; }
        
        [Required]
        public decimal CantidadAjuste { get; set; }
        
        public decimal? PesoAjuste { get; set; }
        
        public int? IdClasificacion { get; set; }
    }
} 