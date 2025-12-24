using System.ComponentModel.DataAnnotations;

namespace backend.Application.DTO
{
    public class CreateCategoryDTO
    {

        [Required]
        [MaxLength(200)]
        public string Name { get; init; } = null!;
        public Guid? ParentCategoryId { get; init; }
    }

    public class UpdateCategoryDTO
    {

        [Required]
        [MaxLength(200)]
        public string Name { get; init; } = null!;
        public Guid? ParentCategoryId { get; init; }
    }

    public class CategoryResponseDto
    {

        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public Guid? ParentCategoryId { get; init; }
    }
}