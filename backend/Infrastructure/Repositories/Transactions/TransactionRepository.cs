using backend.Domain.Entities;
using backend.Domain.Enums;
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

        public async Task<IReadOnlyList<Transaction>> GetAllAsync(Guid userId, int mes, int ano)
        {
            return await _context.Transactions
                    .AsNoTracking()
                    .Where(e => e.UserId == userId
                    && e.Ano == ano
                    && e.Mes == mes)
                    .ToListAsync();
        }

        public async Task<IReadOnlyList<Transaction>> GetAllByTypeAsync(Guid userId, int mes, int ano, TransactionType type, ExpenseType? expenseType)
        {
            var query = _context.Transactions
                .AsNoTracking()
                .Where(e => e.UserId == userId
                    && e.Ano == ano
                    && e.Mes == mes
                    && e.Type == type);

            if (expenseType.HasValue)
            {
                query = query.Where(e => e.ExpenseType == expenseType.Value);
            }

            return await query.ToListAsync();
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