using System.ComponentModel.DataAnnotations;

namespace AppAPIEmpacadora.Models.Entities
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Codigo { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(100)]
        public string Variedad { get; set; }

        [StringLength(20)]
        public string UnidadMedida { get; set; }

        public decimal Precio { get; set; }

        public bool Activo { get; set; }

        [StringLength(250)]
        public string Imagen { get; set; }

        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        [StringLength(50)]
        public string UsuarioRegistro { get; set; }

        [StringLength(50)]
        public string? UsuarioModificacion { get; set; }

        // Navegación
        public ICollection<ProductoPedido> ProductosPedido { get; set; }
    }
} 