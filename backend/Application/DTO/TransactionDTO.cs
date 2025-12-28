using System.ComponentModel.DataAnnotations;
using backend.Domain.Enums;

namespace backend.Application.DTO
{
    public class CreateTransactionDTO : IValidatableObject
    {
        [Required]
        public DateTime Date { get; init; }

        [Required]
        [Range(typeof(decimal), "0.01", "999999999999.99", ErrorMessage = "Amount deve ser maior que zero")]
        public decimal Amount { get; init; }

        [Required]
        [EnumDataType(typeof(TransactionType))]
        public TransactionType Type { get; init; }

        [EnumDataType(typeof(ExpenseType))]
        public ExpenseType? ExpenseType { get; init; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(500)]
        [DataType(DataType.Text)]
        public string Description { get; init; } = null!;

        [Required]
        public int Mes { get; init; }
        [Required]
        public int Ano { get; init; }

        public Guid? CategoryId { get; init; }

        public bool IsPaid { get; init; } = false;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Type == TransactionType.EXPENSE && ExpenseType == null)
            {
                yield return new ValidationResult(
                    "ExpenseType é obrigatório quando Type é Expense",
                    [nameof(ExpenseType)]
                );
            }
        }
    }

    public class UpdateTransactionDTO
    {
        [Required]
        public DateTime Date { get; init; }

        [Required]
        [Range(typeof(decimal), "0.01", "999999999999.99", ErrorMessage = "Amount deve ser maior que zero")]
        public decimal Amount { get; init; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(500)]
        [DataType(DataType.Text)]
        public string Description { get; init; } = null!;

        [Required]
        public int Mes { get; init; }
        [Required]
        public int Ano { get; init; }

        [Required]
        public Guid? CategoryId { get; init; }

        public bool IsPaid { get; init; } = false;

    }

    public class TransactionResponseDTO
    {
        public Guid Id { get; init; }
        public DateTime Date { get; init; }
        public decimal Amount { get; init; }
        public TransactionType Type { get; init; }
        public ExpenseType? ExpenseType { get; init; }
        public string Description { get; init; } = null!;
        public Guid? CategoryId { get; init; }
        public int Mes { get; init; }
        public int Ano { get; init; }
        public bool IsPaid { get; init; } = false;
    }
}