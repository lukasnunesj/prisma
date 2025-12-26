using backend.Domain.Entities;
using backend.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace backend.Infrastructure.Repositories.Categories
{
    public class CategoryRepository(ApplicationDbContext context) : ICategoryRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<Category>> GetAllAsync(Guid userID)
        {
            return await _context.Categories
                            .AsNoTracking()
                            .Where(x => x.UserId == userID)
                            .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(Guid id, Guid userId)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(
                x => x.Id == id
                && x.UserId == userId
            );

            return category;
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
    }
}