namespace backend.Domain.Entities
{
    public class Investment : BaseEntity
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid UserId { get; private set; }
        public string Name { get; private set; } = null!;
        public decimal Amount { get; private set; }
        public DateOnly Date { get; private set; }
        public int Mes { get; private set; }
        public int Ano { get; private set; }

        protected Investment() { }

        public Investment(Guid userId, string name, decimal amount, DateOnly date, int mes, int ano)
        {
            UserId = userId;
            ChangeName(name);
            ChangeAmount(amount);
            ChangeDate(date);
            ChangePeriod(mes, ano);
        }

        public void ChangeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome não pode ser vazio!");

            Name = name;
            MarkAsUpdated();
        }

        public void ChangeAmount(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Valor não pode ser zero ou negativo!");

            Amount = amount;
            MarkAsUpdated();
        }

        public void ChangeDate(DateOnly date)
        {
            Date = date;
            MarkAsUpdated();
        }

        public void ChangePeriod(int mes, int ano)
        {
            if (mes < 1 || mes > 12) throw new ArgumentException("Mês Inválido!");
            Mes = mes;
            Ano = ano;
            MarkAsUpdated();
        }
    }
}
