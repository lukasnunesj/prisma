export interface AuthUser {
    id: string;
    name: string;
    email: string;
    avatar?: string;
}

export interface LoginRequest {
    email: string;
    password: string;
}

export interface AuthResponse {
    user: AuthUser;
    token: string;
    expiration: string;
}

import { apiClient } from '@/services/apiClient';

export interface RegisterRequest {
    name: string;
    email: string;
    password: string;
}

export interface ChangePasswordRequest {
    currentPassword: string;
    newPassword: string;
}

// Refresh returns same structure as AuthResponse
export type RefreshResponse = AuthResponse;

export const authService = {
    login: async ({ email, password }: LoginRequest): Promise<AuthResponse> => {
        return apiClient.post<AuthResponse>('/auth/login', { email, password });
    },
    register: async ({ name, email, password }: RegisterRequest): Promise<AuthResponse> => {
        return apiClient.post<AuthResponse>('/auth/register', { name, email, password });
    },
    refresh: async (): Promise<AuthResponse> => {
        return apiClient.post<AuthResponse>('/auth/refresh');
    },
    logout: async (): Promise<void> => {
        await apiClient.post<void>('/auth/logout');
    },
    changePassword: async (body: ChangePasswordRequest): Promise<void> => {
        await apiClient.post<void>('/auth/change-password', body);
    },
};
