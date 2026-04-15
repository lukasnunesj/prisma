import { EditableTable, Column } from './EditableTable';
import { useContas, useAddConta, useUpdateConta, useDeleteConta } from '@/hooks/useFinancialData';
import { formatCurrency, formatDate, formatDateInput } from '@/utils/formatters';
import type { Conta } from '@/types/finance';

export const ContasTable = () => {
  const { data: contas = [], isLoading } = useContas();
  const addMutation = useAddConta();
  const updateMutation = useUpdateConta();
  const deleteMutation = useDeleteConta();

  const columns: Column<Conta>[] = [
    {
      key: 'nome',
      header: 'Nome',
      type: 'text',
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
      width: '110px',
      type: 'date',
      format: (v) => (typeof v === 'string' && v ? formatDate(v) : '-'),
    },
    {
      key: 'pago',
      header: 'Pago',
      width: '50px',
      type: 'checkbox',
    },
  ];

  const emptyTemplate: Omit<Conta, 'id' | 'mes' | 'ano'> = {
    nome: '',
    valor: 0,
    data: formatDateInput(new Date().toISOString()),
    pago: false,
  };

  return (
    <EditableTable<Conta>
      title="Contas"
      columns={columns}
      data={contas}
      onAdd={async (item) => { await addMutation.mutateAsync(item as Omit<Conta, 'id' | 'mes' | 'ano'>); }}
      onUpdate={async (item) => { await updateMutation.mutateAsync(item); }}
      onDelete={async (id) => { await deleteMutation.mutateAsync(id); }}
      emptyRowTemplate={emptyTemplate as Omit<Conta, 'id'>}
      isLoading={isLoading}
    />
  );
};
