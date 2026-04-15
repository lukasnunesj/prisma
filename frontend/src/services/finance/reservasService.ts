import type { ReservaInvestimento } from '@/types/finance';
import { apiClient } from '@/services/apiClient';
import { financeStore } from '@/services/finance/financeStore';

type InvestmentApi = {
    id: string;
    name: string;
    amount: number;
    date: string;
    mes: number;
    ano: number;
};

const mapInvestmentToReserva = (row: InvestmentApi): ReservaInvestimento => ({
    id: row.id,
    nome: row.name,
    valor: row.amount,
    data: row.date,
    mes: row.mes,
    ano: row.ano,
});

const mapReservaToPayload = (row: Omit<ReservaInvestimento, 'id' | 'mes' | 'ano'>) => ({
    name: row.nome,
    amount: row.valor,
    date: row.data,
});

export const reservasService = {
    list: async (mes: number, ano: number) => {
        const rows = await apiClient.get<InvestmentApi[]>(`/investment/${mes}/${ano}`);
        const mapped = rows.map(mapInvestmentToReserva);
        financeStore.setReservas(mapped);
        return mapped;
    },

    add: async (data: Omit<ReservaInvestimento, 'id' | 'mes' | 'ano'>, mes: number, ano: number) => {
        const payload = mapReservaToPayload(data);
        const created = await apiClient.post<InvestmentApi>(`/investment/${mes}/${ano}`, payload);
        const mapped = mapInvestmentToReserva(created);
        financeStore.setReservas([...financeStore.getReservas(), mapped]);
        return mapped;
    },

    update: async (data: ReservaInvestimento) => {
        const payload = mapReservaToPayload({
            nome: data.nome,
            valor: data.valor,
            data: data.data,
        });
        const updated = await apiClient.put<InvestmentApi | null>(`/investment/${data.id}`, payload);
        const resolved = updated ? mapInvestmentToReserva(updated) : data;
        financeStore.setReservas(financeStore.getReservas().map((row) => (row.id === data.id ? resolved : row)));
        return resolved;
    },

    remove: async (id: string) => {
        await apiClient.delete(`/investment/${id}`);
        financeStore.setReservas(financeStore.getReservas().filter((r) => r.id !== id));
        return id;
    },
};
