using SalesAPI.DTOs;
using SalesAPI.Specifications;

namespace SalesAPI.Validations
{
    public class PedidoValidation
    {
        private readonly CnpjSpecification _cnpjSpecification;
        public List<string> Errors { get; private set; } = new();

        public PedidoValidation()
        {
            _cnpjSpecification = new CnpjSpecification();
        }

        public bool IsValidForCreate(CriarPedidoDTO dto)
        {
            Errors.Clear();

            if (!_cnpjSpecification.IsSatisfiedBy(dto.CNPJ))
                Errors.Add(_cnpjSpecification.ErrorMessage);

            return !Errors.Any();
        }

        public bool IsValidForAddItem(AdicionarItemDTO dto, int estoqueDisponivel)
        {
            Errors.Clear();

            // Validação específica de pedido - não reutilizável
            if (dto.Quantidade <= 0)
                Errors.Add("Quantidade deve ser maior que zero.");

            if (estoqueDisponivel < dto.Quantidade)
                Errors.Add($"Estoque insuficiente. Disponível: {estoqueDisponivel}, Solicitado: {dto.Quantidade}.");

            return !Errors.Any();
        }
    }
}