using backend.Api.Services;
using backend.Application.DTO;
using backend.Application.Services.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoryController(ICategoryService categoryService, ICurrentUser currentUser) : ControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;
        private readonly ICurrentUser _currentUser = currentUser;

        [HttpGet()]
        public async Task<ActionResult<List<CategoryResponseDto>>> GetAllAsync()
        {
            var categories = await _categoryService.GetAllCategoriesAsync(_currentUser.UserId);
            if (categories is null || categories.Count <= 0)
            {
                return Ok("Nenhuma categoria encontrada!");
            }

            return Ok(categories);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CategoryResponseDto>> GetByIdAsync(Guid id)
        {
            var category = await _categoryService.GetCategoryById(_currentUser.UserId, id);

            if (category is null)
            {
                return NotFound("Categoria não encontrada!");
            }

            return Ok(category);

        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDTO createCategoryDTO)
        {
            var id = await _categoryService.CreateCategoryAsync(_currentUser.UserId, createCategoryDTO);

            return CreatedAtAction(
                "GetById",
                new { id },
                new CategoryResponseDto { Id = id, Name = createCategoryDTO.Name, ParentCategoryId = createCategoryDTO.ParentCategoryId }
            );

        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryDTO updateCategoryDTO)
        {
            var updated = await _categoryService.UpdateCategoryAsync(_currentUser.UserId, id, updateCategoryDTO);

            return updated ? NoContent() : NotFound("Categoria não encontrada!");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _categoryService.DeleteCategoryAsync(_currentUser.UserId, id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

    }
}