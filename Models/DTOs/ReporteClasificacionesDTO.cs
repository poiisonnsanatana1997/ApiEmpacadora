namespace AppAPIEmpacadora.Models.DTOs
{
    public class ReporteClasificacionesDTO
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public ProveedorSimpleDTO Proveedor { get; set; }
        public decimal TotalMermas { get; set; }
        public decimal TotalRetornos { get; set; }
        public string FechaRecepcion { get; set; }
        public decimal PesoNetoRecibido { get; set; }
        public InformacionTipoDTO[] InformacionTipos { get; set; }
    }

    public class InformacionTipoDTO
    {
        public string Tipo { get; set; }
        public decimal Peso { get; set; }
        public decimal Precio { get; set; }
    }
}
