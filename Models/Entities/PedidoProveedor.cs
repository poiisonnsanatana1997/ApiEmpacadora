using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppAPIEmpacadora.Models.Entities
{
    public class PedidoProveedor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Codigo { get; set; }

        [Required]
        [StringLength(50)]
        public string Estado { get; set; }

        [StringLength(300)]
        public string Observaciones { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        [Required]
        public DateTime FechaEstimada { get; set; }

        public DateTime? FechaRecepcion { get; set; }

        [Required]
        [StringLength(50)]
        public string UsuarioRegistro { get; set; }

        [StringLength(50)]
        public string? UsuarioRecepcion { get; set; }

        [ForeignKey("Proveedor")]
        public int IdProveedor { get; set; }
        public Proveedor Proveedor { get; set; }

        // Navegación
        public ICollection<ProductoPedido> ProductosPedido { get; set; }
        public ICollection<CantidadPedido> CantidadesPedido { get; set; }
        public ICollection<Clasificacion> Clasificaciones { get; set; }
    }
} 