using backend.Application.DTO;

namespace backend.Application.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardMetricsDTO> GetMetricsAsync(Guid userId, int mes, int ano);
    }
}
