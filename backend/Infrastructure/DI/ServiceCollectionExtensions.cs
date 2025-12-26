using backend.Infrastructure.Repositories.Categories;

namespace backend.Infrastructure.DI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services)
        {
            // Registrar repositórios
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            return services;
        }
    }
}