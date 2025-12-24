using backend.Application.DTO;
using backend.Infrastructure.Repositories;

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
                    ParentCategoryId = category.ParentCategoryId
                };
        }

        public async Task<bool> UpdateCategoryAsync(Guid userId, Guid id, UpdateCategoryDTO updateCategoryDTO)
        {
            var category = await _repository.GetByIdAsync(id, userId);

            if (category is null) return false;

            category.Rename(updateCategoryDTO.Name);

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


    }

}