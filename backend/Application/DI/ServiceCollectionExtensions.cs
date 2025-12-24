using backend.Application.Services.Auth;
using backend.Application.Services.Category;

namespace backend.Application.DI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(
        this IServiceCollection services)
        {
            // Registrar Services
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}