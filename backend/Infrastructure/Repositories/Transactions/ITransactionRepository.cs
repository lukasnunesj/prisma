using backend.Domain.Entities;

namespace backend.Infrastructure.Repositories.Transactions
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction);
        Task<IReadOnlyList<Transaction>> GetAllAsync(Guid userId);
        Task<Transaction?> GetByIdAsync(Guid id, Guid userId);
        Task UpdateAsync(Transaction transaction);
        Task DeleteAsync(Transaction transaction);
    }
}