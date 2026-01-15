import { useAuthStore } from '@/stores/appStore';

const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5000/api';

let refreshing: Promise<void> | null = null;

function getCookie(name: string) {
    const match = document.cookie.match(new RegExp(`(?:^|; )${name.replace(/([.$?*|{}()\\[\\]\\\\\/\+^])/g, '\\$1')}=([^;]*)`));
    return match ? decodeURIComponent(match[1]) : null;
}

async function request<T>(url: string, options: RequestInit = {}, retry = false): Promise<T> {
    const token = useAuthStore.getState().token;

    const headers: Record<string, string> = {
        'Content-Type': 'application/json',
        ...(options.headers as Record<string, string> | undefined),
    };

    const method = (options.method ?? 'GET').toUpperCase();
    if (method !== 'GET' && method !== 'HEAD') {
        const xsrf = getCookie('XSRF-TOKEN');
        if (xsrf) {
            headers['X-XSRF-TOKEN'] = xsrf;
        }
    }

    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }

    const response = await fetch(`${BASE_URL}${url}`, {
        ...options,
        headers,
        credentials: 'include',
    });

    if (!response.ok) {
        if (response.status === 401 && !retry && !url.startsWith('/auth/refresh') && !url.startsWith('/auth/login')) {
            if (!refreshing) {
                refreshing = (async () => {
                    try {
                        const result = await authService.refresh();
                        useAuthStore.getState().setToken(result.token);
                        useAuthStore.getState().setUser(result.user);
                    } catch (_err) {
                        useAuthStore.getState().logout();
                    } finally {
                        refreshing = null;
                    }
                })();
            }

            await refreshing;
            return request<T>(url, options, true);
        }
        const errorText = await response.text();
        throw new Error(errorText || response.statusText);
    }

    // For 204 No Content responses, return undefined
    if (response.status === 204) {
        return undefined as unknown as T;
    }

    return (await response.json()) as T;
}

import { authService } from '@/services/auth/authService';

export const apiClient = {
    get: <T>(url: string) => request<T>(url),
    post: <T>(url: string, body?: unknown) =>
        request<T>(url, {
            method: 'POST',
            body: body !== undefined ? JSON.stringify(body) : undefined,
        }),
    put: <T>(url: string, body?: unknown) =>
        request<T>(url, {
            method: 'PUT',
            body: body !== undefined ? JSON.stringify(body) : undefined,
        }),
    delete: <T>(url: string) => request<T>(url, { method: 'DELETE' }),
};
