using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppAPIEmpacadora.Models.Entities
{
    public class Sucursal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(200)]
        public string Direccion { get; set; }

        [StringLength(50)]
        public string? EncargadoAlmacen { get; set; }

        [Required]
        [StringLength(20)]
        public string Telefono { get; set; }

        [StringLength(100)]
        public string? Correo { get; set; }

        public bool Activo { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        [Required]
        [StringLength(50)]
        public string UsuarioRegistro { get; set; }

        [ForeignKey("Cliente")]
        public int IdCliente { get; set; }

        // Navigation property
        public virtual Cliente Cliente { get; set; }
    }
} 