using SalesAPI.DTOs;
using SalesAPI.Specifications;

namespace SalesAPI.Validations
{
    public class ClienteValidation
    {
        private readonly CnpjSpecification _cnpjSpecification;
        private readonly EmailSpecification _emailSpecification;

        public List<string> Errors { get; private set; } = new();

        public ClienteValidation()
        {
            _cnpjSpecification = new CnpjSpecification();
            _emailSpecification = new EmailSpecification();
        }

        public bool IsValidForCreate(CriarClienteDTO dto)
        {
            Errors.Clear();

            // Validar CNPJ
            if (!_cnpjSpecification.IsSatisfiedBy(dto.CNPJ))
                Errors.Add(_cnpjSpecification.ErrorMessage);

            // Validar Nome
            if (string.IsNullOrWhiteSpace(dto.Nome))
                Errors.Add("Nome é obrigatório.");
            else if (dto.Nome.Length > 100)
                Errors.Add("Nome deve ter no máximo 100 caracteres.");

            // Validar Email (opcional para cliente)
            if (!string.IsNullOrEmpty(dto.Email))
            {
                if (!_emailSpecification.IsSatisfiedBy(dto.Email))
                    Errors.Add(_emailSpecification.ErrorMessage);
            }

            return !Errors.Any();
        }

        public bool IsValidForUpdate(AtualizarClienteDTO dto)
        {
            Errors.Clear();

            if (string.IsNullOrWhiteSpace(dto.Nome))
                Errors.Add("Nome é obrigatório.");
            else if (dto.Nome.Length > 100)
                Errors.Add("Nome deve ter no máximo 100 caracteres.");

            // Email opcional na atualização também
            if (!string.IsNullOrEmpty(dto.Email))
            {
                if (!_emailSpecification.IsSatisfiedBy(dto.Email))
                    Errors.Add(_emailSpecification.ErrorMessage);
            }

            return !Errors.Any();
        }
    }
}