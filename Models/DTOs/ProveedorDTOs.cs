using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AppAPIEmpacadora.Models.DTOs
{
    public class ProveedorDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string RFC { get; set; }
        public bool Activo { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string DireccionFiscal { get; set; }
        public string SituacionFiscal { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
    }

    public class CreateProveedorDTO
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(13)]
        public string? RFC { get; set; }

        public bool Activo { get; set; } = true;

        [StringLength(20)]
        public string? Telefono { get; set; }

        [StringLength(100)]
        public string? Correo { get; set; }

        [StringLength(200)]
        public string? DireccionFiscal { get; set; }

        public IFormFile? SituacionFiscal { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }
    }

    public class UpdateProveedorDTO
    {
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(13)]
        public string? RFC { get; set; }

        public bool? Activo { get; set; }

        [StringLength(20)]
        public string? Telefono { get; set; }

        [StringLength(100)]
        public string? Correo { get; set; }

        [StringLength(200)]
        public string? DireccionFiscal { get; set; }

        public IFormFile? SituacionFiscal { get; set; }
    }
} 