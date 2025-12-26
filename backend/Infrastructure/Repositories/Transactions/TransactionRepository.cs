using backend.Domain.Entities;
using backend.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace backend.Infrastructure.Repositories.Transactions
{
    public class TransactionRepository(ApplicationDbContext context) : ITransactionRepository
    {
        private ApplicationDbContext _context = context;

        public async Task AddAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Transaction transaction)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<Transaction>> GetAllAsync(Guid userId)
        {
            return await _context.Transactions
                    .AsNoTracking()
                    .Where(e => e.UserId == userId)
                    .ToListAsync();
        }

        public async Task<Transaction?> GetByIdAsync(Guid id, Guid userId)
        {
            return await _context.Transactions.FirstOrDefaultAsync(
                e => e.Id == id
                && e.UserId == userId
            );
        }

        public async Task UpdateAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }
    }
}