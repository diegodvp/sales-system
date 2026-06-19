using Dapper;
using SalesAPI.Data;
using SalesAPI.Models;
using SalesAPI.Repositories.Interfaces;

namespace SalesAPI.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public PedidoRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Pedido>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT p.*, c.CNPJ, c.Nome 
                       FROM Pedido p 
                       INNER JOIN Cliente c ON p.CodCliente = c.CodCliente 
                       ORDER BY p.DataPedido DESC";
            
            return await connection.QueryAsync<Pedido, Cliente, Pedido>(
                sql,
                (pedido, cliente) => {
                    pedido.Cliente = cliente;
                    return pedido;
                },
                splitOn: "CNPJ");
        }

        public async Task<Pedido?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT p.*, c.CNPJ, c.Nome 
                       FROM Pedido p 
                       INNER JOIN Cliente c ON p.CodCliente = c.CodCliente 
                       WHERE p.CodPedido = @Id";
            
            var pedido = await connection.QueryAsync<Pedido, Cliente, Pedido>(
                sql,
                (pedido, cliente) => {
                    pedido.Cliente = cliente;
                    return pedido;
                },
                new { Id = id },
                splitOn: "CNPJ");

            var pedidoResult = pedido.FirstOrDefault();
            if (pedidoResult != null)
            {
                var itensSql = @"SELECT ip.*, pr.Nome 
                                FROM ItensPedido ip 
                                INNER JOIN Produto pr ON ip.CodProduto = pr.CodProduto 
                                WHERE ip.CodPedido = @CodPedido";
                
                pedidoResult.Itens = (await connection.QueryAsync<ItemPedido, Produto, ItemPedido>(
                    itensSql,
                    (item, produto) => {
                        item.Produto = produto;
                        return item;
                    },
                    new { CodPedido = id },
                    splitOn: "Nome")).ToList();
            }

            return pedidoResult;
        }

        public async Task<IEnumerable<Pedido>> GetByFilterAsync(DateTime? dataInicio, DateTime? dataFim, string clienteNome)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT p.*, c.CNPJ, c.Nome 
                       FROM Pedido p 
                       INNER JOIN Cliente c ON p.CodCliente = c.CodCliente 
                       WHERE 1=1";
            
            var parameters = new DynamicParameters();
            
            if (dataInicio.HasValue)
            {
                sql += " AND p.DataPedido >= @DataInicio";
                parameters.Add("DataInicio", dataInicio.Value);
            }
            
            if (dataFim.HasValue)
            {
                sql += " AND p.DataPedido <= @DataFim";
                parameters.Add("DataFim", dataFim.Value);
            }
            
            if (!string.IsNullOrEmpty(clienteNome))
            {
                sql += " AND c.Nome LIKE @ClienteNome";
                parameters.Add("ClienteNome", $"%{clienteNome}%");
            }
            
            sql += " ORDER BY p.DataPedido DESC";
            
            return await connection.QueryAsync<Pedido, Cliente, Pedido>(
                sql,
                (pedido, cliente) => {
                    pedido.Cliente = cliente;
                    return pedido;
                },
                parameters,
                splitOn: "CNPJ");
        }

        public async Task<int> CreateAsync(Pedido pedido)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"INSERT INTO Pedido (CodCliente, DataPedido, ValorTotal) 
                       VALUES (@CodCliente, @DataPedido, @ValorTotal);
                       SELECT CAST(SCOPE_IDENTITY() as int)";
            
            pedido.DataPedido = DateTime.Now;
            pedido.ValorTotal = 0;
            return await connection.QuerySingleAsync<int>(sql, pedido);
        }

        public async Task<bool> AddItemAsync(ItemPedido item)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"INSERT INTO ItensPedido (CodPedido, CodProduto, Quantidade, PrecoUnitario) 
                       VALUES (@CodPedido, @CodProduto, @Quantidade, @PrecoUnitario)";
            
            var result = await connection.ExecuteAsync(sql, item);
            return result > 0;
        }

        public async Task<bool> UpdateItemAsync(ItemPedido item)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"UPDATE ItensPedido 
                       SET Quantidade = @Quantidade 
                       WHERE CodPedido = @CodPedido AND CodProduto = @CodProduto";
            
            var result = await connection.ExecuteAsync(sql, item);
            return result > 0;
        }

        public async Task<bool> RemoveItemAsync(int codPedido, int codProduto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.ExecuteAsync(
                "DELETE FROM ItensPedido WHERE CodPedido = @CodPedido AND CodProduto = @CodProduto",
                new { CodPedido = codPedido, CodProduto = codProduto });
            return result > 0;
        }

        public async Task<bool> UpdateValorTotalAsync(int codPedido, decimal valorTotal)
        {
            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.ExecuteAsync(
                "UPDATE Pedido SET ValorTotal = @ValorTotal WHERE CodPedido = @CodPedido",
                new { CodPedido = codPedido, ValorTotal = valorTotal });
            return result > 0;
        }

        public async Task<decimal> CalculateValorTotalAsync(int codPedido)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<decimal>(
                "SELECT ISNULL(SUM(Quantidade * PrecoUnitario), 0) FROM ItensPedido WHERE CodPedido = @CodPedido",
                new { CodPedido = codPedido });
        }
    }
}