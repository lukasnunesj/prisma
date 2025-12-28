import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuthStore, useMonthNavigationStore } from '@/stores/appStore';
import { useDashboardMetrics } from '@/hooks/useFinancialData';
import { getCurrentDateMessage, getMonthName } from '@/utils/formatters';
import { Header } from '@/components/layout/Header';
import { MonthNavigator } from '@/components/layout/MonthNavigator';
import { MetricCard } from '@/components/dashboard/MetricCard';
import { DonutChart } from '@/components/dashboard/DonutChart';
import { FluxoContasChart } from '@/components/dashboard/FluxoContasChart';
import { AlocacaoCategoriasChart } from '@/components/dashboard/AlocacaoCategoriasChart';
import { EntradasTable } from '@/components/tables/EntradasTable';
import { GastosTable } from '@/components/tables/GastosTable';
import { ContasTable } from '@/components/tables/ContasTable';
import { ReservasTable } from '@/components/tables/ReservasTable';
import { 
  TrendingUp, 
  TrendingDown, 
  PiggyBank, 
  CreditCard, 
  Wallet,
  Sparkles 
} from 'lucide-react';

const Dashboard = () => {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuthStore();
  const { selectedMonth, selectedYear } = useMonthNavigationStore();
  const { data: metrics, isLoading: metricsLoading } = useDashboardMetrics();

  useEffect(() => {
    if (!isAuthenticated) {
      navigate('/login');
    }
  }, [isAuthenticated, navigate]);

  if (!isAuthenticated) {
    return null;
  }

  return (
    <div className="h-screen w-screen flex flex-col overflow-hidden bg-background">
      <Header />
      
      {/* Main Content Area */}
      <main className="flex-1 overflow-y-auto custom-scrollbar mt-16 mb-14">
        <div className="max-w-[1920px] mx-auto px-6 py-6 space-y-6">
          {/* Welcome Section */}
          <div className="space-y-2">
            <div className="flex items-center gap-2">
              <Sparkles className="w-5 h-5 text-primary" />
              <h1 className="text-2xl font-bold text-foreground">
                {getMonthName(selectedMonth)} {selectedYear}
              </h1>
            </div>
            <p className="text-muted-foreground">{getCurrentDateMessage()}</p>
            <p className="text-sm text-primary font-medium">
              Pequenas ações geram grandes resultados ✨
            </p>
          </div>

          {/* Metrics Cards */}
          <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-4">
            <MetricCard
              title="Total Entradas"
              value={metrics?.totais.entradas ?? 0}
              previousValue={metrics?.comparacaoMesAnterior.entradas}
              colorScheme="success"
              delay={0}
              icon={<TrendingUp className="w-4 h-4" />}
            />
            <MetricCard
              title="Total Saídas"
              value={metrics?.totais.saidas ?? 0}
              previousValue={metrics?.comparacaoMesAnterior.saidas}
              colorScheme="danger"
              delay={1}
              icon={<TrendingDown className="w-4 h-4" />}
            />
            <MetricCard
              title="Reservas & Invest."
              value={metrics?.totais.reservas ?? 0}
              previousValue={metrics?.comparacaoMesAnterior.reservas}
              colorScheme="default"
              delay={2}
              icon={<PiggyBank className="w-4 h-4" />}
            />
            <MetricCard
              title="Uso do Débito"
              value={metrics?.totais.usoDebito ?? 0}
              previousValue={metrics?.comparacaoMesAnterior.usoDebito}
              colorScheme="default"
              delay={3}
              icon={<Wallet className="w-4 h-4" />}
            />
            <MetricCard
              title="Uso do Crédito"
              value={metrics?.totais.usoCredito ?? 0}
              previousValue={metrics?.comparacaoMesAnterior.usoCredito}
              colorScheme="warning"
              delay={4}
              icon={<CreditCard className="w-4 h-4" />}
            />
          </div>

          {/* Charts */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <DonutChart
              title="Restante para Gastar"
              value={metrics?.totais.restante ?? 0}
              maxValue={metrics?.totais.entradas ?? 0}
              description="Baseado nas entradas do mês"
            />
            <FluxoContasChart
              data={metrics?.graficos.fluxoContas ?? []}
            />
            <AlocacaoCategoriasChart
              data={metrics?.graficos.alocacaoCategorias ?? []}
            />
          </div>

          {/* Data Tables - 2x2 Grid */}
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
            <EntradasTable />
            <GastosTable />
            <ContasTable />
            <ReservasTable />
          </div>
        </div>
      </main>

      <MonthNavigator />
    </div>
  );
};

export default Dashboard;
