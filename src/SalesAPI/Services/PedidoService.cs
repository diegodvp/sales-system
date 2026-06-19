using SalesAPI.DTOs;
using SalesAPI.Models;
using SalesAPI.Repositories.Interfaces;
using SalesAPI.Services.Interfaces;
using SalesAPI.Specifications;
using SalesAPI.Validations;

namespace SalesAPI.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly PedidoValidation _validation;

        public PedidoService(
            IPedidoRepository pedidoRepository,
            IClienteRepository clienteRepository,
            IProdutoRepository produtoRepository,
            PedidoValidation validation)
        {
            _pedidoRepository = pedidoRepository;
            _clienteRepository = clienteRepository;
            _produtoRepository = produtoRepository;
            _validation = validation;
        }

        public async Task<IEnumerable<PedidoResponseDTO>> GetAllAsync(DateTime? dataInicio, DateTime? dataFim, string? clienteNome)
        {
            var pedidos = await _pedidoRepository.GetByFilterAsync(dataInicio, dataFim, clienteNome);
            return pedidos.Select(p => MapToResponseDTO(p));
        }

        public async Task<PedidoResponseDTO> GetByIdAsync(int id)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(id);

            if (pedido == null)
                throw new KeyNotFoundException("Pedido não encontrado.");

            return MapToResponseDTO(pedido);
        }

        public async Task<PedidoResponseDTO> CriarPedidoAsync(CriarPedidoDTO dto)
        {
            // Validação centralizada
            if (!_validation.IsValidForCreate(dto))
            {
                var mensagem = string.Join("; ", _validation.Errors);
                throw new InvalidOperationException(mensagem);
            }

            // Limpar CNPJ
            var cnpj = CnpjSpecification.Limpar(dto.CNPJ);

            // Regra de negócio: cliente deve existir
            var cliente = await _clienteRepository.GetByCNPJAsync(cnpj);
            if (cliente == null)
                throw new InvalidOperationException("Cliente não encontrado com o CNPJ informado.");

            // Criar pedido
            var pedido = new Pedido
            {
                CodCliente = cliente.CodCliente,
                DataPedido = DateTime.Now,
                ValorTotal = 0
            };

            pedido.CodPedido = await _pedidoRepository.CreateAsync(pedido);
            pedido.Cliente = cliente;

            return MapToResponseDTO(pedido);
        }

        public async Task<PedidoResponseDTO> AdicionarItemAsync(int codPedido, AdicionarItemDTO dto)
        {
            // Regra de negócio: pedido deve existir
            var pedido = await _pedidoRepository.GetByIdAsync(codPedido);
            if (pedido == null)
                throw new KeyNotFoundException("Pedido não encontrado.");

            // Regra de negócio: produto deve existir
            var produto = await _produtoRepository.GetByIdAsync(dto.CodProduto);
            if (produto == null)
                throw new KeyNotFoundException("Produto não encontrado.");

            // Validação de estoque (específica do pedido)
            if (!_validation.IsValidForAddItem(dto, produto.Estoque))
            {
                var mensagem = string.Join("; ", _validation.Errors);
                throw new InvalidOperationException(mensagem);
            }

            // Regra de negócio: item não pode estar duplicado no pedido
            var itemExistente = pedido.Itens?.FirstOrDefault(i => i.CodProduto == dto.CodProduto);
            if (itemExistente != null)
                throw new InvalidOperationException("Este produto já está no pedido. Utilize a opção de atualizar quantidade.");

            // Criar item
            var item = new ItemPedido
            {
                CodPedido = codPedido,
                CodProduto = dto.CodProduto,
                Quantidade = dto.Quantidade,
                PrecoUnitario = produto.Preco
            };

            // Persistir item
            await _pedidoRepository.AddItemAsync(item);

            // Atualizar estoque (regra de negócio)
            await _produtoRepository.UpdateEstoqueAsync(dto.CodProduto, -dto.Quantidade);

            // Atualizar valor total do pedido
            await AtualizarValorTotalPedidoAsync(codPedido);

            return await GetByIdAsync(codPedido);
        }

        public async Task<PedidoResponseDTO> AtualizarItemAsync(int codPedido, int codProduto, AtualizarItemDTO dto)
        {
            // Regra de negócio: pedido deve existir
            var pedido = await _pedidoRepository.GetByIdAsync(codPedido);
            if (pedido == null)
                throw new KeyNotFoundException("Pedido não encontrado.");

            // Regra de negócio: item deve existir no pedido
            var itemExistente = pedido.Itens?.FirstOrDefault(i => i.CodProduto == codProduto);
            if (itemExistente == null)
                throw new KeyNotFoundException("Item não encontrado no pedido.");

            // Se quantidade for zero ou negativa, remover o item
            if (dto.Quantidade <= 0)
            {
                return await RemoverItemAsync(codPedido, codProduto);
            }

            // Calcular diferença para atualizar estoque
            var produto = await _produtoRepository.GetByIdAsync(codProduto);
            var diferenca = dto.Quantidade - itemExistente.Quantidade;

            // Validar estoque se estiver aumentando a quantidade
            if (diferenca > 0)
            {
                var addItemDto = new AdicionarItemDTO
                {
                    CodProduto = codProduto,
                    Quantidade = diferenca
                };

                if (!_validation.IsValidForAddItem(addItemDto, produto.Estoque))
                {
                    var mensagem = string.Join("; ", _validation.Errors);
                    throw new InvalidOperationException(mensagem);
                }
            }

            // Atualizar item
            var item = new ItemPedido
            {
                CodPedido = codPedido,
                CodProduto = codProduto,
                Quantidade = dto.Quantidade,
                PrecoUnitario = produto.Preco
            };

            await _pedidoRepository.UpdateItemAsync(item);

            // Atualizar estoque
            await _produtoRepository.UpdateEstoqueAsync(codProduto, -diferenca);

            // Atualizar valor total
            await AtualizarValorTotalPedidoAsync(codPedido);

            return await GetByIdAsync(codPedido);
        }

        public async Task<PedidoResponseDTO> RemoverItemAsync(int codPedido, int codProduto)
        {
            // Regra de negócio: pedido deve existir
            var pedido = await _pedidoRepository.GetByIdAsync(codPedido);
            if (pedido == null)
                throw new KeyNotFoundException("Pedido não encontrado.");

            // Regra de negócio: item deve existir no pedido
            var item = pedido.Itens?.FirstOrDefault(i => i.CodProduto == codProduto);
            if (item == null)
                throw new KeyNotFoundException("Item não encontrado no pedido.");

            // Remover item
            await _pedidoRepository.RemoveItemAsync(codPedido, codProduto);

            // Devolver estoque
            await _produtoRepository.UpdateEstoqueAsync(codProduto, item.Quantidade);

            // Atualizar valor total
            await AtualizarValorTotalPedidoAsync(codPedido);

            return await GetByIdAsync(codPedido);
        }

        // Métodos privados auxiliares

        private async Task AtualizarValorTotalPedidoAsync(int codPedido)
        {
            var valorTotal = await _pedidoRepository.CalculateValorTotalAsync(codPedido);
            await _pedidoRepository.UpdateValorTotalAsync(codPedido, valorTotal);
        }

        private static PedidoResponseDTO MapToResponseDTO(Pedido pedido)
        {
            return new PedidoResponseDTO
            {
                CodPedido = pedido.CodPedido,
                ClienteNome = pedido.Cliente?.Nome ?? "N/A",
                ClienteCNPJ = pedido.Cliente?.CNPJ ?? "N/A",
                DataPedido = pedido.DataPedido,
                ValorTotal = pedido.ValorTotal,
                Itens = pedido.Itens?.Select(i => new ItemPedidoResponseDTO
                {
                    CodProduto = i.CodProduto,
                    NomeProduto = i.Produto?.Nome ?? "N/A",
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    Subtotal = i.Quantidade * i.PrecoUnitario
                }).ToList() ?? new List<ItemPedidoResponseDTO>()
            };
        }
    }
}