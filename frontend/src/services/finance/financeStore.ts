import { mockEntradas, mockGastos, mockContas, mockReservas } from '@/data/mockData';
import type { Categoria, Conta, Entrada, FormaPagamento, Gasto, ReservaInvestimento } from '@/types/finance';

const delay = (ms: number) => new Promise((resolve) => setTimeout(resolve, ms));

let entradas: Entrada[] = [...mockEntradas];
let gastos: Gasto[] = [...mockGastos];
let contas: Conta[] = [...mockContas];
let reservas: ReservaInvestimento[] = [...mockReservas];
let categorias: Categoria[] = [];
let formasPagamento: FormaPagamento[] = [];

export const financeStore = {
    delay,

    getEntradas: () => entradas,
    setEntradas: (value: Entrada[]) => {
        entradas = value;
    },

    getGastos: () => gastos,
    setGastos: (value: Gasto[]) => {
        gastos = value;
    },

    getContas: () => contas,
    setContas: (value: Conta[]) => {
        contas = value;
    },

    getReservas: () => reservas,
    setReservas: (value: ReservaInvestimento[]) => {
        reservas = value;
    },

    getCategorias: () => categorias,
    setCategorias: (value: Categoria[]) => {
        categorias = value;
    },

    getFormasPagamento: () => formasPagamento,
    setFormasPagamento: (value: FormaPagamento[]) => {
        formasPagamento = value;
    },
};
