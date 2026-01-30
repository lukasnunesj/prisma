using backend.Application.DTO;

namespace backend.Application.Services.VariableExpense
{
    public interface IVariableExpenseService
    {
        Task<IReadOnlyList<VariableExpenseResponseDTO>> ListAllVariableExpensesAsync(Guid userId, int mes, int ano);
        Task<VariableExpenseResponseDTO> GetVariableExpenseByIdAsync(Guid userId, Guid id);
        Task<VariableExpenseResponseDTO> CreateVariableExpenseAsync(Guid userId, CreateVariableExpanseDTO dto, int mes, int ano);
        Task<VariableExpenseResponseDTO> UpdateVariableExpenseAsync(Guid userId, Guid id, UpdateVariableExpanseDTO dto);
        Task<bool> DeleteVariableExpenseAsync(Guid userId, Guid id);
    }
}