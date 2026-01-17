import type { Gasto } from '@/types/finance';
import { financeStore } from '@/services/finance/financeStore';

export const gastosService = {
    list: async (mes: number, ano: number) => {
        return financeStore.getGastos().filter((g) => g.mes === mes && g.ano === ano);
    },

    add: async (data: Omit<Gasto, 'id' | 'mes' | 'ano'>, mes: number, ano: number) => {
        const newGasto: Gasto = {
            ...data,
            id: Date.now().toString(),
            mes,
            ano,
        };

        financeStore.setGastos([...financeStore.getGastos(), newGasto]);
        return newGasto;
    },

    update: async (data: Gasto) => {
        const list = financeStore.getGastos();
        const index = list.findIndex((g) => g.id === data.id);
        if (index !== -1) {
            const updated = [...list];
            updated[index] = data;
            financeStore.setGastos(updated);
        }
        return data;
    },

    remove: async (id: string) => {
        financeStore.setGastos(financeStore.getGastos().filter((g) => g.id !== id));
        return id;
    },
};
