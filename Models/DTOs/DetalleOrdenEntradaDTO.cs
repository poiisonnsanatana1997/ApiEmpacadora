using AppAPIEmpacadora.Models.DTOs;

namespace AppAPIEmpacadora.Models.DTOs
{
    public class DetalleOrdenEntradaDTO
    {
        public OrdenEntradaDTO OrdenEntrada { get; set; }
        public List<TarimaDetalleDTO> Tarimas { get; set; }
    }

    public class TarimaDetalleDTO
    {
        public string Numero { get; set; }
        public string? CodigoOrden { get; set; }
        public decimal PesoBruto { get; set; }
        public decimal? PesoTara { get; set; }
        public decimal? PesoTarima { get; set; }
        public decimal? PesoPatin { get; set; }
        public decimal? PesoNeto { get; set; }
        public int CantidadCajas { get; set; }
        public decimal PesoPorCaja { get; set; }
        public string? Observaciones { get; set; }
    }
} 