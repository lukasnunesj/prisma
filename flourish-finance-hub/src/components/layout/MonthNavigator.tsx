import { useMonthNavigationStore } from '@/stores/appStore';
import { getMonthShortName } from '@/utils/formatters';
import { cn } from '@/lib/utils';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { ChevronLeft, ChevronRight } from 'lucide-react';
import { Button } from '@/components/ui/button';

const months = Array.from({ length: 12 }, (_, i) => i + 1);
const years = Array.from({ length: 10 }, (_, i) => new Date().getFullYear() - 5 + i);

export const MonthNavigator = () => {
  const { selectedMonth, selectedYear, setMonth, setYear, setMonthYear } = useMonthNavigationStore();

  const handlePrevMonth = () => {
    if (selectedMonth === 1) {
      setMonthYear(12, selectedYear - 1);
    } else {
      setMonth(selectedMonth - 1);
    }
  };

  const handleNextMonth = () => {
    if (selectedMonth === 12) {
      setMonthYear(1, selectedYear + 1);
    } else {
      setMonth(selectedMonth + 1);
    }
  };

  return (
    <div
      className="fixed bottom-0 left-0 right-0 h-14 bg-card border-t border-border z-50 shadow-lg"
    >
      <div className="h-full max-w-[1920px] mx-auto px-6 flex items-center justify-center gap-4">
        {/* Previous month button */}
        <Button
          variant="ghost"
          size="icon"
          onClick={handlePrevMonth}
          className="h-8 w-8"
        >
          <ChevronLeft className="w-4 h-4" />
        </Button>

        {/* Month tabs */}
        <div className="flex items-center gap-1 bg-muted rounded-lg p-1">
          {months.map((month) => (
            <button
              key={month}
              onClick={() => setMonth(month)}
              className={cn(
                'px-3 py-1.5 text-sm font-medium rounded-md transition-all duration-200',
                selectedMonth === month
                  ? 'bg-primary text-primary-foreground shadow-sm'
                  : 'text-muted-foreground hover:text-foreground hover:bg-background/50'
              )}
            >
              {getMonthShortName(month)}
            </button>
          ))}
        </div>

        {/* Next month button */}
        <Button
          variant="ghost"
          size="icon"
          onClick={handleNextMonth}
          className="h-8 w-8"
        >
          <ChevronRight className="w-4 h-4" />
        </Button>

        {/* Year selector */}
        <Select
          value={selectedYear.toString()}
          onValueChange={(value) => setYear(parseInt(value))}
        >
          <SelectTrigger className="w-24 h-9">
            <SelectValue />
          </SelectTrigger>
          <SelectContent className="bg-popover">
            {years.map((year) => (
              <SelectItem key={year} value={year.toString()}>
                {year}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>
    </div>
  );
};
