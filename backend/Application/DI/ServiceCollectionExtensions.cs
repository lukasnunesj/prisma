using backend.Application.Services.Auth;
using backend.Application.Services.Category;
using backend.Application.Services.Dashboard;
using backend.Application.Services.FixedExpense;
using backend.Application.Services.Income;
using backend.Application.Services.Investments;
using backend.Application.Services.PaymentMethods;
using backend.Application.Services.Transactions;
using backend.Application.Services.VariableExpense;

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
            services.AddScoped<IGoogleTokenValidator, GoogleTokenValidator>();
            services.AddScoped<IIncomeService, IncomeService>();
            services.AddScoped<IPaymentMethodService, PaymentMethodService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IVariableExpenseService, VariableExpenseService>();
            services.AddScoped<IFixedExpenseService, FixedExpenseService>();
            services.AddScoped<IInvestmentService, InvestmentService>();
            services.AddScoped<IDashboardService, DashboardService>();

            return services;
        }
    }
}