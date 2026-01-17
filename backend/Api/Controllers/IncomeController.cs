using backend.Api.Services;
using backend.Application.DTO;
using backend.Application.Services.Income;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class IncomeController(IIncomeService incomeService, ICurrentUser currentUser) : ControllerBase
    {
        private readonly IIncomeService _incomeService = incomeService;
        private readonly ICurrentUser _currentUser = currentUser;

        [HttpGet("{mes:int}/{ano:int}")]
        public async Task<ActionResult<IReadOnlyList<IncomeResponseDTO>>> GetAllAsync(int mes, int ano)
        {
            var income = await _incomeService.ListAllIncomeAsync(_currentUser.UserId, mes, ano);

            if (income is null || income.Count <= 0)
            {
                return Ok("Nenhuma Entrada encontrada!");
            }

            return Ok(income);
        }

        [HttpPost("{mes:int}/{ano:int}")]
        public async Task<ActionResult> AddAsync(int mes, int ano, [FromBody] CreateIncomeDTO dto)
        {
            if (mes < 1 || mes > 12) return BadRequest("Mes Inválido");

            var income = await _incomeService.AddIncomeAsync(_currentUser.UserId, mes, ano, dto);

            return CreatedAtAction(
                "GetById",
                new { id = income.Id },
                income
            );
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> PutAsync(Guid id, [FromBody] UpdateIncomeDTO dto)
        {
            var updated = await _incomeService.UdateIncomeAsync(_currentUser.UserId, id, dto);

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _incomeService.RemoveIncomeAsync(_currentUser.UserId, id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<IncomeResponseDTO>> GetByIdAsync(Guid id)
        {
            var income = await _incomeService.GetIncomeByIdAsync(_currentUser.UserId, id);

            if (income is null)
                return NotFound();

            return Ok(income);
        }

    }
}