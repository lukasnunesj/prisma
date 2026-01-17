using backend.Application.DTO;
using backend.Domain.Entities;
using backend.Domain.Enums;
using backend.Infrastructure.Repositories.Transactions;

namespace backend.Application.Services.VariableExpense
{
    public class VariableExpenseService(ITransactionRepository transactionRepository) : IVariableExpenseService
    {
        private readonly ITransactionRepository _transactionRepository = transactionRepository;

        public async Task<IReadOnlyList<VariableExpenseResponseDTO>> ListAllVariableExpensesAsync(Guid userId, int mes, int ano)
        {
            var transactions = await _transactionRepository.GetAllByTypeAsync(userId, mes, ano, TransactionType.EXPENSE, ExpenseType.VARIABLE);
            return transactions.Select(e => new VariableExpenseResponseDTO
            {
                Id = e.Id,
                Amount = e.Amount,
                Mes = e.Mes,
                Ano = e.Ano,
                Date = e.Date,
                Description = e.Description,
                CategoryId = e.CategoryId
            }).ToList();
        }

        public async Task<VariableExpenseResponseDTO> CreateVariableExpenseAsync(Guid userId, CreateVariableExpanseDTO dto, int mes, int ano)
        {
            var transaction = new Transaction(
                userId,
                dto.Date,
                dto.Amount,
                TransactionType.EXPENSE,
                ExpenseType.VARIABLE,
                dto.Description,
                dto.CategoryId,
                mes,
                ano,
                false
            );

            await _transactionRepository.AddAsync(transaction);

            return new VariableExpenseResponseDTO
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Mes = transaction.Mes,
                Ano = transaction.Ano,
                Date = transaction.Date,
                Description = transaction.Description,
                CategoryId = transaction.CategoryId,
                PaymentMethodId = null,
                IsEssential = dto.IsEssential
            };
        }

        public Task<VariableExpenseResponseDTO> UpdateVariableExpenseAsync(Guid userId, UpdateVariableExpanseDTO dto)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteVariableExpenseAsync(Guid userId, Guid id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id, userId);
            if (transaction is null) return false;

            await _transactionRepository.DeleteAsync(transaction);
            return true;
        }
    }
}