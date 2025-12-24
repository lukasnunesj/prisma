using backend.Domain.Enums;

namespace backend.Domain.Entities
{
    public class Transaction(
        Guid UserId,
        DateTime Date,
        decimal Amount,
        TransactionType Type,
        ExpenseType? ExpenseType,
        string Description,
        Guid CategoryId
    )
    {
        private Guid Id { get; set; } = Guid.NewGuid();
        private Guid UserId { get; set; } = UserId;
        private DateTime Date { get; set; } = Date;
        private decimal Amount { get; set; } = Amount;
        private TransactionType Type { get; set; } = Type;
        private ExpenseType? ExpenseType { get; set; } = ExpenseType;
        private string Description { get; set; } = Description;
        private Guid CategoryId { get; set; } = CategoryId;
    }
}