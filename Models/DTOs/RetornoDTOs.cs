using System;
using System.ComponentModel.DataAnnotations;

namespace AppAPIEmpacadora.Models.DTOs
{
    public class CreateRetornoDTO
    {
        [Required]
        public string Numero { get; set; }
        [Required]
        public decimal Peso { get; set; }
        public string Observaciones { get; set; }
        [Required]
        public DateTime FechaRegistro { get; set; }
        [Required]
        public int IdClasificacion { get; set; }
    }

    public class UpdateRetornoDTO
    {
        public string? Numero { get; set; }
        public decimal? Peso { get; set; }
        public string? Observaciones { get; set; }
    }

    public class RetornoResponseDTO
    {
        public int Id { get; set; }
        public string Numero { get; set; }
        public decimal Peso { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public int IdClasificacion { get; set; }
    }
} 