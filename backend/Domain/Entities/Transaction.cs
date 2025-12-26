using backend.Domain.Enums;

namespace backend.Domain.Entities
{
    public class Transaction : BaseEntity
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid UserId { get; private set; }
        public DateTime Date { get; private set; }
        public decimal Amount { get; private set; }
        public TransactionType Type { get; private set; }
        public ExpenseType? ExpenseType { get; private set; }
        public string Description { get; private set; } = null!;
        public Guid CategoryId { get; private set; }

        protected Transaction()
        {
        } // EF

        public Transaction(
            Guid userId,
            DateTime date,
            decimal amount,
            TransactionType type,
            ExpenseType? expenseType,
            string description,
            Guid categoryId
        )
        {
            UserId = userId;
            Date = date;
            Amount = amount;
            Type = type;
            ExpenseType = expenseType;
            Description = description;
            CategoryId = categoryId;
        }

        public void ChangeDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Descrição não pode ser vazia!");
            Description = description;
            MarkAsUpdated();
        }

        public void ChangeTransactionDate(DateTime dateTime)
        {
            Date = dateTime;
            MarkAsUpdated();
        }

        public void ChangeAmount(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Quantidade não pode ser zero ou negativo!");

            Amount = amount;
            MarkAsUpdated();
        }

        public void ChangeCategory(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
                throw new ArgumentException("Categoria inválida.");

            CategoryId = categoryId;
            MarkAsUpdated();
        }

    }
}
