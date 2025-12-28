import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '@/stores/appStore';
import { useFormasPagamento, useAddFormaPagamento, useUpdateFormaPagamento, useDeleteFormaPagamento } from '@/hooks/useFinancialData';
import { Header } from '@/components/layout/Header';
import { EditableTable, Column } from '@/components/tables/EditableTable';
import type { FormaPagamento } from '@/types/finance';

const FormasPagamento = () => {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuthStore();
  const { data: formasPagamento = [], isLoading } = useFormasPagamento();
  const addMutation = useAddFormaPagamento();
  const updateMutation = useUpdateFormaPagamento();
  const deleteMutation = useDeleteFormaPagamento();

  useEffect(() => {
    if (!isAuthenticated) {
      navigate('/login');
    }
  }, [isAuthenticated, navigate]);

  if (!isAuthenticated) {
    return null;
  }

  const tipoOptions = [
    { value: 'debito', label: 'Débito' },
    { value: 'credito', label: 'Crédito' },
    { value: 'dinheiro', label: 'Dinheiro' },
    { value: 'pix', label: 'PIX' },
  ];

  const columns: Column<FormaPagamento>[] = [
    {
      key: 'nome',
      header: 'Nome',
      type: 'text',
    },
    {
      key: 'tipo',
      header: 'Tipo',
      width: '150px',
      type: 'select',
      options: tipoOptions,
      format: (v: string) => tipoOptions.find(o => o.value === v)?.label || v,
    },
  ];

  const emptyTemplate: Omit<FormaPagamento, 'id'> = {
    nome: '',
    tipo: 'debito',
  };

  return (
    <div className="h-screen w-screen flex flex-col overflow-hidden bg-background">
      <Header />

      <main className="flex-1 overflow-y-auto custom-scrollbar mt-16">
        <div className="max-w-4xl mx-auto px-6 py-8">
          <div className="mb-6">
            <h1 className="text-2xl font-bold text-foreground">Formas de Pagamento</h1>
            <p className="text-muted-foreground mt-1">
              Configure as formas de pagamento disponíveis
            </p>
          </div>

          <EditableTable<FormaPagamento>
            title="Formas de Pagamento"
            columns={columns}
            data={formasPagamento}
            onAdd={async (item) => { await addMutation.mutateAsync(item); }}
            onUpdate={async (item) => { await updateMutation.mutateAsync(item); }}
            onDelete={async (id) => { await deleteMutation.mutateAsync(id); }}
            emptyRowTemplate={emptyTemplate}
            isLoading={isLoading}
          />
        </div>
      </main>
    </div>
  );
};

export default FormasPagamento;
