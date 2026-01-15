using backend.Domain.Entities;
using backend.Domain.Enums;

namespace backend.Infrastructure.Repositories.Transactions
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction);
        Task<IReadOnlyList<Transaction>> GetAllAsync(Guid userId, int mes, int ano);
        Task<IReadOnlyList<Transaction>> GetAllByTypeAsync(Guid userId, int mes, int ano, TransactionType Type, ExpenseType? ExpenseType);
        Task<Transaction?> GetByIdAsync(Guid id, Guid userId);
        Task UpdateAsync(Transaction transaction);
        Task DeleteAsync(Transaction transaction);
    }
}