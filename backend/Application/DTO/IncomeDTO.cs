using System.ComponentModel.DataAnnotations;

namespace backend.Application.DTO
{
    public class CreateIncomeDTO
    {
        [Required]
        public DateOnly Date { get; init; }

        [Required]
        [Range(typeof(decimal), "0.01", "999999999999.99", ErrorMessage = "Amount deve ser maior que zero")]
        public decimal Amount { get; init; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(500)]
        [DataType(DataType.Text)]
        public string Name { get; init; } = null!;

    }

    public class UpdateIncomeDTO
    {
        [Required]
        public DateOnly Date { get; init; }

        [Required]
        [Range(typeof(decimal), "0.01", "999999999999.99", ErrorMessage = "Amount deve ser maior que zero")]
        public decimal Amount { get; init; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(500)]
        [DataType(DataType.Text)]
        public string Name { get; init; } = null!;
    }

    public class IncomeResponseDTO
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public decimal Amount { get; init; }
        public DateOnly Date { get; init; }
        public int Mes { get; init; }
        public int Ano { get; init; }
    }

}