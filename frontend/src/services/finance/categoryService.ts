import type { Categoria } from '@/types/finance';
import { apiClient } from '@/services/apiClient';
import { financeStore } from '@/services/finance/financeStore';

export const categoryService = {
    list: async () => {
        const cached = financeStore.getCategorias();
        if (cached.length === 0) {
            const categorias = await apiClient.get<Categoria[]>('/category');
            financeStore.setCategorias(categorias);
            return categorias;
        }

        return cached;
    },

    add: async (data: Omit<Categoria, 'id'>) => {
        const newCategoria = await apiClient.post<Categoria>('/category', data);
        financeStore.setCategorias([...financeStore.getCategorias(), newCategoria]);
        return newCategoria;
    },

    update: async (data: Categoria) => {
        const updated = await apiClient.put<Categoria | null>(`/category/${data.id}`, data);
        const resolved = updated ?? data;
        financeStore.setCategorias(
            financeStore.getCategorias().map((row) => (row.id === data.id ? resolved : row)),
        );
        return resolved;
    },

    remove: async (id: string) => {
        await apiClient.delete(`/category/${id}`);
        financeStore.setCategorias(financeStore.getCategorias().filter((c) => c.id !== id));
        return id;
    },
};
