using Dapper;
using SalesAPI.Data;
using SalesAPI.Models;
using SalesAPI.Repositories.Interfaces;

namespace SalesAPI.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public ProdutoRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Produto>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Produto>("SELECT * FROM Produto ORDER BY Nome");
        }

        public async Task<Produto?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Produto>(
                "SELECT * FROM Produto WHERE CodProduto = @Id", new { Id = id });
        }

        public async Task<int> CreateAsync(Produto produto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"INSERT INTO Produto (Nome, Preco, Estoque) 
                       VALUES (@Nome, @Preco, @Estoque);
                       SELECT CAST(SCOPE_IDENTITY() as int)";
            
            return await connection.QuerySingleAsync<int>(sql, produto);
        }

        public async Task<bool> UpdateAsync(Produto produto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"UPDATE Produto 
                       SET Nome = @Nome, Preco = @Preco, Estoque = @Estoque 
                       WHERE CodProduto = @CodProduto";
            
            var result = await connection.ExecuteAsync(sql, produto);
            return result > 0;
        }

        public async Task<bool> UpdateEstoqueAsync(int codProduto, int quantidade)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"UPDATE Produto 
                       SET Estoque = Estoque + @Quantidade 
                       WHERE CodProduto = @CodProduto";
            
            var result = await connection.ExecuteAsync(sql, new { CodProduto = codProduto, Quantidade = quantidade });
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.ExecuteAsync(
                "DELETE FROM Produto WHERE CodProduto = @Id", new { Id = id });
            return result > 0;
        }
    }
}