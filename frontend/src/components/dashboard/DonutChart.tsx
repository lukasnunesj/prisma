import { PieChart, Pie, Cell } from 'recharts';
import { ChartContainer, ChartTooltip, ChartTooltipContent } from '@/components/ui/chart';
import { formatCurrency } from '@/utils/formatters';

interface DonutChartProps {
  title: string;
  value: number;
  maxValue: number;
  description?: string;
}

export const DonutChart = ({ title, value, maxValue, description }: DonutChartProps) => {
  const percentage = maxValue > 0 ? Math.min((value / maxValue) * 100, 100) : 0;
  const isNegative = value < 0;

  const chartConfig = {
    restante: {
      label: 'Restante',
      color: isNegative ? 'var(--destructive)' : 'var(--primary)',
    },
    gasto: {
      label: 'Gasto',
      color: 'var(--muted)',
    },
  };

  const data = [
    { name: 'restante', value: Math.abs(value) },
    { name: 'gasto', value: Math.max(0, maxValue - Math.abs(value)) },
  ];

  return (
    <div
      className="bg-card rounded-xl border border-border p-4 h-full"
    >
      <h3 className="text-sm font-medium text-muted-foreground mb-4">{title}</h3>

      <div className="relative h-48">
        <ChartContainer config={chartConfig} className="h-48 w-full aspect-auto">
          <PieChart>
            <ChartTooltip
              content={
                <ChartTooltipContent
                  nameKey="name"
                  formatter={(val, name) => {
                    const label = chartConfig[name as keyof typeof chartConfig]?.label ?? name;
                    return (
                      <div className="flex w-full justify-between gap-6">
                        <span className="text-muted-foreground">{label}</span>
                        <span className="font-mono font-medium tabular-nums text-foreground">
                          {formatCurrency(val as number)}
                        </span>
                      </div>
                    );
                  }}
                />
              }
            />
            <Pie
              data={data}
              cx="50%"
              cy="50%"
              innerRadius={55}
              outerRadius={75}
              paddingAngle={2}
              dataKey="value"
              stroke="none"
            >
              {data.map((entry, index) => (
                <Cell
                  key={`cell-${index}`}
                  fill={`var(--color-${entry.name})`}
                />
              ))}
            </Pie>
          </PieChart>
        </ChartContainer>

        {/* Center text */}
        <div className="absolute inset-0 flex items-center justify-center">
          <div className="text-center">
            <p className={`text-lg font-bold ${isNegative ? 'text-destructive' : 'text-primary'}`}>
              {formatCurrency(value)}
            </p>
            <p className="text-xs text-muted-foreground">
              {isNegative ? 'Excedido' : `${percentage.toFixed(0)}% restante`}
            </p>
          </div>
        </div>
      </div>

      {description && (
        <p className="text-xs text-muted-foreground text-center mt-2">{description}</p>
      )}
    </div>
  );
};
