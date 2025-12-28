import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '@/stores/appStore';
import { useCategorias, useAddCategoria, useUpdateCategoria, useDeleteCategoria } from '@/hooks/useFinancialData';
import { Header } from '@/components/layout/Header';
import { EditableTable, Column } from '@/components/tables/EditableTable';
import type { Categoria } from '@/types/finance';

const Categorias = () => {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuthStore();
  const { data: categorias = [], isLoading } = useCategorias();
  const addMutation = useAddCategoria();
  const updateMutation = useUpdateCategoria();
  const deleteMutation = useDeleteCategoria();

  useEffect(() => {
    if (!isAuthenticated) {
      navigate('/login');
    }
  }, [isAuthenticated, navigate]);

  if (!isAuthenticated) {
    return null;
  }

  const columns: Column<Categoria>[] = [
    {
      key: 'nome',
      header: 'Nome',
      type: 'text',
    },
    {
      key: 'cor',
      header: 'Cor',
      width: '100px',
      type: 'color',
    },
    {
      key: 'icone',
      header: 'Ícone',
      width: '120px',
      type: 'text',
      format: (v) => v || '-',
    },
  ];

  const emptyTemplate: Omit<Categoria, 'id'> = {
    nome: '',
    cor: '#2D7A6E',
    icone: '',
  };

  return (
    <div className="h-screen w-screen flex flex-col overflow-hidden bg-background">
      <Header />

      <main className="flex-1 overflow-y-auto custom-scrollbar mt-16">
        <div className="max-w-4xl mx-auto px-6 py-8">
          <div className="mb-6">
            <h1 className="text-2xl font-bold text-foreground">Categorias</h1>
            <p className="text-muted-foreground mt-1">
              Gerencie as categorias de gastos do seu orçamento
            </p>
          </div>

          <EditableTable<Categoria>
            title="Categorias"
            columns={columns}
            data={categorias}
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

export default Categorias;
