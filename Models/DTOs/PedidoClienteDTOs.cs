using System;
using System.ComponentModel.DataAnnotations;

namespace AppAPIEmpacadora.Models.DTOs
{
    public class CreatePedidoClienteDTO
    {
        [Required]
        public string Observaciones { get; set; }
        [Required]
        public string Estatus { get; set; }
        public DateTime? FechaEmbarque { get; set; }
        public int IdSucursal { get; set; }
        public int IdCliente { get; set; }
        [Required]
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; } = true;
    }

    public class UpdatePedidoClienteDTO
    {
        public string? Observaciones { get; set; }
        public string? Estatus { get; set; }
        public DateTime? FechaEmbarque { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool? Activo { get; set; }
    }

    public class PedidoClienteResponseDTO
    {
        public int Id { get; set; }
        public string Observaciones { get; set; }
        public string Estatus { get; set; }
        public DateTime? FechaEmbarque { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public bool Activo { get; set; }
        public int IdSucursal { get; set; }
        public int IdCliente { get; set; }
    }

    public class PedidoClienteConDetallesDTO
    {
        public int Id { get; set; }
        public string RazonSocialCliente { get; set; }
        public decimal PesoCajaCliente { get; set; }
    }
} 