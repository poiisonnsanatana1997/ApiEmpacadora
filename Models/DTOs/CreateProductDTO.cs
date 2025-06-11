using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AppAPIEmpacadora.Models.DTOs
{
    public class CreateProductDTO
    {
        [Required(ErrorMessage = "El código es obligatorio")]
        [StringLength(50, ErrorMessage = "El código no puede tener más de 50 caracteres")]
        public string Code { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Name { get; set; }

        [StringLength(50, ErrorMessage = "El tamaño no puede tener más de 50 caracteres")]
        public string Size { get; set; }

        [StringLength(50, ErrorMessage = "El tipo de empaque no puede tener más de 50 caracteres")]
        public string PackagingType { get; set; }

        [StringLength(20, ErrorMessage = "La unidad no puede tener más de 20 caracteres")]
        public string Unit { get; set; }

        [Required(ErrorMessage = "La variedad es obligatoria")]
        [StringLength(50, ErrorMessage = "La variedad no puede tener más de 50 caracteres")]
        public string Variety { get; set; }

        public string Data1 { get; set; }
        public string Data2 { get; set; }

        public IFormFile? Image { get; set; }
    }
} 