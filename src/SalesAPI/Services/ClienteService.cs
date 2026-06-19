using SalesAPI.DTOs;
using SalesAPI.Models;
using SalesAPI.Repositories.Interfaces;
using SalesAPI.Services.Interfaces;
using SalesAPI.Specifications;
using SalesAPI.Validations;

namespace SalesAPI.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly ClienteValidation _validation;

        public ClienteService(IClienteRepository clienteRepository, ClienteValidation validation)
        {
            _clienteRepository = clienteRepository;
            _validation = validation;
        }

        public async Task<IEnumerable<ClienteDTO>> GetAllAsync()
        {
            var clientes = await _clienteRepository.GetAllAsync();
            return clientes.Select(c => MapToDTO(c));
        }

        public async Task<ClienteDTO> GetByIdAsync(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);

            if (cliente == null)
                throw new KeyNotFoundException("Cliente não encontrado.");

            return MapToDTO(cliente);
        }

        public async Task<ClienteDTO> GetByCNPJAsync(string cnpj)
        {
            var cnpjLimpo = CnpjSpecification.Limpar(cnpj);
            var cliente = await _clienteRepository.GetByCNPJAsync(cnpjLimpo);

            if (cliente == null)
                throw new KeyNotFoundException("Cliente não encontrado com o CNPJ informado.");

            return MapToDTO(cliente);
        }

        public async Task<ClienteDTO> CreateAsync(CriarClienteDTO dto)
        {
            // Validação centralizada
            if (!_validation.IsValidForCreate(dto))
            {
                var mensagem = string.Join("; ", _validation.Errors);
                throw new InvalidOperationException(mensagem);
            }

            // Limpar CNPJ
            var cnpj = CnpjSpecification.Limpar(dto.CNPJ);

            // Regra de negócio: unicidade do CNPJ
            var clienteExistente = await _clienteRepository.GetByCNPJAsync(cnpj);
            if (clienteExistente != null)
                throw new InvalidOperationException("Já existe um cliente cadastrado com este CNPJ.");

            // Criar entidade
            var cliente = new Cliente
            {
                CNPJ = cnpj,
                Nome = dto.Nome,
                Email = dto.Email,
                DataCadastro = DateTime.Now
            };

            // Persistir
            var id = await _clienteRepository.CreateAsync(cliente);
            cliente.CodCliente = id;

            return MapToDTO(cliente);
        }

        public async Task<ClienteDTO> UpdateAsync(int id, AtualizarClienteDTO dto)
        {
            // Validação centralizada
            if (!_validation.IsValidForUpdate(dto))
            {
                var mensagem = string.Join("; ", _validation.Errors);
                throw new InvalidOperationException(mensagem);
            }

            // Regra de negócio: cliente deve existir
            var clienteExistente = await _clienteRepository.GetByIdAsync(id);
            if (clienteExistente == null)
                throw new KeyNotFoundException("Cliente não encontrado.");

            // Atualizar dados
            clienteExistente.Nome = dto.Nome;
            clienteExistente.Email = dto.Email;

            // Persistir
            await _clienteRepository.UpdateAsync(clienteExistente);

            return MapToDTO(clienteExistente);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Regra de negócio: cliente deve existir
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null)
                throw new KeyNotFoundException("Cliente não encontrado.");

            return await _clienteRepository.DeleteAsync(id);
        }

        private static ClienteDTO MapToDTO(Cliente cliente)
        {
            return new ClienteDTO
            {
                CodCliente = cliente.CodCliente,
                CNPJ = cliente.CNPJ,
                Nome = cliente.Nome,
                Email = cliente.Email,
                DataCadastro = cliente.DataCadastro
            };
        }
    }
}