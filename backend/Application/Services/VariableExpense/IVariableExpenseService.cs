using backend.Application.DTO;

namespace backend.Application.Services.VariableExpense
{
    public interface IVariableExpenseService
    {
        Task<IReadOnlyList<VariableExpenseResponseDTO>> ListAllVariableExpensesAsync(Guid userId, int mes, int ano);
    }
}