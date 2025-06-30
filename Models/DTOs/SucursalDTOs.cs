using System.ComponentModel.DataAnnotations;

namespace AppAPIEmpacadora.Models.DTOs
{
    public class SucursalDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string? EncargadoAlmacen { get; set; }
        public string Telefono { get; set; }
        public string? Correo { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public int IdCliente { get; set; }
    }

    public class CreateSucursalDTO
    {
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

        public bool Activo { get; set; } = true;

        [Required]
        public int IdCliente { get; set; }
    }

    public class UpdateSucursalDTO
    {
        [StringLength(150)]
        public string? Nombre { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }

        [StringLength(50)]
        public string? EncargadoAlmacen { get; set; }

        [StringLength(20)]
        public string? Telefono { get; set; }

        [StringLength(100)]
        public string? Correo { get; set; }

        public bool? Activo { get; set; }
        
        public int? IdCliente { get; set; }
    }
} 