using SalesAPI.Models;

namespace SalesAPI.Repositories.Interfaces
{
    public interface IProdutoRepository
    {
        Task<IEnumerable<Produto>> GetAllAsync();
        Task<Produto?> GetByIdAsync(int id);
        Task<int> CreateAsync(Produto produto);
        Task<bool> UpdateAsync(Produto produto);
        Task<bool> UpdateEstoqueAsync(int codProduto, int quantidade);
        Task<bool> DeleteAsync(int id);
    }
}