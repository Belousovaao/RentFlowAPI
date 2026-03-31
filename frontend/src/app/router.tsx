import { createBrowserRouter, Outlet, Link } from "react-router-dom";
import { AssetsPage } from "../pages/assets/AssetPage";
import { BookingsPage } from "../pages/bookings/BookingsPage";

const Layout = () => {
    return (
        <div>
            <nav style={{ display: "flex", gap: "10px" }}>
                <Link to="/assets">Assets</Link>
                <Link to="/bookings">Bookings</Link>
            </nav>

            <Outlet />
        </div>
    );
};

export const router = createBrowserRouter([
    {
        path: "/",
        element: <AssetsPage />,
    },
    {
        path: "/assets",
        element: <AssetsPage />,
    },
    {
        path: "/bookings",
        element: <BookingsPage />,
    },
]);