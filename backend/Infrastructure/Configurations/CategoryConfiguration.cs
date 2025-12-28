using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Configurations
{
  public class CategoryConfiguration : IEntityTypeConfiguration<Category>
  {
    public void Configure(EntityTypeBuilder<Category> builder)
    {
      // Configuração da entidade Category        
      builder.HasKey(e => e.Id);
      builder.Property(e => e.Id).ValueGeneratedNever();
      builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
      builder.Property(e => e.Color).IsRequired().HasMaxLength(7);
      builder.Property(e => e.Icon).HasMaxLength(200);
      builder.Property(e => e.UserId).IsRequired();

      // Relacionamento self-referencial para categorias pai/filho
      builder.HasOne<Category>()
          .WithMany()
          .HasForeignKey(e => e.ParentCategoryId)
          .OnDelete(DeleteBehavior.Restrict);

      // Timestamps e Soft Delete
      builder.Property(e => e.CreatedAt).IsRequired();
      builder.Property(e => e.UpdatedAt);
      builder.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);
      builder.Property(e => e.DeletedAt);

      // Índice para melhorar performance de queries com soft delete
      builder.HasIndex(e => e.IsDeleted);
    }
  }
}