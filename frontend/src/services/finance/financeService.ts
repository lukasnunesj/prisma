import { incomeService } from '@/services/finance/incomeService';
import { gastosService } from '@/services/finance/gastosService';
import { contasService } from '@/services/finance/contasService';
import { reservasService } from '@/services/finance/reservasService';
import { categoryService } from '@/services/finance/categoryService';
import { paymentMethodService } from '@/services/finance/paymentMethodService';
import { dashboardService } from '@/services/finance/dashboardService';

export const financeService = {
    entradas: incomeService,
    gastos: gastosService,
    contas: contasService,
    reservas: reservasService,
    categorias: categoryService,
    formasPagamento: paymentMethodService,
    dashboard: dashboardService,
};
