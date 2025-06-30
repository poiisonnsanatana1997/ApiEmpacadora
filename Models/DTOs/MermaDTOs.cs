using System;
using System.ComponentModel.DataAnnotations;

namespace AppAPIEmpacadora.Models.DTOs
{
    public class CreateMermaDTO
    {
        [Required]
        public string Tipo { get; set; }
        [Required]
        public decimal Peso { get; set; }
        public string Observaciones { get; set; }
        [Required]
        public DateTime FechaRegistro { get; set; }
        [Required]
        public int IdClasificacion { get; set; }
    }

    public class UpdateMermaDTO
    {
        public string? Tipo { get; set; }
        public decimal? Peso { get; set; }
        public string? Observaciones { get; set; }
    }

    public class MermaResponseDTO
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public decimal Peso { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public int IdClasificacion { get; set; }
    }
} 