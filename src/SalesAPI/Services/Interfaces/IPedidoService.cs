using SalesAPI.DTOs;

namespace SalesAPI.Services.Interfaces
{
    public interface IPedidoService
    {
        Task<IEnumerable<PedidoResponseDTO>> GetAllAsync(DateTime? dataInicio, DateTime? dataFim, string? clienteNome);
        Task<PedidoResponseDTO> GetByIdAsync(int id);
        Task<PedidoResponseDTO> CriarPedidoAsync(CriarPedidoDTO dto);
        Task<PedidoResponseDTO> AdicionarItemAsync(int codPedido, AdicionarItemDTO dto);
        Task<PedidoResponseDTO> AtualizarItemAsync(int codPedido, int codProduto, AtualizarItemDTO dto);
        Task<PedidoResponseDTO> RemoverItemAsync(int codPedido, int codProduto);
    }
}