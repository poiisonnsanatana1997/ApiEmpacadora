namespace AppAPIEmpacadora.Models.DTOs
{
    public class ActualizarPesajeTarimaDTO
    {
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