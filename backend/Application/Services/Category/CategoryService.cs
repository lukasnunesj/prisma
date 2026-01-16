using backend.Application.DTO;
using backend.Infrastructure.Repositories.Categories;

namespace backend.Application.Services.Category
{
    public class CategoryService(ICategoryRepository repository) : ICategoryService
    {
        private readonly ICategoryRepository _repository = repository;

        public async Task<Guid> CreateCategoryAsync(Guid userId, CreateCategoryDTO createCategoryDTO)
        {
            var category = new Domain.Entities.Category(
                userId,
                createCategoryDTO.Name,
                createCategoryDTO.Color,
                createCategoryDTO.Icon,
                createCategoryDTO.ParentCategoryId
            );
            await _repository.AddAsync(category);
            return category.Id;
        }

        public async Task<CategoryResponseDto?> GetCategoryById(Guid userId, Guid id)
        {
            var category = await _repository.GetByIdAsync(id, userId);

            return category is null ?
                null
                : new CategoryResponseDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Color = category.Color,
                    Icon = category.Icon,
                    ParentCategoryId = category.ParentCategoryId
                };
        }

        public async Task<bool> UpdateCategoryAsync(Guid userId, Guid id, UpdateCategoryDTO updateCategoryDTO)
        {
            var category = await _repository.GetByIdAsync(id, userId);

            if (category is null) return false;

            category.Rename(updateCategoryDTO.Name);
            category.ChangeColor(updateCategoryDTO.Color);
            category.ChangeIcon(updateCategoryDTO.Icon);

            await _repository.UpdateAsync(category);

            return true;
        }

        public async Task<bool> DeleteCategoryAsync(Guid userId, Guid id)
        {
            var category = await _repository.GetByIdAsync(id, userId);

            if (category is null) return false;

            await _repository.DeleteAsync(category);
            return true;
        }

        public async Task<IReadOnlyList<CategoryResponseDto?>> GetAllCategoriesAsync(Guid userId)
        {
            var categories = await _repository.GetAllAsync(userId);

            return [.. categories.Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Color = c.Color,
                Icon = c.Icon,
                ParentCategoryId = c.ParentCategoryId
            })];

        }
    }

}