using System.ComponentModel.DataAnnotations;

namespace backend.Application.DTO
{
    public class InvestmentResponseDTO
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public decimal Amount { get; init; }
        public DateOnly Date { get; init; }
        public int Mes { get; init; }
        public int Ano { get; init; }
    }

    public class CreateInvestmentDTO
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(200)]
        public string Name { get; init; } = null!;

        [Required]
        [Range(typeof(decimal), "0.01", "999999999999.99", ErrorMessage = "Amount deve ser maior que zero")]
        public decimal Amount { get; init; }

        [Required]
        public DateOnly Date { get; init; }
    }

    public class UpdateInvestmentDTO
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(200)]
        public string Name { get; init; } = null!;

        [Required]
        [Range(typeof(decimal), "0.01", "999999999999.99", ErrorMessage = "Amount deve ser maior que zero")]
        public decimal Amount { get; init; }

        [Required]
        public DateOnly Date { get; init; }
    }
}
