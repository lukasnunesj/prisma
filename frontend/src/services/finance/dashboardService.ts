import type { DashboardMetrics } from '@/types/finance';
import { apiClient } from '@/services/apiClient';

export const dashboardService = {
    metrics: async (mes: number, ano: number): Promise<DashboardMetrics> => {
        return apiClient.get<DashboardMetrics>(`/dashboard/metrics/${mes}/${ano}`);
    },
};
