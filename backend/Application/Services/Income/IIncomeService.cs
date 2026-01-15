using backend.Application.DTO;

namespace backend.Application.Services.Income
{
    public interface IIncomeService
    {
        Task<IReadOnlyList<IncomeResponseDTO>> ListAllIncomeAsync(Guid userId, int mes, int ano);
        Task<IncomeResponseDTO?> GetIncomeByIdAsync(Guid userId, Guid id);
        Task<IncomeResponseDTO> AddIncomeAsync(Guid userId, int mes, int ano, AddIncomeDTO addIncomeDTO);
        Task<IncomeResponseDTO?> UdateIncomeAsync(Guid userId, UpdateIncomeDTO updateIncomeDTO);
        Task<bool> RemoveIncomeAsync(Guid userId, Guid id);
    }
}