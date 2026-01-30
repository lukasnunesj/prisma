using System.ComponentModel.DataAnnotations;

namespace backend.Application.DTO
{
    public class VariableExpenseResponseDTO
    {
        public Guid Id { get; init; }
        public Guid? CategoryId { get; init; }
        public Guid? PaymentMethodId { get; init; }
        public decimal Amount { get; init; }
        public DateOnly Date { get; init; }
        public string Description { get; init; } = null!;
        public int Mes { get; init; }
        public int Ano { get; init; }
    }

    public class CreateVariableExpanseDTO
    {
        public Guid? CategoryId { get; init; }
        public Guid? PaymentMethodId { get; init; }

        [Required]
        [Range(typeof(decimal), "0.01", "999999999999.99", ErrorMessage = "Amount deve ser maior que zero")]
        public decimal Amount { get; init; }

        [Required]
        public DateOnly Date { get; init; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(500)]
        [DataType(DataType.Text)]
        public string Description { get; init; } = null!;

        [Required]
        public int Mes { get; init; }

        [Required]
        public int Ano { get; init; }
    }

    public class UpdateVariableExpanseDTO
    {
        public Guid? CategoryId { get; init; }
        public Guid? PaymentMethodId { get; init; }

        [Required]
        [Range(typeof(decimal), "0.01", "999999999999.99", ErrorMessage = "Amount deve ser maior que zero")]
        public decimal Amount { get; init; }

        [Required]
        public DateOnly Date { get; init; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(500)]
        [DataType(DataType.Text)]
        public string Description { get; init; } = null!;
    }
}