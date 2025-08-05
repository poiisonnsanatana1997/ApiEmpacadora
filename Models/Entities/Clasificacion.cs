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

        [Column(TypeName = "decimal(18,2)")]
        public decimal PorcentajeClasificado { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        [Required]
        [StringLength(50)]
        public string UsuarioRegistro { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal XL { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal L { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal M { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal S { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Retornos { get; set; }

        [ForeignKey("PedidoProveedor")]
        public int IdPedidoProveedor { get; set; }

        // Navigation properties
        public virtual PedidoProveedor PedidoProveedor { get; set; }
        public ICollection<TarimaClasificacion> TarimasClasificaciones { get; set; }
        public ICollection<Merma> Mermas { get; set; }
        public ICollection<Retorno> RetornosDetalle { get; set; }
    }
}