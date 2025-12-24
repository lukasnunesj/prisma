using backend.Application.DTO;

namespace backend.Application.Services.Category
{
    public interface ICategoryService
    {
        public Task<Guid> CreateCategoryAsync(Guid userId, CreateCategoryDTO createCategoryDTO);
        public Task<CategoryResponseDto?> GetCategoryById(Guid userId, Guid id);
        public Task<bool> UpdateCategoryAsync(Guid userId, Guid id, UpdateCategoryDTO updateCategoryDTO);
        public Task<bool> DeleteCategoryAsync(Guid userId, Guid id);
    }
}