using SalesAPI.Models;

namespace SalesAPI.Repositories.Interfaces
{
    public interface IPedidoRepository
    {
        Task<IEnumerable<Pedido>> GetAllAsync();
        Task<Pedido?> GetByIdAsync(int id);
        Task<IEnumerable<Pedido>> GetByFilterAsync(DateTime? dataInicio, DateTime? dataFim, string clienteNome);
        Task<int> CreateAsync(Pedido pedido);
        Task<bool> AddItemAsync(ItemPedido item);
        Task<bool> UpdateItemAsync(ItemPedido item);
        Task<bool> RemoveItemAsync(int codPedido, int codProduto);
        Task<bool> UpdateValorTotalAsync(int codPedido, decimal valorTotal);
        Task<decimal> CalculateValorTotalAsync(int codPedido);
    }
}