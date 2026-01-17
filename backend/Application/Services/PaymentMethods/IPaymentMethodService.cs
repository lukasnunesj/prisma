using backend.Application.DTO;

namespace backend.Application.Services.PaymentMethods
{
    public interface IPaymentMethodService
    {
        Task<IReadOnlyList<PaymentMethodResponseDTO>> GetAllAsync(Guid userId);
        Task<PaymentMethodResponseDTO?> GetByIdAsync(Guid userId, Guid id);
        Task<Guid> CreateAsync(Guid userId, CreatePaymentMethodDTO dto);
        Task<bool> UpdateAsync(Guid userId, Guid id, UpdatePaymentMethodDTO dto);
        Task<bool> DeleteAsync(Guid userId, Guid id);
    }
}
