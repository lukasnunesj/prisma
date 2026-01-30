using backend.Api.Extensions;
using backend.Api.Services;
using backend.Application.DTO;
using backend.Application.Services.VariableExpense;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VariableExpenseController(IVariableExpenseService variableExpenseService, ICurrentUser currentUser) : ControllerBase
    {
        private readonly ICurrentUser _currentUser = currentUser;

        private readonly IVariableExpenseService _variableExpenseService = variableExpenseService;

        [HttpGet("{mes:int}/{ano:int}")]
        public async Task<ActionResult<IReadOnlyList<VariableExpenseResponseDTO>>> GetAllAsync(int mes, int ano)
        {
            var expenses = await _variableExpenseService.ListAllVariableExpensesAsync(_currentUser.UserId, mes, ano);

            if (expenses is null || expenses.Count <= 0)
            {
                return Ok(Array.Empty<VariableExpenseResponseDTO>());
            }

            return Ok(expenses);

        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<VariableExpenseResponseDTO>> GetByIdAsync(Guid id)
        {
            var expense = await _variableExpenseService.GetVariableExpenseByIdAsync(_currentUser.UserId, id);

            if (expense is null)
                return NotFound();

            return Ok(expense);
        }

        [HttpPost("{mes:int}/{ano:int}")]
        public async Task<ActionResult> AddAsync([FromBody] CreateVariableExpanseDTO dto, int mes, int ano)
        {
            if (mes < 1 || mes > 12) return BadRequest("Mes Inválido");

            var expenses = await _variableExpenseService.CreateVariableExpenseAsync(_currentUser.UserId, dto, mes, ano);

            return CreatedAtAction(
                "GetById",
                new { id = expenses.Id },
                expenses
            );

        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateVariableExpanseDTO dto)
        {
            var updated = await _variableExpenseService.UpdateVariableExpenseAsync(_currentUser.UserId, id, dto);

            return Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _variableExpenseService.DeleteVariableExpenseAsync(_currentUser.UserId, id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}