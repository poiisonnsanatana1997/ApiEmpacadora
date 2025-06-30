using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppAPIEmpacadora.Models.Entities
{
    public class Clasificacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Lote { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PesoTotal { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        [Required]
        [StringLength(50)]
        public string UsuarioRegistro { get; set; }

        [ForeignKey("PedidoProveedor")]
        public int IdPedidoProveedor { get; set; }

        // Navigation properties
        public virtual PedidoProveedor PedidoProveedor { get; set; }
        public ICollection<TarimaClasificacion> TarimasClasificaciones { get; set; }
    }
}