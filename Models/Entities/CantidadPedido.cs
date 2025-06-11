using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppAPIEmpacadora.Models.Entities
{
    public class CantidadPedido
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Codigo { get; set; }

        [Required]
        public int CantidadCajas { get; set; }

        [Required]
        public decimal PesoPorCaja { get; set; }

        [Required]
        public decimal PesoBruto { get; set; }

        public decimal? PesoTara { get; set; }
        public decimal? PesoTarima { get; set; }
        public decimal? PesoPatin { get; set; }
        public decimal? PesoNeto { get; set; }

        [StringLength(300)]
        public string? Observaciones { get; set; }

        [ForeignKey("PedidoProveedor")]
        public int IdPedidoProveedor { get; set; }
        public PedidoProveedor PedidoProveedor { get; set; }
    }
} 