using Dapper;
using SalesAPI.Data;
using SalesAPI.Models;
using SalesAPI.Repositories.Interfaces;

namespace SalesAPI.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public ClienteRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Cliente>("SELECT * FROM Cliente ORDER BY Nome");
        }

        public async Task<Cliente?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Cliente>(
                "SELECT * FROM Cliente WHERE CodCliente = @Id", new { Id = id });
        }

        public async Task<Cliente?> GetByCNPJAsync(string cnpj)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Cliente>(
                "SELECT * FROM Cliente WHERE CNPJ = @CNPJ", new { CNPJ = cnpj });
        }

        public async Task<int> CreateAsync(Cliente cliente)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"INSERT INTO Cliente (CNPJ, Nome, Email, DataCadastro) 
                       VALUES (@CNPJ, @Nome, @Email, @DataCadastro);
                       SELECT CAST(SCOPE_IDENTITY() as int)";
            
            cliente.DataCadastro = DateTime.Now;
            return await connection.QuerySingleAsync<int>(sql, cliente);
        }

        public async Task<bool> UpdateAsync(Cliente cliente)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"UPDATE Cliente 
                       SET Nome = @Nome, Email = @Email 
                       WHERE CodCliente = @CodCliente";
            
            var result = await connection.ExecuteAsync(sql, cliente);
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.ExecuteAsync(
                "DELETE FROM Cliente WHERE CodCliente = @Id", new { Id = id });
            return result > 0;
        }
    }
}