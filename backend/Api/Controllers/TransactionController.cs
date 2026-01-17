using backend.Api.Services;
using backend.Application.DTO;
using backend.Application.Services.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionController(ITransactionService transactionService, ICurrentUser currentUser) : ControllerBase
    {
        private readonly ITransactionService _transactionService = transactionService;
        private readonly ICurrentUser _currentUser = currentUser;

        [HttpGet("{mes:int}/{ano:int}")]
        public async Task<ActionResult<List<TransactionResponseDTO>>> GetAllAsync(int mes, int ano)
        {
            if (mes < 1 || mes > 12) return BadRequest("Mes Inválido");

            var transactions = await _transactionService.GetAllTransactionsAsync(_currentUser.UserId, mes, ano);
            if (transactions is null || transactions.Count <= 0)
            {
                return Ok("Nenhuma transação encontrada!");
            }

            return Ok(transactions);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TransactionResponseDTO>> GetByIdAsync(Guid id)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id, _currentUser.UserId);
            if (transaction is null)
            {
                return NotFound("Transação não encontrada!");
            }

            return Ok(transaction);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTransactionDTO createTransactionDTO)
        {
            var id = await _transactionService.CreateTransactionAsync(createTransactionDTO, _currentUser.UserId);
            return CreatedAtAction("GetById", new { id }, new TransactionResponseDTO { Id = id });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromBody] UpdateTransactionDTO updateTransactionDTO, Guid id)
        {
            var updated = await _transactionService.UpdateTransactionAsync(updateTransactionDTO, id, _currentUser.UserId);

            return updated ? NoContent() : NotFound("Transação não encontrada!");
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _transactionService.DeleteTransactionAsync(id, _currentUser.UserId);
            return deleted ? NoContent() : NotFound("Transação não encontrada!");
        }
    }
}