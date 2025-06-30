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
    }
} 