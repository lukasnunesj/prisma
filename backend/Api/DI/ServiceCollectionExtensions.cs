using backend.Api.Services;
using Microsoft.Extensions.DependencyInjection;

namespace backend.Api.DI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUser, CurrentUser>();

            return services;
        }
    }
}
