using backend.Api.Extensions;
using backend.Application.DTO;
using backend.Application.Services.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionController(ITransactionService transactionService) : ControllerBase
    {
        private ITransactionService _transactionService = transactionService;

        [HttpGet()]
        public async Task<ActionResult<List<TransactionResponseDTO>>> GetAllAsync()
        {
            var userId = User.GetUserId();
            var transactions = await _transactionService.GetAllTransactionsAsync(userId);
            if (transactions is null || transactions.Count <= 0)
            {
                return NotFound("Nenhuma transação encontrada!");
            }

            return Ok(transactions);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TransactionResponseDTO>> GetByIdAsync(Guid id)
        {
            var userId = User.GetUserId();
            var transaction = await _transactionService.GetTransactionByIdAsync(id, userId);
            if (transaction is null)
            {
                return NotFound("Transação não encontrada!");
            }

            return Ok(transaction);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTransactionDTO createTransactionDTO)
        {
            var userId = User.GetUserId();
            var id = await _transactionService.CreateTransactionAsync(createTransactionDTO, userId);
            return CreatedAtAction("GetById", new { id }, new TransactionResponseDTO { Id = id });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromBody] UpdateTransactionDTO updateTransactionDTO, Guid id)
        {
            var userId = User.GetUserId();

            var updated = await _transactionService.UpdateTransactionAsync(updateTransactionDTO, id, userId);

            return updated ? NoContent() : NotFound("Transação não encontrada!");
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.GetUserId();
            var deleted = await _transactionService.DeleteTransactionAsync(id, userId);
            return deleted ? NoContent() : NotFound("Transação não encontrada!");
        }
    }
}