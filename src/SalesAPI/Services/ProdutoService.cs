using SalesAPI.DTOs;
using SalesAPI.Models;
using SalesAPI.Repositories.Interfaces;
using SalesAPI.Services.Interfaces;
using SalesAPI.Validations;

namespace SalesAPI.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly ProdutoValidation _validation;

        public ProdutoService(IProdutoRepository produtoRepository, ProdutoValidation validation)
        {
            _produtoRepository = produtoRepository;
            _validation = validation;
        }

        public async Task<IEnumerable<ProdutoDTO>> GetAllAsync()
        {
            var produtos = await _produtoRepository.GetAllAsync();
            return produtos.Select(p => MapToDTO(p));
        }

        public async Task<ProdutoDTO> GetByIdAsync(int id)
        {
            var produto = await _produtoRepository.GetByIdAsync(id);

            if (produto == null)
                throw new KeyNotFoundException("Produto não encontrado.");

            return MapToDTO(produto);
        }

        public async Task<ProdutoDTO> CreateAsync(CriarProdutoDTO dto)
        {
            // Validação centralizada
            if (!_validation.IsValidForCreate(dto))
            {
                var mensagem = string.Join("; ", _validation.Errors);
                throw new InvalidOperationException(mensagem);
            }

            // Criar entidade
            var produto = new Produto
            {
                Nome = dto.Nome,
                Preco = dto.Preco,
                Estoque = dto.Estoque
            };

            // Persistir
            var id = await _produtoRepository.CreateAsync(produto);
            produto.CodProduto = id;

            return MapToDTO(produto);
        }

        public async Task<ProdutoDTO> UpdateAsync(int id, AtualizarProdutoDTO dto)
        {
            // Validação centralizada
            if (!_validation.IsValidForUpdate(dto))
            {
                var mensagem = string.Join("; ", _validation.Errors);
                throw new InvalidOperationException(mensagem);
            }

            // Regra de negócio: produto deve existir
            var produtoExistente = await _produtoRepository.GetByIdAsync(id);
            if (produtoExistente == null)
                throw new KeyNotFoundException("Produto não encontrado.");

            // Atualizar dados
            produtoExistente.Nome = dto.Nome;
            produtoExistente.Preco = dto.Preco;
            produtoExistente.Estoque = dto.Estoque;

            // Persistir
            await _produtoRepository.UpdateAsync(produtoExistente);

            return MapToDTO(produtoExistente);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var produto = await _produtoRepository.GetByIdAsync(id);
            if (produto == null)
                throw new KeyNotFoundException("Produto não encontrado.");

            return await _produtoRepository.DeleteAsync(id);
        }

        private static ProdutoDTO MapToDTO(Produto produto)
        {
            return new ProdutoDTO
            {
                CodProduto = produto.CodProduto,
                Nome = produto.Nome,
                Preco = produto.Preco,
                Estoque = produto.Estoque
            };
        }
    }
}