using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppAPIEmpacadora.Models.Entities
{
    public class CajaCliente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Peso { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Precio { get; set; }

        public int IdCliente { get; set; }

        [ForeignKey("IdCliente")]
        public virtual Cliente Cliente { get; set; }
    }
} 