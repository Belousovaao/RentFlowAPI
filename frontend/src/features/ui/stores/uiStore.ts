import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface UIState {
  theme: 'light' | 'dark';
  language: 'en' | 'ru' | 'zh';
  isHeaderVisible: boolean;
  isScrolled: boolean;
  setTheme: (theme: 'light' | 'dark') => void;
  setLanguage: (language: 'en' | 'ru' | 'zh') => void;
  setIsScrolled: (isScrolled: boolean) => void;
}

export const useUIStore = create<UIState>()(
  persist(
    (set) => ({
      theme: 'light',
      language: 'en',
      isHeaderVisible: true,
      isScrolled: false,
      setTheme: (theme) => set({ theme }),
      setLanguage: (language) => set({ language }),
      setIsScrolled: (isScrolled) => set({ isScrolled }),
    }),
    {
      name: 'ui-storage',
    }
  )
);