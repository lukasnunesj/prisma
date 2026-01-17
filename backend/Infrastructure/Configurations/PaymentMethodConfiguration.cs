using backend.Domain.Entities;
using backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Configurations
{
    public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.UserId).IsRequired();

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Type)
                .IsRequired()
                .HasConversion<int>();

            builder.HasIndex(e => new { e.UserId, e.Name });

            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.UpdatedAt);
            builder.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);
            builder.Property(e => e.DeletedAt);

            builder.HasIndex(e => e.IsDeleted);
        }
    }
}
