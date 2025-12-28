import { BarChart, Bar, XAxis, YAxis, Cell } from 'recharts';
import { ChartContainer, ChartTooltip, ChartTooltipContent } from '@/components/ui/chart';
import { formatCurrency } from '@/utils/formatters';

interface AlocacaoCategoriasChartProps {
  data: Array<{
    categoria: string;
    valor: number;
    cor: string;
  }>;
}

type AlocacaoRow = AlocacaoCategoriasChartProps['data'][number];

export const AlocacaoCategoriasChart = ({ data }: AlocacaoCategoriasChartProps) => {
  const chartConfig = {
    valor: {
      label: 'Valor',
    },
  };

  return (
    <div
      className="bg-card rounded-xl border border-border p-4 h-full"
    >
      <h3 className="text-sm font-medium text-muted-foreground mb-4">Alocação por Categoria</h3>

      <div className="h-48">
        <ChartContainer config={chartConfig} className="h-48 w-full aspect-auto">
          <BarChart
            data={data}
            margin={{ top: 5, right: 10, left: 10, bottom: 5 }}
          >
            <XAxis
              dataKey="categoria"
              tick={{ fontSize: 10 }}
              angle={-45}
              textAnchor="end"
              height={50}
            />
            <YAxis
              tickFormatter={(v) => `R$ ${v}`}
              tick={{ fontSize: 10 }}
              width={50}
            />
            <ChartTooltip
              content={
                <ChartTooltipContent
                  nameKey="categoria"
                  formatter={(val, _name, item) => {
                    const row = item.payload as AlocacaoRow | undefined;
                    if (!row) return null;

                    return (
                      <div className="flex w-full items-center justify-between gap-6">
                        <div className="flex items-center gap-2">
                          <div className="h-2 w-2 rounded-full" style={{ backgroundColor: row.cor }} />
                          <span className="text-muted-foreground">{row.categoria}</span>
                        </div>
                        <span className="font-mono font-medium tabular-nums text-foreground">
                          {formatCurrency(val as number)}
                        </span>
                      </div>
                    );
                  }}
                />
              }
            />
            <Bar dataKey="valor" radius={[4, 4, 0, 0]}>
              {data.map((entry, index) => (
                <Cell key={`cell-${index}`} fill={entry.cor} />
              ))}
            </Bar>
          </BarChart>
        </ChartContainer>
      </div>
    </div>
  );
};
