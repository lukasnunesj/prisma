using backend.Application.DTO;

namespace backend.Application.Services.FixedExpense
{
    public interface IFixedExpenseService
    {
        Task<IReadOnlyList<FixedExpenseResponseDTO>> ListAllAsync(Guid userId, int mes, int ano);
        Task<FixedExpenseResponseDTO?> GetByIdAsync(Guid userId, Guid id);
        Task<FixedExpenseResponseDTO> CreateAsync(Guid userId, int mes, int ano, CreateFixedExpenseDTO dto);
        Task<FixedExpenseResponseDTO?> UpdateAsync(Guid userId, Guid id, UpdateFixedExpenseDTO dto);
        Task<bool> DeleteAsync(Guid userId, Guid id);
    }
}
