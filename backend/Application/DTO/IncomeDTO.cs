namespace backend.Application.DTO
{
    public class AddIncomeDTO
    {
        public string Name { get; init; } = null!;
        public decimal Amount { get; init; }
        public DateTime Date { get; init; }

    }

    public class UpdateIncomeDTO
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public decimal Amount { get; init; }
        public DateTime Date { get; init; }
        public int Mes { get; init; }
        public int Ano { get; init; }
    }

    public class IncomeResponseDTO
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public decimal Amount { get; init; }
        public DateTime Date { get; init; }
        public int Mes { get; init; }
        public int Ano { get; init; }
    }

}