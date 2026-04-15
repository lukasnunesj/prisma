using backend.Domain.Entities;
using backend.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace backend.Infrastructure.Repositories.Investments
{
    public class InvestmentRepository(ApplicationDbContext context) : IInvestmentRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task AddAsync(Investment investment)
        {
            await _context.Investments.AddAsync(investment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Investment investment)
        {
            _context.Investments.Remove(investment);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<Investment>> GetAllAsync(Guid userId, int mes, int ano)
        {
            return await _context.Investments
                .AsNoTracking()
                .Where(e => e.UserId == userId && e.Mes == mes && e.Ano == ano)
                .ToListAsync();
        }

        public async Task<Investment?> GetByIdAsync(Guid id, Guid userId)
        {
            return await _context.Investments.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        }

        public async Task UpdateAsync(Investment investment)
        {
            _context.Investments.Update(investment);
            await _context.SaveChangesAsync();
        }
    }
}
