import { EditableTable, Column } from './EditableTable';
import { useReservas, useAddReserva, useUpdateReserva, useDeleteReserva } from '@/hooks/useFinancialData';
import { formatCurrency, formatDate, formatDateInput } from '@/utils/formatters';
import type { ReservaInvestimento } from '@/types/finance';

export const ReservasTable = () => {
  const { data: reservas = [], isLoading } = useReservas();
  const addMutation = useAddReserva();
  const updateMutation = useUpdateReserva();
  const deleteMutation = useDeleteReserva();

  const columns: Column<ReservaInvestimento>[] = [
    {
      key: 'nome',
      header: 'Nome',
      type: 'text',
    },
    {
      key: 'tipo',
      header: 'Tipo',
      width: '120px',
      type: 'select',
      options: [
        { value: 'reserva', label: 'Reserva' },
        { value: 'investimento', label: 'Investimento' },
      ],
      format: (v) => v === 'reserva' ? 'Reserva' : 'Investimento',
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
      format: formatDate,
    },
  ];

  const emptyTemplate: Omit<ReservaInvestimento, 'id' | 'mes' | 'ano'> = {
    nome: '',
    tipo: 'reserva',
    valor: 0,
    data: formatDateInput(new Date().toISOString()),
  };

  return (
    <EditableTable<ReservaInvestimento>
      title="Reservas & Investimentos"
      columns={columns}
      data={reservas}
      onAdd={async (item) => { await addMutation.mutateAsync(item as Omit<ReservaInvestimento, 'id' | 'mes' | 'ano'>); }}
      onUpdate={async (item) => { await updateMutation.mutateAsync(item); }}
      onDelete={async (id) => { await deleteMutation.mutateAsync(id); }}
      emptyRowTemplate={emptyTemplate as Omit<ReservaInvestimento, 'id'>}
      isLoading={isLoading}
    />
  );
};
