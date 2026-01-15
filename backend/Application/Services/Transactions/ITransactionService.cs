using backend.Application.DTO;

namespace backend.Application.Services.Transactions
{
    public interface ITransactionService
    {
        public Task<IReadOnlyList<TransactionResponseDTO?>> GetAllTransactionsAsync(Guid userId, int mes, int ano);
        public Task<TransactionResponseDTO?> GetTransactionByIdAsync(Guid id, Guid userId);
        public Task<Guid> CreateTransactionAsync(CreateTransactionDTO createTransactionDTO, Guid userId);
        public Task<bool> UpdateTransactionAsync(UpdateTransactionDTO updateTransactionDTO, Guid id, Guid userId);
        public Task<bool> DeleteTransactionAsync(Guid id, Guid userId);
    }
}