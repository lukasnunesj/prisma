namespace backend.Application.DTO
{
    public class DashboardMetricsDTO
    {
        public DashboardTotalsDTO Totais { get; init; } = new();
        public DashboardComparisonDTO ComparacaoMesAnterior { get; init; } = new();
        public DashboardChartsDTO Graficos { get; init; } = new();
    }

    public class DashboardTotalsDTO
    {
        public decimal Entradas { get; init; }
        public decimal Saidas { get; init; }
        public decimal Reservas { get; init; }
        public decimal UsoDebito { get; init; }
        public decimal UsoCredito { get; init; }
        public decimal Restante { get; init; }
    }

    public class DashboardComparisonDTO
    {
        public decimal Entradas { get; init; }
        public decimal Saidas { get; init; }
        public decimal Reservas { get; init; }
        public decimal UsoDebito { get; init; }
        public decimal UsoCredito { get; init; }
    }

    public class DashboardChartsDTO
    {
        public List<DashboardBillFlowRowDTO> FluxoContas { get; init; } = [];
        public List<DashboardCategoryAllocationRowDTO> AlocacaoCategorias { get; init; } = [];
    }

    public class DashboardBillFlowRowDTO
    {
        public string Nome { get; init; } = null!;
        public decimal Valor { get; init; }
        public bool Pago { get; init; }
    }

    public class DashboardCategoryAllocationRowDTO
    {
        public string Categoria { get; init; } = null!;
        public decimal Valor { get; init; }
        public string Cor { get; init; } = null!;
    }
}
