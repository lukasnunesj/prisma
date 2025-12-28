import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useMonthNavigationStore } from '@/stores/appStore';
import { financeService } from '@/services/finance/financeService';
import type { Entrada, Gasto, Conta, ReservaInvestimento, Categoria, FormaPagamento } from '@/types/finance';

export const useEntradas = () => {
  const { selectedMonth, selectedYear } = useMonthNavigationStore();

  return useQuery({
    queryKey: ['entradas', selectedMonth, selectedYear],
    queryFn: async () => {
      return financeService.entradas.list(selectedMonth, selectedYear);
    },
    placeholderData: (prev) => prev,
    staleTime: 30000,
  });
};

export const useGastos = () => {
  const { selectedMonth, selectedYear } = useMonthNavigationStore();

  return useQuery({
    queryKey: ['gastos', selectedMonth, selectedYear],
    queryFn: async () => {
      return financeService.gastos.list(selectedMonth, selectedYear);
    },
    placeholderData: (prev) => prev,
    staleTime: 30000,
  });
};

export const useContas = () => {
  const { selectedMonth, selectedYear } = useMonthNavigationStore();

  return useQuery({
    queryKey: ['contas', selectedMonth, selectedYear],
    queryFn: async () => {
      return financeService.contas.list(selectedMonth, selectedYear);
    },
    placeholderData: (prev) => prev,
    staleTime: 30000,
  });
};

export const useReservas = () => {
  const { selectedMonth, selectedYear } = useMonthNavigationStore();

  return useQuery({
    queryKey: ['reservas', selectedMonth, selectedYear],
    queryFn: async () => {
      return financeService.reservas.list(selectedMonth, selectedYear);
    },
    placeholderData: (prev) => prev,
    staleTime: 30000,
  });
};

export const useCategorias = () => {
  return useQuery({
    queryKey: ['categorias'],
    queryFn: async () => {
      return financeService.categorias.list();
    },
    placeholderData: (prev) => prev,
    staleTime: 60000,
  });
};

export const useFormasPagamento = () => {
  return useQuery({
    queryKey: ['formasPagamento'],
    queryFn: async () => {
      return financeService.formasPagamento.list();
    },
    placeholderData: (prev) => prev,
    staleTime: 60000,
  });
};

export const useDashboardMetrics = () => {
  const { selectedMonth, selectedYear } = useMonthNavigationStore();

  return useQuery({
    queryKey: ['metrics', selectedMonth, selectedYear],
    queryFn: async () => {
      return financeService.dashboard.metrics(selectedMonth, selectedYear);
    },
    placeholderData: (prev) => prev,
    staleTime: 10000,
  });
};

// Mutations
export const useAddEntrada = () => {
  const queryClient = useQueryClient();
  const { selectedMonth, selectedYear } = useMonthNavigationStore();

  return useMutation({
    mutationFn: async (data: Omit<Entrada, 'id' | 'mes' | 'ano'>) => {
      return financeService.entradas.add(data, selectedMonth, selectedYear);
    },
    onMutate: async (data) => {
      await queryClient.cancelQueries({ queryKey: ['entradas', selectedMonth, selectedYear] });

      const previous = queryClient.getQueryData<Entrada[]>(['entradas', selectedMonth, selectedYear]);
      const optimistic: Entrada = {
        ...(data as Omit<Entrada, 'id' | 'mes' | 'ano'>),
        id: `tmp-${Date.now()}`,
        mes: selectedMonth,
        ano: selectedYear,
      };

      queryClient.setQueryData<Entrada[]>(['entradas', selectedMonth, selectedYear], (current) => {
        const list = current ?? [];
        return [...list, optimistic];
      });

      return { previous };
    },
    onError: (_err, _data, ctx) => {
      if (ctx?.previous) {
        queryClient.setQueryData(['entradas', selectedMonth, selectedYear], ctx.previous);
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['entradas'] });
      queryClient.invalidateQueries({ queryKey: ['metrics'] });
    },
  });
};

export const useUpdateEntrada = () => {
  const queryClient = useQueryClient();
  const { selectedMonth, selectedYear } = useMonthNavigationStore();

  return useMutation({
    mutationFn: async (data: Entrada) => {
      return financeService.entradas.update(data);
    },
    onMutate: async (data) => {
      await queryClient.cancelQueries({ queryKey: ['entradas', selectedMonth, selectedYear] });
      const previous = queryClient.getQueryData<Entrada[]>(['entradas', selectedMonth, selectedYear]);

      queryClient.setQueryData<Entrada[]>(['entradas', selectedMonth, selectedYear], (current) => {
        const list = current ?? [];
        return list.map((row) => (row.id === data.id ? data : row));
      });

      return { previous };
    },
    onError: (_err, _data, ctx) => {
      if (ctx?.previous) {
        queryClient.setQueryData(['entradas', selectedMonth, selectedYear], ctx.previous);
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['entradas'] });
      queryClient.invalidateQueries({ queryKey: ['metrics'] });
    },
  });
};

export const useDeleteEntrada = () => {
  const queryClient = useQueryClient();
  const { selectedMonth, selectedYear } = useMonthNavigationStore();

  return useMutation({
    mutationFn: async (id: string) => {
      return financeService.entradas.remove(id);
    },
    onMutate: async (id) => {
      await queryClient.cancelQueries({ queryKey: ['entradas', selectedMonth, selectedYear] });
      const previous = queryClient.getQueryData<Entrada[]>(['entradas', selectedMonth, selectedYear]);

      queryClient.setQueryData<Entrada[]>(['entradas', selectedMonth, selectedYear], (current) => {
        const list = current ?? [];
        return list.filter((row) => row.id !== id);
      });

      return { previous };
    },
    onError: (_err, _id, ctx) => {
      if (ctx?.previous) {
        queryClient.setQueryData(['entradas', selectedMonth, selectedYear], ctx.previous);
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['entradas'] });
      queryClient.invalidateQueries({ queryKey: ['metrics'] });
    },
  });
};

export const useAddGasto = () => {
  const queryClient = useQueryClient();
  const { selectedMonth, selectedYear } = useMonthNavigationStore();

  return useMutation({
    mutationFn: async (data: Omit<Gasto, 'id' | 'mes' | 'ano'>) => {
      return financeService.gastos.add(data, selectedMonth, selectedYear);
    },
    onMutate: async (data) => {
      await queryClient.cancelQueries({ queryKey: ['gastos', selectedMonth, selectedYear] });
      const previous = queryClient.getQueryData<Gasto[]>(['gastos', selectedMonth, selectedYear]);

      const optimistic: Gasto = {
        ...(data as Omit<Gasto, 'id' | 'mes' | 'ano'>),
        id: `tmp-${Date.now()}`,
        mes: selectedMonth,
        ano: selectedYear,
      };

      queryClient.setQueryData<Gasto[]>(['gastos', selectedMonth, selectedYear], (current) => {
        const list = current ?? [];
        return [...list, optimistic];
      });

      return { previous };
    },
    onError: (_err, _data, ctx) => {
      if (ctx?.previous) {
        queryClient.setQueryData(['gastos', selectedMonth, selectedYear], ctx.previous);
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['gastos'] });
      queryClient.invalidateQueries({ queryKey: ['metrics'] });
    },
  });
};

export const useUpdateGasto = () => {
  const queryClient = useQueryClient();
  const { selectedMonth, selectedYear } = useMonthNavigationStore();

  return useMutation({
    mutationFn: async (data: Gasto) => {
      return financeService.gastos.update(data);
    },
    onMutate: async (data) => {
      await queryClient.cancelQueries({ queryKey: ['gastos', selectedMonth, selectedYear] });
      const previous = queryClient.getQueryData<Gasto[]>(['gastos', selectedMonth, selectedYear]);

      queryClient.setQueryData<Gasto[]>(['gastos', selectedMonth, selectedYear], (current) => {
        const list = current ?? [];
        return list.map((row) => (row.id === data.id ? data : row));
      });

      return { previous };
    },
    onError: (_err, _data, ctx) => {
      if (ctx?.previous) {
        queryClient.setQueryData(['gastos', selectedMonth, selectedYear], ctx.previous);
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['gastos'] });
      queryClient.invalidateQueries({ queryKey: ['metrics'] });
    },
  });
};

export const useDeleteGasto = () => {
  const queryClient = useQueryClient();
  const { selectedMonth, selectedYear } = useMonthNavigationStore();

  return useMutation({
    mutationFn: async (id: string) => {
      return financeService.gastos.remove(id);
    },
    onMutate: async (id) => {
      await queryClient.cancelQueries({ queryKey: ['gastos', selectedMonth, selectedYear] });
      const previous = queryClient.getQueryData<Gasto[]>(['gastos', selectedMonth, selectedYear]);

      queryClient.setQueryData<Gasto[]>(['gastos', selectedMonth, selectedYear], (current) => {
        const list = current ?? [];
        return list.filter((row) => row.id !== id);
      });

      return { previous };
    },
    onError: (_err, _id, ctx) => {
      if (ctx?.previous) {
        queryClient.setQueryData(['gastos', selectedMonth, selectedYear], ctx.previous);
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['gastos'] });
      queryClient.invalidateQueries({ queryKey: ['metrics'] });
    },
  });
};

export const useAddConta = () => {
  const queryClient = useQueryClient();
  const { selectedMonth, selectedYear } = useMonthNavigationStore();

  return useMutation({
    mutationFn: async (data: Omit<Conta, 'id' | 'mes' | 'ano'>) => {
      return financeService.contas.add(data, selectedMonth, selectedYear);
    },
    onMutate: async (data) => {
      await queryClient.cancelQueries({ queryKey: ['contas', selectedMonth, selectedYear] });
      const previous = queryClient.getQueryData<Conta[]>(['contas', selectedMonth, selectedYear]);

      const optimistic: Conta = {
        ...(data as Omit<Conta, 'id' | 'mes' | 'ano'>),
        id: `tmp-${Date.now()}`,
        mes: selectedMonth,
        ano: selectedYear,
      };

      queryClient.setQueryData<Conta[]>(['contas', selectedMonth, selectedYear], (current) => {
        const list = current ?? [];
        return [...list, optimistic];
      });

      return { previous };
    },
    onError: (_err, _data, ctx) => {
      if (ctx?.previous) {
        queryClient.setQueryData(['contas', selectedMonth, selectedYear], ctx.previous);
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['contas'] });
      queryClient.invalidateQueries({ queryKey: ['metrics'] });
    },
  });
};

export const useUpdateConta = () => {
  const queryClient = useQueryClient();
  const { selectedMonth, selectedYear } = useMonthNavigationStore();

  return useMutation({
    mutationFn: async (data: Conta) => {
      return financeService.contas.update(data);
    },
    onMutate: async (data) => {
      await queryClient.cancelQueries({ queryKey: ['contas', selectedMonth, selectedYear] });
      const previous = queryClient.getQueryData<Conta[]>(['contas', selectedMonth, selectedYear]);

      queryClient.setQueryData<Conta[]>(['contas', selectedMonth, selectedYear], (current) => {
        const list = current ?? [];
        return list.map((row) => (row.id === data.id ? data : row));
      });

      return { previous };
    },
    onError: (_err, _data, ctx) => {
      if (ctx?.previous) {
        queryClient.setQueryData(['contas', selectedMonth, selectedYear], ctx.previous);
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['contas'] });
      queryClient.invalidateQueries({ queryKey: ['metrics'] });
    },
  });
};

export const useDeleteConta = () => {
  const queryClient = useQueryClient();
  const { selectedMonth, selectedYear } = useMonthNavigationStore();

  return useMutation({
    mutationFn: async (id: string) => {
      return financeService.contas.remove(id);
    },
    onMutate: async (id) => {
      await queryClient.cancelQueries({ queryKey: ['contas', selectedMonth, selectedYear] });
      const previous = queryClient.getQueryData<Conta[]>(['contas', selectedMonth, selectedYear]);

      queryClient.setQueryData<Conta[]>(['contas', selectedMonth, selectedYear], (current) => {
        const list = current ?? [];
        return list.filter((row) => row.id !== id);
      });

      return { previous };
    },
    onError: (_err, _id, ctx) => {
      if (ctx?.previous) {
        queryClient.setQueryData(['contas', selectedMonth, selectedYear], ctx.previous);
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['contas'] });
      queryClient.invalidateQueries({ queryKey: ['metrics'] });
    },
  });
};

export const useAddReserva = () => {
  const queryClient = useQueryClient();
  const { selectedMonth, selectedYear } = useMonthNavigationStore();

  return useMutation({
    mutationFn: async (data: Omit<ReservaInvestimento, 'id' | 'mes' | 'ano'>) => {
      return financeService.reservas.add(data, selectedMonth, selectedYear);
    },
    onMutate: async (data) => {
      await queryClient.cancelQueries({ queryKey: ['reservas', selectedMonth, selectedYear] });
      const previous = queryClient.getQueryData<ReservaInvestimento[]>(['reservas', selectedMonth, selectedYear]);

      const optimistic: ReservaInvestimento = {
        ...(data as Omit<ReservaInvestimento, 'id' | 'mes' | 'ano'>),
        id: `tmp-${Date.now()}`,
        mes: selectedMonth,
        ano: selectedYear,
      };

      queryClient.setQueryData<ReservaInvestimento[]>(['reservas', selectedMonth, selectedYear], (current) => {
        const list = current ?? [];
        return [...list, optimistic];
      });

      return { previous };
    },
    onError: (_err, _data, ctx) => {
      if (ctx?.previous) {
        queryClient.setQueryData(['reservas', selectedMonth, selectedYear], ctx.previous);
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['reservas'] });
      queryClient.invalidateQueries({ queryKey: ['metrics'] });
    },
  });
};

export const useUpdateReserva = () => {
  const queryClient = useQueryClient();
  const { selectedMonth, selectedYear } = useMonthNavigationStore();

  return useMutation({
    mutationFn: async (data: ReservaInvestimento) => {
      return financeService.reservas.update(data);
    },
    onMutate: async (data) => {
      await queryClient.cancelQueries({ queryKey: ['reservas', selectedMonth, selectedYear] });
      const previous = queryClient.getQueryData<ReservaInvestimento[]>(['reservas', selectedMonth, selectedYear]);

      queryClient.setQueryData<ReservaInvestimento[]>(['reservas', selectedMonth, selectedYear], (current) => {
        const list = current ?? [];
        return list.map((row) => (row.id === data.id ? data : row));
      });

      return { previous };
    },
    onError: (_err, _data, ctx) => {
      if (ctx?.previous) {
        queryClient.setQueryData(['reservas', selectedMonth, selectedYear], ctx.previous);
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['reservas'] });
      queryClient.invalidateQueries({ queryKey: ['metrics'] });
    },
  });
};

export const useDeleteReserva = () => {
  const queryClient = useQueryClient();
  const { selectedMonth, selectedYear } = useMonthNavigationStore();

  return useMutation({
    mutationFn: async (id: string) => {
      return financeService.reservas.remove(id);
    },
    onMutate: async (id) => {
      await queryClient.cancelQueries({ queryKey: ['reservas', selectedMonth, selectedYear] });
      const previous = queryClient.getQueryData<ReservaInvestimento[]>(['reservas', selectedMonth, selectedYear]);

      queryClient.setQueryData<ReservaInvestimento[]>(['reservas', selectedMonth, selectedYear], (current) => {
        const list = current ?? [];
        return list.filter((row) => row.id !== id);
      });

      return { previous };
    },
    onError: (_err, _id, ctx) => {
      if (ctx?.previous) {
        queryClient.setQueryData(['reservas', selectedMonth, selectedYear], ctx.previous);
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['reservas'] });
      queryClient.invalidateQueries({ queryKey: ['metrics'] });
    },
  });
};

export const useAddCategoria = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (data: Omit<Categoria, 'id'>) => {
      return financeService.categorias.add(data);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['categorias'] });
    },
  });
};

export const useUpdateCategoria = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (data: Categoria) => {
      return financeService.categorias.update(data);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['categorias'] });
    },
  });
};

export const useDeleteCategoria = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: string) => {
      return financeService.categorias.remove(id);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['categorias'] });
    },
  });
};

export const useAddFormaPagamento = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (data: Omit<FormaPagamento, 'id'>) => {
      return financeService.formasPagamento.add(data);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['formasPagamento'] });
    },
  });
};

export const useUpdateFormaPagamento = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (data: FormaPagamento) => {
      return financeService.formasPagamento.update(data);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['formasPagamento'] });
    },
  });
};

export const useDeleteFormaPagamento = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: string) => {
      return financeService.formasPagamento.remove(id);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['formasPagamento'] });
    },
  });
};
