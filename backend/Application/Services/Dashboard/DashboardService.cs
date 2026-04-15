using backend.Application.DTO;
using backend.Domain.Enums;
using backend.Infrastructure.Repositories.Categories;
using backend.Infrastructure.Repositories.Investments;
using backend.Infrastructure.Repositories.PaymentMethods;
using backend.Infrastructure.Repositories.Transactions;

namespace backend.Application.Services.Dashboard
{
    public class DashboardService(
        ITransactionRepository transactionRepository,
        ICategoryRepository categoryRepository,
        IPaymentMethodRepository paymentMethodRepository,
        IInvestmentRepository investmentRepository
    ) : IDashboardService
    {
        private readonly ITransactionRepository _transactionRepository = transactionRepository;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository = paymentMethodRepository;
        private readonly IInvestmentRepository _investmentRepository = investmentRepository;

        private static (int mes, int ano) PreviousPeriod(int mes, int ano)
        {
            if (mes == 1) return (12, ano - 1);
            return (mes - 1, ano);
        }

        public async Task<DashboardMetricsDTO> GetMetricsAsync(Guid userId, int mes, int ano)
        {
            var (prevMes, prevAno) = PreviousPeriod(mes, ano);

            var categorias = await _categoryRepository.GetAllAsync(userId);
            var paymentMethods = await _paymentMethodRepository.GetAllAsync(userId);

            var incomes = await _transactionRepository.GetAllByTypeAsync(userId, mes, ano, TransactionType.INCOME, null);
            var variableExpenses = await _transactionRepository.GetAllByTypeAsync(userId, mes, ano, TransactionType.EXPENSE, ExpenseType.VARIABLE);
            var fixedExpenses = await _transactionRepository.GetAllByTypeAsync(userId, mes, ano, TransactionType.EXPENSE, ExpenseType.FIXED);
            var investments = await _investmentRepository.GetAllAsync(userId, mes, ano);

            var prevIncomes = await _transactionRepository.GetAllByTypeAsync(userId, prevMes, prevAno, TransactionType.INCOME, null);
            var prevVariableExpenses = await _transactionRepository.GetAllByTypeAsync(userId, prevMes, prevAno, TransactionType.EXPENSE, ExpenseType.VARIABLE);
            var prevFixedExpenses = await _transactionRepository.GetAllByTypeAsync(userId, prevMes, prevAno, TransactionType.EXPENSE, ExpenseType.FIXED);
            var prevInvestments = await _investmentRepository.GetAllAsync(userId, prevMes, prevAno);

            var totalEntradas = incomes.Sum(e => e.Amount);
            var totalGastos = variableExpenses.Sum(e => e.Amount);
            var totalContas = fixedExpenses.Sum(e => e.Amount);
            var totalInvestments = investments.Sum(e => e.Amount);

            var saidas = totalGastos + totalContas;

            bool IsDebitoPaymentMethod(Guid? paymentMethodId)
            {
                if (!paymentMethodId.HasValue) return false;
                var method = paymentMethods.FirstOrDefault(m => m.Id == paymentMethodId.Value);
                return method != null && method.Type == PaymentMethodType.DEBITO;
            }

            bool IsCreditoPaymentMethod(Guid? paymentMethodId)
            {
                if (!paymentMethodId.HasValue) return false;
                var method = paymentMethods.FirstOrDefault(m => m.Id == paymentMethodId.Value);
                return method != null && method.Type == PaymentMethodType.CREDITO;
            }

            var usoDebito = variableExpenses.Where(e => IsDebitoPaymentMethod(e.PaymentMethodId)).Sum(e => e.Amount);
            var usoCredito = variableExpenses.Where(e => IsCreditoPaymentMethod(e.PaymentMethodId)).Sum(e => e.Amount);

            var prevEntradas = prevIncomes.Sum(e => e.Amount);
            var prevGastos = prevVariableExpenses.Sum(e => e.Amount);
            var prevContas = prevFixedExpenses.Sum(e => e.Amount);
            var prevReservas = prevInvestments.Sum(e => e.Amount);

            var prevSaidas = prevGastos + prevContas;
            var prevUsoDebito = prevVariableExpenses.Where(e => IsDebitoPaymentMethod(e.PaymentMethodId)).Sum(e => e.Amount);
            var prevUsoCredito = prevVariableExpenses.Where(e => IsCreditoPaymentMethod(e.PaymentMethodId)).Sum(e => e.Amount);

            var fluxoContas = fixedExpenses
                .OrderBy(e => e.Date)
                .Select(e => new DashboardBillFlowRowDTO
                {
                    Nome = e.Description,
                    Valor = e.Amount,
                    Pago = e.IsPaid
                })
                .ToList();

            var alocacaoCategorias = categorias
                .Select(cat => new
                {
                    cat.Name,
                    cat.Color,
                    Total = variableExpenses.Where(e => e.CategoryId == cat.Id).Sum(e => e.Amount)
                })
                .Where(x => x.Total > 0)
                .Select(x => new DashboardCategoryAllocationRowDTO
                {
                    Categoria = x.Name,
                    Valor = x.Total,
                    Cor = x.Color
                })
                .ToList();

            return new DashboardMetricsDTO
            {
                Totais = new DashboardTotalsDTO
                {
                    Entradas = totalEntradas,
                    Saidas = saidas,
                    Reservas = totalInvestments,
                    UsoDebito = usoDebito,
                    UsoCredito = usoCredito,
                    Restante = totalEntradas - saidas - totalInvestments
                },
                ComparacaoMesAnterior = new DashboardComparisonDTO
                {
                    Entradas = prevEntradas,
                    Saidas = prevSaidas,
                    Reservas = prevReservas,
                    UsoDebito = prevUsoDebito,
                    UsoCredito = prevUsoCredito
                },
                Graficos = new DashboardChartsDTO
                {
                    FluxoContas = fluxoContas,
                    AlocacaoCategorias = alocacaoCategorias
                }
            };
        }
    }
}
