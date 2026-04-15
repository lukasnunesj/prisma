using backend.Infrastructure.Repositories.Categories;
using backend.Infrastructure.Repositories.Investments;
using backend.Infrastructure.Repositories.PaymentMethods;
using backend.Infrastructure.Repositories.Transactions;

namespace backend.Infrastructure.DI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services)
        {
            // Registrar repositórios
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IInvestmentRepository, InvestmentRepository>();

            return services;
        }
    }
}