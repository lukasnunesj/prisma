using backend.Domain.Enums;

namespace backend.Domain.Entities
{
    public class Transaction : BaseEntity
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid UserId { get; private set; }
        public DateOnly Date { get; private set; }
        public decimal Amount { get; private set; }
        public TransactionType Type { get; private set; }
        public ExpenseType? ExpenseType { get; private set; }
        public string Description { get; private set; } = null!;
        public Guid? CategoryId { get; private set; }
        public Guid? PaymentMethodId { get; private set; }
        public PaymentMethod? PaymentMethod { get; private set; }
        public int Mes { get; private set; }
        public int Ano { get; private set; }
        public bool IsPaid { get; private set; } = false;

        protected Transaction()
        {
        } // EF

        public Transaction(
            Guid userId,
            DateOnly date,
            decimal amount,
            TransactionType type,
            ExpenseType? expenseType,
            string description,
            Guid? categoryId,
            Guid? paymentMethodId,
            int mes,
            int ano,
            bool isPaid
        )
        {
            UserId = userId;
            Date = date;
            Amount = amount;
            Type = type;
            ExpenseType = expenseType;
            Description = description;
            CategoryId = categoryId;
            PaymentMethodId = paymentMethodId;
            Mes = mes;
            Ano = ano;
            IsPaid = isPaid;
        }

        public void ChangeDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Descrição não pode ser vazia!");
            Description = description;
            MarkAsUpdated();
        }

        public void ChangeTransactionDate(DateOnly dateTime)
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

        public void ChangeCategory(Guid? categoryId)
        {
            CategoryId = categoryId;
            MarkAsUpdated();
        }

        public void ChangePaymentMethod(Guid? paymentMethodId)
        {
            PaymentMethodId = paymentMethodId;
            MarkAsUpdated();
        }

        public void ChangePeriod(int mes, int ano)
        {
            if (mes < 1 || mes > 12) throw new ArgumentException("Mês Inválido!");
            Mes = mes;
            Ano = ano;
        }

        public void MarkAsPayed()
        {
            IsPaid = true;
            MarkAsUpdated();
        }

        public void MarkAsUnpayed()
        {
            IsPaid = false;
            MarkAsUpdated();
        }

    }
}
