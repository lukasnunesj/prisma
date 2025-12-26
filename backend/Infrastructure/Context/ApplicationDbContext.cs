using backend.Domain.Entities;
using backend.Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace backend.Infrastructure.Context
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplicar filtro global de soft delete para entidades que herdam de BaseEntity
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                    var filter = Expression.Lambda(Expression.Equal(property, Expression.Constant(false)), parameter);
                    entityType.SetQueryFilter(filter);
                }
            }

            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
        }

        public override int SaveChanges()
        {
            ApplyTimestamps();
            ApplySoftDelete();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyTimestamps();
            ApplySoftDelete();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    // CreatedAt já é definido no construtor, mas garantimos aqui também
                    if (entity.CreatedAt == default)
                    {
                        entity.CreatedAt = DateTime.UtcNow;
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    // Atualizar UpdatedAt automaticamente
                    entity.MarkAsUpdated();
                }
            }

            // Para User (não herda de BaseEntity)
            var userEntries = ChangeTracker.Entries()
                .Where(e => e.Entity is User && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in userEntries)
            {
                var user = (User)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    if (user.CreatedAt == default)
                    {
                        user.CreatedAt = DateTime.UtcNow;
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    user.MarkAsUpdated();
                }
            }
        }

        private void ApplySoftDelete()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && e.State == EntityState.Deleted);

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;
                entity.MarkAsDeleted();
                entry.State = EntityState.Modified;
            }

            // Para User
            var userEntries = ChangeTracker.Entries()
                .Where(e => e.Entity is User && e.State == EntityState.Deleted);

            foreach (var entry in userEntries)
            {
                var user = (User)entry.Entity;
                user.MarkAsDeleted();
                entry.State = EntityState.Modified;
            }
        }
    }


}
