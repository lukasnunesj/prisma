import type { FormaPagamento } from '@/types/finance';
import { apiClient } from '@/services/apiClient';
import { financeStore } from '@/services/finance/financeStore';
import type { PaymentMethodApi } from '@/services/finance/paymentMethodMappers';
import { mapFormaPagamentoToPayload, mapPaymentMethodToFormaPagamento } from '@/services/finance/paymentMethodMappers';

export const paymentMethodService = {
    list: async () => {
        const cached = financeStore.getFormasPagamento();
        if (cached.length === 0) {
            const methods = await apiClient.get<PaymentMethodApi[]>('/paymentmethod');
            const mapped = methods.map(mapPaymentMethodToFormaPagamento);
            financeStore.setFormasPagamento(mapped);
            return mapped;
        }

        return cached;
    },

    add: async (data: Omit<FormaPagamento, 'id'>) => {
        const payload = mapFormaPagamentoToPayload(data);
        const created = await apiClient.post<PaymentMethodApi>('/paymentmethod', payload);
        const mapped = mapPaymentMethodToFormaPagamento(created);
        financeStore.setFormasPagamento([...financeStore.getFormasPagamento(), mapped]);
        return mapped;
    },

    update: async (data: FormaPagamento) => {
        const payload = mapFormaPagamentoToPayload({ nome: data.nome, tipo: data.tipo });
        await apiClient.put(`/paymentmethod/${data.id}`, payload);

        financeStore.setFormasPagamento(
            financeStore.getFormasPagamento().map((row) => (row.id === data.id ? data : row)),
        );

        return data;
    },

    remove: async (id: string) => {
        await apiClient.delete(`/paymentmethod/${id}`);
        financeStore.setFormasPagamento(financeStore.getFormasPagamento().filter((f) => f.id !== id));
        return id;
    },
};
