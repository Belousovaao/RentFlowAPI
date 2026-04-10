import { createBrowserRouter } from 'react-router-dom';
import { AssetsPage } from '../pages/assets/AssetPage';
import { AssetDetailPage } from '../pages/assets/AssetDetailPage';
import { BookingsPage } from '../pages/bookings/BookingsPage';
import { HomePage } from '../pages/home/HomePage';
import { Header } from '../features/ui/components/Header';
import { Footer } from '../features/ui/components/Footer';
import { Outlet } from 'react-router-dom';


export const Layout = () => {
    return (
        <>
            <Header />
            <main className="min-h-screen">
                <Outlet />
            </main>
            <Footer />
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
                path: 'bookings',
                element: <BookingsPage />,
            },
            {
                path: 'how-it-works',
                element: <div>How It Works Page</div>, // Временная заглушка
            },
            {
                path: 'contact',
                element: <div>Contact Page</div>, // Временная заглушка
            },
        ],
    },
]);