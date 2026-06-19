using Microsoft.AspNetCore.Mvc;
using SalesAPI.DTOs;
using SalesAPI.Services.Interfaces;

namespace SalesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        private readonly ILogger<ClienteController> _logger;

        public ClienteController(IClienteService clienteService, ILogger<ClienteController> logger)
        {
            _clienteService = clienteService;
            _logger = logger;
        }

        /// <summary>
        /// Lista todos os clientes cadastrados
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ClienteDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var clientes = await _clienteService.GetAllAsync();
            return Ok(clientes);
        }

        /// <summary>
        /// Busca um cliente pelo ID
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ClienteDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            return Ok(cliente);
        }

        /// <summary>
        /// Busca um cliente pelo CNPJ
        /// </summary>
        [HttpGet("cnpj/{cnpj}")]
        [ProducesResponseType(typeof(ClienteDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCNPJ(string cnpj)
        {
            var cliente = await _clienteService.GetByCNPJAsync(cnpj);
            return Ok(cliente);
        }

        /// <summary>
        /// Cadastra um novo cliente
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ClienteDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CriarClienteDTO dto)
        {
            var cliente = await _clienteService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = cliente.CodCliente }, cliente);
        }

        /// <summary>
        /// Atualiza os dados de um cliente
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ClienteDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] AtualizarClienteDTO dto)
        {
            var cliente = await _clienteService.UpdateAsync(id, dto);
            return Ok(cliente);
        }

        /// <summary>
        /// Remove um cliente
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            await _clienteService.DeleteAsync(id);
            return NoContent();
        }
    }
}