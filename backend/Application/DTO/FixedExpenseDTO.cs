using System.ComponentModel.DataAnnotations;

namespace backend.Application.DTO
{
    public class FixedExpenseResponseDTO
    {
        public Guid Id { get; init; }
        public decimal Amount { get; init; }
        public DateOnly Date { get; init; }
        public string Description { get; init; } = null!;
        public int Mes { get; init; }
        public int Ano { get; init; }
        public bool IsPaid { get; init; }
    }

    public class CreateFixedExpenseDTO
    {
        [Required]
        [Range(typeof(decimal), "0.01", "999999999999.99", ErrorMessage = "Amount deve ser maior que zero")]
        public decimal Amount { get; init; }

        [Required]
        public DateOnly Date { get; init; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(500)]
        [DataType(DataType.Text)]
        public string Description { get; init; } = null!;

        public bool IsPaid { get; init; } = false;
    }

    public class UpdateFixedExpenseDTO
    {
        [Required]
        [Range(typeof(decimal), "0.01", "999999999999.99", ErrorMessage = "Amount deve ser maior que zero")]
        public decimal Amount { get; init; }

        [Required]
        public DateOnly Date { get; init; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(500)]
        [DataType(DataType.Text)]
        public string Description { get; init; } = null!;

        public bool IsPaid { get; init; } = false;
    }
}
