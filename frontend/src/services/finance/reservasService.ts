import type { ReservaInvestimento } from '@/types/finance';
import { financeStore } from '@/services/finance/financeStore';

export const reservasService = {
    list: async (mes: number, ano: number) => {
        return financeStore.getReservas().filter((r) => r.mes === mes && r.ano === ano);
    },

    add: async (data: Omit<ReservaInvestimento, 'id' | 'mes' | 'ano'>, mes: number, ano: number) => {
        const newReserva: ReservaInvestimento = {
            ...data,
            id: Date.now().toString(),
            mes,
            ano,
        };

        financeStore.setReservas([...financeStore.getReservas(), newReserva]);
        return newReserva;
    },

    update: async (data: ReservaInvestimento) => {
        const list = financeStore.getReservas();
        const index = list.findIndex((r) => r.id === data.id);
        if (index !== -1) {
            const updated = [...list];
            updated[index] = data;
            financeStore.setReservas(updated);
        }
        return data;
    },

    remove: async (id: string) => {
        financeStore.setReservas(financeStore.getReservas().filter((r) => r.id !== id));
        return id;
    },
};
