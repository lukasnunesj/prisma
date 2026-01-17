import type { Entrada } from '@/types/finance';
import { apiClient } from '@/services/apiClient';
import { formatDateInput } from '@/utils/formatters';
import { financeStore } from '@/services/finance/financeStore';

type IncomeResponse = {
    id: string;
    name: string;
    amount: number;
    date: string;
    mes: number;
    ano: number;
};

const mapIncomeToEntrada = (income: IncomeResponse): Entrada => ({
    id: income.id,
    nome: income.name,
    valor: income.amount,
    data: formatDateInput(income.date),
    mes: income.mes,
    ano: income.ano,
});

const mapEntradaToIncomePayload = (data: Omit<Entrada, 'id' | 'mes' | 'ano'>) => ({
    name: data.nome,
    amount: data.valor,
    date: data.data,
});

export const incomeService = {
    list: async (mes: number, ano: number) => {
        try {
            const income = await apiClient.get<IncomeResponse[]>(`/income/${mes}/${ano}`);
            const mapped = income.map(mapIncomeToEntrada);
            financeStore.setEntradas(mapped);
            return mapped;
        } catch (error) {
            if (error instanceof Error && error.message.includes('Nenhuma Entrada')) {
                financeStore.setEntradas([]);
                return [];
            }
            throw error;
        }
    },

    add: async (data: Omit<Entrada, 'id' | 'mes' | 'ano'>, mes: number, ano: number) => {
        const payload = mapEntradaToIncomePayload(data);
        const income = await apiClient.post<IncomeResponse>(`/income/${mes}/${ano}`, payload);
        const newEntrada = mapIncomeToEntrada(income);
        financeStore.setEntradas([...financeStore.getEntradas(), newEntrada]);
        return newEntrada;
    },

    update: async (data: Entrada) => {
        const payload = mapEntradaToIncomePayload(data);

        const updated = await apiClient.put<IncomeResponse | null>(`/income/${data.id}`, payload);
        if (!updated) {
            return data;
        }

        const mapped = mapIncomeToEntrada(updated);
        financeStore.setEntradas(financeStore.getEntradas().map((row) => (row.id === mapped.id ? mapped : row)));
        return mapped;
    },

    remove: async (id: string) => {
        await apiClient.delete(`/income/${id}`);
        financeStore.setEntradas(financeStore.getEntradas().filter((e) => e.id !== id));
        return id;
    },
};
