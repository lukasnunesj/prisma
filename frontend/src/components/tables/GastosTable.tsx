import { EditableTable, Column } from './EditableTable';
import { useGastos, useCategorias, useFormasPagamento, useAddGasto, useUpdateGasto, useDeleteGasto } from '@/hooks/useFinancialData';
import { formatCurrency, formatDate, formatDateInput } from '@/utils/formatters';
import type { Gasto } from '@/types/finance';

export const GastosTable = () => {
  const { data: gastos = [], isLoading } = useGastos();
  const { data: categorias = [] } = useCategorias();
  const { data: formasPagamento = [] } = useFormasPagamento();
  const addMutation = useAddGasto();
  const updateMutation = useUpdateGasto();
  const deleteMutation = useDeleteGasto();

  const columns: Column<Gasto>[] = [
    {
      key: 'descricao',
      header: 'Nome',
      type: 'text',
      format: (v) => String(v ?? ''),
    },
    {
      key: 'categoriaId',
      header: 'Categoria',
      type: 'select',
      options: categorias.map(c => ({ value: c.id, label: c.name })),
      format: (v) => categorias.find(c => c.id === v)?.name || '-',
    },
    {
      key: 'formaPagamentoId',
      header: 'Pagamento',
      type: 'select',
      options: formasPagamento.map(f => ({ value: f.id, label: f.nome })),
      format: (v) => formasPagamento.find(f => f.id === v)?.nome || '-',
    },
    {
      key: 'valor',
      header: 'Valor',
      width: '100px',
      type: 'number',
      format: formatCurrency,
    },
    {
      key: 'data',
      header: 'Data',
      width: '120px',
      type: 'date',
      // format: formatDate,
    },
    {
      key: 'essencial',
      header: 'Ess.',
      width: '50px',
      type: 'checkbox',
    },
  ];

  const emptyTemplate: Omit<Gasto, 'id' | 'mes' | 'ano'> = {
    descricao: '',
    categoriaId: '',
    formaPagamentoId: '',
    valor: 0,
    data: formatDateInput(new Date().toISOString()),
    essencial: false,
  };

  return (
    <EditableTable<Gasto>
      title="Gastos"
      columns={columns}
      data={gastos}
      onAdd={async (item) => { await addMutation.mutateAsync(item as Omit<Gasto, 'id' | 'mes' | 'ano'>); }}
      onUpdate={async (item) => { await updateMutation.mutateAsync(item); }}
      onDelete={async (id) => { await deleteMutation.mutateAsync(id); }}
      emptyRowTemplate={emptyTemplate as Omit<Gasto, 'id'>}
      isLoading={isLoading}
    />
  );
};
