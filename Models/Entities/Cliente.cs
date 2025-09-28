using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AppAPIEmpacadora.Models.Entities;

namespace AppAPIEmpacadora.Models.Entities
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(50)]
        public string RazonSocial { get; set; }

        [Required]
        [StringLength(13)]
        public string Rfc { get; set; }

        [StringLength(250)]
        public string? ConstanciaFiscal { get; set; }

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
        public bool Activo { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        [Required]
        [StringLength(50)]
        public string UsuarioRegistro { get; set; }

        // Navigation properties
        public ICollection<Sucursal> Sucursales { get; set; }
        public ICollection<CajaCliente> CajasCliente { get; set; }
    }
}