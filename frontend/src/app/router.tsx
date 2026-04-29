import { createBrowserRouter } from 'react-router-dom';
import { AssetsPage } from '../pages/assets/AssetPage';
import { AssetDetailPage } from '../pages/assets/AssetDetailPage';
import { BookingsPage } from '../pages/bookings/BookingsPage';
import { HomePage } from '../pages/home/HomePage';
import { AccountPage } from '../pages/account/AccountPage';
import { Header } from '../features/ui/components/Header';
import { Footer } from '../features/ui/components/Footer';
import { Outlet } from 'react-router-dom';
import { ProtectedRoute } from '@/features/auth/components/ProtectedRoute';
import { AdminAssetsListPage } from '@/pages/admin/AssetPage';
import { AssetFormPage } from '@/pages/admin/AssetFormPage';
import { useAuthModalStore } from '@/features/auth/stores/authModalStore';
import { AuthModal } from '@/features/auth/components/AuthModal';

export const Layout = () => {
    const { isOpen, close } = useAuthModalStore();
    return (
        <>
            <Header />
            <main className="min-h-screen">
                <Outlet />
            </main>
            <Footer />
            <AuthModal isOpen={isOpen} onClose={close} />
        </>
    );
};

export const router = createBrowserRouter([
    {
        path: '/',
        element: <Layout />,
        children: [
            {
                index: true,
                element: <HomePage />,
            },
            {
                path: 'assets',
                element: <AssetsPage />,
            },
            {
                path: 'assets/:id',
                element: <AssetDetailPage />,
            },
            {
                path: 'contact',
                element: <div>Contact Page</div>, // Временная заглушка
            },
            { //маршруты, ребуюзие авторизации
                element: <ProtectedRoute />,
                children: [
                    {
                        path: 'bookings',
                        element: <BookingsPage />,
                    },
                    {
                        path: 'account',
                        element: <AccountPage />,
                    },
                ]
            },
            {
                // админские маршруты
                path: 'admin',
                element: <ProtectedRoute requiredRole='admin'/>,
                children: [
                    {
                        path: 'assets', 
                        element: <AdminAssetsListPage />,
                    },
                    {
                        path: 'assets/new',
                        element: <AssetFormPage />,
                    },
                    {
                        path: 'assets/:id/edit',
                        element: <AssetFormPage />,
                    },
                    {
                        path: 'assets/:id',
                        element: <div>Contact Page</div>,
                    }
                ]
            }
        ],
    },
]);