using backend.Api.Services;
using backend.Application.DTO;
using backend.Application.Services.PaymentMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentMethodController(IPaymentMethodService paymentMethodService, ICurrentUser currentUser) : ControllerBase
    {
        private readonly IPaymentMethodService _paymentMethodService = paymentMethodService;
        private readonly ICurrentUser _currentUser = currentUser;

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<PaymentMethodResponseDTO>>> GetAllAsync()
        {
            var methods = await _paymentMethodService.GetAllAsync(_currentUser.UserId);
            if (methods is null || methods.Count <= 0)
            {
                return Ok(Array.Empty<PaymentMethodResponseDTO>());
            }

            return Ok(methods);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PaymentMethodResponseDTO>> GetByIdAsync(Guid id)
        {
            var method = await _paymentMethodService.GetByIdAsync(_currentUser.UserId, id);
            if (method is null)
            {
                return NotFound("Forma de pagamento não encontrada!");
            }

            return Ok(method);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePaymentMethodDTO dto)
        {
            var id = await _paymentMethodService.CreateAsync(_currentUser.UserId, dto);

            return CreatedAtAction(
                "GetById",
                new { id },
                new PaymentMethodResponseDTO { Id = id, Name = dto.Name, Type = dto.Type }
            );
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePaymentMethodDTO dto)
        {
            var updated = await _paymentMethodService.UpdateAsync(_currentUser.UserId, id, dto);
            return updated ? NoContent() : NotFound("Forma de pagamento não encontrada!");
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _paymentMethodService.DeleteAsync(_currentUser.UserId, id);
            return deleted ? NoContent() : NotFound("Forma de pagamento não encontrada!");
        }
    }
}
