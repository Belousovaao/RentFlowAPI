import { useAuthModalStore } from '@/features/auth/stores/authModalStore';
import { useAuthStore } from '@/features/auth/stores/authStore';
import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5102/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Добавляем токен авторизации
api.interceptors.request.use((config) => {
  const raw = localStorage.getItem('auth-storage');
  let token = localStorage.getItem('token');
  if (!token && raw) {
    try {
      token = JSON.parse(raw)?.state?.token ?? null;
    } catch {
      /* ignore */
    }
  }
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});


// Централизованная обработка ошибок авторизации
api.interceptors.response.use((response) => response,
  (error) => {
    if (error.response?.status === 401) {
      const { isAuthenticated, logout } = useAuthStore.getState();

      // Не открываем модальное окно повторно,
      // если авторизация уже сброшена
      if (isAuthenticated) {
        logout();
        useAuthModalStore.getState().open();
      }
    }

    return Promise.reject(error);
  }
);

export default api;