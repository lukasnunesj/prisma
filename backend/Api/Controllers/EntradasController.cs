using backend.Api.Extensions;
using backend.Application.DTO;
using backend.Application.Services.Income;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class IncomeController(IIncomeService incomeService) : ControllerBase
    {
        private readonly IIncomeService _incomeService = incomeService;

        [HttpGet()]
        public async Task<ActionResult<IReadOnlyList<IncomeResponseDTO>>> GetAllAsync()
        {
            var userId = User.GetUserId();

            var income = await _incomeService.ListAllIncomeAsync(userId);

            if (income is null || income.Count <= 0)
            {
                return NotFound("Nenhuma Entrada encontrada!");
            }

            return Ok(income);
        }

        [HttpPost("{mes:int}/{ano:int}")]
        public async Task<ActionResult> AddAsync(int mes, int ano, [FromBody] AddIncomeDTO dto)
        {
            var userId = User.GetUserId();

            if (mes < 1 || mes > 12) return BadRequest("Mes Inválido");

            var income = await _incomeService.AddIncomeAsync(userId, mes, ano, dto);

            return CreatedAtAction(
                nameof(GetByIdAsync),
                new { id = income.Id },
                income
            );
        }

        [HttpPut()]
        public async Task<ActionResult> PutAsync([FromBody] UpdateIncomeDTO dto)
        {
            var userId = User.GetUserId();

            var updated = await _incomeService.UdateIncomeAsync(userId, dto);

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            var userId = User.GetUserId();

            var deleted = await _incomeService.RemoveIncomeAsync(userId, id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<IncomeResponseDTO>> GetByIdAsync(Guid id)
        {
            var userId = User.GetUserId();

            var income = await _incomeService.GetIncomeByIdAsync(userId, id);

            if (income is null)
                return NotFound();

            return Ok(income);
        }

    }
}