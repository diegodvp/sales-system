using Microsoft.AspNetCore.Mvc;
using SalesAPI.DTOs;
using SalesAPI.Services.Interfaces;

namespace SalesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoService _produtoService;
        private readonly ILogger<ProdutoController> _logger;

        public ProdutoController(IProdutoService produtoService, ILogger<ProdutoController> logger)
        {
            _produtoService = produtoService;
            _logger = logger;
        }

        /// <summary>
        /// Lista todos os produtos disponíveis
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProdutoDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var produtos = await _produtoService.GetAllAsync();
            return Ok(produtos);
        }

        /// <summary>
        /// Busca um produto pelo ID
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProdutoDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var produto = await _produtoService.GetByIdAsync(id);
            return Ok(produto);
        }

        /// <summary>
        /// Cadastra um novo produto
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ProdutoDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CriarProdutoDTO dto)
        {
            var produto = await _produtoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = produto.CodProduto }, produto);
        }

        /// <summary>
        /// Atualiza os dados de um produto
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ProdutoDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] AtualizarProdutoDTO dto)
        {
            var produto = await _produtoService.UpdateAsync(id, dto);
            return Ok(produto);
        }

        /// <summary>
        /// Remove um produto
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            await _produtoService.DeleteAsync(id);
            return NoContent();
        }
    }
}