import axios from 'axios';
import { useAuthStore } from '@/features/auth/stores/authStore';


const BASE_URL = "http://localhost:5102";

// Создаем axios инстанс
export const axiosClient = axios.create({
  baseURL: BASE_URL,
  timeout: 30000,
  headers: { 'Content-Type': 'application/json',},
});

// Интерсептор для добавления токена
axiosClient.interceptors.request.use((config) => {
  const token = useAuthStore.getState().token;

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Интерсептор для обработки ошибок
axiosClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status == 401) {
      useAuthStore.getState().logout();
      window.location.href = '/login';
    }

    return Promise.reject(error);
  });

export const apiClient = {
  async get<T>(url: string): Promise<T> {
    const response = await axiosClient.get<T>(url);

    return response.data;
  },

  async post<T>(url: string, body: unknown): Promise<T> {
    const response = await axiosClient.post<T>(url, body);

    return response.data;
  },

  async delete<T>(url: string): Promise<T> {
    const response = await axiosClient.delete<T>(url);
    
    return response.data;
  },
};