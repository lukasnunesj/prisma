using System.ComponentModel.DataAnnotations;

namespace backend.Application.DTO
{
    public class CreateCategoryDTO
    {

        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
        public string Name { get; init; } = null!;

        [Required]
        [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Color must be in HEX format (#RRGGBB).")]
        [StringLength(7)]
        public string Color { get; init; } = null!;

        public string? Icon { get; init; }
        public Guid? ParentCategoryId { get; init; }
    }

    public class UpdateCategoryDTO
    {

        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
        public string Name { get; init; } = null!;

        [Required]
        [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Color must be in HEX format (#RRGGBB).")]
        [StringLength(7)]
        public string Color { get; init; } = null!;

        public string? Icon { get; init; }
    }

    public class CategoryResponseDto
    {

        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public string Color { get; init; } = null!;
        public string? Icon { get; init; }
        public Guid? ParentCategoryId { get; init; }
    }
}