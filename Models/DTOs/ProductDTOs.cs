using System.ComponentModel.DataAnnotations;

namespace AppAPIEmpacadora.Models.DTOs
{
    public class ProductoSimpleDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class ProductoResponseDTO
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Variedad { get; set; }
        public string UnidadMedida { get; set; }
        public decimal Precio { get; set; }
        public string Estatus { get; set; }
        public string Imagen { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string UsuarioRegistro { get; set; }
        public string UsuarioModificacion { get; set; }
    }

    public class CrearProductoDTO
    {
        [Required(ErrorMessage = "El código es obligatorio")]
        [StringLength(50, ErrorMessage = "El código no puede tener más de 50 caracteres")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La variedad es obligatoria")]
        [StringLength(100, ErrorMessage = "La variedad no puede tener más de 100 caracteres")]
        public string Variedad { get; set; }

        [Required(ErrorMessage = "La unidad de medida es obligatoria")]
        [StringLength(20, ErrorMessage = "La unidad de medida no puede tener más de 20 caracteres")]
        public string UnidadMedida { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        public decimal Precio { get; set; }

        public IFormFile? Imagen { get; set; }
    }

    public class ActualizarProductoDTO
    {
        [Required(ErrorMessage = "El código es obligatorio")]
        [StringLength(50, ErrorMessage = "El código no puede tener más de 50 caracteres")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La variedad es obligatoria")]
        [StringLength(100, ErrorMessage = "La variedad no puede tener más de 100 caracteres")]
        public string Variedad { get; set; }

        [Required(ErrorMessage = "La unidad de medida es obligatoria")]
        [StringLength(20, ErrorMessage = "La unidad de medida no puede tener más de 20 caracteres")]
        public string UnidadMedida { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El estatus es obligatorio")]
        [StringLength(25, ErrorMessage = "El estatus no puede tener más de 25 caracteres")]
        public string Estatus { get; set; }

        public IFormFile? Imagen { get; set; }
    }
} 