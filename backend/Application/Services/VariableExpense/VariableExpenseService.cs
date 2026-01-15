using backend.Application.DTO;
using backend.Infrastructure.Repositories.Transactions;

namespace backend.Application.Services.VariableExpense
{
    public class VariableExpenseService(ITransactionRepository transactionRepository) : IVariableExpenseService
    {
        private readonly ITransactionRepository _transactionRepository = transactionRepository;

        public async Task<IReadOnlyList<VariableExpenseResponseDTO>> ListAllVariableExpensesAsync(Guid userId, int mes, int ano)
        {
            var transactions = await _transactionRepository.GetAllByTypeAsync(userId, mes, ano, Domain.Enums.TransactionType.EXPENSE, Domain.Enums.ExpenseType.VARIABLE);
            return [.. transactions.Select(e=>new VariableExpenseResponseDTO{
                Id = e.Id,
                Amount = e.Amount,
                Mes = e.Mes,
                Ano = e.Ano,
                Date = e.Date,
                Description = e.Description,
                CategoryId = e.CategoryId
            })];
        }
    }
}