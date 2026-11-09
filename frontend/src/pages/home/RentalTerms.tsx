import { useState } from 'react';
import { useI18n } from '@/app/providers/I18nProvider';
import { Link } from 'react-router-dom';
import { BadgeCheck, CalendarClock, CarFront, ChevronDown, ClipboardCheck, CreditCard, FileText, Gauge, ShieldCheck, Wallet, } from 'lucide-react';

type TermsSection = {
    id: string;
    title: string;
    icon: typeof FileText;
    paragraphs?: string[];
    bullets?: string[];
    subSections?: { title: string; bullets: string[] }[];
};

const sections: TermsSection[] = [
    {
        id: 'booking',
        title: 'Как работает аренда',
        icon: CalendarClock,
        paragraphs: [
            'Выберите автомобиль, даты поездки и дополнительные услуги. На странице бронирования вы увидите стоимость аренды, размер залога и условия для выбранной машины.',
            'Бронирование считается подтвержденным после внесения предоплаты в размере стоимости 1 суток аренды. Остальная сумма оплачивается после подписания договора аренды.',
        ],
    },
    {
        id: 'driver',
        title: 'Требования к водителю',
        icon: BadgeCheck,
        paragraphs: ['Для аренды автомобиля водитель должен соответствовать следующим требованиям:'],
        bullets: [
            'Возраст — не менее 25 лет.',
            'Водительский стаж — не менее 5 лет.',
            'Водительское удостоверение соответствующей категории, действительное на территории РФ.',
            'Предоставление документов, необходимых для оформления аренды.',
            'К управлению допускаются только водители, согласованные с арендодателем и указанные в договоре или заявке. Передавать управление другим лицам без согласования нельзя.',
        ],
    },
    {
        id: 'documents',
        title: 'Какие документы нужны',
        icon: FileText,
        subSections: [
            {
                title: 'Физическому лицу',
                bullets: [
                    'Паспорт.',
                    'Водительское удостоверение, действительное на территории РФ.',
                ],
            },
            {
                title: 'Индивидуальному предпринимателю (ИП)',
                bullets: [
                    'Паспорт.',
                    'Водительское удостоверение каждого водителя, допущенного к управлению.',
                    'Карточка с реквизитами ИП.',
                ],
            },
            {
                title: 'Юридическому лицу (ООО)',
                bullets: [
                    'Карточка организации с реквизитами.',
                    'Доверенность на подписание договора аренды, если договор подписывает представитель.',
                    'Доверенность на водителя, подтверждающая право управления автомобилем.',
                    'Паспорт водителя.',
                    'Водительское удостоверение, действительное на территории РФ.',
                ],
            },
        ],
        paragraphs: [
            'В отдельных случаях могут потребоваться дополнительные документы для подтверждения полномочий представителя и оформления договора.',
        ],
    },
    {
        id: 'deposit',
        title: 'Залог и его возврат',
        icon: Wallet,
        paragraphs: [
            'Размер залога зависит от автомобиля. Точная сумма указывается в карточке автомобиля или при оформлении бронирования.',
            'Залог возвращается после возврата автомобиля и проверки его состояния, а также после расчёта по обязательствам, предусмотренным договором.',
            'Автомобиль необходимо вернуть чистым, без новых повреждений (с учётом обычного износа и состояния, зафиксированного при выдаче) и с уровнем топлива не ниже уровня при получении.',
            'Если автомобиль возвращается в тёмное время суток или в загрязнённом состоянии и это не позволяет полноценно проверить его целостность, арендодатель вправе отложить окончательный расчёт и возврат залога до мойки и осмотра автомобиля.',
            'При наличии предусмотренных договором оснований из залога могут быть удержаны расходы и задолженность, связанные с арендой. Основания и размер удержаний сообщаются арендатору.',
        ],
    },
    {
        id: 'insurance',
        title: 'Страхование',
        icon: ShieldCheck,
        paragraphs: [
            'Для автомобилей предусмотрены ОСАГО и КАСКО. Условия и объём покрытия зависят от страховых договоров конкретного автомобиля.',
            'До начала аренды ознакомьтесь с применимыми условиями страхования: страховыми рисками и исключениями, франшизой (если она предусмотрена), ограничениями использования автомобиля и порядком действий при ДТП или другом страховом событии.',
            'Наличие ОСАГО и КАСКО не означает, что любой ущерб автоматически покрывается страховщиком. Ответственность определяется обстоятельствами происшествия, условиями страхования, договором аренды и применимым законодательством.',
        ],
    },
    {
        id: 'mileage',
        title: 'Лимит пробега',
        icon: Gauge,
        paragraphs: [
            'В стоимость включено 250 км пробега за каждые сутки аренды. Лимит суммируется за весь срок бронирования: неиспользованные километры за один день можно использовать в другой день аренды.',
            'При превышении общего лимита доплата составляет 15 ₽ за каждый дополнительный километр.',
            'Например, при аренде на 4 суток общий лимит составляет 1 000 км. Если за весь срок автомобиль проехал 1 150 км, превышение составит 150 км, а доплата — 2 250 ₽.',
            'Пробег определяется по разнице показаний одометра при передаче и возврате автомобиля. Показания рекомендуется фиксировать в акте и подтверждать фотографиями.',
        ],
    },
    {
        id: 'payment',
        title: 'Оплата и дополнительные расходы',
        icon: CreditCard,
        paragraphs: [
            'Стоимость аренды зависит от выбранного автомобиля, периода и дополнительных услуг. Условия и итоговая сумма сообщаются до подтверждения бронирования.',
            'В зависимости от ситуации дополнительно могут оплачиваться превышение лимита пробега, мойка и/или дозаправка, штрафы за нарушения в период аренды, а также расходы на устранение повреждений или утрату переданного имущества — если это предусмотрено договором.',
            'Применимые дополнительные платежи и основания их начисления определяются условиями бронирования и договором аренды.',
        ],
    },
    {
        id: 'handover',
        title: 'Получение и возврат автомобиля',
        icon: CarFront,
        paragraphs: [
            'Дата, время и место передачи и возврата автомобиля согласовываются при оформлении бронирования.',
            'При получении автомобиля проверьте его состояние, комплектность, уровень топлива и показания одометра. Замечания и уже имеющиеся повреждения следует зафиксировать в акте приёма-передачи и, по возможности, на фото или видео.',
            'При возврате проводится осмотр и сверка состояния автомобиля с данными, зафиксированными при выдаче. Вместе с автомобилем необходимо вернуть ключи, документы и дополнительное оборудование, если они передавались.',
            'О ДТП, повреждениях, неисправностях, угоне и других существенных происшествиях необходимо незамедлительно сообщить арендодателю и действовать по договору и требованиям законодательства.',
        ],
    },
    {
        id: 'changes',
        title: 'Изменение срока, продление и отмена',
        icon: ClipboardCheck,
        subSections: [
            {
                title: 'Изменение срока и продление',
                bullets: [
                    'Изменить первоначальный срок аренды можно по согласованию с арендодателем.',
                    'Продление нужно согласовать до окончания текущего срока. Дополнительный период оплачивается по применимому тарифу и в порядке, согласованном сторонами.',
                    'Досрочный возврат возможен. При сокращении срока уже начавшейся аренды расчёт производится с учётом фактического времени использования и оплаты дополнительных суток согласно условиям аренды.',
                ],
            },
            {
                title: 'Отмена бронирования клиентом',
                bullets: [
                    'Для подтверждения бронирования требуется предоплата в размере 1 суток аренды.',
                    'При отмене не позднее чем за 24 часа до начала бронирования предоплата возвращается полностью.',
                    'При отмене менее чем за 24 часа до начала аренды удерживается сумма, равная стоимости 1 суток аренды. Если внесённая предоплата была больше, оставшаяся часть возвращается клиенту.',
                ],
            },
            {
                title: 'Отмена со стороны арендодателя',
                bullets: [
                    'Если арендодатель отменяет бронирование или не может предоставить согласованный автомобиль, предоплата возвращается клиенту полностью.',
                ],
            },
        ],
    },
    {
        id: 'responsibility',
        title: 'Использование автомобиля и ответственность',
        icon: ShieldCheck,
        paragraphs: [
            'Во время аренды соблюдайте ПДД, бережно обращайтесь с автомобилем и используйте его по назначению.',
            'Нельзя передавать управление несогласованным водителям, управлять автомобилем в состоянии опьянения, использовать его для запрещённых договором целей или проводить ремонт и изменять конструкцию без согласования, кроме действий, необходимых в предусмотренных законом случаях.',
            'Арендатор обязан своевременно сообщать о ДТП, повреждениях, неисправностях и других существенных инцидентах. Ответственность за ущерб, штрафы, утрату ключей, документов или оборудования определяется договором аренды, обстоятельствами конкретного случая и применимым законодательством.',
        ],
    },
];


type LocalizedTermsSection = {
    title: string;
    paragraphs?: string[];
    bullets?: string[];
    subSections?: { title: string; bullets: string[] }[];
};

const localizedSections: Record<'en' | 'zh', Record<string, LocalizedTermsSection>> = {
    en: {
        booking: { title: 'How the rental works', paragraphs: [
            'Choose a vehicle, your travel dates and any additional services. The booking page shows the rental price, deposit amount and terms for the selected vehicle.',
            'A booking is confirmed after you pay a prepayment equal to one day of rental. The remaining amount is paid after the rental agreement has been signed.',
        ] },
        driver: { title: 'Driver requirements', paragraphs: ['To rent a vehicle, the driver must meet the following requirements:'], bullets: [
            'At least 25 years of age.',
            'At least 5 years of driving experience.',
            'A valid driving licence of the appropriate category, recognised in the Russian Federation.',
            'The documents required to arrange the rental must be provided.',
            'Only drivers approved by the lessor and named in the agreement or booking request may drive the vehicle. The vehicle must not be handed over to another driver without prior approval.',
        ] },
        documents: { title: 'Required documents', paragraphs: [
            'Additional documents may be requested in certain cases to verify a representative’s authority and complete the agreement.',
        ], subSections: [
            { title: 'Private individual', bullets: ['Passport.', 'A driving licence valid in the Russian Federation.'] },
            { title: 'Sole proprietor', bullets: ['Passport.', 'Driving licences for every driver authorised to use the vehicle.', 'A business details card with the sole proprietor’s registration and banking details.'] },
            { title: 'Legal entity (LLC)', bullets: ['Company details card.', 'A power of attorney to sign the rental agreement if it is signed by a representative.', 'A power of attorney for the driver confirming their right to use the vehicle.', 'The driver’s passport.', 'A driving licence valid in the Russian Federation.'] },
        ] },
        deposit: { title: 'Security deposit and refund', paragraphs: [
            'The deposit amount depends on the vehicle. The exact amount is shown in the vehicle listing or during booking.',
            'The deposit is refunded after the vehicle has been returned and inspected, and after any obligations under the rental agreement have been settled.',
            'The vehicle must be returned clean, without new damage (allowing for normal wear and taking into account its condition at handover), and with at least the same fuel level as at pickup.',
            'If the vehicle is returned after dark or in a condition that prevents a proper inspection, the lessor may postpone the final settlement and deposit refund until the vehicle has been washed and inspected.',
            'Where permitted by the agreement, rental-related costs and outstanding amounts may be deducted from the deposit. The renter will be informed of the grounds and amount of any deduction.',
        ] },
        insurance: { title: 'Insurance', paragraphs: [
            'Vehicles are covered by Russian compulsory third-party motor insurance (OSAGO) and comprehensive motor insurance (CASCO). The terms and scope of cover depend on the insurance policies for the specific vehicle.',
            'Before the rental begins, review the applicable insurance terms, including insured risks and exclusions, any deductible, vehicle-use restrictions and the steps to take in the event of an accident or another insured incident.',
            'The existence of OSAGO and CASCO coverage does not mean that every type of damage is automatically covered by the insurer. Liability depends on the circumstances, insurance terms, rental agreement and applicable law.',
        ] },
        mileage: { title: 'Mileage limit', paragraphs: [
            'The rental price includes 250 km for each rental day. The allowance is cumulative for the entire booking period, so unused kilometres from one day may be used on another day.',
            'Mileage exceeding the total allowance is charged at ₽15 per additional kilometre.',
            'For example, a four-day rental includes 1,000 km. If the vehicle travels 1,150 km during the rental, the excess is 150 km and the additional charge is ₽2,250.',
            'Mileage is calculated from the difference between the odometer readings at handover and return. We recommend recording the readings in the handover report and supporting them with photographs.',
        ] },
        payment: { title: 'Payment and additional charges', paragraphs: [
            'The rental price depends on the selected vehicle, rental period and additional services. The applicable terms and total amount are provided before the booking is confirmed.',
            'Depending on the circumstances and where provided for in the agreement, additional charges may include excess mileage, washing and/or refuelling, traffic fines incurred during the rental, and the cost of repairing damage or replacing lost property.',
            'Any applicable additional payments and the grounds for charging them are determined by the booking terms and rental agreement.',
        ] },
        handover: { title: 'Vehicle pickup and return', paragraphs: [
            'The date, time and location for vehicle pickup and return are agreed when the booking is arranged.',
            'At pickup, check the vehicle’s condition, equipment, fuel level and odometer reading. Any issues or existing damage should be recorded in the handover report and, where possible, in photographs or video.',
            'At return, the vehicle is inspected and its condition is compared with the information recorded at pickup. Return the keys, documents and any additional equipment supplied with the vehicle.',
            'The lessor must be notified immediately of any accident, damage, malfunction, theft or other significant incident. Follow the rental agreement and applicable legal requirements.',
        ] },
        changes: { title: 'Changing the rental period, extensions and cancellations', subSections: [
            { title: 'Changing or extending the rental period', bullets: [
                'The original rental period may be changed by agreement with the lessor.',
                'An extension must be agreed before the current rental period ends. The additional period is charged at the applicable rate and under the terms agreed by both parties.',
                'Early return is possible. If a rental that has already started is shortened, the amount due is recalculated based on the actual usage period and any additional day charges under the rental terms.',
            ] },
            { title: 'Cancellation by the customer', bullets: [
                'A prepayment equal to one day of rental is required to confirm a booking.',
                'If the booking is cancelled at least 24 hours before the rental start time, the prepayment is refunded in full.',
                'If the rental is cancelled less than 24 hours before it starts, an amount equal to one day of rental is retained. If the prepayment was higher, the remaining amount is refunded to the customer.',
            ] },
            { title: 'Cancellation by the lessor', bullets: [
                'If the lessor cancels the booking or cannot provide the agreed vehicle, the customer receives a full refund of the prepayment.',
            ] },
        ] },
        responsibility: { title: 'Vehicle use and liability', paragraphs: [
            'During the rental, comply with traffic rules, take proper care of the vehicle and use it only for its intended purpose.',
            'Do not allow unauthorised drivers to use the vehicle, drive under the influence, use the vehicle for purposes prohibited by the agreement, carry out repairs or modify the vehicle without approval, except where action is required by law.',
            'The renter must promptly report accidents, damage, malfunctions and other significant incidents. Liability for damage, fines, or the loss of keys, documents or equipment is determined by the rental agreement, the circumstances of the case and applicable law.',
        ] },
    },
    zh: {
        booking: { title: '租车流程', paragraphs: [
            '选择车辆、出行日期和所需的附加服务。预订页面会显示所选车辆的租金、押金金额及相关租赁条件。',
            '支付相当于一天租金的预付款后，预订即得到确认。签署租赁合同后支付剩余款项。',
        ] },
        driver: { title: '驾驶员要求', paragraphs: ['租赁车辆的驾驶员须满足以下条件：'], bullets: [
            '年满25周岁。',
            '驾龄不少于5年。',
            '持有在俄罗斯联邦境内有效且类别相符的驾驶证。',
            '提供办理租赁所需的相关文件。',
            '只有经出租方批准并列入合同或预订申请的驾驶员才可以驾驶车辆。未经许可，不得将车辆交由其他人员驾驶。',
        ] },
        documents: { title: '所需文件', paragraphs: [
            '在某些情况下，可能需要提供其他文件，以核实代理人的授权并完成合同手续。',
        ], subSections: [
            { title: '个人承租人', bullets: ['护照。', '在俄罗斯联邦境内有效的驾驶证。'] },
            { title: '个体经营者', bullets: ['护照。', '所有获准驾驶该车辆人员的驾驶证。', '载有个体经营者注册及银行信息的资料卡。'] },
            { title: '法人实体（有限责任公司）', bullets: ['载有公司详细信息的资料卡。', '如由代理人签署租赁合同，须提供签署合同的授权委托书。', '证明驾驶员有权驾驶车辆的授权委托书。', '驾驶员护照。', '在俄罗斯联邦境内有效的驾驶证。'] },
        ] },
        deposit: { title: '押金及退还', paragraphs: [
            '押金金额取决于所选车辆。具体金额会显示在车辆详情页或预订过程中。',
            '车辆归还并完成车况检查，且根据租赁合同结清相关义务后，押金将予以退还。',
            '归还车辆时应保持清洁，不得有新增损坏（正常磨损及交车时记录的车况除外），燃油量不得低于取车时的水平。',
            '如果车辆在夜间或因污损而无法进行完整检查，出租方有权在车辆清洗并检查后再完成最终结算及押金退还。',
            '如合同规定了相应情形，押金可用于抵扣与租赁有关的费用及欠款。出租方会向承租人说明扣款依据及金额。',
        ] },
        insurance: { title: '保险', paragraphs: [
            '车辆投保俄罗斯机动车第三者责任强制保险（OSAGO）及综合车险（CASCO）。具体保障条件和范围以该车辆的保险合同为准。',
            '租赁开始前，请仔细了解适用的保险条款，包括承保风险及除外责任、免赔额（如有）、车辆使用限制，以及发生交通事故或其他保险事故时应采取的措施。',
            '投保OSAGO和CASCO并不意味着所有损失都会自动获得保险赔付。责任认定取决于事故情况、保险条款、租赁合同及适用法律。',
        ] },
        mileage: { title: '里程限制', paragraphs: [
            '租金包含每天250公里的行驶里程。里程额度按整个预订期间累计计算，因此某一天未使用的里程可以在租期内的其他日期使用。',
            '超过总里程额度的部分，每公里加收15卢布。',
            '例如，租赁4天的总里程额度为1,000公里。如果整个租期行驶了1,150公里，则超出150公里，需额外支付2,250卢布。',
            '实际里程根据车辆交付和归还时的里程表读数差计算。建议在交接单中记录读数，并拍照留存。',
        ] },
        payment: { title: '付款及额外费用', paragraphs: [
            '租金取决于所选车辆、租赁期限和附加服务。预订确认前会告知适用条件及最终金额。',
            '根据具体情况及合同约定，可能产生额外费用，包括超里程费、洗车费和/或补油费、租赁期间的交通罚款，以及维修损坏或赔偿遗失物品的费用。',
            '适用的额外付款项目及其收取依据，以预订条件和租赁合同为准。',
        ] },
        handover: { title: '车辆交付与归还', paragraphs: [
            '车辆交付和归还的日期、时间及地点在办理预订时协商确定。',
            '取车时请检查车辆状况、随车设备、燃油量和里程表读数。发现的问题及已有损坏应记录在车辆交接单中，并尽可能拍照或录像留存。',
            '还车时将检查车辆，并与取车时记录的状况进行核对。请同时归还车辆钥匙、文件以及随车提供的附加设备。',
            '如发生交通事故、车辆损坏、故障、被盗或其他重大事件，必须立即通知出租方，并按照租赁合同及法律规定处理。',
        ] },
        changes: { title: '租期变更、延期及取消', subSections: [
            { title: '变更或延长租期', bullets: [
                '经与出租方协商一致，可以变更最初约定的租期。',
                '延期须在当前租期结束前协商确认。延长期间按照适用费率及双方约定的方式付款。',
                '可以提前还车。对于已经开始的租赁，如需缩短租期，将根据实际使用时间及租赁条件中有关额外天数的计费规则重新结算。',
            ] },
            { title: '客户取消预订', bullets: [
                '确认预订需支付相当于一天租金的预付款。',
                '如果在租赁开始前至少24小时取消预订，预付款将全额退还。',
                '如果在租赁开始前不足24小时取消，将扣留相当于一天租金的金额。如已支付的预付款高于该金额，差额将退还给客户。',
            ] },
            { title: '出租方取消预订', bullets: [
                '如果出租方取消预订或无法提供双方约定的车辆，将向客户全额退还预付款。',
            ] },
        ] },
        responsibility: { title: '车辆使用及责任', paragraphs: [
            '租赁期间请遵守交通规则，爱护车辆，并按照车辆用途使用。',
            '不得将车辆交由未经批准的驾驶员驾驶，不得酒后驾驶，不得将车辆用于合同禁止的用途，也不得未经许可维修或改装车辆；法律要求采取措施的情形除外。',
            '承租人必须及时报告交通事故、车辆损坏、故障及其他重大事件。对于损坏、罚款以及钥匙、文件或设备遗失的责任，根据租赁合同、具体情况和适用法律确定。',
        ] },
    },
};

export const RentalTermsPage = () => {
    const { language, t } = useI18n();
    const [expandedSections, setExpandedSections] = useState<string[]>(['booking', 'changes']);

    const toggleSection = (id: string) => {
        setExpandedSections((current) =>
            current.includes(id) ? current.filter((item) => item !== id) : [...current, id],
        );
    };
    return (
        <main className="min-h-screen bg-gray-50 pb-16 pt-24 text-gray-900 dark:bg-gray-900 dark:text-gray-100">
            <div className="container mx-auto max-w-6xl px-4 sm:px-6">
                <section className="mb-8 rounded-3xl bg-white p-6 shadow-sm ring-1 ring-gray-200 dark:bg-gray-800 dark:ring-gray-800 sm:p-10">
                    <div className="max-w-3xl">
                        <h1 className="text-3xl font-bold tracking-tight sm:text-4xl text-black dark:text-white">
                            {t('rentalTerms.title')}
                        </h1>
                        <p className="mt-4 text-base leading-7 text-gray-600 dark:text-gray-300 sm:text-lg">
                            {t('rentalTerms.intro')}
                        </p>
                        <div className="mt-6 flex flex-wrap gap-3">
                            <Link
                                to="/assets"
                                className="inline-flex items-center justify-center rounded-xl bg-primary-600 px-5 py-3 text-sm font-semibold text-white transition-colors hover:bg-primary-700"
                            >
                                {t('rentalTerms.chooseVehicle')}
                            </Link>
                            <a
                                href="#changes"
                                className="inline-flex items-center justify-center rounded-xl border border-gray-300 px-5 py-3 text-sm font-semibold text-gray-800 transition-colors hover:bg-gray-50 dark:border-gray-700 dark:text-gray-100 dark:hover:bg-gray-800"
                            >
                                {t('rentalTerms.cancellationRules')}
                            </a>
                        </div>
                    </div>
                </section>

                <section className="mb-8 grid gap-4 sm:grid-cols-2 lg:grid-cols-4" aria-label={t('rentalTerms.summary')}>
                    <FactCard label={t('rentalTerms.age')} value={language === 'ru' ? 'от 25 лет' : language === 'en' ? '25+' : '25岁及以上'} icon={BadgeCheck} />
                    <FactCard label={t('rentalTerms.experience')} value={language === 'ru' ? 'от 5 лет' : language === 'en' ? '5+ years' : '驾龄5年以上'} icon={CarFront} />
                    <FactCard label={t('rentalTerms.includedMileage')} value={language === 'ru' ? '250 км/сутки' : language === 'en' ? '250 km/day' : '250公里/天'} icon={Gauge} />
                    <FactCard label={t('rentalTerms.overLimit')} value={language === 'ru' ? '15 ₽/км' : language === 'en' ? '₽15/km' : '15卢布/公里'} icon={Wallet} />
                </section>

                <section className="space-y-3" aria-label={t('rentalTerms.details')}>
                    {sections.map((section) => {
                        const Icon = section.icon;
                        const isExpanded = expandedSections.includes(section.id);

                        return (
                            <article
                                key={section.id}
                                id={section.id}
                                className="scroll-mt-28 overflow-hidden rounded-2xl bg-white shadow-sm ring-1 ring-gray-200 dark:bg-gray-800 dark:ring-gray-800"
                            >
                                <button
                                    type="button"
                                    onClick={() => toggleSection(section.id)}
                                    aria-expanded={isExpanded}
                                    aria-controls={`${section.id}-content`}
                                    className="flex w-full items-center gap-4 px-5 py-5 text-left sm:px-6"
                                >
                                    <span className="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl bg-primary-50 text-primary-700 dark:bg-primary-950 dark:text-primary-300">
                                        <Icon className="h-5 w-5" aria-hidden="true" />
                                    </span>
                                    <span className="flex-1 text-base font-semibold sm:text-lg">{language === 'ru' ? section.title : localizedSections[language][section.id].title}</span>
                                    <ChevronDown
                                        className={`h-5 w-5 shrink-0 text-gray-500 transition-transform ${isExpanded ? 'rotate-180' : ''}`}
                                        aria-hidden="true"
                                    />
                                </button>

                                {isExpanded && (
                                    <div id={`${section.id}-content`} className="space-y-4 px-5 pb-6 pl-5 sm:px-6 sm:pl-[5.75rem]">
                                        {section.paragraphs?.map((paragraph, index) => (
                                            <p key={`${section.id}-p-${index}`} className="leading-7 text-gray-600 dark:text-gray-300">
                                                {language === 'ru' ? paragraph : localizedSections[language][section.id].paragraphs?.[index]}
                                            </p>
                                        ))}

                                        {section.bullets && (
                                            <ul className="list-disc space-y-2 pl-5 leading-7 text-gray-600 dark:text-gray-300">
                                                {section.bullets.map((bullet, index) => <li key={bullet}>{language === 'ru' ? bullet : localizedSections[language][section.id].bullets?.[index]}</li>)}
                                            </ul>
                                        )}

                                        {section.subSections?.map((subSection) => (
                                            <div key={subSection.title} className="space-y-2">
                                                <h3 className="font-semibold text-gray-800 dark:text-white">{language === 'ru' ? subSection.title : localizedSections[language][section.id].subSections?.find((item) => item.title === subSection.title)?.title}</h3>
                                                <ul className="list-disc space-y-2 pl-5 leading-7 text-gray-600 dark:text-gray-300">
                                                    {subSection.bullets.map((bullet, index) => <li key={bullet}>{language === 'ru' ? bullet : localizedSections[language][section.id].subSections?.find((item) => item.title === subSection.title)?.bullets[index]}</li>)}
                                                </ul>
                                            </div>
                                        ))}
                                    </div>
                                )}
                            </article>
                        );
                    })}
                </section>

                <section className="mt-8 rounded-3xl bg-gray-800 p-6 text-white dark:bg-gray-800 sm:p-8">
                    <div className="flex flex-col gap-5 sm:flex-row sm:items-center sm:justify-between">
                        <div>
                            <h2 className="text-xl font-bold sm:text-2xl text-white">{t('rentalTerms.ctaTitle')}</h2>
                            <p className="mt-2 max-w-2xl leading-6 text-gray-300">
                                {t('rentalTerms.ctaDescription')}
                            </p>
                        </div>
                        <Link
                            to="/assets"
                            className="inline-flex shrink-0 items-center justify-center rounded-xl bg-white px-5 py-3 text-sm font-semibold text-gray-900 transition-colors hover:bg-gray-100"
                        >
                            {t('rentalTerms.goToVehicles')}
                        </Link>
                    </div>
                </section>

                <p className="mt-6 text-sm leading-6 text-gray-500 dark:text-gray-400">
                    {t('rentalTerms.disclaimer')}
                </p>
            </div>
        </main>
    );
};

type FactCardProps = {
    label: string;
    value: string;
    icon: typeof FileText;
};

const FactCard = ({ label, value, icon: Icon }: FactCardProps) => (
    <div className="rounded-2xl bg-white p-5 shadow-sm ring-1 ring-gray-200 dark:bg-gray-800 dark:ring-gray-800">
        <div className="mb-4 flex h-10 w-10 items-center justify-center rounded-xl bg-primary-50 text-primary-700 dark:bg-primary-950 dark:text-primary-300">
            <Icon className="h-5 w-5" aria-hidden="true" />
        </div>
        <p className="text-sm text-gray-500 dark:text-gray-400">{label}</p>
        <p className="mt-1 text-lg font-bold">{value}</p>
    </div>
);

export default RentalTermsPage;