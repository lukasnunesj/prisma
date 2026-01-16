import { useState, useEffect, useRef, KeyboardEvent } from 'react';
import { cn } from '@/lib/utils';
import { Check, Plus, Trash2 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { useToast } from '@/hooks/use-toast';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog';

export interface Column<T> {
  key: keyof T | string;
  header: string;
  width?: string;
  type?: 'text' | 'number' | 'date' | 'select' | 'checkbox' | 'color';
  options?: Array<{ value: string; label: string }>;
  render?: (value: unknown, row: T, onChange: (value: unknown) => void) => React.ReactNode;
  format?: (value: unknown) => string;
}

interface EditableTableProps<T extends { id: string }> {
  columns: Column<T>[];
  data: T[];
  onAdd: (item: Omit<T, 'id'>) => Promise<void>;
  onUpdate: (item: T) => Promise<void>;
  onDelete: (id: string) => Promise<void>;
  emptyRowTemplate: Omit<T, 'id'>;
  title: string;
  isLoading?: boolean;
}

export function EditableTable<T extends { id: string }>({
  columns,
  data,
  onAdd,
  onUpdate,
  onDelete,
  emptyRowTemplate,
  title,
  isLoading,
}: EditableTableProps<T>) {
  const { toast } = useToast();
  const [editingId, setEditingId] = useState<string | null>(null);
  const [editingData, setEditingData] = useState<Partial<T>>({});
  const [newRow, setNewRow] = useState<Partial<T>>({});
  const [isAddingNew, setIsAddingNew] = useState(false);
  const [highlightedId, setHighlightedId] = useState<string | null>(null);
  const [pendingDeleteId, setPendingDeleteId] = useState<string | null>(null);
  const inputRefs = useRef<{ [key: string]: HTMLInputElement | null }>({});

  const focusFirstInput = (rowId: string) => {
    const firstKey = columns[0].key as string;
    const inputRef = inputRefs.current[`${rowId}-${firstKey}`];
    if (inputRef) {
      inputRef.focus();
    }
  };

  useEffect(() => {
    if (isAddingNew) {
      setTimeout(() => focusFirstInput('new'), 50);
    }
  }, [isAddingNew]);

  const handleStartEdit = (row: T) => {
    setEditingId(row.id);
    setEditingData(row);
    setTimeout(() => focusFirstInput(row.id), 50);
  };

  const handleCancelEdit = () => {
    setEditingId(null);
    setEditingData({});
  };

  const handleSaveEdit = async () => {
    if (!editingId || !editingData) return;

    try {
      await onUpdate(editingData as T);
      setHighlightedId(editingId);
      setTimeout(() => setHighlightedId(null), 500);
      setEditingId(null);
      setEditingData({});
      toast({ title: 'Registro atualizado com sucesso!' });
    } catch {
      toast({ title: 'Erro ao atualizar', variant: 'destructive' });
    }
  };

  const handleStartAdd = () => {
    setIsAddingNew(true);
    setNewRow({ ...emptyRowTemplate } as Partial<T>);
  };

  const handleCancelAdd = () => {
    setIsAddingNew(false);
    setNewRow({});
  };

  const handleSaveNew = async () => {
    try {
      await onAdd(newRow as Omit<T, 'id'>);
      setIsAddingNew(false);
      setNewRow({});
      toast({ title: 'Registro adicionado com sucesso!' });
    } catch {
      toast({ title: 'Erro ao adicionar', variant: 'destructive' });
    }
  };

  const startEditAndChange = (row: T, key: keyof T, value: unknown) => {
    if (editingId !== row.id) {
      setEditingId(row.id);
      setEditingData({ ...row, [key]: value });
      setTimeout(() => focusFirstInput(row.id), 50);
      return;
    }

    setEditingData(prev => ({ ...prev, [key]: value }));
  };

  const isMoneyColumn = (col: Column<T>) => {
    if (col.type !== 'number') return false;
    const key = String(col.key);
    return key === 'valor' || key === 'planejado' || key.endsWith('Valor') || key.endsWith('Planejado');
  };

  const formatMoneyInput = (val: unknown) => {
    const num = typeof val === 'number' ? val : Number(val);
    if (!Number.isFinite(num)) return '';
    return new Intl.NumberFormat('pt-BR', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
      currency: "BRL",
      style: "currency",
    }).format(num);
  };

  const parseMoneyInput = (text: string) => {
    const digits = text.replace(/\D/g, '');
    if (!digits) return 0;
    return Number(digits) / 100;
  };

  const handleDelete = async (id: string) => {
    try {
      await onDelete(id);
      toast({ title: 'Registro removido com sucesso!' });
    } catch {
      toast({ title: 'Erro ao remover', variant: 'destructive' });
    }
  };
  const handleKeyDown = (e: KeyboardEvent, mode: 'edit' | 'new', colIndex: number) => {
    if (e.key === 'Enter') {
      e.preventDefault();
      if (mode === 'edit') {
        handleSaveEdit();
      } else {
        handleSaveNew();
      }
    } else if (e.key === 'Escape') {
      e.preventDefault();
      if (mode === 'edit') {
        handleCancelEdit();
      } else {
        handleCancelAdd();
      }
    } else if (e.key === 'Tab' && !e.shiftKey && colIndex === columns.length - 1) {
      e.preventDefault();
      if (mode === 'edit') {
        handleSaveEdit();
      } else {
        handleSaveNew();
      }
    }
  };

  const renderCell = (
    col: Column<T>,
    value: unknown,
    isEditing: boolean,
    onChange: (val: unknown) => void,
    rowId: string,
    colIndex: number,
    mode: 'edit' | 'new',
    row?: T
  ) => {
    if (col.render) {
      return col.render(value, row ?? ({} as T), onChange);
    }

    if (!isEditing) {
      if (col.type === 'checkbox') {
        return (
          <button
            type="button"
            className={cn(
              'w-5 h-5 rounded border-2 flex items-center justify-center transition-colors',
              value ? 'bg-primary border-primary' : 'border-muted-foreground/30'
            )}
            onClick={(e) => {
              e.stopPropagation();
              if (!row) return;
              startEditAndChange(row, col.key as keyof T, !value);
            }}
          >
            {value && <Check className="w-3 h-3 text-primary-foreground" />}
          </button>
        );
      }

      if (col.type === 'select' && col.options) {
        return (
          <Select
            value={value == null ? '' : String(value)}
            onValueChange={(val) => {
              if (!row) return;
              startEditAndChange(row, col.key as keyof T, val);
            }}
          >
            <SelectTrigger className="h-7 bg-transparent border-0 px-2 py-0 shadow-none focus:ring-0">
              <SelectValue placeholder="Selecione..." />
            </SelectTrigger>
            <SelectContent>
              {col.options.map((opt) => (
                <SelectItem key={opt.value} value={opt.value}>{opt.label}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        );
      }

      if (col.type === 'color') {
        return (
          <input
            type="color"
            value={typeof value === 'string' ? value : '#2D7A6E'}
            onClick={(e) => {
              e.stopPropagation();
            }}
            onChange={(e) => {
              if (!row) return;
              startEditAndChange(row, col.key as keyof T, e.target.value);
            }}
            className="w-8 h-6 rounded cursor-pointer"
          />
        );
      }

      const isMoney = isMoneyColumn(col);
      const inputValue = col.format
        ? col.format(value)
        : isMoney
          ? formatMoneyInput(value)
          : (value == null ? '' : String(value));

      return (
        <input
          type={col.type === 'date' ? 'date' : 'text'}
          value={inputValue}
          readOnly
          tabIndex={-1}
          className="w-full h-7 bg-transparent border-0 px-2 text-sm focus:outline-none"
          placeholder={col.header}
        />
      );
    }

    if (col.type === 'checkbox') {
      return (
        <button
          type="button"
          onClick={() => onChange(!value)}
          className={cn(
            'w-5 h-5 rounded border-2 flex items-center justify-center transition-colors',
            value ? 'bg-primary border-primary' : 'border-muted-foreground/30 hover:border-primary/50'
          )}
        >
          {value && <Check className="w-3 h-3 text-primary-foreground" />}
        </button>
      );
    }

    if (col.type === 'select' && col.options) {
      return (
        <Select
          value={value == null ? '' : String(value)}
          onValueChange={(val) => onChange(val)}
        >
          <SelectTrigger className="h-7 bg-transparent border-0 px-2 py-0 shadow-none focus:ring-0">
            <SelectValue placeholder="Selecione..." />
          </SelectTrigger>
          <SelectContent>
            {col.options.map((opt) => (
              <SelectItem key={opt.value} value={opt.value}>{opt.label}</SelectItem>
            ))}
          </SelectContent>
        </Select>
      );
    }

    if (col.type === 'color') {
      return (
        <input
          type="color"
          value={typeof value === 'string' ? value : '#2D7A6E'}
          onChange={(e) => onChange(e.target.value)}
          className="w-8 h-6 rounded cursor-pointer"
        />
      );
    }

    const isMoney = isMoneyColumn(col);
    const inputValue = isMoney
      ? formatMoneyInput(value)
      : (value == null ? '' : String(value));

    return (
      <input
        ref={(el) => { inputRefs.current[`${rowId}-${col.key as string}`] = el; }}
        type={col.type === 'date' ? 'date' : 'text'}
        value={inputValue}
        onChange={(e) => {
          const newValue = isMoney
            ? parseMoneyInput(e.target.value)
            : col.type === 'number'
              ? (parseFloat(e.target.value) || 0)
              : e.target.value;
          onChange(newValue);
        }}
        onKeyDown={(e) => handleKeyDown(e, mode, colIndex)}
        className="w-full h-7 bg-transparent border-0 px-2 text-sm focus:outline-none focus:bg-muted/20"
        placeholder={col.header}
      />
    );
  };

  return (
    <>
      <div className="bg-card rounded-xl border border-border overflow-hidden relative">
        <div className="flex items-center justify-between px-4 py-3 bg-muted/30 border-b border-border">
          <h3 className="text-sm font-semibold text-foreground">{title}</h3>
          <Button
            variant="ghost"
            size="sm"
            onClick={handleStartAdd}
            disabled={isAddingNew}
            className="h-7 text-xs"
          >
            <Plus className="w-3 h-3 mr-1" />
            Adicionar
          </Button>
        </div>

        <div className="overflow-x-auto">
          <div className="flex items-center bg-muted/20 border-b border-border">
            {columns.map((col) => (
              <div
                key={col.key as string}
                className={cn(
                  'px-3 py-2 text-xs font-medium text-muted-foreground uppercase tracking-wider',
                  col.type === 'checkbox' && 'text-center'
                )}
                style={{ width: col.width || 'auto', flex: col.width ? 'none' : 1 }}
              >
                {col.header}
              </div>
            ))}
            <div className="w-20 px-3 py-2 text-xs font-medium text-muted-foreground uppercase tracking-wider text-center">
              Ações
            </div>
          </div>

          <div className="max-h-64 overflow-y-auto custom-scrollbar">
            {data.map((row) => {
              const isEditing = editingId === row.id;
              const isHighlighted = highlightedId === row.id;

              return (
                <div
                  key={row.id}
                  className={cn(
                    'flex items-center border-b border-border',
                    isHighlighted && 'bg-success/10'
                  )}
                  onClick={() => !isEditing && handleStartEdit(row)}
                >
                  {columns.map((col, colIndex) => (
                    <div
                      key={col.key as string}
                      className={cn(
                        'h-9 flex items-center',
                        col.type === 'checkbox' && 'justify-center'
                      )}
                      style={{ width: col.width || 'auto', flex: col.width ? 'none' : 1 }}
                    >
                      {renderCell(
                        col,
                        isEditing ? editingData[col.key as keyof T] : row[col.key as keyof T],
                        isEditing,
                        (val) => setEditingData(prev => ({ ...prev, [col.key]: val })),
                        row.id,
                        colIndex,
                        'edit',
                        row
                      )}
                    </div>
                  ))}
                  <div className="w-20 h-9 flex items-center justify-center gap-1">
                    {isEditing ? (
                      <>
                        <Button
                          variant="ghost"
                          size="icon"
                          className="h-6 w-6 text-success hover:text-success hover:bg-success/10"
                          onClick={(e) => { e.stopPropagation(); handleSaveEdit(); }}
                        >
                          <Check className="w-3.5 h-3.5" />
                        </Button>
                        <Button
                          variant="ghost"
                          size="icon"
                          className="h-6 w-6 text-destructive hover:text-destructive hover:bg-destructive/10"
                          onClick={(e) => { e.stopPropagation(); handleCancelEdit(); }}
                        >
                          <Trash2 className="w-3.5 h-3.5" />
                        </Button>
                      </>
                    ) : (
                      <Button
                        variant="ghost"
                        size="icon"
                        className="h-6 w-6 text-muted-foreground hover:text-destructive hover:bg-destructive/10"
                        onClick={(e) => { e.stopPropagation(); setPendingDeleteId(row.id); }}
                      >
                        <Trash2 className="w-3.5 h-3.5" />
                      </Button>
                    )}
                  </div>
                </div>
              );
            })}

            {isAddingNew && (
              <div className="flex items-center border-b border-border bg-primary/5">
                {columns.map((col, colIndex) => (
                  <div
                    key={col.key as string}
                    className={cn(
                      'h-9 flex items-center',
                      col.type === 'checkbox' && 'justify-center'
                    )}
                    style={{ width: col.width || 'auto', flex: col.width ? 'none' : 1 }}
                  >
                    {renderCell(
                      col,
                      newRow[col.key as keyof Omit<T, 'id'>],
                      true,
                      (val) => setNewRow(prev => ({ ...prev, [col.key]: val })),
                      'new',
                      colIndex,
                      'new'
                    )}
                  </div>
                ))}
                <div className="w-20 h-9 flex items-center justify-center gap-1">
                  <Button
                    variant="ghost"
                    size="icon"
                    className="h-6 w-6 text-success hover:text-success hover:bg-success/10"
                    onClick={handleSaveNew}
                  >
                    <Check className="w-3.5 h-3.5" />
                  </Button>
                  <Button
                    variant="ghost"
                    size="icon"
                    className="h-6 w-6 text-destructive hover:text-destructive hover:bg-destructive/10"
                    onClick={handleCancelAdd}
                  >
                    <Trash2 className="w-3.5 h-3.5" />
                  </Button>
                </div>
              </div>
            )}

            {data.length === 0 && !isAddingNew && (
              <div className="px-4 py-8 text-center text-muted-foreground text-sm">
                Nenhum registro encontrado. Clique em "Adicionar" para criar um novo.
              </div>
            )}
          </div>
        </div>

        {isLoading && (
          <div className="absolute inset-0 bg-background/50 flex items-center justify-center">
            <div className="w-6 h-6 border-2 border-primary border-t-transparent rounded-full animate-spin" />
          </div>
        )}
      </div>

      <AlertDialog open={pendingDeleteId != null} onOpenChange={(open) => !open && setPendingDeleteId(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Tem certeza que deseja excluir este registro?</AlertDialogTitle>
            <AlertDialogDescription>
              Essa ação não pode ser desfeita.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancelar</AlertDialogCancel>
            <AlertDialogAction onClick={async () => {
              if (pendingDeleteId) {
                await handleDelete(pendingDeleteId);
                setPendingDeleteId(null);
              }
            }}>
              Excluir
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}
