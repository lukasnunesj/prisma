using backend.Application.DTO;
using backend.Domain.Entities;
using backend.Domain.Enums;
using backend.Infrastructure.Repositories.Transactions;

namespace backend.Application.Services.FixedExpense
{
    public class FixedExpenseService(ITransactionRepository transactionRepository) : IFixedExpenseService
    {
        private readonly ITransactionRepository _transactionRepository = transactionRepository;

        public async Task<IReadOnlyList<FixedExpenseResponseDTO>> ListAllAsync(Guid userId, int mes, int ano)
        {
            var transactions = await _transactionRepository.GetAllByTypeAsync(userId, mes, ano, TransactionType.EXPENSE, ExpenseType.FIXED);
            return transactions.Select(e => new FixedExpenseResponseDTO
            {
                Id = e.Id,
                Amount = e.Amount,
                Mes = e.Mes,
                Ano = e.Ano,
                Date = e.Date,
                Description = e.Description,
                IsPaid = e.IsPaid
            }).ToList();
        }

        public async Task<FixedExpenseResponseDTO?> GetByIdAsync(Guid userId, Guid id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id, userId);
            if (transaction is null) return null;

            if (transaction.Type != TransactionType.EXPENSE || transaction.ExpenseType != ExpenseType.FIXED)
                return null;

            return new FixedExpenseResponseDTO
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Mes = transaction.Mes,
                Ano = transaction.Ano,
                Date = transaction.Date,
                Description = transaction.Description,
                IsPaid = transaction.IsPaid
            };
        }

        public async Task<FixedExpenseResponseDTO> CreateAsync(Guid userId, int mes, int ano, CreateFixedExpenseDTO dto)
        {
            var transaction = new Transaction(
                userId,
                dto.Date,
                dto.Amount,
                TransactionType.EXPENSE,
                ExpenseType.FIXED,
                dto.Description,
                null,
                null,
                mes,
                ano,
                dto.IsPaid
            );

            await _transactionRepository.AddAsync(transaction);

            return new FixedExpenseResponseDTO
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Mes = transaction.Mes,
                Ano = transaction.Ano,
                Date = transaction.Date,
                Description = transaction.Description,
                IsPaid = transaction.IsPaid
            };
        }

        public async Task<FixedExpenseResponseDTO?> UpdateAsync(Guid userId, Guid id, UpdateFixedExpenseDTO dto)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id, userId);
            if (transaction is null) return null;

            if (transaction.Type != TransactionType.EXPENSE || transaction.ExpenseType != ExpenseType.FIXED)
                return null;

            transaction.ChangeAmount(dto.Amount);
            transaction.ChangeDescription(dto.Description);
            transaction.ChangeTransactionDate(dto.Date);
            if (dto.IsPaid)
                transaction.MarkAsPayed();
            else
                transaction.MarkAsUnpayed();

            await _transactionRepository.UpdateAsync(transaction);

            return new FixedExpenseResponseDTO
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Mes = transaction.Mes,
                Ano = transaction.Ano,
                Date = transaction.Date,
                Description = transaction.Description,
                IsPaid = transaction.IsPaid
            };
        }

        public async Task<bool> DeleteAsync(Guid userId, Guid id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id, userId);
            if (transaction is null) return false;

            if (transaction.Type != TransactionType.EXPENSE || transaction.ExpenseType != ExpenseType.FIXED)
                return false;

            await _transactionRepository.DeleteAsync(transaction);
            return true;
        }
    }
}
