import CountUp from 'react-countup';
import { cn } from '@/lib/utils';
import { TrendingUp, TrendingDown, Minus } from 'lucide-react';

interface MetricCardProps {
  title: string;
  value: number;
  previousValue?: number;
  format?: 'currency' | 'number' | 'percent';
  colorScheme?: 'default' | 'success' | 'warning' | 'danger';
  delay?: number;
  icon?: React.ReactNode;
}

export const MetricCard = ({
  title,
  value,
  previousValue,
  format = 'currency',
  colorScheme = 'default',
  delay = 0,
  icon,
}: MetricCardProps) => {
  const formatValue = (val: number) => {
    if (format === 'currency') {
      return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL',
      }).format(val);
    }
    if (format === 'percent') {
      return `${val.toFixed(1)}%`;
    }
    return val.toLocaleString('pt-BR');
  };

  const diff = previousValue !== undefined ? value - previousValue : 0;
  const diffPercent = previousValue && previousValue !== 0
    ? ((value - previousValue) / previousValue) * 100
    : 0;

  const getTrendIcon = () => {
    if (diff > 0) return <TrendingUp className="w-3.5 h-3.5" />;
    if (diff < 0) return <TrendingDown className="w-3.5 h-3.5" />;
    return <Minus className="w-3.5 h-3.5" />;
  };

  const getTrendColor = () => {
    if (diff > 0) return 'text-success';
    if (diff < 0) return 'text-destructive';
    return 'text-muted-foreground';
  };

  const getAccentColor = () => {
    switch (colorScheme) {
      case 'success': return 'from-success/10 to-transparent';
      case 'warning': return 'from-warning/10 to-transparent';
      case 'danger': return 'from-destructive/10 to-transparent';
      default: return 'from-primary/10 to-transparent';
    }
  };

  const getBorderColor = () => {
    switch (colorScheme) {
      case 'success': return 'border-l-success';
      case 'warning': return 'border-l-warning';
      case 'danger': return 'border-l-destructive';
      default: return 'border-l-primary';
    }
  };

  return (
    <div
      className={cn(
        'metric-card relative rounded-xl bg-card border border-border p-4',
        'border-l-4 overflow-hidden',
        getBorderColor()
      )}
    >
      {/* Gradient overlay */}
      <div className={cn(
        'absolute inset-0 bg-gradient-to-br pointer-events-none',
        getAccentColor()
      )} />

      <div className="relative z-10">
        <div className="flex items-center justify-between mb-2">
          <span className="text-sm font-medium text-muted-foreground">{title}</span>
          {icon && <div className="text-muted-foreground">{icon}</div>}
        </div>

        <div className="flex items-baseline gap-2">
          <span className="text-2xl font-bold text-foreground">
            <CountUp
              end={value}
              duration={1.5}
              decimals={format === 'currency' ? 2 : 0}
              decimal=","
              separator="."
              prefix={format === 'currency' ? 'R$ ' : ''}
              suffix={format === 'percent' ? '%' : ''}
            />
          </span>
        </div>

        {previousValue !== undefined && (
          <div className={cn('flex items-center gap-1 mt-2 text-xs', getTrendColor())}>
            {getTrendIcon()}
            <span>
              {diff >= 0 ? '+' : ''}{formatValue(diff)} ({diffPercent >= 0 ? '+' : ''}{diffPercent.toFixed(1)}%)
            </span>
            <span className="text-muted-foreground ml-1">vs mês anterior</span>
          </div>
        )}
      </div>
    </div>
  );
};
