using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AppAPIEmpacadora.Models.DTOs
{
    public class ClienteDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string RazonSocial { get; set; }
        public string Rfc { get; set; }
        public string? ConstanciaFiscal { get; set; }
        public string? RepresentanteComercial { get; set; }
        public string? TipoCliente { get; set; }
        public string Telefono { get; set; }
        public string? Correo { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }

        public ICollection<SucursalDTO> Sucursales { get; set; }
        public ICollection<CajaClienteDTO> CajasCliente { get; set; }
    }

    public class ClienteSummaryDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string RazonSocial { get; set; }
        public string Rfc { get; set; }
        public string Telefono { get; set; }
        public string? Correo { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
    }

    public class CreateClienteDTO
    {
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(50)]
        public string RazonSocial { get; set; }

        [Required]
        [StringLength(13)]
        public string Rfc { get; set; }

        public IFormFile? ConstanciaFiscal { get; set; }

        [StringLength(50)]
        public string? RepresentanteComercial { get; set; }

        [StringLength(50)]
        public string? TipoCliente { get; set; }

        [Required]
        [StringLength(20)]
        public string Telefono { get; set; }

        [StringLength(100)]
        public string? Correo { get; set; }

        [Required]
        public bool Activo { get; set; } = true;

        [Required]
        public DateTime FechaRegistro { get; set; }
    }

    public class UpdateClienteDTO
    {
        [StringLength(50)]
        public string? Nombre { get; set; }

        [StringLength(50)]
        public string? RazonSocial { get; set; }

        [StringLength(13)]
        public string? Rfc { get; set; }

        public IFormFile? ConstanciaFiscal { get; set; }

        [StringLength(50)]
        public string? RepresentanteComercial { get; set; }

        [StringLength(50)]
        public string? TipoCliente { get; set; }

        [StringLength(20)]
        public string? Telefono { get; set; }

        [StringLength(100)]
        public string? Correo { get; set; }

        public bool? Activo { get; set; }
    }
} 