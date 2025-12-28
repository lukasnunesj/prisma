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
  setToken: (token: string | null) => void;
  setUser: (user: User | null) => void;
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
    setToken: (token) => set({ token, isAuthenticated: !!token }),
    setUser: (user) => set({ user }),
    logout: () => {
      // best-effort: revoga refresh token no servidor e limpa cookies
      authService.logout().catch(() => {
        // ignore
      });
      set({
        user: null,
        token: null,
        isAuthenticated: false,
      });
    },
  })
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
