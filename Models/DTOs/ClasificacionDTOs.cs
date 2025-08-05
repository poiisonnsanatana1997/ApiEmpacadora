using System.ComponentModel.DataAnnotations;

namespace AppAPIEmpacadora.Models.DTOs
{
    public class ClasificacionDTO
    {
        public int Id { get; set; }
        public string Lote { get; set; }
        public decimal PesoTotal { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public int IdPedidoProveedor { get; set; }
        public decimal XL { get; set; }
        public decimal L { get; set; }
        public decimal M { get; set; }
        public decimal S { get; set; }
        public decimal Retornos { get; set; }
        public decimal PorcentajeClasificado { get; set; }
    }

    public class CreateClasificacionDTO
    {
        [Required]
        public int IdPedidoProveedor { get; set; }
    }

    public class UpdateClasificacionDTO
    {
        [StringLength(50)]
        public string? Lote { get; set; }
        public decimal? PesoTotal { get; set; }
        public int? IdPedidoProveedor { get; set; }
        public decimal? XL { get; set; }
        public decimal? L { get; set; }
        public decimal? M { get; set; }
        public decimal? S { get; set; }
        public decimal? Retornos { get; set; }
        public decimal? PorcentajeClasificado { get; set; }
    }

    public class AjustePesoClasificacionDTO
    {
        public decimal? XL { get; set; }
        public decimal? L { get; set; }
        public decimal? M { get; set; }
        public decimal? S { get; set; }
    }

    public class AjustePesoClasificacionResponseDTO
    {
        public int IdClasificacion { get; set; }
        public string Lote { get; set; }
        public bool AjusteRealizado { get; set; }
        public string Mensaje { get; set; }
    }
} 
