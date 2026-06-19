using SalesAPI.DTOs;

namespace SalesAPI.Validations
{
    /// <summary>
    /// Validador para operações de Produto
    /// </summary>
    public class ProdutoValidation
    {
        public List<string> Errors { get; private set; } = new();

        public bool IsValidForCreate(CriarProdutoDTO dto)
        {
            Errors.Clear();

            if (string.IsNullOrWhiteSpace(dto.Nome))
                Errors.Add("Nome do produto é obrigatório.");

            if (dto.Preco <= 0)
                Errors.Add("Preço deve ser maior que zero.");

            if (dto.Estoque < 0)
                Errors.Add("Estoque não pode ser negativo.");

            return !Errors.Any();
        }

        public bool IsValidForUpdate(AtualizarProdutoDTO dto)
        {
            Errors.Clear();

            if (string.IsNullOrWhiteSpace(dto.Nome))
                Errors.Add("Nome do produto é obrigatório.");

            if (dto.Preco <= 0)
                Errors.Add("Preço deve ser maior que zero.");

            if (dto.Estoque < 0)
                Errors.Add("Estoque não pode ser negativo.");

            return !Errors.Any();
        }
    }
}