import { Link } from 'react-router-dom';
import { useAuthStore } from '@/features/auth/stores/authStore';
import { useI18n } from '@/app/providers/I18nProvider';
import { User, Mail, Phone, Car, Calendar, Users, BarChart3, TrendingUp, Image} from 'lucide-react';
import { useEffect, useState } from 'react';
import { Asset } from '@/entities/asset/asset.types';
import { adminAssetsApi } from '@/shared/api/adminAssetApi';
import api from '@/shared/api/axios';

export const AccountPage = () => {
    const { user, isAuthenticated } = useAuthStore();
    const { t } = useI18n();

    if (!isAuthenticated || !user) {
        return (
            <div className="container mx-auto px-4 pt-28 pb-16">
                <div className="mx-auto max-w-lg rounded-2xl border border-gray-200 bg-white p-8 text-center dark:border-gray-700 dark:bg-gray-800">
                    <p className="text-gray-600 dark:text-gray-300">{t('auth.notSignedIn')}</p>
                    <p className="mt-2 text-sm text-gray-500">{t('auth.useSignIn')}</p>
                </div>
            </div>
        );
    }
    const isAdmin = user.role === 'admin';
    if (isAdmin) {
        return <AdminDashboard user={user} />;
    }
    return <UserDashboard user={user} />;
};

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

/* ЛК Клиента */
const UserDashboard = ({ user }: { user: any }) => {
    const { t } = useI18n();

    return (
        <div className="min-h-screen bg-gray-50 dark:bg-gray-900 pt-24 pb-16">
            <div className="container mx-auto px-4">
                <h1 className="mb-8 text-3xl font-bold text-gray-900 dark:text-white">
                    {t('account.title')}
                </h1>

                <div className="grid gap-6 lg:grid-cols-3">
                    {/* Профиль */}
                    <div className="rounded-2xl border border-gray-200 bg-white p-6 shadow-sm dark:border-gray-700 dark:bg-gray-800 lg:col-span-2">
                        <h2 className="mb-4 flex items-center gap-2 text-lg font-semibold text-gray-900 dark:text-white">
                            <User className="h-5 w-5 text-primary-600" />
                            {t('account.profile')}
                        </h2>
                        <dl className="space-y-3 text-sm">
                            <div className="flex justify-between gap-4 border-b border-gray-100 pb-3 dark:border-gray-700">
                                <dt className="text-gray-500">{t('auth.firstName')}</dt>
                                <dd className="font-medium text-gray-900 dark:text-white">{user.firstName}</dd>
                            </div>
                            <div className="flex justify-between gap-4 border-b border-gray-100 pb-3 dark:border-gray-700">
                                <dt className="text-gray-500">{t('auth.lastName')}</dt>
                                <dd className="font-medium text-gray-900 dark:text-white">{user.lastName}</dd>
                            </div>
                            {user.email && (
                                <div className="flex items-start justify-between gap-4 border-b border-gray-100 pb-3 dark:border-gray-700">
                                    <dt className="flex items-center gap-1 text-gray-500">
                                        <Mail className="h-4 w-4" /> Email
                                    </dt>
                                    <dd className="break-all text-right font-medium text-gray-900 dark:text-white">
                                        {user.email}
                                    </dd>
                                </div>
                            )}
                            {user.phone && (
                                <div className="flex items-start justify-between gap-4 pb-1">
                                    <dt className="flex items-center gap-1 text-gray-500">
                                        <Phone className="h-4 w-4" /> {t('auth.phone')}
                                    </dt>
                                    <dd className="font-medium text-gray-900 dark:text-white">{user.phone}</dd>
                                </div>
                            )}
                        </dl>
                    </div>

                    {/* Боковая колонка: быстрые ссылки */}
                    <div className="space-y-4">
                        <div className="rounded-2xl border border-gray-200 bg-white p-6 shadow-sm dark:border-gray-700 dark:bg-gray-800">
                            <h3 className="mb-3 text-sm font-semibold text-gray-900 dark:text-white">
                                {t('account.quickLinks')}
                            </h3>
                            <ul className="space-y-2">
                                <li>
                                    <Link
                                        to="/bookings"
                                        className="block rounded-lg px-3 py-2 text-sm text-primary-600 hover:bg-primary-50 dark:hover:bg-primary-900/20"
                                    >
                                        {t('account.myBookings')}
                                    </Link>
                                </li>
                                <li>
                                    <Link
                                        to="/assets"
                                        className="block rounded-lg px-3 py-2 text-sm text-primary-600 hover:bg-primary-50 dark:hover:bg-primary-900/20"
                                    >
                                        {t('nav.exploreCars')}
                                    </Link>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

/* ЛК Админа */
const AdminDashboard = ({ user }: { user: any }) => {

    const adminModules = [
        {
            title: 'Автомобили',
            icon: Car,
            description: 'Управление каталогом автомобилей: добавление, редактирование, удаление, загрузка фото',
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

    const [stats, setStats] = useState({
        availableAssets: 0,
    });
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        fetchStats();
    }, []);

    const fetchStats = async () => {
        setLoading(true);
        try {
            // Получаем данные для статистики
            const [assetsRes,] = await Promise.all([
                adminAssetsApi.getList({ page: 1, pageSize: 100, showDisabled: true }),
            ]);

            const allAssets = assetsRes.data.data || [];
            console.log('All assets count:', allAssets.length);
            console.log('First asset status:', allAssets[0]?.status);
            //const activeBookings = bookingsRes.data?.filter((b: any) => b.status === 'Active').length || 0;
            const availableAssets = allAssets.filter((a: Asset) => a.status === "Available").length;
            console.log('Available assets:', availableAssets);
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
            .reduce((sum, b) => sum + (b.totalPrice || 0), 0);
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
                                className="group rounded-2xl border border-gray-200 bg-white p-6 shadow-sm transition-all hover:shadow-md dark:border-gray-700 dark:bg-gray-800"
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

