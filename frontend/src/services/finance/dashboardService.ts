import type { DashboardMetrics } from '@/types/finance';
import { financeStore } from '@/services/finance/financeStore';

export const dashboardService = {
    metrics: async (mes: number, ano: number): Promise<DashboardMetrics> => {
        await financeStore.delay(300);

        const entradas = financeStore.getEntradas();
        const gastos = financeStore.getGastos();
        const contas = financeStore.getContas();
        const reservas = financeStore.getReservas();
        const categorias = financeStore.getCategorias();
        const formasPagamento = financeStore.getFormasPagamento();

        const isDebitoPaymentMethod = (paymentMethodId: string) => {
            const method = formasPagamento.find((f) => f.id === paymentMethodId);
            if (method) return method.tipo === 'debito';
            return ['1', '3'].includes(paymentMethodId);
        };

        const isCreditoPaymentMethod = (paymentMethodId: string) => {
            const method = formasPagamento.find((f) => f.id === paymentMethodId);
            if (method) return method.tipo === 'credito';
            return ['2', '4'].includes(paymentMethodId);
        };

        const totalEntradas = entradas
            .filter((e) => e.mes === mes && e.ano === ano)
            .reduce((acc, e) => acc + e.valor, 0);

        const totalGastos = gastos
            .filter((g) => g.mes === mes && g.ano === ano)
            .reduce((acc, g) => acc + g.valor, 0);

        const totalContas = contas
            .filter((c) => c.mes === mes && c.ano === ano)
            .reduce((acc, c) => acc + c.valor, 0);

        const totalReservas = reservas
            .filter((r) => r.mes === mes && r.ano === ano)
            .reduce((acc, r) => acc + r.valor, 0);

        const totalDebito = gastos
            .filter((g) => g.mes === mes && g.ano === ano && isDebitoPaymentMethod(g.formaPagamentoId))
            .reduce((acc, g) => acc + g.valor, 0);

        const totalCredito = gastos
            .filter((g) => g.mes === mes && g.ano === ano && isCreditoPaymentMethod(g.formaPagamentoId))
            .reduce((acc, g) => acc + g.valor, 0);

        const saidas = totalGastos + totalContas;

        return {
            totais: {
                entradas: totalEntradas,
                saidas,
                reservas: totalReservas,
                usoDebito: totalDebito,
                usoCredito: totalCredito,
                restante: totalEntradas - saidas - totalReservas,
            },
            comparacaoMesAnterior: {
                entradas: totalEntradas * 0.95,
                saidas: saidas * 1.1,
                reservas: totalReservas * 0.8,
                usoDebito: totalDebito * 0.9,
                usoCredito: totalCredito * 1.05,
            },
            graficos: {
                fluxoContas: contas
                    .filter((c) => c.mes === mes && c.ano === ano)
                    .map((c) => ({
                        nome: c.nome,
                        valor: c.valor,
                        planejado: c.planejado,
                        pago: c.pago,
                    })),
                alocacaoCategorias: categorias
                    .map((cat) => ({
                        categoria: cat.name,
                        valor: gastos
                            .filter((g) => g.mes === mes && g.ano === ano && g.categoriaId === cat.id)
                            .reduce((acc, g) => acc + g.valor, 0),
                        cor: cat.color,
                    }))
                    .filter((c) => c.valor > 0),
            },
        };
    },
};
