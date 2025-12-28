using backend.Application.DTO;
using backend.Domain.Entities;
using backend.Infrastructure.Repositories.Transactions;

namespace backend.Application.Services.Income
{
    public class IncomeService(ITransactionRepository transactionRepository) : IIncomeService
    {
        private readonly ITransactionRepository _transactionRepository = transactionRepository;

        public async Task<IncomeResponseDTO> AddIncomeAsync(Guid userId, int mes, int ano, AddIncomeDTO addIncomeDTO)
        {
            var transaction = new Transaction(
                userId,
                addIncomeDTO.Date,
                addIncomeDTO.Amount,
                Domain.Enums.TransactionType.INCOME,
                null,
                addIncomeDTO.Name,
                null,
                mes,
                ano,
                false
            );

            await _transactionRepository.AddAsync(transaction);
            return new IncomeResponseDTO
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Mes = transaction.Mes,
                Ano = transaction.Ano,
                Date = transaction.Date,
                Name = transaction.Description
            };
        }

        public async Task<IncomeResponseDTO?> GetIncomeByIdAsync(Guid userId, Guid id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id, userId);
            if (transaction is null) return null;

            return new IncomeResponseDTO
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Mes = transaction.Mes,
                Ano = transaction.Ano,
                Date = transaction.Date,
                Name = transaction.Description
            };
        }

        public async Task<IReadOnlyList<IncomeResponseDTO>> ListAllIncomeAsync(Guid userId)
        {
            var transactions = await _transactionRepository.GetAllAsync(userId);
            return [.. transactions.Select(e=>new IncomeResponseDTO{
                Id = e.Id,
                Amount = e.Amount,
                Mes = e.Mes,
                Ano = e.Ano,
                Date = e.Date,
                Name = e.Description
            })];
        }

        public async Task<bool> RemoveIncomeAsync(Guid userId, Guid id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id, userId);
            if (transaction is null) return false;

            await _transactionRepository.DeleteAsync(transaction);
            return true;
        }

        public async Task<IncomeResponseDTO?> UdateIncomeAsync(Guid userId, UpdateIncomeDTO updateIncomeDTO)
        {
            var transaction = await _transactionRepository.GetByIdAsync(updateIncomeDTO.Id, userId);
            if (transaction is null) return null;

            transaction.ChangeDescription(updateIncomeDTO.Name);
            transaction.ChangeTransactionDate(updateIncomeDTO.Date);
            transaction.ChangeAmount(updateIncomeDTO.Amount);
            transaction.ChangePeriod(updateIncomeDTO.Mes, updateIncomeDTO.Ano);

            await _transactionRepository.UpdateAsync(transaction);
            return new IncomeResponseDTO
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Mes = transaction.Mes,
                Ano = transaction.Ano,
                Date = transaction.Date,
                Name = transaction.Description
            };
        }
    }
}