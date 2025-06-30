using System;

namespace AppAPIEmpacadora.Models.Entities
{
    public class Retorno
    {
        public int Id { get; set; }
        public string Numero { get; set; }
        public decimal Peso { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public int IdClasificacion { get; set; }

        // Navegación
        public virtual Clasificacion Clasificacion { get; set; }
    }
} 