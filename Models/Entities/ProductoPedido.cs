using System.ComponentModel.DataAnnotations.Schema;

namespace AppAPIEmpacadora.Models.Entities
{
    public class ProductoPedido
    {
        [ForeignKey("PedidoProveedor")]
        public int IdPedidoProveedor { get; set; }
        public PedidoProveedor PedidoProveedor { get; set; }

        [ForeignKey("Producto")]
        public int IdProducto { get; set; }
        public Producto Producto { get; set; }
    }
} 