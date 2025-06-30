using System.ComponentModel.DataAnnotations.Schema;

namespace AppAPIEmpacadora.Models.Entities
{
    public class TarimaClasificacion
    {
        public int IdTarima { get; set; }
        public int IdClasificacion { get; set; }
        public decimal Peso { get; set; }
        public string Tipo { get; set; }

        public Tarima Tarima { get; set; }
        public Clasificacion Clasificacion { get; set; }
    }
} 