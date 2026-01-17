using backend.Domain.Entities;
using backend.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace backend.Infrastructure.Repositories.PaymentMethods
{
    public class PaymentMethodRepository(ApplicationDbContext context) : IPaymentMethodRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task AddAsync(PaymentMethod paymentMethod)
        {
            await _context.PaymentMethods.AddAsync(paymentMethod);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(PaymentMethod paymentMethod)
        {
            _context.PaymentMethods.Remove(paymentMethod);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<PaymentMethod>> GetAllAsync(Guid userId)
        {
            return await _context.PaymentMethods
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<PaymentMethod?> GetByIdAsync(Guid id, Guid userId)
        {
            return await _context.PaymentMethods.FirstOrDefaultAsync(
                x => x.Id == id
                && x.UserId == userId
            );
        }

        public async Task UpdateAsync(PaymentMethod paymentMethod)
        {
            _context.PaymentMethods.Update(paymentMethod);
            await _context.SaveChangesAsync();
        }
    }
}
