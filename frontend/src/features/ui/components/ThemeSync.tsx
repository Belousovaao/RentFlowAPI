import { useEffect } from 'react';
import { useUIStore } from '../stores/uiStore';

export const ThemeSync = () => {
    const theme = useUIStore(state => state.theme);

    useEffect(() => {
        const root = document.documentElement;

        root.classList.toggle('dark', theme === 'dark');

        root.style.colorScheme = theme;
    }, [theme]);

    return null;
};