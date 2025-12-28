import { EditableTable, Column } from './EditableTable';
import { useEntradas, useAddEntrada, useUpdateEntrada, useDeleteEntrada } from '@/hooks/useFinancialData';
import { formatCurrency } from '@/utils/formatters';
import type { Entrada } from '@/types/finance';

export const EntradasTable = () => {
  const { data: entradas = [], isLoading } = useEntradas();
  const addMutation = useAddEntrada();
  const updateMutation = useUpdateEntrada();
  const deleteMutation = useDeleteEntrada();

  const totalEntradas = entradas.reduce((sum, row) => sum + (row.valor || 0), 0);

  const columns: Column<Entrada>[] = [
    {
      key: 'nome',
      header: 'Nome',
      type: 'text',
    },
    {
      key: 'valor',
      header: 'Valor',
      width: '120px',
      type: 'number',
      format: formatCurrency,
    },
    {
      key: 'porcentagem',
      header: '%',
      width: '80px',
      render: (_value, row) => {
        const pct = totalEntradas > 0 ? (row.valor / totalEntradas) * 100 : 0;
        return <span className="text-sm text-muted-foreground">{pct > 0 ? `${pct.toFixed(1)}%` : '-'}</span>;
      },
    },
  ];

  const emptyTemplate: Omit<Entrada, 'id' | 'mes' | 'ano'> = {
    nome: '',
    valor: 0,
  };

  return (
    <EditableTable<Entrada>
      title="Entradas"
      columns={columns}
      data={entradas}
      onAdd={async (item) => {
        const { porcentagem: _porcentagem, ...rest } = item as Omit<Entrada, 'id' | 'mes' | 'ano'>;
        await addMutation.mutateAsync(rest);
      }}
      onUpdate={async (item) => {
        const { porcentagem: _porcentagem, ...rest } = item;
        await updateMutation.mutateAsync(rest as Entrada);
      }}
      onDelete={async (id) => { await deleteMutation.mutateAsync(id); }}
      emptyRowTemplate={emptyTemplate as Omit<Entrada, 'id'>}
      isLoading={isLoading}
    />
  );
};
