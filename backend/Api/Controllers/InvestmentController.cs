using backend.Api.Services;
using backend.Application.DTO;
using backend.Application.Services.Investments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InvestmentController(IInvestmentService investmentService, ICurrentUser currentUser) : ControllerBase
    {
        private readonly IInvestmentService _investmentService = investmentService;
        private readonly ICurrentUser _currentUser = currentUser;

        [HttpGet("{mes:int}/{ano:int}")]
        public async Task<ActionResult<IReadOnlyList<InvestmentResponseDTO>>> GetAllAsync(int mes, int ano)
        {
            if (mes < 1 || mes > 12) return BadRequest("Mes Inválido");

            var investments = await _investmentService.ListAllAsync(_currentUser.UserId, mes, ano);
            return Ok(investments);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<InvestmentResponseDTO>> GetByIdAsync(Guid id)
        {
            var investment = await _investmentService.GetByIdAsync(_currentUser.UserId, id);
            if (investment is null) return NotFound();

            return Ok(investment);
        }

        [HttpPost("{mes:int}/{ano:int}")]
        public async Task<ActionResult> CreateAsync(int mes, int ano, [FromBody] CreateInvestmentDTO dto)
        {
            if (mes < 1 || mes > 12) return BadRequest("Mes Inválido");

            var created = await _investmentService.CreateAsync(_currentUser.UserId, mes, ano, dto);

            return CreatedAtAction(
                "GetById",
                new { id = created.Id },
                created
            );
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] UpdateInvestmentDTO dto)
        {
            var updated = await _investmentService.UpdateAsync(_currentUser.UserId, id, dto);
            if (updated is null) return NotFound();

            return Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _investmentService.DeleteAsync(_currentUser.UserId, id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
