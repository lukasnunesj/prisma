using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Configurations
{
  public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
  {
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {

      // PK
      builder.HasKey(x => x.Id);

      builder.Property(x => x.Id)
          .ValueGeneratedNever();

      // User
      builder.Property(x => x.UserId)
          .IsRequired();

      // Date
      builder.Property(x => x.Date)
          .IsRequired();

      // Amount (dinheiro)
      builder.Property(x => x.Amount)
          .IsRequired()
          .HasColumnType("decimal(18,2)");

      // TransactionType (enum)
      builder.Property(x => x.Type)
          .IsRequired()
          .HasConversion<int>();

      // ExpenseType (enum nullable)
      builder.Property(x => x.ExpenseType)
          .HasConversion<int>()
          .IsRequired(false);

      // Description
      builder.Property(x => x.Description)
          .IsRequired()
          .HasMaxLength(500);

      // Category
      builder.Property(x => x.CategoryId)
          .IsRequired();

      // Índices úteis
      builder.HasIndex(x => x.UserId);
      builder.HasIndex(x => x.CategoryId);
      builder.HasIndex(x => x.Date);

      // Relacionamento com Category
      builder.HasOne<Category>()
          .WithMany()
          .HasForeignKey(e => e.CategoryId)
          .OnDelete(DeleteBehavior.Restrict); // Impede deletar categoria que tem transações

      // Timestamps e Soft Delete
      builder.Property(x => x.CreatedAt).IsRequired();
      builder.Property(x => x.UpdatedAt);
      builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
      builder.Property(x => x.DeletedAt);

      // Índice para melhorar performance de queries com soft delete
      builder.HasIndex(x => x.IsDeleted);

    }
  }
}

