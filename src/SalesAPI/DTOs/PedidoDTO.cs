namespace SalesAPI.DTOs
{
    public class CriarPedidoDTO
    {
        public string CNPJ { get; set; } = string.Empty;
    }

    public class AdicionarItemDTO
    {
        public int CodProduto { get; set; }
        public int Quantidade { get; set; }
    }

    public class AtualizarItemDTO
    {
        public int Quantidade { get; set; }
    }

    public class PedidoResponseDTO
    {
        public int CodPedido { get; set; }
        public string ClienteNome { get; set; } = string.Empty;
        public string ClienteCNPJ { get; set; } = string.Empty;
        public DateTime DataPedido { get; set; }
        public decimal ValorTotal { get; set; }
        public List<ItemPedidoResponseDTO> Itens { get; set; } = new();
    }

    public class ItemPedidoResponseDTO
    {
        public int CodProduto { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}