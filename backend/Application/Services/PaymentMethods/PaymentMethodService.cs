using backend.Application.DTO;
using backend.Infrastructure.Repositories.PaymentMethods;

namespace backend.Application.Services.PaymentMethods
{
    public class PaymentMethodService(IPaymentMethodRepository repository) : IPaymentMethodService
    {
        private readonly IPaymentMethodRepository _repository = repository;

        public async Task<Guid> CreateAsync(Guid userId, CreatePaymentMethodDTO dto)
        {
            var paymentMethod = new Domain.Entities.PaymentMethod(userId, dto.Name, dto.Type);
            await _repository.AddAsync(paymentMethod);
            return paymentMethod.Id;
        }

        public async Task<bool> DeleteAsync(Guid userId, Guid id)
        {
            var paymentMethod = await _repository.GetByIdAsync(id, userId);
            if (paymentMethod is null) return false;

            await _repository.DeleteAsync(paymentMethod);
            return true;
        }

        public async Task<IReadOnlyList<PaymentMethodResponseDTO>> GetAllAsync(Guid userId)
        {
            var methods = await _repository.GetAllAsync(userId);

            return [.. methods.Select(e => new PaymentMethodResponseDTO
            {
                Id = e.Id,
                Name = e.Name,
                Type = e.Type
            })];
        }

        public async Task<PaymentMethodResponseDTO?> GetByIdAsync(Guid userId, Guid id)
        {
            var method = await _repository.GetByIdAsync(id, userId);
            if (method is null) return null;

            return new PaymentMethodResponseDTO
            {
                Id = method.Id,
                Name = method.Name,
                Type = method.Type
            };
        }

        public async Task<bool> UpdateAsync(Guid userId, Guid id, UpdatePaymentMethodDTO dto)
        {
            var method = await _repository.GetByIdAsync(id, userId);
            if (method is null) return false;

            method.Rename(dto.Name);
            method.ChangeType(dto.Type);
            await _repository.UpdateAsync(method);

            return true;
        }
    }
}
