export interface Entrada {
  id: string;
  nome: string;
  valor: number;
  data: string;
  porcentagem?: number;
  mes: number;
  ano: number;
}

export interface Gasto {
  id: string;
  categoriaId: string;
  formaPagamentoId: string;
  valor: number;
  data: string;
  essencial: boolean;
  descricao?: string;
  mes: number;
  ano: number;
}

export interface Conta {
  id: string;
  nome: string;
  planejado: number;
  valor: number;
  dataPago?: string;
  pago: boolean;
  mes: number;
  ano: number;
}

export interface ReservaInvestimento {
  id: string;
  nome: string;
  tipo: 'reserva' | 'investimento';
  valor: number;
  data: string;
  mes: number;
  ano: number;
}

export interface Categoria {
  id: string;
  name: string;
  color: string;
  icon?: string;
}

export interface FormaPagamento {
  id: string;
  nome: string;
  tipo: 'debito' | 'credito' | 'dinheiro' | 'pix';
}

export interface DashboardMetrics {
  totais: {
    entradas: number;
    saidas: number;
    reservas: number;
    usoDebito: number;
    usoCredito: number;
    restante: number;
  };
  comparacaoMesAnterior: {
    entradas: number;
    saidas: number;
    reservas: number;
    usoDebito: number;
    usoCredito: number;
  };
  graficos: {
    fluxoContas: Array<{
      nome: string;
      valor: number;
      planejado: number;
      pago: boolean;
    }>;
    alocacaoCategorias: Array<{
      categoria: string;
      valor: number;
      cor: string;
    }>;
  };
}
