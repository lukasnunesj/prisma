using backend.Api.Extensions;
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
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CategoryResponseDto>> GetByIdAsync(Guid id)
        {
            var userId = User.GetUserId();

            var category = await _categoryService.GetCategoryById(userId, id);

            if (category is null)
            {
                return NotFound("Categoria não encontrada!");
            }

            return Ok(category);

        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDTO createCategoryDTO)
        {
            var userId = User.GetUserId();

            var id = await _categoryService.CreateCategoryAsync(userId, createCategoryDTO);

            return CreatedAtAction(
                "GetById",
                new { id },
                new CategoryResponseDto { Id = id, Name = createCategoryDTO.Name, ParentCategoryId = createCategoryDTO.ParentCategoryId }
            );

        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryDTO updateCategoryDTO)
        {
            var userId = User.GetUserId();

            var updated = await _categoryService.UpdateCategoryAsync(userId, id, updateCategoryDTO);

            return updated ? NoContent() : NotFound("Categoria não encontrada!");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var userId = User.GetUserId();

            var deleted = await _categoryService.DeleteCategoryAsync(userId, id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

    }
}