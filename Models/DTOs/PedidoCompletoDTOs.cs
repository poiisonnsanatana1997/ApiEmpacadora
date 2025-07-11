using System;
using System.Collections.Generic;

namespace AppAPIEmpacadora.Models.DTOs
{
    public class PedidoCompletoDTO
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Estado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaEstimada { get; set; }
        public DateTime? FechaRecepcion { get; set; }
        public string UsuarioRegistro { get; set; }
        public string? UsuarioRecepcion { get; set; }
        public string? Observaciones { get; set; }
        public ProveedorSimpleDTO Proveedor { get; set; }
        public ProductoSimpleDTO Producto { get; set; }
        public ICollection<ClasificacionCompletaDTO> Clasificaciones { get; set; }
    }

    public class ClasificacionCompletaDTO
    {
        public int Id { get; set; }
        public string Lote { get; set; }
        public decimal PesoTotal { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public decimal XL { get; set; }
        public decimal L { get; set; }
        public decimal M { get; set; }
        public decimal S { get; set; }
        public decimal Retornos { get; set; }
        public ICollection<MermaDetalleDTO> Mermas { get; set; }
        public ICollection<RetornoDetalleDTO> RetornosDetalle { get; set; }
        public ICollection<TarimaClasificacionDTO> TarimasClasificaciones { get; set; }
    }

    public class MermaDetalleDTO
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public decimal Peso { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
    }

    public class RetornoDetalleDTO
    {
        public int Id { get; set; }
        public string Numero { get; set; }
        public decimal Peso { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
    }

    public class TarimaClasificacionDTO
    {
        public int IdTarima { get; set; }
        public int IdClasificacion { get; set; }
        public decimal Peso { get; set; }
        public string Tipo { get; set; }
        public TarimaDTO Tarima { get; set; }
    }
} 