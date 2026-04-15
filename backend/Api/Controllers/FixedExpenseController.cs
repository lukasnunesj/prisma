using backend.Api.Services;
using backend.Application.DTO;
using backend.Application.Services.FixedExpense;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FixedExpenseController(IFixedExpenseService fixedExpenseService, ICurrentUser currentUser) : ControllerBase
    {
        private readonly IFixedExpenseService _fixedExpenseService = fixedExpenseService;
        private readonly ICurrentUser _currentUser = currentUser;

        [HttpGet("{mes:int}/{ano:int}")]
        public async Task<ActionResult<IReadOnlyList<FixedExpenseResponseDTO>>> GetAllAsync(int mes, int ano)
        {
            if (mes < 1 || mes > 12) return BadRequest("Mes Inválido");

            var expenses = await _fixedExpenseService.ListAllAsync(_currentUser.UserId, mes, ano);
            return Ok(expenses);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FixedExpenseResponseDTO>> GetByIdAsync(Guid id)
        {
            var expense = await _fixedExpenseService.GetByIdAsync(_currentUser.UserId, id);
            if (expense is null) return NotFound();

            return Ok(expense);
        }

        [HttpPost("{mes:int}/{ano:int}")]
        public async Task<ActionResult> AddAsync(int mes, int ano, [FromBody] CreateFixedExpenseDTO dto)
        {
            if (mes < 1 || mes > 12) return BadRequest("Mes Inválido");

            var created = await _fixedExpenseService.CreateAsync(_currentUser.UserId, mes, ano, dto);

            return CreatedAtAction(
                "GetById",
                new { id = created.Id },
                created
            );
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> PutAsync(Guid id, [FromBody] UpdateFixedExpenseDTO dto)
        {
            var updated = await _fixedExpenseService.UpdateAsync(_currentUser.UserId, id, dto);
            if (updated is null) return NotFound();

            return Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _fixedExpenseService.DeleteAsync(_currentUser.UserId, id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
