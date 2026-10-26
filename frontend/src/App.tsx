import { RouterProvider } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { I18nProvider } from './app/providers/I18nProvider';
import { router } from './app/router';
import { ThemeSync } from './features/ui/components/ThemeSync';

// Создаем queryClient вне компонента
const queryClient = new QueryClient({
    defaultOptions: {
        queries: {
            staleTime: 5 * 60 * 1000,
            gcTime: 10 * 60 * 1000,
            retry: 1,
            refetchOnWindowFocus: false,
        },
    },
});

function App() {
    return (
        <QueryClientProvider client={queryClient}>
                <I18nProvider>
                    <ThemeSync />
                    <RouterProvider router={router} />
                </I18nProvider>
        </QueryClientProvider>
    );
}

export default App;