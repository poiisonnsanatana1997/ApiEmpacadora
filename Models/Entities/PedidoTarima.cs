using System;

namespace AppAPIEmpacadora.Models.Entities
{
    public class PedidoTarima
    {
        public int IdPedidoCliente { get; set; }
        public int IdTarima { get; set; }

        // Navegación
        public virtual PedidoCliente PedidoCliente { get; set; }
        public virtual Tarima Tarima { get; set; }
    }
} 