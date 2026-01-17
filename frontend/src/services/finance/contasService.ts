import type { Conta } from '@/types/finance';
import { financeStore } from '@/services/finance/financeStore';

export const contasService = {
    list: async (mes: number, ano: number) => {
        return financeStore.getContas().filter((c) => c.mes === mes && c.ano === ano);
    },

    add: async (data: Omit<Conta, 'id' | 'mes' | 'ano'>, mes: number, ano: number) => {
        const newConta: Conta = {
            ...data,
            id: Date.now().toString(),
            mes,
            ano,
        };

        financeStore.setContas([...financeStore.getContas(), newConta]);
        return newConta;
    },

    update: async (data: Conta) => {
        const list = financeStore.getContas();
        const index = list.findIndex((c) => c.id === data.id);
        if (index !== -1) {
            const updated = [...list];
            updated[index] = data;
            financeStore.setContas(updated);
        }
        return data;
    },

    remove: async (id: string) => {
        financeStore.setContas(financeStore.getContas().filter((c) => c.id !== id));
        return id;
    },
};
