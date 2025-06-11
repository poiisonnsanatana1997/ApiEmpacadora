using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppAPIEmpacadora.Models.Entities
{
    public class Proveedor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(13)]
        public string RFC { get; set; }

        [StringLength(25)]
        public string Estatus { get; set; }

        [StringLength(20)]
        public string Telefono { get; set; }

        [StringLength(100)]
        public string Correo { get; set; }

        [StringLength(200)]
        public string DireccionFiscal { get; set; }

        [StringLength(250)]
        public string SituacionFiscal { get; set; }

        public DateTime FechaRegistro { get; set; }

        [StringLength(50)]
        public string UsuarioRegistro { get; set; }

        // Navegación
        public ICollection<PedidoProveedor> PedidosProveedor { get; set; }
    }
} 