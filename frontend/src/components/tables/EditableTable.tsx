import { useState, useEffect, useRef } from 'react';
import { cn } from '@/lib/utils';
import { Trash2 } from 'lucide-react';
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
  const [newRow, setNewRow] = useState({ ...emptyRowTemplate });
  const [creatingRow, setCreatingRow] = useState<Omit<T, 'id'> | null>(null);
  const [isCreatingRowSaving, setIsCreatingRowSaving] = useState(false);
  const [pendingDeleteId, setPendingDeleteId] = useState<string | null>(null);
  const [hoveredId, setHoveredId] = useState<string | null>(null);

  const [editingValues, setEditingValues] = useState<Record<string, Record<string, unknown>>>({});
  const [debouncedTrigger, setDebouncedTrigger] = useState<{ rowId: string; key: string; value: unknown } | null>(null);
  const [pendingDebounce, setPendingDebounce] = useState<{ rowId: string; key: string } | null>(null);

  const latestNewRowRef = useRef(newRow);
  const latestCreatingRowRef = useRef(creatingRow);
  const latestEditingValuesRef = useRef(editingValues);
  const debounceTimeoutRef = useRef<number | null>(null);

  const lastFocusedColKeyRef = useRef<string | null>(null);
  const creatingRowSnapshotRef = useRef<Omit<T, 'id'> | null>(null);
  const dataLengthAtSaveRef = useRef<number>(data.length);

  useEffect(() => {
    latestNewRowRef.current = newRow;
  }, [newRow]);

  useEffect(() => {
    latestCreatingRowRef.current = creatingRow;
  }, [creatingRow]);

  useEffect(() => {
    latestEditingValuesRef.current = editingValues;
  }, [editingValues]);

  useEffect(() => {
    if (!isCreatingRowSaving || !creatingRowSnapshotRef.current) return;
    if (data.length <= dataLengthAtSaveRef.current) return;

    const snapshot = creatingRowSnapshotRef.current;
    const match = data.find((row) => {
      return columns.every((col) => {
        const colKey = col.key as string;
        if (colKey === 'id') return true;

        const a = (snapshot as Record<string, unknown>)[colKey];
        const b = (row as Record<string, unknown>)[colKey];

        if (a == null && b == null) return true;
        return String(a ?? '') === String(b ?? '');
      });
    });

    const colKey = lastFocusedColKeyRef.current;
    if (match && colKey) {
      window.requestAnimationFrame(() => {
        const el = document.querySelector(
          `[data-row-id="${match.id}"][data-col-key="${colKey}"]`
        ) as HTMLElement | null;
        el?.focus();
      });
    }

    setCreatingRow(null);
    setIsCreatingRowSaving(false);
    creatingRowSnapshotRef.current = null;
  }, [data, columns, isCreatingRowSaving]);

  useEffect(() => {
    if (!pendingDebounce) return;

    if (debounceTimeoutRef.current != null) {
      window.clearTimeout(debounceTimeoutRef.current);
    }

    debounceTimeoutRef.current = window.setTimeout(() => {
      const pending = pendingDebounce;
      if (!pending) return;

      if (pending.rowId === 'new') {
        const payload = latestNewRowRef.current as Record<string, unknown>;
        setDebouncedTrigger({ rowId: 'new', key: pending.key, value: payload[pending.key] });
        return;
      }

      if (pending.rowId === 'creating') {
        const payload = latestCreatingRowRef.current as unknown as Record<string, unknown> | null;
        setDebouncedTrigger({ rowId: 'creating', key: pending.key, value: payload?.[pending.key] });
        return;
      }

      const rowValues = latestEditingValuesRef.current[pending.rowId];
      setDebouncedTrigger({ rowId: pending.rowId, key: pending.key, value: rowValues?.[pending.key] });
    }, 500);

    return () => {
      if (debounceTimeoutRef.current != null) {
        window.clearTimeout(debounceTimeoutRef.current);
      }
    };
  }, [pendingDebounce]);

  useEffect(() => {
    if (!debouncedTrigger) return;

    const { rowId, key, value } = debouncedTrigger;

    if (rowId === 'new') {
      const payload = latestNewRowRef.current;
      const hasValue = Object.entries(payload).some(([k, v]) => k !== 'id' && isValueFilled(v));

      if (hasValue) {
        onAdd(payload as Omit<T, 'id'>)
          .then(() => {
            setNewRow({ ...emptyRowTemplate });
          })
          .catch(() => {
            toast({ title: 'Erro ao adicionar', variant: 'destructive' });
          });
      }
    } else if (rowId === 'creating') {
      if (isCreatingRowSaving) {
        // já em processo de salvar; ignore
      } else {
        const payload = latestCreatingRowRef.current;
        const hasValue = payload && Object.entries(payload).some(([k, v]) => k !== 'id' && isValueFilled(v));
        if (payload && hasValue) {
          creatingRowSnapshotRef.current = payload;
          dataLengthAtSaveRef.current = data.length;
          setIsCreatingRowSaving(true);
          onAdd(payload).catch(() => {
            toast({ title: 'Erro ao adicionar', variant: 'destructive' });
            setIsCreatingRowSaving(false);
          });
        }
      }
    } else {
      const row = data.find(r => r.id === rowId);
      if (row) {
        handleUpdate(row, key as keyof T, value);
      }
    }

    setDebouncedTrigger(null);
  }, [debouncedTrigger, data, emptyRowTemplate, onAdd, toast]);

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

  const getEditingValue = (rowId: string, key: string, originalValue: unknown) => {
    return editingValues[rowId]?.[key] ?? originalValue;
  };

  const setEditingValue = (rowId: string, key: string, value: unknown) => {
    setEditingValues(prev => ({
      ...prev,
      [rowId]: { ...(prev[rowId] || {}), [key]: value }
    }));
  };

  const scheduleDebounce = (rowId: string, key: string) => {
    if (rowId === 'creating' && isCreatingRowSaving) return; // evita debounce enquanto salva
    setPendingDebounce({ rowId, key });
  };

  const handleUpdate = async (row: T, key: keyof T, value: unknown) => {
    try {
      await onUpdate({ ...row, [key]: value });
      setEditingValues(prev => {
        const newValues = { ...prev };
        if (newValues[row.id]) {
          delete newValues[row.id][key as string];
          if (Object.keys(newValues[row.id]).length === 0) {
            delete newValues[row.id];
          }
        }
        return newValues;
      });
    } catch {
      toast({ title: 'Erro ao atualizar', variant: 'destructive' });
    }
  };

  const handleDelete = async (id: string) => {
    try {
      await onDelete(id);
      toast({ title: 'Registro removido com sucesso!' });
    } catch {
      toast({ title: 'Erro ao remover', variant: 'destructive' });
    }
  };

  const isValueFilled = (value: unknown): boolean => {
    if (value == null) return false;
    if (typeof value === 'string') return value.trim() !== '';
    if (typeof value === 'boolean') return value === true;
    if (typeof value === 'number') return value !== 0;
    return true;
  };

  const handleNewRowChange = (key: keyof T, value: unknown) => {
    setNewRow(prev => ({ ...prev, [key]: value }));
  };

  const promoteNewRowToCreating = (nextRow: Omit<T, 'id'>) => {
    if (creatingRow) return;
    setCreatingRow(nextRow);
    setNewRow({ ...emptyRowTemplate });
  };

  const handleNewRowBlur = async () => {
    const values = Object.entries(newRow).filter(([k]) => k !== 'id');
    const hasAnyValue = values.some(([_, v]) => isValueFilled(v));

    if (hasAnyValue) {
      try {
        await onAdd(newRow as Omit<T, 'id'>);
        setNewRow({ ...emptyRowTemplate });
      } catch {
        toast({ title: 'Erro ao adicionar', variant: 'destructive' });
      }
    }
  };

  const renderCell = (col: Column<T>, value: unknown, rowId: string, row?: T) => {
    const isNewRow = rowId === 'new';
    const isCreatingRow = rowId === 'creating';
    const key = col.key as string;
    const displayValue = isNewRow
      ? value
      : isCreatingRow
        ? value
        : getEditingValue(rowId, key, value);

    if (col.render) {
      return col.render(displayValue, (row ?? ({} as T)), (val) => {
        if (isNewRow) {
          const nextRow = { ...latestNewRowRef.current, [key]: val } as Omit<T, 'id'>;
          promoteNewRowToCreating(nextRow);
          if (col.type === 'select' || col.type === 'checkbox') {
            onAdd(nextRow)
              .then(() => setCreatingRow(null))
              .catch(() => toast({ title: 'Erro ao adicionar', variant: 'destructive' }));
          } else {
            scheduleDebounce('creating', key);
          }
          return;
        }

        if (isCreatingRow) {
          const nextRow = { ...(latestCreatingRowRef.current ?? ({} as Omit<T, 'id'>)), [key]: val };
          setCreatingRow(nextRow);
          if (col.type === 'select' || col.type === 'checkbox') {
            onAdd(nextRow)
              .then(() => setCreatingRow(null))
              .catch(() => toast({ title: 'Erro ao adicionar', variant: 'destructive' }));
          } else {
            scheduleDebounce('creating', key);
          }
        } else {
          setEditingValue(rowId, key, val);
          scheduleDebounce(rowId, key);
        }
      });
    }

    if (col.type === 'checkbox') {
      return (
        <button
          type="button"
          onClick={() => {
            const newValue = !displayValue;
            if (isNewRow) {
              const nextRow = { ...latestNewRowRef.current, [key]: newValue } as Omit<T, 'id'>;
              promoteNewRowToCreating(nextRow);
              onAdd(nextRow)
                .then(() => {
                  creatingRowSnapshotRef.current = nextRow;
                  dataLengthAtSaveRef.current = data.length;
                  setIsCreatingRowSaving(true);
                })
                .catch(() => toast({ title: 'Erro ao adicionar', variant: 'destructive' }));
              return;
            }

            if (isCreatingRow) {
              const nextRow = { ...(latestCreatingRowRef.current ?? ({} as Omit<T, 'id'>)), [key]: newValue };
              setCreatingRow(nextRow);
              onAdd(nextRow)
                .then(() => {
                  creatingRowSnapshotRef.current = nextRow;
                  dataLengthAtSaveRef.current = data.length;
                  setIsCreatingRowSaving(true);
                })
                .catch(() => toast({ title: 'Erro ao adicionar', variant: 'destructive' }));
              return;
            }

            if (row) {
              handleUpdate(row, col.key as keyof T, newValue);
            }
          }}
          className={cn(
            'w-5 h-5 rounded border-2 flex items-center justify-center transition-colors',
            displayValue ? 'bg-primary border-primary' : 'border-muted-foreground/30 hover:border-primary/50'
          )}
        >
          {displayValue && (
            <svg className="w-3 h-3 text-primary-foreground" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="3">
              <polyline points="20 6 9 17 4 12" />
            </svg>
          )}
        </button>
      );
    }

    if (col.type === 'select' && col.options) {
      return (
        <Select
          value={displayValue == null ? '' : String(displayValue)}
          onValueChange={(val) => {
            if (isNewRow) {
              const nextRow = { ...latestNewRowRef.current, [key]: val } as Omit<T, 'id'>;
              promoteNewRowToCreating(nextRow);
              onAdd(nextRow)
                .then(() => {
                  creatingRowSnapshotRef.current = nextRow;
                  dataLengthAtSaveRef.current = data.length;
                  setIsCreatingRowSaving(true);
                })
                .catch(() => toast({ title: 'Erro ao adicionar', variant: 'destructive' }));
              return;
            }

            if (isCreatingRow) {
              const nextRow = { ...(latestCreatingRowRef.current ?? ({} as Omit<T, 'id'>)), [key]: val };
              setCreatingRow(nextRow);
              onAdd(nextRow)
                .then(() => {
                  creatingRowSnapshotRef.current = nextRow;
                  dataLengthAtSaveRef.current = data.length;
                  setIsCreatingRowSaving(true);
                })
                .catch(() => toast({ title: 'Erro ao adicionar', variant: 'destructive' }));
              return;
            }

            if (row) {
              handleUpdate(row, col.key as keyof T, val);
            }
          }}
        >
          <SelectTrigger className="h-7 bg-transparent border-0 px-2 py-0 shadow-none focus:ring-0">
            <SelectValue placeholder="Selecione..." />
          </SelectTrigger>
          <SelectContent>
            {col.options.map(opt => (
              <SelectItem key={opt.value} value={opt.value}>{opt.label}</SelectItem>
            ))}
          </SelectContent>
        </Select>
      );
    }

    if (col.type === 'color') {
      const colorValue = typeof displayValue === 'string' ? displayValue : '#2D7A6E';
      return (
        <input
          type="color"
          value={colorValue}
          onChange={(e) => {
            if (isNewRow) {
              handleNewRowChange(col.key as keyof T, e.target.value);
            } else if (row) {
              handleUpdate(row, col.key as keyof T, e.target.value);
            }
          }}
          className="w-8 h-6 rounded cursor-pointer"
        />
      );
    }

    const isMoney = isMoneyColumn(col);
    const inputValue = isMoney ? formatMoneyInput(displayValue) : (displayValue == null ? '' : String(displayValue));

    return (
      <input
        type={col.type === 'date' ? 'date' : 'text'}
        value={inputValue}
        data-row-id={rowId}
        data-col-key={key}
        onFocus={() => {
          lastFocusedColKeyRef.current = key;
        }}
        onChange={(e) => {
          const newValue = isMoney
            ? parseMoneyInput(e.target.value)
            : col.type === 'number'
              ? (parseFloat(e.target.value) || 0)
              : e.target.value;

          if (isNewRow) {
            const nextRow = { ...latestNewRowRef.current, [key]: newValue } as Omit<T, 'id'>;
            promoteNewRowToCreating(nextRow);
            scheduleDebounce('creating', key);
            return;
          }

          if (isCreatingRow) {
            const nextRow = { ...(latestCreatingRowRef.current ?? ({} as Omit<T, 'id'>)), [key]: newValue };
            setCreatingRow(nextRow);
            scheduleDebounce('creating', key);
            return;
          }

          setEditingValue(rowId, key, newValue);
          scheduleDebounce(rowId, key);
        }}
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
        </div>

        <div className="overflow-x-auto">
          <div className="flex items-center bg-muted/20 border-b border-border">
            {columns.map((col) => (
              <div
                key={col.key as string}
                className="px-3 py-2 text-xs font-medium text-muted-foreground uppercase tracking-wider"
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
            {data.map((row) => (
              <div
                key={row.id}
                className="flex items-center border-b border-border hover:bg-muted/30 transition-colors group"
                onMouseEnter={() => setHoveredId(row.id)}
                onMouseLeave={() => setHoveredId(null)}
              >
                {columns.map((col) => (
                  <div
                    key={col.key as string}
                    className="h-9 flex items-center"
                    style={{ width: col.width || 'auto', flex: col.width ? 'none' : 1 }}
                  >
                    {renderCell(col, row[col.key as keyof T], row.id, row)}
                  </div>
                ))}
                <div className="w-20 h-9 flex items-center justify-center">
                  <Button
                    variant="ghost"
                    size="icon"
                    className="h-6 w-6 text-muted-foreground hover:text-destructive hover:bg-destructive/10 opacity-0 group-hover:opacity-100 transition-opacity"
                    onClick={() => setPendingDeleteId(row.id)}
                  >
                    <Trash2 className="w-3.5 h-3.5" />
                  </Button>
                </div>
              </div>
            ))}

            <div
              key="draft-active"
              className={cn(
                'flex items-center border-b border-border transition-colors',
                creatingRow ? 'bg-muted/5' : 'bg-muted/10 opacity-50 hover:opacity-100 transition-opacity'
              )}
            >
              {columns.map((col) => (
                <div
                  key={col.key as string}
                  className="h-9 flex items-center"
                  style={{ width: col.width || 'auto', flex: col.width ? 'none' : 1 }}
                >
                  {creatingRow
                    ? renderCell(col, (creatingRow as Record<string, unknown>)[col.key as string], 'creating')
                    : renderCell(col, newRow[col.key as keyof Omit<T, 'id'>], 'new')}
                </div>
              ))}
              <div className="w-20 h-9 flex items-center justify-center">
                <span className="text-xs text-muted-foreground/50"></span>
              </div>
            </div>

            {creatingRow && (
              <div
                key="draft-empty"
                className="flex items-center border-b border-border bg-muted/10 opacity-50 hover:opacity-100 transition-opacity"
              >
                {columns.map((col) => (
                  <div
                    key={col.key as string}
                    className="h-9 flex items-center"
                    style={{ width: col.width || 'auto', flex: col.width ? 'none' : 1 }}
                  >
                    {renderCell(col, newRow[col.key as keyof Omit<T, 'id'>], 'new')}
                  </div>
                ))}
                <div className="w-20 h-9 flex items-center justify-center">
                  <span className="text-xs text-muted-foreground/50"></span>
                </div>
              </div>
            )}

            {data.length === 0 && (
              <div className="px-4 py-8 text-center text-muted-foreground text-sm">
                Nenhum registro encontrado. Digite na linha abaixo para criar um novo.
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
