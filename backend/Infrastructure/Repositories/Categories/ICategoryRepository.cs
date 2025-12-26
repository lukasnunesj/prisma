using backend.Domain.Entities;

namespace backend.Infrastructure.Repositories.Categories
{
    public interface ICategoryRepository
    {
        Task AddAsync(Category category);
        Task<Category?> GetByIdAsync(Guid id, Guid userID);
        Task<IReadOnlyList<Category>> GetAllAsync(Guid userID);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);
    }
}