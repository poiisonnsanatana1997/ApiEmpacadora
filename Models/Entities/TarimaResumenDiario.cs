using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppAPIEmpacadora.Models.Entities
{
    public class TarimaResumenDiario
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [Column(TypeName = "date")]
        public DateTime Fecha { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Tipo { get; set; } // XL, L, M, S, Retorno
        
        [Required]
        public int CantidadTarimas { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CantidadTotal { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PesoTotal { get; set; }
        
        [Required]
        public int TarimasCompletas { get; set; }
        
        [Required]
        public int TarimasParciales { get; set; }
        
        [Required]
        public DateTime FechaRegistro { get; set; }
        
        [Required]
        [StringLength(50)]
        public string UsuarioRegistro { get; set; }
        
        [Required]
        public DateTime UltimaActualizacion { get; set; }
        
        [StringLength(50)]
        public string? UsuarioUltimaActualizacion { get; set; }
    }
}
