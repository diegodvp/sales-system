using Microsoft.AspNetCore.Mvc;
using SalesAPI.DTOs;
using SalesAPI.Services.Interfaces;

namespace SalesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;
        private readonly ILogger<PedidoController> _logger;

        public PedidoController(IPedidoService pedidoService, ILogger<PedidoController> logger)
        {
            _pedidoService = pedidoService;
            _logger = logger;
        }

        /// <summary>
        /// Lista todos os pedidos com opção de filtros
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PedidoResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] DateTime? dataInicio = null,
            [FromQuery] DateTime? dataFim = null,
            [FromQuery] string? clienteNome = null)
        {
            var pedidos = await _pedidoService.GetAllAsync(dataInicio, dataFim, clienteNome);
            return Ok(pedidos);
        }

        /// <summary>
        /// Busca um pedido pelo ID com seus itens
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PedidoResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var pedido = await _pedidoService.GetByIdAsync(id);
            return Ok(pedido);
        }

        /// <summary>
        /// Cria um novo pedido para um cliente
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(PedidoResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CriarPedidoDTO dto)
        {
            var pedido = await _pedidoService.CriarPedidoAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = pedido.CodPedido }, pedido);
        }

        /// <summary>
        /// Adiciona um produto ao pedido
        /// </summary>
        [HttpPost("{codPedido:int}/itens")]
        [ProducesResponseType(typeof(PedidoResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AdicionarItem(int codPedido, [FromBody] AdicionarItemDTO dto)
        {
            var pedido = await _pedidoService.AdicionarItemAsync(codPedido, dto);
            return Ok(pedido);
        }

        /// <summary>
        /// Atualiza a quantidade de um item no pedido
        /// </summary>
        [HttpPut("{codPedido:int}/itens/{codProduto:int}")]
        [ProducesResponseType(typeof(PedidoResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AtualizarItem(int codPedido, int codProduto, [FromBody] AtualizarItemDTO dto)
        {
            var pedido = await _pedidoService.AtualizarItemAsync(codPedido, codProduto, dto);
            return Ok(pedido);
        }

        /// <summary>
        /// Remove um item do pedido
        /// </summary>
        [HttpDelete("{codPedido:int}/itens/{codProduto:int}")]
        [ProducesResponseType(typeof(PedidoResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoverItem(int codPedido, int codProduto)
        {
            var pedido = await _pedidoService.RemoverItemAsync(codPedido, codProduto);
            return Ok(pedido);
        }
    }
}