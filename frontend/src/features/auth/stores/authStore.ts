// frontend/src/features/auth/stores/authStore.ts
import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface User {
    id: string;
    email: string;
    firstName: string;
    lastName: string;
    role: 'user' | 'admin';
}

interface AuthState {
    user: User | null;
    token: string | null;
    isAuthenticated: boolean;
    isLoading: boolean;
    
    login: (email: string, password: string) => Promise<void>;
    logout: () => void;
    register: (userData: RegisterData) => Promise<void>;
    setUser: (user: User | null) => void;
    setToken: (token: string | null) => void;
}

interface RegisterData {
    email: string;
    password: string;
    firstName: string;
    lastName: string;
}

export const useAuthStore = create<AuthState>()(
    persist(
        (set) => ({
            user: null,
            token: null,
            isAuthenticated: false,
            isLoading: false,
            
            login: async (email: string, password: string) => {
                set({ isLoading: true });
                try {
                    // Здесь будет реальный API запрос
                    // const response = await apiClient.post('/auth/login', { email, password });
                    // const { user, token } = response;
                    
                    // Временная заглушка
                    const mockUser: User = {
                        id: '1',
                        email,
                        firstName: 'John',
                        lastName: 'Doe',
                        role: 'user',
                    };
                    const mockToken = 'mock-jwt-token';
                    
                    set({
                        user: mockUser,
                        token: mockToken,
                        isAuthenticated: true,
                        isLoading: false,
                    });
                } catch (error) {
                    set({ isLoading: false });
                    throw error;
                }
            },
            
            logout: () => {
                set({
                    user: null,
                    token: null,
                    isAuthenticated: false,
                    isLoading: false,
                });
                localStorage.removeItem('auth-storage');
            },
            
            register: async (userData: RegisterData) => {
                set({ isLoading: true });
                try {
                    // Здесь будет реальный API запрос
                    // const response = await apiClient.post('/auth/register', userData);
                    
                    // Временная заглушка
                    const mockUser: User = {
                        id: '1',
                        email: userData.email,
                        firstName: userData.firstName,
                        lastName: userData.lastName,
                        role: 'user',
                    };
                    const mockToken = 'mock-jwt-token';
                    
                    set({
                        user: mockUser,
                        token: mockToken,
                        isAuthenticated: true,
                        isLoading: false,
                    });
                } catch (error) {
                    set({ isLoading: false });
                    throw error;
                }
            },
            
            setUser: (user) => set({ user }),
            setToken: (token) => set({ token }),
        }),
        {
            name: 'auth-storage',
            partialize: (state) => ({
                user: state.user,
                token: state.token,
                isAuthenticated: state.isAuthenticated,
            }),
        }
    )
);