using backend.Application.DTO;

namespace backend.Application.Services.Investments
{
    public interface IInvestmentService
    {
        Task<IReadOnlyList<InvestmentResponseDTO>> ListAllAsync(Guid userId, int mes, int ano);
        Task<InvestmentResponseDTO?> GetByIdAsync(Guid userId, Guid id);
        Task<InvestmentResponseDTO> CreateAsync(Guid userId, int mes, int ano, CreateInvestmentDTO dto);
        Task<InvestmentResponseDTO?> UpdateAsync(Guid userId, Guid id, UpdateInvestmentDTO dto);
        Task<bool> DeleteAsync(Guid userId, Guid id);
    }
}
