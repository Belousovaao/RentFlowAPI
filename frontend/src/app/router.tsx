import { createBrowserRouter } from 'react-router-dom';
import { AssetsPage } from '../pages/assets/AssetPage';
import { AssetDetailPage } from '../pages/assets/AssetDetailPage';
import { CheckoutPage } from '../pages/bookings/CheckoutPage';
import { HomePage } from '../pages/home/HomePage';
import { AccountPage } from '../pages/account/AccountPage';
import { Header } from '../features/ui/components/Header';
import { Footer } from '../features/ui/components/Footer';
import { Outlet } from 'react-router-dom';
import { ProtectedRoute } from '@/features/auth/components/ProtectedRoute';
import { AdminAssetsListPage } from '@/pages/admin/AssetPage';
import { AssetFormPage } from '@/pages/admin/AssetFormPage';
import { AuthModal } from '@/features/auth/components/AuthModal';
import { MyBookingsPage } from '@/pages/account/MyBookingsPage';
import { AdminAccountPage } from '@/pages/admin/AdminAccountPage';
import { AdminBookingsPage } from '@/pages/admin/AdminBookingsPage';
import { AdminBookingDetailsPage } from '@/pages/admin/AdminBookingDetailsPage';
import { AdminCustomersPage } from '@/pages/admin/AdminCustomersPage';
import { AdminCustomerDetailsPage } from '@/pages/admin/AdminCustomerDetailsPage';
import { AccountBookingDetailsPage } from '@/pages/account/AccountBookingDetailsPage';
import RentalTermsPage from '@/pages/home/RentalTerms';
import AboutUsPage from '@/pages/home/AboutUs';
import ContactPage from '@/pages/home/Contact';
import PrivacyPolicy from '@/pages/home/PrivacyPolicy';
import FAQ from '@/pages/home/FAQ';

export const Layout = () => {
    return (
        <>
            <Header />
            <main className="min-h-screen">
                <Outlet />
            </main>
            <Footer />
            <AuthModal />
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
                path: 'checkout',
                element: <CheckoutPage />,
            },
            {
                path: 'rental-terms',
                element: <RentalTermsPage />,
            },
            {
                path: 'about-us',
                element: <AboutUsPage />,
            },
            {
                path: 'contact',
                element: <ContactPage />,
            },
            {
                path: 'privacy',
                element: <PrivacyPolicy />,
            },
            {
                path: 'faq',
                element: <FAQ />,
            },
            { //маршруты, требуюzие авторизации
                element: <ProtectedRoute />,
                children: [
                    {
                        path: 'account/bookings',
                        element: <MyBookingsPage />,
                    },
                    {
                        path: 'account/bookings/:id',
                        element: <AccountBookingDetailsPage />,
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
                        index: true,
                        element: <AdminAccountPage />,
                    },
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
                    },
                    {
                        path: 'bookings',
                        element: <AdminBookingsPage />,
                    },
                    {
                        path: 'bookings/:id',
                        element: <AdminBookingDetailsPage />,
                    },
                    {
                        path: 'users',
                        element: <AdminCustomersPage/>,
                    },
                    {
                        path: 'customers/:id',
                        element: <AdminCustomerDetailsPage/>
                    }
                ]
            }
        ],
    },
]);