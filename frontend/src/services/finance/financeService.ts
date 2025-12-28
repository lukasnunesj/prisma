import {
    mockEntradas,
    mockGastos,
    mockContas,
    mockReservas,
    mockFormasPagamento,
} from '@/data/mockData';
import type {
    Entrada,
    Gasto,
    Conta,
    ReservaInvestimento,
    Categoria,
    FormaPagamento,
    DashboardMetrics,
} from '@/types/finance';
import { apiClient } from '@/services/apiClient';

const delay = (ms: number) => new Promise((resolve) => setTimeout(resolve, ms));

let entradas = [...mockEntradas];
let gastos = [...mockGastos];
let contas = [...mockContas];
let reservas = [...mockReservas];
let categorias: Categoria[] = [];
let formasPagamento = [...mockFormasPagamento];

const fetchDashboardMetrics = async (mes: number, ano: number): Promise<DashboardMetrics> => {
    await delay(300);

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
        .filter((g) => g.mes === mes && g.ano === ano && ['1', '3'].includes(g.formaPagamentoId))
        .reduce((acc, g) => acc + g.valor, 0);

    const totalCredito = gastos
        .filter((g) => g.mes === mes && g.ano === ano && ['2', '4'].includes(g.formaPagamentoId))
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
};

export const financeService = {
    entradas: {
        list: async (mes: number, ano: number) => {
            return entradas.filter((e) => e.mes === mes && e.ano === ano);
        },
        add: async (data: Omit<Entrada, 'id' | 'mes' | 'ano'>, mes: number, ano: number) => {
            const newEntrada: Entrada = {
                ...data,
                id: Date.now().toString(),
                mes,
                ano,
            };
            entradas.push(newEntrada);
            return newEntrada;
        },
        update: async (data: Entrada) => {
            const index = entradas.findIndex((e) => e.id === data.id);
            if (index !== -1) entradas[index] = data;
            return data;
        },
        remove: async (id: string) => {
            entradas = entradas.filter((e) => e.id !== id);
            return id;
        },
    },

    gastos: {
        list: async (mes: number, ano: number) => {
            return gastos.filter((g) => g.mes === mes && g.ano === ano);
        },
        add: async (data: Omit<Gasto, 'id' | 'mes' | 'ano'>, mes: number, ano: number) => {
            const newGasto: Gasto = {
                ...data,
                id: Date.now().toString(),
                mes,
                ano,
            };
            gastos.push(newGasto);
            return newGasto;
        },
        update: async (data: Gasto) => {
            const index = gastos.findIndex((g) => g.id === data.id);
            if (index !== -1) gastos[index] = data;
            return data;
        },
        remove: async (id: string) => {
            gastos = gastos.filter((g) => g.id !== id);
            return id;
        },
    },

    contas: {
        list: async (mes: number, ano: number) => {
            return contas.filter((c) => c.mes === mes && c.ano === ano);
        },
        add: async (data: Omit<Conta, 'id' | 'mes' | 'ano'>, mes: number, ano: number) => {
            const newConta: Conta = {
                ...data,
                id: Date.now().toString(),
                mes,
                ano,
            };
            contas.push(newConta);
            return newConta;
        },
        update: async (data: Conta) => {
            const index = contas.findIndex((c) => c.id === data.id);
            if (index !== -1) contas[index] = data;
            return data;
        },
        remove: async (id: string) => {
            contas = contas.filter((c) => c.id !== id);
            return id;
        },
    },

    reservas: {
        list: async (mes: number, ano: number) => {
            return reservas.filter((r) => r.mes === mes && r.ano === ano);
        },
        add: async (data: Omit<ReservaInvestimento, 'id' | 'mes' | 'ano'>, mes: number, ano: number) => {
            const newReserva: ReservaInvestimento = {
                ...data,
                id: Date.now().toString(),
                mes,
                ano,
            };
            reservas.push(newReserva);
            return newReserva;
        },
        update: async (data: ReservaInvestimento) => {
            const index = reservas.findIndex((r) => r.id === data.id);
            if (index !== -1) reservas[index] = data;
            return data;
        },
        remove: async (id: string) => {
            reservas = reservas.filter((r) => r.id !== id);
            return id;
        },
    },


    categorias: {
        list: async () => {
            if (categorias.length === 0) {
                categorias = await apiClient.get<Categoria[]>('/category');
            }
            return categorias;
        },
        add: async (data: Omit<Categoria, 'id'>) => {
            const newCategoria = await apiClient.post<Categoria>('/category', data);
            categorias.push(newCategoria);
            return newCategoria;
        },
        update: async (data: Categoria) => {
            const updated = await apiClient.put<Categoria>(`/category/${data.id}`, data);
            const index = categorias.findIndex((c) => c.id === data.id);
            if (index !== -1) categorias[index] = updated;
            return updated;
        },
        remove: async (id: string) => {
            await apiClient.delete(`/category/${id}`);
            categorias = categorias.filter((c) => c.id !== id);
            return id;
        },
    },

    formasPagamento: {
        list: async () => {
            return formasPagamento;
        },
        add: async (data: Omit<FormaPagamento, 'id'>) => {
            const newForma: FormaPagamento = {
                ...data,
                id: Date.now().toString(),
            };
            formasPagamento.push(newForma);
            return newForma;
        },
        update: async (data: FormaPagamento) => {
            const index = formasPagamento.findIndex((f) => f.id === data.id);
            if (index !== -1) formasPagamento[index] = data;
            return data;
        },
        remove: async (id: string) => {
            formasPagamento = formasPagamento.filter((f) => f.id !== id);
            return id;
        },
    },

    dashboard: {
        metrics: fetchDashboardMetrics,
    },
};
