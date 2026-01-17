using backend.Application.Services.Auth;
using backend.Application.Services.Category;
using backend.Application.Services.Income;
using backend.Application.Services.PaymentMethods;
using backend.Application.Services.Transactions;

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
            services.AddScoped<IIncomeService, IncomeService>();
            services.AddScoped<IPaymentMethodService, PaymentMethodService>();
            services.AddScoped<ITransactionService, TransactionService>();

            return services;
        }
    }
}