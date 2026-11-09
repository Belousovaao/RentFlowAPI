import { Link } from 'react-router-dom';
import { ArrowRight, CarFront, CheckCircle2, ClipboardList, MessageCircle, } from 'lucide-react';
import type { LucideIcon } from 'lucide-react';
import { useI18n } from '@/app/providers/I18nProvider';

type Principle = {
    titleKey: string;
    descriptionKey: string;
    icon: LucideIcon;
};

const principles: Principle[] = [
    {
        titleKey: 'about.principle.clearTitle',
        descriptionKey: 'about.principle.clearDescription',
        icon: ClipboardList,
    },
    {
        titleKey: 'about.principle.vehicleTitle',
        descriptionKey: 'about.principle.vehicleDescription',
        icon: CarFront,
    },
    {
        titleKey: 'about.principle.contactTitle',
        descriptionKey: 'about.principle.contactDescription',
        icon: MessageCircle,
    },
];

export const AboutUsPage = () => {
    const { t } = useI18n();
    return (
        <main className="min-h-screen bg-gray-50 pb-16 pt-24 text-gray-900 dark:bg-gray-900 dark:text-gray-100">
            <div className="container mx-auto max-w-6xl px-4 sm:px-6">
                <section className="relative mb-8 overflow-hidden rounded-3xl bg-white p-6 shadow-sm ring-1 ring-gray-200 dark:bg-gray-800 dark:ring-gray-800 sm:p-10 lg:p-14">
                    <div
                        aria-hidden="true"
                        className="pointer-events-none absolute -right-20 -top-20 h-64 w-64 rounded-full bg-primary-100/70 blur-3xl dark:bg-primary-900/20"
                    />
                    <div className="relative max-w-3xl">
                        <h1 className="text-3xl font-bold tracking-tight sm:text-4xl lg:text-5xl text-black dark:text-white">
                            {t("about.title")}
                        </h1>
                        <p className="mt-4 text-lg font-medium text-gray-700 dark:text-gray-200 sm:text-xl">
                            {t("about.slogan")}
                        </p>
                        <p className="mt-5 max-w-2xl text-base leading-7 text-gray-600 dark:text-gray-300 sm:text-lg">
                            {t("about.intro")}
                        </p>
                        <p className="mt-4 max-w-2xl leading-7 text-gray-600 dark:text-gray-300">
                            {t("about.secondParagraph")}
                        </p>
                        <Link
                            to="/assets"
                            className="mt-7 inline-flex items-center justify-center gap-2 rounded-xl bg-primary-600 px-5 py-3 text-sm font-semibold text-white transition-colors hover:bg-primary-700 focus:outline-none focus-visible:ring-2 focus-visible:ring-primary-500 focus-visible:ring-offset-2 dark:focus-visible:ring-offset-gray-900"
                        >
                            {t("account.bookings.chooseVehicle")}
                            <ArrowRight className="h-4 w-4" aria-hidden="true" />
                        </Link>
                    </div>
                </section>

                <section className="mb-8" aria-labelledby="principles-heading">
                    <div className="mb-5 max-w-2xl">
                        <p className="text-sm font-semibold uppercase tracking-wider text-primary-700 dark:text-primary-300">
                            {t("about.approach")}
                        </p>
                        <h2
                            id="principles-heading"
                            className="mt-2 text-2xl font-bold tracking-tight sm:text-3xl text-black dark:text-white"
                        >
                            {t("about.principlesTitle")}
                        </h2>
                    </div>

                    <div className="grid gap-4 md:grid-cols-3">
                        {principles.map((principle) => {
                            const Icon = principle.icon;

                            return (
                                <article
                                    key={t(principle.titleKey)}
                                    className="rounded-2xl bg-white p-6 shadow-sm ring-1 ring-gray-200 dark:bg-gray-800 dark:ring-gray-800"
                                >
                                    <div className="mb-4 flex h-11 w-11 items-center justify-center rounded-xl bg-primary-50 text-primary-700 dark:bg-primary-950/50 dark:text-primary-300">
                                        <Icon className="h-5 w-5" aria-hidden="true" />
                                    </div>
                                    <h3 className="text-lg font-semibold text-gray-900 dark:text-white">
                                        {t(principle.titleKey)}
                                    </h3>
                                    <p className="mt-2 leading-7 text-gray-600 dark:text-gray-300">
                                        {t(principle.descriptionKey)}
                                    </p>
                                </article>
                            );
                        })}
                    </div>
                </section>

                <section className="mb-8 rounded-3xl bg-gray-100 p-6 dark:bg-gray-800 sm:p-8">
                    <div className="flex flex-col gap-4 sm:flex-row sm:items-start">
                        <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl bg-white text-primary-700 shadow-sm dark:bg-gray-800 dark:text-primary-300">
                            <CheckCircle2 className="h-5 w-5" aria-hidden="true" />
                        </div>
                        <div>
                            <h2 className="text-xl font-bold tracking-tight text-black dark:text-white">
                                {t("about.transparencyTitle")}
                            </h2>
                            <p className="mt-2 max-w-3xl leading-7 text-gray-600 dark:text-gray-300">
                                {t("about.transparencyDescription")}
                            </p>
                            <Link
                                to="/rental-terms"
                                className="mt-4 inline-flex items-center gap-2 text-sm font-semibold text-primary-700 transition-colors hover:text-primary-800 dark:text-primary-300 dark:hover:text-primary-200"
                            >
                                {t("about.rentalTermsLink")}
                                <ArrowRight className="h-4 w-4" aria-hidden="true" />
                            </Link>
                        </div>
                    </div>
                </section>

            </div>
        </main>
    );
};

export default AboutUsPage;
