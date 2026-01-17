using backend.Domain.Entities;

namespace backend.Infrastructure.Repositories.PaymentMethods
{
    public interface IPaymentMethodRepository
    {
        Task AddAsync(PaymentMethod paymentMethod);
        Task<PaymentMethod?> GetByIdAsync(Guid id, Guid userId);
        Task<IReadOnlyList<PaymentMethod>> GetAllAsync(Guid userId);
        Task UpdateAsync(PaymentMethod paymentMethod);
        Task DeleteAsync(PaymentMethod paymentMethod);
    }
}
