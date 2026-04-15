using backend.Domain.Entities;

namespace backend.Infrastructure.Repositories.Investments
{
    public interface IInvestmentRepository
    {
        Task AddAsync(Investment investment);
        Task<Investment?> GetByIdAsync(Guid id, Guid userId);
        Task<IReadOnlyList<Investment>> GetAllAsync(Guid userId, int mes, int ano);
        Task UpdateAsync(Investment investment);
        Task DeleteAsync(Investment investment);
    }
}
