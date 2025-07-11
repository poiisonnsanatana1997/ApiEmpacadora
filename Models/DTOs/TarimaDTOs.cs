using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AppAPIEmpacadora.Models.DTOs
{
    public class TarimaDTO
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Estatus { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string UsuarioRegistro { get; set; }
        public string? UsuarioModificacion { get; set; }
        public decimal Cantidad { get; set; }
        public string? Observaciones { get; set; }
        public string? UPC { get; set; }
        public decimal? Peso { get; set; }
    }

    public class CreateTarimaDTO
    {
        [Required]
        [StringLength(50)]
        public string Estatus { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        [Required]
        [StringLength(20)]
        public string Tipo { get; set; }

        [Required]
        public decimal Cantidad { get; set; }

        [StringLength(200)]
        public string? Observaciones { get; set; }

        [StringLength(50)]
        public string? UPC { get; set; }

        public decimal? Peso { get; set; }

        public int? IdClasificacion { get; set; }

        public int? IdPedidoCliente { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad de tarimas debe ser mayor a 0")]
        public int CantidadTarimas { get; set; }
    }

    public class UpdateTarimaDTO
    {
        [StringLength(50)]
        public string? Codigo { get; set; }

        [StringLength(50)]
        public string? Estatus { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        [StringLength(20)]
        public string? Tipo { get; set; }

        public decimal? Cantidad { get; set; }

        [StringLength(200)]
        public string? Observaciones { get; set; }

        [StringLength(50)]
        public string? UPC { get; set; }

        public decimal? Peso { get; set; }
    }

    public class CreateTarimaResponseDTO
    {
        public List<TarimaDTO> TarimasCreadas { get; set; }
        public int CantidadCreada { get; set; }
        public string Mensaje { get; set; }
    }

    public class PedidoTarimaDTO
    {
        public int IdPedidoCliente { get; set; }
        public string Observaciones { get; set; }
        public string Estatus { get; set; }
        public DateTime? FechaEmbarque { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public string NombreCliente { get; set; }
        public string NombreSucursal { get; set; }
    }

    public class TarimaClasificacionParcialDTO
    {
        public int IdClasificacion { get; set; }
        public string Lote { get; set; }
        public decimal Peso { get; set; }
        public string Tipo { get; set; }
        public decimal PesoTotal { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
    }

    public class TarimaParcialCompletaDTO
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Estatus { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string UsuarioRegistro { get; set; }
        public string? UsuarioModificacion { get; set; }
        public decimal Cantidad { get; set; }
        public string? Observaciones { get; set; }
        public string? UPC { get; set; }
        public decimal? Peso { get; set; }
        public List<TarimaClasificacionParcialDTO> TarimasClasificaciones { get; set; } = new List<TarimaClasificacionParcialDTO>();
        public List<PedidoTarimaDTO> PedidoTarimas { get; set; } = new List<PedidoTarimaDTO>();
    }

    public class TarimaUpdateParcialDTO{
        [Required(ErrorMessage = "El estatus es requerido")]
        [StringLength(50, ErrorMessage = "El estatus no puede exceder 50 caracteres")]
        public string Estatus { get; set; }
        
        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal Cantidad { get; set; }
        
        [Required(ErrorMessage = "El ID de la tarima es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID de la tarima debe ser mayor a 0")]
        public int IdTarima { get; set; }
        
        [Required(ErrorMessage = "El ID de la clasificación es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID de la clasificación debe ser mayor a 0")]
        public int IdClasificacion { get; set; }
        
        [Required(ErrorMessage = "El tipo es requerido")]
        [StringLength(50, ErrorMessage = "El tipo no puede exceder 50 caracteres")]
        public string Tipo { get; set; }
    }
} 