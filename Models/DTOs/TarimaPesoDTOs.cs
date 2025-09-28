using System.ComponentModel.DataAnnotations;

namespace AppAPIEmpacadora.Models.DTOs
{
    public class TarimaPesoDiarioDTO
    {
        public DateTime Fecha { get; set; }
        public Dictionary<string, decimal> PesoPorTipo { get; set; } = new Dictionary<string, decimal>();
        public decimal PesoTotal { get; set; }
    }

    public class TarimaPesoPorTipoDTO
    {
        public string Tipo { get; set; } = string.Empty;
        public List<PesoDiarioDTO> PesosDiarios { get; set; } = new List<PesoDiarioDTO>();
        public decimal PesoTotalMes { get; set; }
        public decimal PesoPromedioDiario { get; set; }
    }

    public class PesoDiarioDTO
    {
        public DateTime Fecha { get; set; }
        public decimal Peso { get; set; }
        public string Etiqueta { get; set; } = string.Empty; // Para gráficas
    }

    public class TarimaDashboardDTO
    {
        // Total de tarimas del día
        public int TotalTarimasHoy { get; set; }
        
        // Eficiencia general
        public decimal EficienciaGeneral { get; set; }
        
        // Distribución por tipos
        public Dictionary<string, int> DistribucionPorTipos { get; set; } = new Dictionary<string, int>();
        
        // Análisis de eficiencia por tipo
        public Dictionary<string, decimal> EficienciaPorTipo { get; set; } = new Dictionary<string, decimal>();
    }

    public class TarimaEvolucionDTO
    {
        public DateTime Fecha { get; set; }
        public int TotalTarimas { get; set; }
        public decimal EficienciaGeneral { get; set; }
        public Dictionary<string, int> TarimasPorTipo { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> PesoPorTipo { get; set; } = new Dictionary<string, decimal>();
        public decimal PesoTotal { get; set; }
    }

    public class TarimaComparativaDTO
    {
        public string Periodo { get; set; } = string.Empty;
        public int TotalTarimas { get; set; }
        public decimal EficienciaGeneral { get; set; }
        public Dictionary<string, int> DistribucionPorTipos { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> EficienciaPorTipo { get; set; } = new Dictionary<string, decimal>();
    }
}
