import { Asset } from "@/entities/asset/asset.types";
import { bookingApi } from "@/entities/booking/booking.api";
import { adminAssetsApi } from "@/features/admin/api/adminAsset.api";
import { BarChart3, Calendar, Car, TrendingUp, Users } from "lucide-react";
import { Link } from "react-router-dom";
import { useEffect, useState } from "react";

/* ЛК Админа */
export const AdminAccountPage = () => {
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
            title: 'Автомобили',
            icon: Car,
            description: 'Добавление, редактирование, удаление, загрузка фото',
            link: '/admin/assets',
            color: 'bg-blue-500',
        },
        {
            title: 'Бронирования',
            icon: Calendar,
            description: 'Просмотр всех бронирований, изменение статусов, отмена',
            link: '/admin/bookings',
            color: 'bg-green-500',
        },
        {
            title: 'Пользователи',
            icon: Users,
            description: 'Управление пользователями, роли, блокировки',
            link: '/admin/users',
            color: 'bg-purple-500',
        },
        {
            title: 'Статистика',
            icon: BarChart3,
            description: 'Аналитика: загрузка автомобилей, выручка, популярные модели',
            link: '/admin/statistics',
            color: 'bg-orange-500',
        },
        {
            title: 'Календарь занятости',
            icon: TrendingUp,
            description: 'Просмотр свободных и занятых автомобилей на даты',
            link: '/admin/availability',
            color: 'bg-teal-500',
        },
    ];

    const [stats, setStats] = useState({availableAssets: 0,});
    const [loading, setLoading] = useState(true);
    const [bookingCount, setBookingCount] = useState(0);
    const [pendingBookings, setPendingBookings] = useState(0);

    useEffect(() => {
        fetchStats();
    }, []);

    const fetchStats = async () => {
        setLoading(true);
        try {
            // Получаем данные для статистики
            const [assetsRes, bookingsRes] = await Promise.all([
                adminAssetsApi.getList({ page: 1, pageSize: 100, showDisabled: true }),
                bookingApi.getAdminBookings(),
            ]);

            const allAssets = assetsRes.data.data || [];
            const pendingBookings = bookingsRes.filter(booking => booking.status === 'Pending').length;
            const activeBookings = bookingsRes.filter((b: any) => b.status === 'Active').length || 0;
            const availableAssets = allAssets.filter((a: Asset) => a.status === "Available").length;
            setPendingBookings(pendingBookings)
            //const totalCustomers = customersRes.data?.length || 0;
            //const uniqCustomers = customersRes.data.Distinct().length;
            
            // Рассчитываем выручку за месяц (пример)
            // const monthlyRevenue = calculateMonthlyRevenue(bookingsRes.data);
            setStats({
                 availableAssets,
             });
        } 
        catch (err) {
            console.error('Error fetching stats:', err);
        } 
        finally {
            setLoading(false);
        }
    };

    const calculateMonthlyRevenue = (bookings: any[]) => {
        if (!bookings) return 0;
        const now = new Date();
        const currentMonth = now.getMonth();
        const currentYear = now.getFullYear();
        
        return bookings
            .filter(b => {
                const bookingDate = new Date(b.startDate);
                return bookingDate.getMonth() === currentMonth && 
                       bookingDate.getFullYear() === currentYear;
            })
            .reduce((sum, b) => sum + (b.price.totalPrice || 0), 0);
    };
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
                                {module.title === "Бронирования" && pendingBookings > 0 && (
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
                        Статистика
                    </h3>
                    {loading? (
                        <div className="flex justify-center py-8">
                            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
                        </div>
                    ) : (
                        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
                            <StatCard label="Активных бронирований" value='0' icon={Calendar} />
                            <StatCard label="Доступно автомобилей" value={stats.availableAssets.toString()} icon={Car} />
                            <StatCard label="Всего клиентов" value='0' icon={Users} />
                            <StatCard label="Количество уникальных клиентов" value='0' icon={Users} />
                            <StatCard label="Выручка за месяц" value="₽ 342 500" icon={TrendingUp} />
                            <StatCard label="Расходы на содержание и обслуживание" value="₽ 342 500" icon={TrendingUp} />
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};


