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
        public string UsuarioModificacion { get; set; }

        [Required]
        [StringLength(20)]
        public string Tipo { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cantidad { get; set; }

        // Navigation properties
        public ICollection<TarimaClasificacion> TarimasClasificaciones { get; set; }
        public virtual ICollection<PedidoTarima> PedidoTarimas { get; set; }
    }
}