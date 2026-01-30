import type { Gasto } from '@/types/finance';
import { apiClient } from '@/services/apiClient';
import { financeStore } from '@/services/finance/financeStore';
import { formatDateInput } from '@/utils/formatters';

type VariableExpenseResponse = {
    id: string;
    categoryId: string | null;
    paymentMethodId: string | null;
    amount: number;
    date: string;
    description: string;
    mes: number;
    ano: number;
};

type CreateVariableExpensePayload = {
    categoryId: string | null;
    paymentMethodId: string | null;
    amount: number;
    date: string;
    description: string;
    mes: number;
    ano: number;
};

type UpdateVariableExpensePayload = {
    categoryId: string | null;
    paymentMethodId: string | null;
    amount: number;
    date: string;
    description: string;
};

const mapExpenseToGasto = (expense: VariableExpenseResponse): Gasto => ({
    id: expense.id,
    categoriaId: expense.categoryId ?? '',
    formaPagamentoId: expense.paymentMethodId ?? '',
    valor: expense.amount,
    data: formatDateInput(expense.date),
    descricao: expense.description,
    mes: expense.mes,
    ano: expense.ano,
});

const mapGastoToCreatePayload = (data: Omit<Gasto, 'id' | 'mes' | 'ano'>, mes: number, ano: number): CreateVariableExpensePayload => ({
    categoryId: data.categoriaId?.trim() ? data.categoriaId : null,
    paymentMethodId: data.formaPagamentoId?.trim() ? data.formaPagamentoId : null,
    amount: data.valor,
    date: data.data,
    description: data.descricao ?? '',
    mes,
    ano,
});

const mapGastoToUpdatePayload = (data: Gasto): UpdateVariableExpensePayload => ({
    categoryId: data.categoriaId?.trim() ? data.categoriaId : null,
    paymentMethodId: data.formaPagamentoId?.trim() ? data.formaPagamentoId : null,
    amount: data.valor,
    date: data.data,
    description: data.descricao ?? '',
});

export const gastosService = {
    list: async (mes: number, ano: number) => {
        try {
            const expenses = await apiClient.get<VariableExpenseResponse[]>(`/variableexpense/${mes}/${ano}`);
            const mapped = expenses.map(mapExpenseToGasto);
            financeStore.setGastos(mapped);
            return mapped;
        } catch (error) {
            if (error instanceof Error && error.message.includes('Nenhum Gasto')) {
                financeStore.setGastos([]);
                return [];
            }
            throw error;
        }
    },

    add: async (data: Omit<Gasto, 'id' | 'mes' | 'ano'>, mes: number, ano: number) => {
        const payload = mapGastoToCreatePayload(data, mes, ano);
        const created = await apiClient.post<VariableExpenseResponse>(`/variableexpense/${mes}/${ano}`, payload);
        const newGasto = mapExpenseToGasto(created);
        financeStore.setGastos([...financeStore.getGastos(), newGasto]);
        return newGasto;
    },

    update: async (data: Gasto) => {
        const payload = mapGastoToUpdatePayload(data);

        const updated = await apiClient.put<VariableExpenseResponse | null>(`/variableexpense/${data.id}`, payload);
        if (!updated) {
            return data;
        }

        const mapped = mapExpenseToGasto(updated);
        financeStore.setGastos(financeStore.getGastos().map((row) => (row.id === mapped.id ? mapped : row)));
        return mapped;
    },

    remove: async (id: string) => {
        await apiClient.delete(`/variableexpense/${id}`);
        financeStore.setGastos(financeStore.getGastos().filter((g) => g.id !== id));
        return id;
    },
};
