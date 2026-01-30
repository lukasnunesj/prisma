using backend.Application.DTO;
using backend.Domain.Entities;
using backend.Infrastructure.Repositories.Transactions;

namespace backend.Application.Services.Transactions
{
    public class TransactionService(ITransactionRepository transactionRepository) : ITransactionService
    {
        private ITransactionRepository _transactionRepository = transactionRepository;

        public async Task<Guid> CreateTransactionAsync(CreateTransactionDTO createTransactionDTO, Guid userId)
        {
            var transaction = new Transaction(
                userId,
                createTransactionDTO.Date,
                createTransactionDTO.Amount,
                createTransactionDTO.Type,
                createTransactionDTO.ExpenseType,
                createTransactionDTO.Description,
                createTransactionDTO.CategoryId,
                createTransactionDTO.PaymentMethodId,
                createTransactionDTO.Mes,
                createTransactionDTO.Ano,
                createTransactionDTO.IsPaid
            );

            await _transactionRepository.AddAsync(transaction);
            return transaction.Id;
        }

        public async Task<bool> DeleteTransactionAsync(Guid id, Guid userId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id, userId);
            if (transaction is null) return false;

            await _transactionRepository.DeleteAsync(transaction);
            return true;
        }

        public async Task<IReadOnlyList<TransactionResponseDTO?>> GetAllTransactionsAsync(Guid userId, int mes, int ano)
        {
            var transactions = await _transactionRepository.GetAllAsync(userId, mes, ano);

            return [..
                transactions.Select(e => new TransactionResponseDTO
                {
                    Id = e.Id,
                    Amount = e.Amount,
                    CategoryId = e.CategoryId,
                    PaymentMethodId = e.PaymentMethodId,
                    Date = e.Date,
                    Description = e.Description,
                    ExpenseType = e.ExpenseType,
                    Type = e.Type,
                    IsPaid = e.IsPaid,
                    Mes = e.Mes,
                    Ano = e.Ano
                })
            ];
        }

        public async Task<TransactionResponseDTO?> GetTransactionByIdAsync(Guid id, Guid userId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id, userId);

            return transaction is null ? null : new TransactionResponseDTO
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                CategoryId = transaction.CategoryId,
                PaymentMethodId = transaction.PaymentMethodId,
                Date = transaction.Date,
                Description = transaction.Description,
                ExpenseType = transaction.ExpenseType,
                Type = transaction.Type,
                IsPaid = transaction.IsPaid,
                Mes = transaction.Mes,
                Ano = transaction.Ano
            };
        }

        public async Task<bool> UpdateTransactionAsync(UpdateTransactionDTO updateTransactionDTO, Guid id, Guid userId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id, userId);
            if (transaction is null) return false;

            transaction.ChangeAmount(updateTransactionDTO.Amount);
            transaction.ChangeCategory(updateTransactionDTO.CategoryId);
            transaction.ChangeDescription(updateTransactionDTO.Description);
            transaction.ChangeTransactionDate(updateTransactionDTO.Date);
            transaction.ChangePaymentMethod(updateTransactionDTO.PaymentMethodId);
            if (updateTransactionDTO.IsPaid)
                transaction.MarkAsPayed();
            else
                transaction.MarkAsUnpayed();
            transaction.ChangePeriod(updateTransactionDTO.Mes, updateTransactionDTO.Ano);
            await _transactionRepository.UpdateAsync(transaction);
            return true;
        }
    }
}