import type { FormaPagamento } from '@/types/finance';

export type PaymentMethodTypeApi = 1 | 2 | 3 | 4;

export type PaymentMethodApi = {
    id: string;
    name: string;
    type: PaymentMethodTypeApi;
};

export const mapPaymentMethodTypeToForma = (type: PaymentMethodTypeApi): FormaPagamento['tipo'] => {
    switch (type) {
        case 1:
            return 'debito';
        case 2:
            return 'credito';
        case 3:
            return 'dinheiro';
        case 4:
            return 'pix';
        default:
            return 'debito';
    }
};

export const mapFormaToPaymentMethodType = (tipo: FormaPagamento['tipo']): PaymentMethodTypeApi => {
    switch (tipo) {
        case 'debito':
            return 1;
        case 'credito':
            return 2;
        case 'dinheiro':
            return 3;
        case 'pix':
            return 4;
        default:
            return 1;
    }
};

export const mapPaymentMethodToFormaPagamento = (method: PaymentMethodApi): FormaPagamento => ({
    id: method.id,
    nome: method.name,
    tipo: mapPaymentMethodTypeToForma(method.type),
});

export const mapFormaPagamentoToPayload = (data: Omit<FormaPagamento, 'id'>) => ({
    name: data.nome,
    type: mapFormaToPaymentMethodType(data.tipo),
});
