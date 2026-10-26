import { useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import { ArrowRight, ChevronDown, HelpCircle, FileText, Search, ShieldCheck,
} from 'lucide-react';

type FAQItem = {
    question: string;
    answer: string;
};

type FAQCategory = {
    id: string;
    title: string;
    description: string;
    items: FAQItem[];
};

const faqCategories: FAQCategory[] = [
    {
        id: 'booking',
        title: 'Бронирование',
        description: 'Оформление, изменение и отмена брони.',
        items: [
            {
                question: 'Как забронировать автомобиль?',
                answer:
                    'Выберите автомобиль, укажите даты аренды и ознакомьтесь с расчётом стоимости. Затем заполните необходимые данные и отправьте заявку. Если для оформления бронирования требуются дополнительные сведения или документы, мы сообщим об этом. Или позвоните нам по номеру, указанному в разделе Контакты, и мы все сделаем сами.',
            },
            {
                question: 'Когда бронирование считается подтверждённым?',
                answer:
                    'Бронирование считается подтвержденным после внесения предоплаты. Отправка заявки сама по себе не означает, что автомобиль уже забронирован. Дождитесь подтверждения бронирования. Статус заявки отображается в личном кабинете, ориентируйтесь на него и полученные уведомления.',
            },
            {
                question: 'Можно ли изменить даты аренды?',
                answer:
                    'Да, вы можете обратиться с просьбой изменить даты. Возможность продления или переноса зависит от доступности автомобиля и условий бронирования. До получения подтверждения не считайте изменение согласованным.',
            },
            {
                question: 'Как отменить бронирование?',
                answer:
                    'Обратитесь к нам или воспользуйтесь доступной функцией отмены в личном кабинете. При отмене бронирования за 24 часа и более до начала аренды, предоплата возвращается в полном объеме. При отмене позднее будет удержана предоплата в размере стоимости одних суток аренды.',
            },
        ],
    },
    {
        id: 'requirements',
        title: 'Документы и требования',
        description: 'Требования к водителю и документы для оформления аренды.',
        items: [
            {
                question: 'Какие требования предъявляются к водителю?',
                answer:
                    'Возраст водителя должен быть не менее 25 лет, а водительский стаж — не менее 5 лет. Необходимо действующее водительское удостоверение соответствующей категории, действительное на территории РФ. К управлению допускаются только водители, согласованные с арендодателем и указанные в договоре или заявке. Передавать управление другим лицам без согласования нельзя.',
            },
            {
                question: 'Какие документы нужны физическому лицу?',
                answer:
                    'Для оформления аренды на физическое лицо необходимы паспорт и водительское удостоверение, действительное на территории РФ.',
            },
            {
                question: 'Какие документы нужны индивидуальному предпринимателю (ИП)?',
                answer:
                    'Для оформления аренды на ИП необходимы паспорт, водительское удостоверение каждого водителя, допущенного к управлению, и карточка с реквизитами ИП.',
            },
            {
                question: 'Какие документы нужны юридическому лицу (ООО)?',
                answer:
                    'Для оформления аренды на ООО необходимы карточка организации с реквизитами, паспорт водителя и его водительское удостоверение, действительное на территории РФ. Если договор подписывает представитель, требуется доверенность на подписание договора аренды. Также необходима доверенность на водителя, подтверждающая право управления автомобилем.',
            },
            {
                question: 'Может ли автомобилем управлять другой человек?',
                answer:
                    'К управлению допускаются только водители, согласованные с арендодателем и указанные в договоре или заявке. Передавать управление другим лицам без согласования нельзя.',
            },
            {
                question: 'Можно ли арендовать автомобиль с водительским стажем менее 5 лет?',
                answer:
                    'Нет. Минимальный водительский стаж для аренды автомобиля составляет 5 лет. Возраст водителя должен быть не менее 25 лет.',
            },
        ],
    },
    {
        id: 'payment',
        title: 'Стоимость, оплата и залог',
        description: 'Из чего складывается цена аренды.',
        items: [
            {
                question: 'Из чего складывается стоимость аренды?',
                answer:
                    'Итоговая стоимость зависит от выбранного автомобиля, продолжительности аренды и применимых дополнительных услуг и условий. В отдельных случаях могут учитываться маршрутные ограничения и другие заранее оговорённые платежи. Перед подтверждением бронирования проверьте итоговый расчёт.',
            },
            {
                question: 'Нужно ли вносить залог?',
                answer:
                    'Необходимость залога и его размер определяются условиями аренды конкретного автомобиля. До оформления бронирования уточните сумму залога, порядок его внесения и условия возврата.',
            },
            {
                question: 'Кто оплачивает штрафы ГИБДД?',
                answer:
                    'Штрафы ГИБДД, полученные в период аренды в связи с использованием автомобиля арендатором, оплачивает арендатор.',
            },
            {
                question: 'Как оплачиваются штрафы ГИБДД, пришедшие после возврата автомобиля?',
                answer:
                    'Если штраф относится к нарушению, допущенному в период аренды, обязанность по его оплате сохраняется за арендатором, даже если постановление поступило после возврата автомобиля.',
            },
        ],
    },
    {
        id: 'rental',
        title: 'Получение, использование и возврат',
        description: 'Что важно знать во время аренды.',
        items: [
            {
                question: 'Как происходит получение автомобиля?',
                answer:
                    'Место, время и порядок передачи автомобиля согласовываются при бронировании. При получении необходимо выполнить предусмотренные условия оформления, проверить автомобиль и зафиксировать обнаруженные повреждения до начала поездки.',
            },
            {
                question: 'Что проверить перед началом поездки?',
                answer:
                    'Осмотрите кузов, стёкла, колёса и салон. Зафиксируйте на фото или видео все заметные повреждения и сообщите о них арендодателю до выезда. Также проверьте комплектность автомобиля, уровень топлива и наличие необходимых документов и принадлежностей.',
            },
            {
                question: 'Какой ежедневный пробег включён в стоимость аренды?',
                answer:
                    'В стоимость аренды включено 250 км пробега в сутки.',
            },
            {
                question: 'Куда можно поехать на арендованном автомобиле?',
                answer:
                    'Вы можете передвигаться по маршруту, выбранному при бронировании. Поездки за пределы согласованного маршрута не допускаются.',
            },
            {
                question: 'Можно ли изменить маршрут во время аренды?',
                answer:
                    'Изменение выбранного при бронировании маршрута без предварительного согласования с арендодателем запрещено.',
            },
            {
                question: 'Что делать при поломке или ДТП?',
                answer:
                    'При ДТП сначала обеспечьте безопасность людей и действуйте в соответствии с требованиями законодательства. При необходимости вызовите экстренные службы. Незамедлительно свяжитесь с арендодателем и следуйте согласованному порядку действий. Не организуйте ремонт и не принимайте решения, которые могут повлиять на урегулирование происшествия, без согласования, если ситуация не требует немедленных мер безопасности.',
            },
            {
                question: 'Как происходит возврат автомобиля?',
                answer:
                    'Верните автомобиль в согласованные дату, время и место. Перед передачей проверьте, что личные вещи извлечены из салона, а автомобиль находится в состоянии, предусмотренном условиями аренды, а именно: чистый, заправленный топливом до уровня не ниже, чем при выдаче. Совместно с арендодателем зафиксируйте состояние машины и завершение аренды. В случае, если автомобиль передается в грязном состоянии, арендатор оставляет за собой право отложить возврат депозита до мойки автомобиля и проверки его состояния.',
            },
        ],
    },
];

const allQuestions = faqCategories.reduce(
    (total, category) => total + category.items.length,
    0,
);

export const FAQ = () => {
    const [searchQuery, setSearchQuery] = useState('');
    const [activeCategory, setActiveCategory] = useState('all');

    const normalizedQuery = searchQuery.trim().toLocaleLowerCase('ru-RU');

    const filteredCategories = useMemo(() => {
        return faqCategories
            .filter((category) => activeCategory === 'all' || category.id === activeCategory,)
            .map((category) => ({
                ...category,
                items: category.items.filter((item) => {
                    if (!normalizedQuery) return true;

                    return `${item.question} ${item.answer}`
                        .toLocaleLowerCase('ru-RU')
                        .includes(normalizedQuery);
                }),
            }))
            .filter((category) => category.items.length > 0);
    }, [activeCategory, normalizedQuery]);

    const visibleCount = filteredCategories.reduce(
        (total, category) => total + category.items.length,
        0,
    );

    const resetFilters = () => {
        setSearchQuery('');
        setActiveCategory('all');
    };

    return (
        <main className="min-h-screen bg-gray-50 pb-16 pt-24 text-gray-900 dark:bg-gray-950 dark:text-gray-100">
            <div className="container mx-auto max-w-6xl px-4 sm:px-6">
                <Link
                    to="/"
                    className="mb-6 inline-flex items-center gap-2 text-sm font-medium text-gray-600 transition-colors hover:text-gray-950 dark:text-gray-400 dark:hover:text-white"
                >
                    <ArrowRight
                        className="h-4 w-4 rotate-180"
                        aria-hidden="true"
                    />
                    На главную
                </Link>

                <header className="relative mb-8 overflow-hidden rounded-3xl bg-white p-6 shadow-sm ring-1 ring-gray-200 dark:bg-gray-900 dark:ring-gray-800 sm:p-10 lg:p-14">
                    <div
                        aria-hidden="true"
                        className="pointer-events-none absolute -right-20 -top-20 h-64 w-64 rounded-full bg-primary-100/70 blur-3xl dark:bg-primary-900/20"
                    />

                    <div className="relative max-w-3xl">
                        <div className="mb-5 flex h-12 w-12 items-center justify-center rounded-2xl bg-primary-50 text-primary-700 dark:bg-primary-950/50 dark:text-primary-300">
                            <HelpCircle
                                className="h-6 w-6"
                                aria-hidden="true"
                            />
                        </div>

                        <h1 className="text-3xl font-bold tracking-tight text-gray-950 dark:text-white sm:text-4xl lg:text-5xl">
                            Часто задаваемые вопросы
                        </h1>

                        <p className="mt-5 max-w-2xl text-base leading-7 text-gray-600 dark:text-gray-300 sm:text-lg">
                            Всё, что нужно знать об аренде автомобиля:
                            от выбора машины и бронирования до получения
                            и возврата.
                        </p>
                    </div>
                </header>

                <section
                    className="mb-8 rounded-2xl bg-white p-5 shadow-sm ring-1 ring-gray-200 dark:bg-gray-900 dark:ring-gray-800 sm:p-6"
                    aria-label="Поиск по вопросам"
                >
                    <label
                        htmlFor="faq-search"
                        className="mb-3 block text-sm font-semibold text-gray-900 dark:text-white"
                    >
                        Найти ответ
                    </label>

                    <div className="relative">
                        <Search
                            className="pointer-events-none absolute left-4 top-1/2 h-5 w-5 -translate-y-1/2 text-gray-400"
                            aria-hidden="true"
                        />

                        <input
                            id="faq-search"
                            type="search"
                            value={searchQuery}
                            onChange={(event) =>
                                setSearchQuery(event.target.value)
                            }
                            placeholder="Например, залог, документы или отмена"
                            className="w-full rounded-xl border border-gray-200 bg-white py-3 pl-12 pr-4 text-sm text-gray-900 outline-none transition placeholder:text-gray-400 focus:border-primary-500 focus:ring-2 focus:ring-primary-500/20 dark:border-gray-700 dark:bg-gray-950 dark:text-gray-100 dark:placeholder:text-gray-500"
                        />
                    </div>

                    <div
                        className="mt-5 flex gap-2 overflow-x-auto pb-1"
                        aria-label="Категории вопросов"
                    >
                        <button
                            type="button"
                            onClick={() => setActiveCategory('all')}
                            aria-pressed={activeCategory === 'all'}
                            className={`shrink-0 rounded-full px-4 py-2 text-sm font-medium transition-colors ${
                                activeCategory === 'all'
                                    ? 'bg-primary-600 text-white'
                                    : 'bg-gray-100 text-gray-700 hover:bg-gray-200 dark:bg-gray-800 dark:text-gray-300 dark:hover:bg-gray-700'
                            }`}
                        >
                            Все вопросы
                        </button>

                        {faqCategories.map((category) => (
                            <button
                                key={category.id}
                                type="button"
                                onClick={() => setActiveCategory(category.id)}
                                aria-pressed={activeCategory === category.id}
                                className={`shrink-0 rounded-full px-4 py-2 text-sm font-medium transition-colors ${
                                    activeCategory === category.id
                                        ? 'bg-primary-600 text-white'
                                        : 'bg-gray-100 text-gray-700 hover:bg-gray-200 dark:bg-gray-800 dark:text-gray-300 dark:hover:bg-gray-700'
                                }`}
                            >
                                {category.title}
                            </button>
                        ))}
                    </div>
                </section>

                <div className="space-y-8">
                    {filteredCategories.length > 0 ? (
                        filteredCategories.map((category) => (
                            <section
                                key={category.id}
                                id={category.id}
                                className="scroll-mt-28"
                                aria-labelledby={`faq-${category.id}`}
                            >
                                <div className="mb-4">
                                    <h2
                                        id={`faq-${category.id}`}
                                        className="text-xl font-bold tracking-tight text-gray-950 dark:text-white sm:text-2xl"
                                    >
                                        {category.title}
                                    </h2>
                                    <p className="mt-1 text-sm leading-6 text-gray-500 dark:text-gray-400">
                                        {category.description}
                                    </p>
                                </div>

                                <div className="divide-y divide-gray-100 overflow-hidden rounded-2xl bg-white shadow-sm ring-1 ring-gray-200 dark:divide-gray-800 dark:bg-gray-900 dark:ring-gray-800">
                                    {category.items.map((item, index) => (
                                        <details
                                            key={item.question}
                                            className="group"
                                        >
                                            <summary className="flex cursor-pointer list-none items-start justify-between gap-4 p-5 transition-colors hover:bg-gray-50 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-inset focus-visible:ring-primary-500 dark:hover:bg-gray-800/60 sm:p-6 [&::-webkit-details-marker]:hidden">
                                                <span className="flex items-start gap-3">
                                                    <span className="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-primary-50 text-xs font-semibold text-primary-700 dark:bg-primary-950/50 dark:text-primary-300">
                                                        {index + 1}
                                                    </span>
                                                    <span className="font-semibold leading-7 text-gray-900 dark:text-gray-100">
                                                        {item.question}
                                                    </span>
                                                </span>

                                                <ChevronDown
                                                    className="mt-1 h-5 w-5 shrink-0 text-gray-400 transition-transform duration-200 group-open:rotate-180"
                                                    aria-hidden="true"
                                                />
                                            </summary>

                                            <div className="pb-5 pl-5 pr-5 sm:pb-6 sm:pl-16 sm:pr-8">
                                                <p className="text-sm leading-7 text-gray-600 dark:text-gray-300 sm:text-base">
                                                    {item.answer}
                                                </p>
                                            </div>
                                        </details>
                                    ))}
                                </div>
                            </section>
                        ))
                    ) : (
                        <section className="rounded-2xl bg-white p-8 text-center shadow-sm ring-1 ring-gray-200 dark:bg-gray-900 dark:ring-gray-800 sm:p-12">
                            <Search
                                className="mx-auto h-8 w-8 text-gray-400"
                                aria-hidden="true"
                            />
                            <h2 className="mt-4 text-lg font-bold text-gray-900 dark:text-white">
                                Ответ не найден
                            </h2>
                            <p className="mt-2 text-sm leading-6 text-gray-600 dark:text-gray-300">
                                Попробуйте изменить запрос или выбрать другую категорию.
                            </p>
                            <button
                                type="button"
                                onClick={resetFilters}
                                className="mt-5 inline-flex items-center justify-center rounded-xl bg-primary-600 px-5 py-3 text-sm font-semibold text-white transition-colors hover:bg-primary-700 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary-500 focus-visible:ring-offset-2 dark:focus-visible:ring-offset-gray-900"
                            >
                                Показать все вопросы
                            </button>
                        </section>
                    )}
                </div>

                <section className="mt-10 rounded-3xl bg-gray-100 p-6 dark:bg-gray-900 sm:p-8">
                    <div className="flex flex-col gap-6 sm:flex-row sm:items-start">
                        <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-2xl bg-white text-primary-700 dark:bg-gray-800 dark:text-primary-300">
                            <ShieldCheck
                                className="h-6 w-6"
                                aria-hidden="true"
                            />
                        </div>

                        <div className="flex-1">
                            <h2 className="text-xl font-bold tracking-tight text-gray-950 dark:text-white sm:text-2xl">
                                Остались вопросы?
                            </h2>
                            <p className="mt-2 max-w-2xl text-sm leading-7 text-gray-600 dark:text-gray-300 sm:text-base">
                                Если вы не нашли нужную информацию, свяжитесь
                                с нами. Поможем разобраться с условиями аренды
                                и оформлением бронирования.
                            </p>

                            <div className="mt-5 flex flex-col gap-3 sm:flex-row">
                                <Link
                                    to="/contact"
                                    className="inline-flex items-center justify-center gap-2 rounded-xl bg-primary-600 px-5 py-3 text-sm font-semibold text-white transition-colors hover:bg-primary-700 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary-500 focus-visible:ring-offset-2 dark:focus-visible:ring-offset-gray-900"
                                >
                                    Связаться с нами
                                    <ArrowRight
                                        className="h-4 w-4"
                                        aria-hidden="true"
                                    />
                                </Link>

                                <Link
                                    to="/rental-terms"
                                    className="inline-flex items-center justify-center gap-2 rounded-xl border border-gray-300 bg-white px-5 py-3 text-sm font-semibold text-gray-800 transition-colors hover:bg-gray-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-100 dark:hover:bg-gray-700"
                                >
                                    <FileText
                                        className="h-4 w-4"
                                        aria-hidden="true"
                                    />
                                    Условия аренды
                                </Link>
                            </div>
                        </div>
                    </div>
                </section>
            </div>
        </main>
    );
};

export default FAQ;