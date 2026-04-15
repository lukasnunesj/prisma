using backend.Application.DTO;
using backend.Domain.Entities;
using backend.Infrastructure.Repositories.Investments;

namespace backend.Application.Services.Investments
{
    public class InvestmentService(IInvestmentRepository repository) : IInvestmentService
    {
        private readonly IInvestmentRepository _repository = repository;

        public async Task<IReadOnlyList<InvestmentResponseDTO>> ListAllAsync(Guid userId, int mes, int ano)
        {
            var investments = await _repository.GetAllAsync(userId, mes, ano);

            return investments.Select(e => new InvestmentResponseDTO
            {
                Id = e.Id,
                Name = e.Name,
                Amount = e.Amount,
                Date = e.Date,
                Mes = e.Mes,
                Ano = e.Ano
            }).ToList();
        }

        public async Task<InvestmentResponseDTO?> GetByIdAsync(Guid userId, Guid id)
        {
            var investment = await _repository.GetByIdAsync(id, userId);
            if (investment is null) return null;

            return new InvestmentResponseDTO
            {
                Id = investment.Id,
                Name = investment.Name,
                Amount = investment.Amount,
                Date = investment.Date,
                Mes = investment.Mes,
                Ano = investment.Ano
            };
        }

        public async Task<InvestmentResponseDTO> CreateAsync(Guid userId, int mes, int ano, CreateInvestmentDTO dto)
        {
            var investment = new Investment(userId, dto.Name, dto.Amount, dto.Date, mes, ano);
            await _repository.AddAsync(investment);

            return new InvestmentResponseDTO
            {
                Id = investment.Id,
                Name = investment.Name,
                Amount = investment.Amount,
                Date = investment.Date,
                Mes = investment.Mes,
                Ano = investment.Ano
            };
        }

        public async Task<InvestmentResponseDTO?> UpdateAsync(Guid userId, Guid id, UpdateInvestmentDTO dto)
        {
            var investment = await _repository.GetByIdAsync(id, userId);
            if (investment is null) return null;

            investment.ChangeName(dto.Name);
            investment.ChangeAmount(dto.Amount);
            investment.ChangeDate(dto.Date);

            await _repository.UpdateAsync(investment);

            return new InvestmentResponseDTO
            {
                Id = investment.Id,
                Name = investment.Name,
                Amount = investment.Amount,
                Date = investment.Date,
                Mes = investment.Mes,
                Ano = investment.Ano
            };
        }

        public async Task<bool> DeleteAsync(Guid userId, Guid id)
        {
            var investment = await _repository.GetByIdAsync(id, userId);
            if (investment is null) return false;

            await _repository.DeleteAsync(investment);
            return true;
        }
    }
}
