import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { authService } from '@/services/auth/authService';

interface User {
  id: string;
  name: string;
  email: string;
  avatar?: string;
}

interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  login: (email: string, password: string) => Promise<boolean>;
  logout: () => void;
}

interface MonthNavigationState {
  selectedMonth: number;
  selectedYear: number;
  setMonth: (month: number) => void;
  setYear: (year: number) => void;
  setMonthYear: (month: number, year: number) => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      token: null,
      isAuthenticated: false,
      login: async (email: string, password: string) => {
        try {
          const result = await authService.login({ email, password });
          set({
            user: result.user,
            token: result.token,
            isAuthenticated: true,
          });
          return true;
        } catch (_err) {
          return false;
        }
      },
      logout: () => {
        set({
          user: null,
          token: null,
          isAuthenticated: false,
        });
      },
    }),
    {
      name: 'finance-auth',
    }
  )
);

export const useMonthNavigationStore = create<MonthNavigationState>()(
  persist(
    (set) => ({
      selectedMonth: new Date().getMonth() + 1,
      selectedYear: new Date().getFullYear(),
      setMonth: (month) => set({ selectedMonth: month }),
      setYear: (year) => set({ selectedYear: year }),
      setMonthYear: (month, year) => set({ selectedMonth: month, selectedYear: year }),
    }),
    {
      name: 'finance-month-nav',
    }
  )
);
