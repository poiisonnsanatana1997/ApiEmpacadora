namespace AppAPIEmpacadora.Models.DTOs
{
    public class OrdenEntradaDTO
    {
        public string Codigo { get; set; }
        public ProveedorSimpleDTO Proveedor { get; set; }
        public ProductoSimpleDTO Producto { get; set; }
        public DateTime FechaEstimada { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaRecepcion { get; set; }
        public string UsuarioRegistro { get; set; }
        public string? UsuarioRecepcion { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }
    }
} 