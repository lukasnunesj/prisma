using backend.Api.Services;
using backend.Application.DTO;
using backend.Application.Services.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController(IDashboardService dashboardService, ICurrentUser currentUser) : ControllerBase
    {
        private readonly IDashboardService _dashboardService = dashboardService;
        private readonly ICurrentUser _currentUser = currentUser;

        [HttpGet("metrics/{mes:int}/{ano:int}")]
        public async Task<ActionResult<DashboardMetricsDTO>> GetMetricsAsync(int mes, int ano)
        {
            if (mes < 1 || mes > 12) return BadRequest("Mes Inválido");

            var metrics = await _dashboardService.GetMetricsAsync(_currentUser.UserId, mes, ano);
            return Ok(metrics);
        }
    }
}
