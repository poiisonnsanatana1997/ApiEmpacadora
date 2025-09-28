using System.ComponentModel.DataAnnotations;

namespace AppAPIEmpacadora.Models.DTOs
{
    public class CajaClienteDTO
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public decimal Peso { get; set; }
        public decimal? Precio { get; set; }
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; }
    }

    public class CreateCajaClienteDTO
    {
        [StringLength(50)]
        public string? Nombre { get; set; }

        [Required]
        public decimal Peso { get; set; }

        public decimal? Precio { get; set; }

        [Required]
        public int IdCliente { get; set; }
    }

    public class UpdateCajaClienteDTO
    {
        [StringLength(50)]
        public string? Nombre { get; set; }
        public decimal? Peso { get; set; }
        public decimal? Precio { get; set; }
        public int? IdCliente { get; set; }
    }
} 