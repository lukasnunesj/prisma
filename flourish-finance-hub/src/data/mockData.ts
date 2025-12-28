import type { Categoria, FormaPagamento, Entrada, Gasto, Conta, ReservaInvestimento, DashboardMetrics } from '@/types/finance';

export const mockCategorias: Categoria[] = [
  { id: '1', nome: 'Alimentação', cor: '#2D7A6E', icone: 'utensils' },
  { id: '2', nome: 'Transporte', cor: '#4A90A4', icone: 'car' },
  { id: '3', nome: 'Moradia', cor: '#6B8E23', icone: 'home' },
  { id: '4', nome: 'Saúde', cor: '#DC143C', icone: 'heart' },
  { id: '5', nome: 'Educação', cor: '#4169E1', icone: 'book' },
  { id: '6', nome: 'Lazer', cor: '#FF8C00', icone: 'gamepad' },
  { id: '7', nome: 'Academia', cor: '#8B4513', icone: 'dumbbell' },
  { id: '8', nome: 'Vestuário', cor: '#9932CC', icone: 'shirt' },
];

export const mockFormasPagamento: FormaPagamento[] = [
  { id: '1', nome: 'Nubank Débito', tipo: 'debito' },
  { id: '2', nome: 'Nubank Crédito', tipo: 'credito' },
  { id: '3', nome: 'Itaú Débito', tipo: 'debito' },
  { id: '4', nome: 'Itaú Crédito', tipo: 'credito' },
  { id: '5', nome: 'PIX', tipo: 'pix' },
  { id: '6', nome: 'Dinheiro', tipo: 'dinheiro' },
];

export const mockEntradas: Entrada[] = [
  { id: '1', nome: 'Salário', valor: 5000, porcentagem: 100, mes: 12, ano: 2025 },
  { id: '2', nome: 'Freelance', valor: 794.14, porcentagem: 15.88, mes: 12, ano: 2025 },
];

export const mockGastos: Gasto[] = [
  { id: '1', categoriaId: '1', formaPagamentoId: '2', valor: 450.00, data: '2025-12-05', essencial: true, mes: 12, ano: 2025 },
  { id: '2', categoriaId: '2', formaPagamentoId: '1', valor: 180.50, data: '2025-12-10', essencial: true, mes: 12, ano: 2025 },
  { id: '3', categoriaId: '6', formaPagamentoId: '2', valor: 250.00, data: '2025-12-15', essencial: false, mes: 12, ano: 2025 },
  { id: '4', categoriaId: '7', formaPagamentoId: '5', valor: 100.00, data: '2025-12-01', essencial: false, mes: 12, ano: 2025 },
];

export const mockContas: Conta[] = [
  { id: '1', nome: 'Luz', planejado: 120.00, valor: 109.18, dataPago: '2025-12-05', pago: true, mes: 12, ano: 2025 },
  { id: '2', nome: 'Água', planejado: 80.00, valor: 75.30, dataPago: '2025-12-10', pago: true, mes: 12, ano: 2025 },
  { id: '3', nome: 'Internet', planejado: 99.90, valor: 99.90, dataPago: '2025-12-08', pago: true, mes: 12, ano: 2025 },
  { id: '4', nome: 'Condomínio', planejado: 500.00, valor: 495.48, dataPago: '2025-12-15', pago: true, mes: 12, ano: 2025 },
  { id: '5', nome: 'Aluguel', planejado: 1500.00, valor: 1500.00, pago: false, mes: 12, ano: 2025 },
];

export const mockReservas: ReservaInvestimento[] = [
  { id: '1', nome: 'Reserva de Emergência', tipo: 'reserva', valor: 500.00, data: '2025-12-01', mes: 12, ano: 2025 },
  { id: '2', nome: 'Tesouro Direto', tipo: 'investimento', valor: 300.00, data: '2025-12-05', mes: 12, ano: 2025 },
];

export const getMockMetrics = (mes: number, ano: number): DashboardMetrics => {
  const totalEntradas = mockEntradas
    .filter(e => e.mes === mes && e.ano === ano)
    .reduce((acc, e) => acc + e.valor, 0);
  
  const totalGastos = mockGastos
    .filter(g => g.mes === mes && g.ano === ano)
    .reduce((acc, g) => acc + g.valor, 0);
  
  const totalContas = mockContas
    .filter(c => c.mes === mes && c.ano === ano)
    .reduce((acc, c) => acc + c.valor, 0);
  
  const totalReservas = mockReservas
    .filter(r => r.mes === mes && r.ano === ano)
    .reduce((acc, r) => acc + r.valor, 0);
  
  const totalDebito = mockGastos
    .filter(g => g.mes === mes && g.ano === ano && ['1', '3'].includes(g.formaPagamentoId))
    .reduce((acc, g) => acc + g.valor, 0);
  
  const totalCredito = mockGastos
    .filter(g => g.mes === mes && g.ano === ano && ['2', '4'].includes(g.formaPagamentoId))
    .reduce((acc, g) => acc + g.valor, 0);
  
  const saidas = totalGastos + totalContas;
  const restante = totalEntradas - saidas - totalReservas;
  
  return {
    totais: {
      entradas: totalEntradas,
      saidas: saidas,
      reservas: totalReservas,
      usoDebito: totalDebito,
      usoCredito: totalCredito,
      restante: restante,
    },
    comparacaoMesAnterior: {
      entradas: totalEntradas * 0.95,
      saidas: saidas * 1.1,
      reservas: totalReservas * 0.8,
      usoDebito: totalDebito * 0.9,
      usoCredito: totalCredito * 1.05,
    },
    graficos: {
      fluxoContas: mockContas
        .filter(c => c.mes === mes && c.ano === ano)
        .map(c => ({
          nome: c.nome,
          valor: c.valor,
          planejado: c.planejado,
          pago: c.pago,
        })),
      alocacaoCategorias: mockCategorias.map(cat => ({
        categoria: cat.nome,
        valor: mockGastos
          .filter(g => g.mes === mes && g.ano === ano && g.categoriaId === cat.id)
          .reduce((acc, g) => acc + g.valor, 0),
        cor: cat.cor,
      })).filter(c => c.valor > 0),
    },
  };
};
