using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppAPIEmpacadora.Models.Entities
{
    public class OrdenPedidoCliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Tipo { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Cantidad { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Peso { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        [Required]
        [StringLength(50)]
        public string UsuarioRegistro { get; set; }

        public int? IdProducto { get; set; }

        [Required]
        public int IdPedidoCliente { get; set; }

        // Navegación
        public virtual Producto Producto { get; set; }
        public virtual PedidoCliente PedidoCliente { get; set; }
    }
} 