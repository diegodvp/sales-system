using SalesAPI.DTOs;
using SalesAPI.Models;

namespace SalesAPI.Services.Interfaces
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteDTO>> GetAllAsync();
        Task<ClienteDTO> GetByIdAsync(int id);
        Task<ClienteDTO> GetByCNPJAsync(string cnpj);
        Task<ClienteDTO> CreateAsync(CriarClienteDTO dto);
        Task<ClienteDTO> UpdateAsync(int id, AtualizarClienteDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}