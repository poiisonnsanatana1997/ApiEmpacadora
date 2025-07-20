using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppAPIEmpacadora.Models.Entities
{
    public class Tarima
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Codigo { get; set; }

        [Required]
        [StringLength(50)]
        public string Estatus { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        [Required]
        [StringLength(50)]
        public string UsuarioRegistro { get; set; }

        [StringLength(50)]
        public string? UsuarioModificacion { get; set; }
        
        [StringLength(200)]
        public string? Observaciones { get; set; }

        [StringLength(50)]
        public string? UPC { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Peso { get; set; }

        // Navigation properties
        public ICollection<TarimaClasificacion> TarimasClasificaciones { get; set; }
        public virtual ICollection<PedidoTarima> PedidoTarimas { get; set; }
    }
}