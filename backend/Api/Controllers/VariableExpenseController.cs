using backend.Api.Extensions;
using backend.Api.Services;
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
        public async Task<ActionResult<string>> GetAllAsync(int mes, int ano)
        {
            var expenses = await _variableExpenseService.ListAllVariableExpensesAsync(_currentUser.UserId, mes, ano);
            
            if (expenses is null || expenses.Count <= 0)
            {
                return NotFound("Nenhum Gasto encontrado!");
            }

            return Ok(expenses);

        }
    }
}