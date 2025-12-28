import { BarChart, Bar, XAxis, YAxis, Cell } from 'recharts';
import { ChartContainer, ChartTooltip, ChartTooltipContent } from '@/components/ui/chart';
import { formatCurrency } from '@/utils/formatters';

interface FluxoContasChartProps {
  data: Array<{
    nome: string;
    valor: number;
    planejado: number;
    pago: boolean;
  }>;
}

type FluxoContasRow = FluxoContasChartProps['data'][number];

export const FluxoContasChart = ({ data }: FluxoContasChartProps) => {
  const chartData = data.map(item => ({
    ...item,
    fill: item.pago ? 'var(--success)' : 'var(--warning)',
  }));

  const chartConfig = {
    pago: {
      label: 'Pago',
      color: 'var(--success)',
    },
    pendente: {
      label: 'Pendente',
      color: 'var(--warning)',
    },
  };

  return (
    <div
      className="bg-card rounded-xl border border-border p-4 h-full"
    >
      <h3 className="text-sm font-medium text-muted-foreground mb-4">Fluxo de Contas</h3>

      <div className="h-48">
        <ChartContainer config={chartConfig} className="h-48 w-full aspect-auto">
          <BarChart
            data={chartData}
            layout="vertical"
            margin={{ top: 5, right: 30, left: 60, bottom: 5 }}
          >
            <XAxis type="number" tickFormatter={(v) => formatCurrency(v)} hide />
            <YAxis
              type="category"
              dataKey="nome"
              tick={{ fontSize: 12 }}
              width={60}
            />
            <ChartTooltip
              content={
                <ChartTooltipContent
                  nameKey="nome"
                  formatter={(val, _name, item) => {
                    const row = item.payload as FluxoContasRow | undefined;
                    if (!row) return null;

                    return (
                      <div className="grid gap-1">
                        <div className="flex items-center justify-between gap-6">
                          <span className="text-muted-foreground">Valor</span>
                          <span className="font-mono font-medium tabular-nums text-foreground">
                            {formatCurrency(val as number)}
                          </span>
                        </div>
                        <div className="flex items-center justify-between gap-6">
                          <span className="text-muted-foreground">Planejado</span>
                          <span className="font-mono font-medium tabular-nums text-foreground">
                            {formatCurrency(row.planejado)}
                          </span>
                        </div>
                        <div className={`text-xs ${row.pago ? 'text-success' : 'text-warning'}`}>
                          {row.pago ? '✓ Pago' : '○ Pendente'}
                        </div>
                      </div>
                    );
                  }}
                />
              }
            />
            <Bar dataKey="valor" radius={[0, 4, 4, 0]}>
              {chartData.map((entry, index) => (
                <Cell key={`cell-${index}`} fill={entry.fill} />
              ))}
            </Bar>
          </BarChart>
        </ChartContainer>
      </div>

      <div className="flex items-center justify-center gap-4 mt-3">
        <div className="flex items-center gap-2">
          <div className="w-3 h-3 rounded-full bg-success" />
          <span className="text-xs text-muted-foreground">Pago</span>
        </div>
        <div className="flex items-center gap-2">
          <div className="w-3 h-3 rounded-full bg-warning" />
          <span className="text-xs text-muted-foreground">Pendente</span>
        </div>
      </div>
    </div>
  );
};
