namespace AppAPIEmpacadora.Models.DTOs
{
    public class CrearOrdenEntradaDTO
    {
        public int ProveedorId { get; set; }
        public int ProductoId { get; set; }
        public DateTime FechaEstimada { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }
    }
} 