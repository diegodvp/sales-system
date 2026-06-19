namespace SalesAPI.Models
{
    public class Pedido
    {
        public int CodPedido { get; set; }
        public int CodCliente { get; set; }
        public DateTime DataPedido { get; set; }
        public decimal ValorTotal { get; set; }
        public Cliente Cliente { get; set; }
        public List<ItemPedido> Itens { get; set; }
    }
}