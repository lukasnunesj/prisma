using System.ComponentModel.DataAnnotations;
using backend.Domain.Enums;

namespace backend.Application.DTO
{
    public class CreatePaymentMethodDTO
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
        public string Name { get; init; } = null!;

        [Required]
        [EnumDataType(typeof(PaymentMethodType))]
        public PaymentMethodType Type { get; init; }
    }

    public class UpdatePaymentMethodDTO
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
        public string Name { get; init; } = null!;

        [Required]
        [EnumDataType(typeof(PaymentMethodType))]
        public PaymentMethodType Type { get; init; }
    }

    public class PaymentMethodResponseDTO
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public PaymentMethodType Type { get; init; }
    }
}
