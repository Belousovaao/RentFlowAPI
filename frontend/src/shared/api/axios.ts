import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5102/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Добавляем токен авторизации
api.interceptors.request.use((config) => {
  console.log('Request URL:', config.baseURL);
  console.log('Request method:', config.method);
  console.log('Request data:', config.data);
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default api;