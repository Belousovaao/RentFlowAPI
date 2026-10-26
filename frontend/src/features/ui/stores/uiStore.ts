import { create } from 'zustand';
import { persist } from 'zustand/middleware';

type Theme = 'light' | 'dark';

interface UIState {
  theme: Theme;
  language: 'en' | 'ru' | 'zh';
  isHeaderVisible: boolean;
  isScrolled: boolean;
  setTheme: (theme: Theme) => void;
  toggleTheme: () => void;
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
      toggleTheme: () => set((state) => ({ theme: state.theme === 'light' ? 'dark' : 'light'})),
      setLanguage: (language) => set({ language }),
      setIsScrolled: (isScrolled) => set({ isScrolled }),
    }),
    {
      name: 'ui-storage',
    }
  )
);