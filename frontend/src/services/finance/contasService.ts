import type { Conta } from '@/types/finance';
import { apiClient } from '@/services/apiClient';
import { financeStore } from '@/services/finance/financeStore';

type FixedExpenseApi = {
    id: string;
    amount: number;
    date: string;
    description: string;
    mes: number;
    ano: number;
    isPaid: boolean;
};

const mapFixedExpenseToConta = (row: FixedExpenseApi): Conta => ({
    id: row.id,
    nome: row.description,
    valor: row.amount,
    data: row.date,
    pago: row.isPaid,
    mes: row.mes,
    ano: row.ano,
});

const mapContaToPayload = (row: Omit<Conta, 'id' | 'mes' | 'ano'>) => ({
    description: row.nome,
    amount: row.valor,
    date: row.data,
    isPaid: row.pago,
});

export const contasService = {
    list: async (mes: number, ano: number) => {
        const rows = await apiClient.get<FixedExpenseApi[]>(`/fixedexpense/${mes}/${ano}`);
        const mapped = rows.map(mapFixedExpenseToConta);
        financeStore.setContas(mapped);
        return mapped;
    },

    add: async (data: Omit<Conta, 'id' | 'mes' | 'ano'>, mes: number, ano: number) => {
        const payload = mapContaToPayload(data);
        const created = await apiClient.post<FixedExpenseApi>(`/fixedexpense/${mes}/${ano}`, payload);
        const mapped = mapFixedExpenseToConta(created);
        financeStore.setContas([...financeStore.getContas(), mapped]);
        return mapped;
    },

    update: async (data: Conta) => {
        const payload = mapContaToPayload({
            nome: data.nome,
            valor: data.valor,
            data: data.data,
            pago: data.pago,
        });

        const updated = await apiClient.put<FixedExpenseApi | null>(`/fixedexpense/${data.id}`, payload);
        const resolved = updated ? mapFixedExpenseToConta(updated) : data;
        financeStore.setContas(financeStore.getContas().map((row) => (row.id === data.id ? resolved : row)));
        return resolved;
    },

    remove: async (id: string) => {
        await apiClient.delete(`/fixedexpense/${id}`);
        financeStore.setContas(financeStore.getContas().filter((c) => c.id !== id));
        return id;
    },
};
