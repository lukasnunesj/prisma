using backend.Application.DTO;

namespace backend.Application.Services.VariableExpense
{
    public interface IVariableExpenseService
    {
        Task<IReadOnlyList<VariableExpenseResponseDTO>> ListAllVariableExpensesAsync(Guid userId, int mes, int ano);
        Task<VariableExpenseResponseDTO> CreateVariableExpenseAsync(Guid userId, CreateVariableExpanseDTO dto, int mes, int ano);
        Task<VariableExpenseResponseDTO> UpdateVariableExpenseAsync(Guid userId, UpdateVariableExpanseDTO dto);
        Task<bool> DeleteVariableExpenseAsync(Guid userId, Guid id);
    }
}