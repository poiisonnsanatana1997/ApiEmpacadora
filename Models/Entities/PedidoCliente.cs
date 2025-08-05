using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppAPIEmpacadora.Models.Entities
{
    public class PedidoCliente
    {
        public int Id { get; set; }
        public string? Observaciones { get; set; }
        public string Estatus { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PorcentajeSurtido { get; set; }
        public DateTime? FechaEmbarque { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public string? UsuarioModificacion { get; set; }
        public bool Activo { get; set; }
        public int IdSucursal { get; set; }
        public int IdCliente { get; set; }

        // Navegación
        public virtual Sucursal Sucursal { get; set; }
        public virtual Cliente Cliente { get; set; }
        public virtual ICollection<PedidoTarima> PedidoTarimas { get; set; }
        public virtual ICollection<OrdenPedidoCliente> OrdenesPedidoCliente { get; set; }
    }
} 