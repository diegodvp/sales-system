namespace SalesAPI.Models
{
    public class ItemPedido
    {
        public int CodPedido { get; set; }
        public int CodProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public Produto Produto { get; set; }
    }
}