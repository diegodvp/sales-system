using SalesAPI.DTOs;

namespace SalesAPI.Services.Interfaces
{
    public interface IProdutoService
    {
        Task<IEnumerable<ProdutoDTO>> GetAllAsync();
        Task<ProdutoDTO> GetByIdAsync(int id);
        Task<ProdutoDTO> CreateAsync(CriarProdutoDTO dto);
        Task<ProdutoDTO> UpdateAsync(int id, AtualizarProdutoDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}