import { Asset } from "@/entities/asset/asset.types";
import { bookingApi } from "@/entities/booking/booking.api";
import { adminAssetsApi } from "@/features/admin/api/adminAsset.api";
import { adminCustomersApi } from "@/features/admin/api/adminCustomers.api";
import { BarChart3, Calendar, Car, TrendingUp, Users } from "lucide-react";
import { Link } from "react-router-dom";
import { useEffect, useState } from "react";
import { useI18n } from "@/app/providers/I18nProvider";

/* ЛК Админа */
export const AdminAccountPage = () => {
    const { t, language } = useI18n();
    const StatCard = ({ label, value, icon: Icon }: { label: string; value: string; icon: any }) => (
        <div className="flex items-center gap-3 rounded-xl border border-gray-100 bg-gray-50 p-4 dark:border-gray-700 dark:bg-gray-800/50">
            <div className="rounded-lg bg-primary-100 p-2 text-primary-600 dark:bg-primary-900/30">
                <Icon className="h-5 w-5" />
            </div>
            <div>
                <p className="text-2xl font-bold text-gray-900 dark:text-white">{value}</p>
                <p className="text-xs text-gray-600 dark:text-gray-400">{label}</p>
            </div>
        </div>
    );

    const adminModules = [
        {
            title: t('admin.module.assets'),
            icon: Car,
            description: t('admin.module.assetsDescription'),
            link: '/admin/assets',
            color: 'bg-blue-500',
        },
        {
            title: t('admin.module.bookings'),
            icon: Calendar,
            description: t('admin.module.bookingsDescription'),
            link: '/admin/bookings',
            color: 'bg-green-500',
        },
        {
            title: t('admin.module.users'),
            icon: Users,
            description: t('admin.module.usersDescription'),
            link: '/admin/users',
            color: 'bg-purple-500',
        },
        {
            title: t('admin.module.statistics'),
            icon: BarChart3,
            description: t('admin.module.statisticsDescription'),
            link: '/admin/statistics',
            color: 'bg-orange-500',
        },
        {
            title: t('admin.module.calendar'),
            icon: TrendingUp,
            description: t('admin.module.calendarDescription'),
            link: '/admin/availability',
            color: 'bg-teal-500',
        },
    ];

    const [stats, setStats] = useState({
        availableAssets: 0,
        activeBookings: 0,
        totalCustomers: 0,
        uniqueCustomers: 0,
        monthlyRevenue: 0,
    });
    const [loading, setLoading] = useState(true);
    const [pendingBookings, setPendingBookings] = useState(0);

    useEffect(() => {
        fetchStats();
    }, []);

    const fetchStats = async () => {
        setLoading(true);
        try {
            const [assetsRes, bookings, customersRes] = await Promise.all([
                adminAssetsApi.getList({ page: 1, pageSize: 1000, showDisabled: true }),
                bookingApi.getAdminBookings(),
                adminCustomersApi.getList({ page: 1, pageSize: 1, showRejected: true }),
            ]);

            const allAssets = assetsRes.data.data || [];
            const activeBookings = bookings.filter(booking => booking.status === 'Active').length;
            const pendingCount = bookings.filter(booking => booking.status === 'Pending').length;
            const availableAssets = allAssets.filter((asset: Asset) => asset.status === "Available").length;
            const uniqueCustomers = new Set(bookings.map(booking => booking.customer.id)).size;

            const now = new Date();
            const monthlyRevenue = bookings
                .filter(booking => {
                    const startDate = new Date(booking.rentalPeriod.startDate);
                    return booking.status !== 'Cancelled' &&
                        startDate.getMonth() === now.getMonth() &&
                        startDate.getFullYear() === now.getFullYear();
                })
                .reduce((sum, booking) => sum + (booking.price?.total ?? 0), 0);

            setPendingBookings(pendingCount);
            setStats({
                availableAssets,
                activeBookings,
                totalCustomers: customersRes.data?.total ?? customersRes.data?.data?.length ?? 0,
                uniqueCustomers,
                monthlyRevenue,
            });
        } catch (err) {
            console.error('Error fetching stats:', err);
        } finally {
            setLoading(false);
        }
    };

    const formatCurrency = (amount: number) =>
        new Intl.NumberFormat(language === 'zh' ? 'zh-CN' : language === 'en' ? 'en-GB' : 'ru-RU', {
            style: 'currency',
            currency: 'RUB',
            maximumFractionDigits: 0,
        }).format(amount);

    return (
        <div className="min-h-screen bg-gray-50 dark:bg-gray-900 pt-24 pb-16">
            <div className="container mx-auto px-4">
                {/* Сетка модулей */}
                <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
                    {adminModules.map((module) => {
                        const Icon = module.icon;
                        return (
                            <Link
                                key={module.title}
                                to={module.link}
                                className="group relative rounded-2xl border border-gray-200 bg-white p-6 shadow-sm transition-all hover:shadow-md dark:border-gray-700 dark:bg-gray-800"
                            >
                                <div className="flex items-start gap-4">
                                    <div className={`rounded-xl ${module.color} p-3 text-white`}>
                                        <Icon className="h-6 w-6" />
                                    </div>
                                    <div className="flex-1">
                                        <h3 className="text-lg font-semibold text-gray-900 dark:text-white">
                                            {module.title}
                                        </h3>
                                        <p className="mt-1 text-sm text-gray-600 dark:text-gray-400">
                                            {module.description}
                                        </p>
                                    </div>
                                </div>
                                {module.title === t('admin.module.bookings') && pendingBookings > 0 && (
                                    <span className="absolute right-4 top-4 flex h-6 min-w-6 items-center justify-center rounded-full bg-red-500 px-1.5 text-xs font-bold text-white shadow-sm">
                                        {pendingBookings}
                                    </span>
                                )}
                            </Link>
                        );
                    })}
                </div>

                {/* Быстрая статистика */}
                <div className="mt-8 rounded-2xl border border-gray-200 bg-white p-6 dark:border-gray-700 dark:bg-gray-800">
                    <h3 className="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
                        {t("admin.stats.title")}
                    </h3>
                    {loading ? (
                        <div className="flex justify-center py-8">
                            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
                        </div>
                    ) : (
                        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
                            <StatCard label={t("admin.stats.activeBookings")} value={(stats.activeBookings ?? 0).toString()} icon={Calendar} />
                            <StatCard label={t("admin.stats.availableAssets")} value={(stats.availableAssets ?? 0).toString()} icon={Car} />
                            <StatCard label={t("admin.stats.totalCustomers")} value={(stats.totalCustomers ?? 0).toString()} icon={Users} />
                            <StatCard label={t("admin.stats.uniqueCustomers")} value={(stats.uniqueCustomers ?? 0).toString()} icon={Users} />
                            <StatCard label={t("admin.stats.monthlyBookingAmount")} value={formatCurrency(stats.monthlyRevenue ?? 0)} icon={TrendingUp} />
                            <StatCard label={t("admin.stats.maintenanceExpenses")} value="—" icon={TrendingUp} />
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};
