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

export interface LoginResponse {
    user: AuthUser;
    token: string;
}

const delay = (ms: number) => new Promise((resolve) => setTimeout(resolve, ms));

export const authService = {
    login: async ({ email, password }: LoginRequest): Promise<LoginResponse> => {
        await delay(800);

        if (!email || password.length < 4) {
            throw new Error('INVALID_CREDENTIALS');
        }

        const user: AuthUser = {
            id: '1',
            name: email.split('@')[0].charAt(0).toUpperCase() + email.split('@')[0].slice(1),
            email,
            avatar: undefined,
        };

        const token = `mock-jwt-token-${Date.now()}`;

        return { user, token };
    },
};
