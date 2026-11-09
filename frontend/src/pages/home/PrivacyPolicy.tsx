import { Link } from 'react-router-dom';
import { ArrowLeft, ShieldCheck } from 'lucide-react';
import { useI18n } from '../../app/providers/I18nProvider';

const sections = [
    {
        id: 'general',
        title: '1. Общие положения',
        content: (
            <>
                <p>
                    Настоящая Политика определяет порядок обработки и защиты
                    персональных данных пользователей сайта «Волнами Драйв»,
                    предоставляющего информацию об аренде автомобилей и
                    возможности оформления бронирования.
                </p>
                <p>
                    <strong>Оператор персональных данных:</strong> Володин Михаил
                    Игоревич.
                </p>
                <p>
                    <strong>ИНН оператора:</strong> 272404167841.
                </p>
                <p>
                    Оператор обрабатывает персональные данные в соответствии с
                    Федеральным законом от 27.07.2006 № 152-ФЗ «О персональных
                    данных» и иными применимыми нормами законодательства РФ.
                </p>
                <p>
                    Политика применяется к данным, которые пользователь сообщает
                    через формы сайта, личный кабинет, при обращении к оператору
                    или оформлении аренды.
                </p>
            </>
        ),
    },
    {
        id: 'terms',
        title: '2. Основные понятия',
        content: (
            <ul>
                <li>
                    <strong>Персональные данные</strong> — информация, относящаяся
                    прямо или косвенно к определённому или определяемому физическому
                    лицу.
                </li>
                <li>
                    <strong>Обработка персональных данных</strong> — любые действия
                    с персональными данными, включая сбор, запись, хранение,
                    использование, передачу, обезличивание и удаление.
                </li>
                <li>
                    <strong>Пользователь</strong> — лицо, посещающее сайт или
                    использующее его функции.
                </li>
                <li>
                    <strong>Оператор</strong> — Володин Михаил Игоревич,
                    самостоятельно организующий обработку персональных данных
                    либо определяющий её цели и состав.
                </li>
            </ul>
        ),
    },
    {
        id: 'purposes',
        title: '3. Цели обработки персональных данных',
        content: (
            <>
                <p>Персональные данные могут обрабатываться для следующих целей:</p>
                <ul>
                    <li>Обработка обращений и обратная связь с пользователем.</li>
                    <li>Оформление и сопровождение бронирования автомобиля.</li>
                    <li>Создание и обслуживание личного кабинета, если он используется.</li>
                    <li>
                        Проверка сведений и документов, необходимых для заключения
                        и исполнения договора аренды.
                    </li>
                    <li>
                        Подготовка, заключение и исполнение договора аренды,
                        расчёт стоимости и учёт платежей.
                    </li>
                    <li>
                        Рассмотрение претензий, исполнение установленных законом
                        обязанностей и защита прав оператора.
                    </li>
                    <li>
                        Обеспечение безопасности сайта и предотвращение технических
                        сбоев и злоупотреблений.
                    </li>
                </ul>
                <p>
                    Рекламные рассылки и иные дополнительные цели, не необходимые
                    для оформления аренды, допускаются только при наличии
                    соответствующего законного основания.
                </p>
            </>
        ),
    },
    {
        id: 'data',
        title: '4. Какие данные могут обрабатываться',
        content: (
            <>
                <p>
                    В зависимости от используемых функций сайта оператор может
                    обрабатывать:
                </p>
                <ul>
                    <li>
                        <strong>Контактные данные:</strong> имя, номер телефона,
                        адрес электронной почты.
                    </li>
                    <li>
                        <strong>Данные бронирования:</strong> выбранный автомобиль,
                        даты аренды, место получения и возврата, дополнительные
                        услуги и сведения о заказе.
                    </li>
                    <li>
                        <strong>Данные документов:</strong> сведения из паспорта,
                        водительского удостоверения и копии документов, реквизиты
                        организации, в том числе банковские реквизиты.
                    </li>
                    <li>
                        <strong>Технические данные:</strong> IP-адрес, сведения
                        о браузере, устройстве, времени обращения и действиях на
                        сайте — если такие данные автоматически собираются
                        используемой инфраструктурой.
                    </li>
                </ul>
                <p>
                    Оператор не должен запрашивать данные, не требующиеся для
                    заявленных целей.
                </p>
            </>
        ),
    },
    {
        id: 'grounds',
        title: '5. Правовые основания обработки',
        content: (
            <>
                <p>
                    В зависимости от ситуации обработка осуществляется на основании:
                </p>
                <ul>
                    <li>
                        Согласия пользователя, если оно требуется для соответствующей
                        обработки.
                    </li>
                    <li>
                        Необходимости заключения или исполнения договора, стороной
                        которого является пользователь, либо совершения действий
                        по его запросу до заключения договора.
                    </li>
                    <li>
                        Обязанностей, возложенных на оператора законодательством РФ.
                    </li>
                    <li>
                        Иных оснований, прямо предусмотренных законодательством РФ.
                    </li>
                </ul>
                <p>
                    Согласие на обработку персональных данных, когда оно требуется,
                    оформляется отдельно от иных документов и подтверждений.
                    Отзыв согласия не исключает продолжения обработки, если для неё
                    имеется самостоятельное законное основание.
                </p>
            </>
        ),
    },
    {
        id: 'procedure',
        title: '6. Порядок обработки и передачи данных',
        content: (
            <>
                <p>
                    Обработка может осуществляться с использованием средств
                    автоматизации или без них. В зависимости от цели оператор
                    вправе собирать, систематизировать, хранить, уточнять,
                    использовать, передавать в предусмотренных законом случаях
                    и удалять персональные данные.
                </p>
                <p>
                    Данные могут передаваться третьим лицам только при наличии
                    законного основания и в объёме, необходимом для соответствующей
                    цели. Например, это может относиться к лицам, участвующим
                    в исполнении договора, работе сайта или выполнении требований
                    законодательства.
                </p>
                <p>
                    Если обработка поручается стороннему лицу, оператор обеспечивает
                    наличие необходимых правовых оснований и условий обработки.
                    Конкретные получатели определяются фактически используемыми
                    сервисами и процессами.
                </p>
                <p>
                    При сборе персональных данных граждан РФ через интернет
                    оператор соблюдает требования законодательства РФ к
                    использованию баз данных, расположенных на территории России.
                </p>
            </>
        ),
    },
    {
        id: 'retention',
        title: '7. Сроки хранения и удаление',
        content: (
            <>
                <p>
                    Персональные данные хранятся не дольше, чем этого требуют
                    цели обработки, если иной срок не установлен законодательством,
                    договором или другим применимым правовым основанием.
                </p>
                <p>
                    После достижения целей обработки данные удаляются или
                    обезличиваются, если их дальнейшее хранение не требуется
                    по закону или для защиты прав и законных интересов сторон.
                </p>
                <p>
                    Срок хранения конкретных документов зависит от их вида,
                    фактического использования и обязательных сроков хранения.
                </p>
            </>
        ),
    },
    {
        id: 'protection',
        title: '8. Защита персональных данных',
        content: (
            <>
                <p>
                    Оператор принимает необходимые и достаточные организационные
                    и технические меры для защиты персональных данных от
                    неправомерного доступа, изменения, раскрытия, уничтожения
                    и иных неправомерных действий.
                </p>
                <p>
                    Объём мер определяется характером обрабатываемых данных,
                    рисками обработки и требованиями законодательства РФ.
                </p>
            </>
        ),
    },
    {
        id: 'rights',
        title: '9. Права пользователя',
        content: (
            <>
                <p>Пользователь вправе в предусмотренных законом случаях:</p>
                <ul>
                    <li>Получить информацию об обработке своих персональных данных.</li>
                    <li>Потребовать уточнения, блокирования или удаления данных.</li>
                    <li>Отозвать ранее предоставленное согласие.</li>
                    <li>
                        Возражать против обработки или обратиться с жалобой
                        в уполномоченный орган.
                    </li>
                    <li>Реализовать иные права, предусмотренные законодательством РФ.</li>
                </ul>
                <p>
                    Для рассмотрения запроса оператор может попросить подтвердить
                    личность заявителя в объёме, необходимом для предотвращения
                    неправомерного раскрытия данных.
                </p>
            </>
        ),
    },
    {
        id: 'cookies',
        title: '10. Файлы cookie и технические данные',
        content: (
            <>
                <p>
                    Сайт и его техническая инфраструктура могут использовать
                    файлы cookie и аналогичные технологии, если они необходимы
                    для работы сайта, сохранения пользовательских настроек,
                    обеспечения безопасности или иных заявленных функций.
                </p>
                <p>
                    Если на сайте используются сторонние системы аналитики,
                    рекламные инструменты или иные сервисы, сведения об их
                    фактическом использовании и соответствующих целях обработки
                    должны быть раскрыты дополнительно.
                </p>
            </>
        ),
    },
    {
        id: 'contact',
        title: '11. Контакты оператора',
        content: (
            <>
                <p>
                    По вопросам обработки персональных данных, реализации прав
                    и отзыва согласия можно обратиться к оператору:
                </p>
                <p>
                    <strong>Оператор:</strong> Володин Михаил Игоревич
                </p>
                <p>
                    <strong>ИНН:</strong> 272404167841
                </p>
                <p>
                    <strong>Электронная почта:</strong>{' '}
                    <a
                        className="font-medium text-primary-700 underline underline-offset-4 transition-colors hover:text-primary-800 dark:text-primary-300 dark:hover:text-primary-200"
                        href="mailto:volodin_rent@mail.ru"
                    >
                        volodin_rent@mail.ru
                    </a>
                </p>
                <p>
                    <strong>Адрес для обращений:</strong> г. Красноярск,
                    ул. Краснодарская, 40/6.
                </p>
                <p>
                    В обращении рекомендуется указать, какие именно персональные
                    данные или действия по их обработке затрагивает запрос.
                </p>
            </>
        ),
    },
    {
        id: 'final',
        title: '12. Заключительные положения',
        content: (
            <>
                <p>
                    Оператор вправе обновлять настоящую Политику при изменении
                    законодательства, функций сайта или порядка обработки данных.
                </p>
                <p>
                    Актуальная редакция публикуется на этой странице. Новая
                    редакция применяется с момента публикации, если в ней
                    не указано иное.
                </p>
            </>
        ),
    },
];


const localizedSections = {
    en: [
        { id: 'general', title: '1. General provisions', content: <><p>This Privacy Policy describes how personal data of users of the Volnami Drive website is processed and protected. The website provides information about vehicle rentals and allows users to submit booking requests.</p><p><strong>Personal data operator:</strong> Mikhail Igorevich Volodin.</p><p><strong>Operator's INN (taxpayer identification number):</strong> 272404167841.</p><p>The operator processes personal data in accordance with Federal Law No. 152-FZ of 27 July 2006 “On Personal Data” and other applicable laws of the Russian Federation.</p><p>This Policy applies to information provided through website forms and the personal account, when contacting the operator, or when arranging a rental.</p></> },
        { id: 'terms', title: '2. Definitions', content: <ul><li><strong>Personal data</strong> means information relating directly or indirectly to an identified or identifiable individual.</li><li><strong>Processing of personal data</strong> means any operation performed on personal data, including collection, recording, storage, use, transfer, anonymisation and deletion.</li><li><strong>User</strong> means a person visiting the website or using its features.</li><li><strong>Operator</strong> means Mikhail Igorevich Volodin, who independently organises the processing of personal data or determines its purposes and scope.</li></ul> },
        { id: 'purposes', title: '3. Purposes of personal data processing', content: <><p>Personal data may be processed for the following purposes:</p><ul><li>Handling enquiries and communicating with users.</li><li>Creating and managing vehicle bookings.</li><li>Creating and maintaining a personal account, where applicable.</li><li>Verifying information and documents required to enter into and perform a rental agreement.</li><li>Preparing, entering into and performing rental agreements, calculating prices and recording payments.</li><li>Handling claims, meeting statutory obligations and protecting the operator's rights.</li><li>Maintaining website security and preventing technical failures and misuse.</li></ul><p>Marketing communications and other purposes not necessary to arrange a rental are permitted only where an appropriate legal basis exists.</p></> },
        { id: 'data', title: '4. Categories of personal data processed', content: <><p>Depending on the website features used, the operator may process:</p><ul><li><strong>Contact details:</strong> name, telephone number and email address.</li><li><strong>Booking details:</strong> selected vehicle, rental dates, pickup and return locations, additional services and order information.</li><li><strong>Document details:</strong> information from passports and driving licences, copies of documents, organisation details and, where relevant, banking details.</li><li><strong>Technical data:</strong> IP address, browser and device information, access time and website activity, where collected automatically by the infrastructure in use.</li></ul><p>The operator should not request data that is unnecessary for the stated purposes.</p></> },
        { id: 'grounds', title: '5. Legal grounds for processing', content: <><p>Depending on the circumstances, processing is based on:</p><ul><li>The user's consent, where consent is required for the relevant processing.</li><li>The necessity of entering into or performing a contract to which the user is a party, or taking steps at the user's request before entering into a contract.</li><li>Legal obligations imposed on the operator under Russian law.</li><li>Other grounds expressly provided for by Russian law.</li></ul><p>Where required, consent to personal data processing is obtained separately from other documents and confirmations. Withdrawal of consent does not prevent further processing where an independent legal basis exists.</p></> },
        { id: 'procedure', title: '6. Processing and disclosure of data', content: <><p>Personal data may be processed with or without the use of automated systems. Depending on the purpose, the operator may collect, organise, store, update, use, disclose where permitted by law, and delete personal data.</p><p>Data may be disclosed to third parties only where there is a legal basis and only to the extent necessary for the relevant purpose. This may include parties involved in performing the contract, operating the website or complying with legal requirements.</p><p>Where processing is entrusted to a third party, the operator ensures that the necessary legal grounds and processing conditions are in place. Specific recipients depend on the services and processes actually used.</p><p>When collecting personal data of Russian citizens online, the operator complies with Russian legal requirements concerning the use of databases located in Russia.</p></> },
        { id: 'retention', title: '7. Data retention and deletion', content: <><p>Personal data is retained no longer than necessary for the purposes of processing, unless a different period is required by law, contract or another applicable legal basis.</p><p>Once the purposes of processing have been achieved, data is deleted or anonymised unless further retention is required by law or necessary to protect the parties' rights and legitimate interests.</p><p>The retention period for specific documents depends on their type, actual use and mandatory retention requirements.</p></> },
        { id: 'protection', title: '8. Personal data security', content: <><p>The operator takes necessary and sufficient organisational and technical measures to protect personal data against unauthorised access, alteration, disclosure, destruction and other unlawful actions.</p><p>The scope of these measures depends on the nature of the data processed, processing risks and Russian legal requirements.</p></> },
        { id: 'rights', title: '9. User rights', content: <><p>Where provided by law, users have the right to:</p><ul><li>Obtain information about the processing of their personal data.</li><li>Request that data be corrected, blocked or deleted.</li><li>Withdraw previously given consent.</li><li>Object to processing or lodge a complaint with the competent authority.</li><li>Exercise other rights provided by Russian law.</li></ul><p>To process a request, the operator may ask the applicant to verify their identity to the extent necessary to prevent unauthorised disclosure of personal data.</p></> },
        { id: 'cookies', title: '10. Cookies and technical data', content: <><p>The website and its technical infrastructure may use cookies and similar technologies where necessary for website operation, saving user preferences, security or other stated features.</p><p>If third-party analytics systems, advertising tools or other services are used, information about their actual use and the related processing purposes should be disclosed separately.</p></> },
        { id: 'contact', title: '11. Operator contact details', content: <><p>For questions about personal data processing, exercising user rights or withdrawing consent, please contact the operator:</p><p><strong>Operator:</strong> Mikhail Igorevich Volodin</p><p><strong>INN:</strong> 272404167841</p><p><strong>Email:</strong>{' '}<a className="font-medium text-primary-700 underline underline-offset-4 transition-colors hover:text-primary-800 dark:text-primary-300 dark:hover:text-primary-200" href="mailto:volodin_rent@mail.ru">volodin_rent@mail.ru</a></p><p><strong>Postal address:</strong> 40/6 Krasnodarskaya Street, Krasnoyarsk, Russia.</p><p>Please specify in your request which personal data or processing activities it concerns.</p></> },
        { id: 'final', title: '12. Final provisions', content: <><p>The operator may update this Policy when legislation, website features or data processing procedures change.</p><p>The current version is published on this page. The updated version takes effect upon publication unless stated otherwise.</p></> },
    ],
    zh: [
        { id: 'general', title: '1. 总则', content: <><p>本隐私政策规定了“Volnami Drive”网站用户个人信息的处理和保护方式。该网站提供车辆租赁信息，并允许用户提交预订申请。</p><p><strong>个人信息处理者：</strong>米哈伊尔·伊戈列维奇·沃洛金（Mikhail Igorevich Volodin）。</p><p><strong>处理者税务识别号（INN）：</strong>272404167841。</p><p>处理者依据俄罗斯联邦2006年7月27日第152-FZ号《个人信息法》及俄罗斯联邦其他适用法律处理个人信息。</p><p>本政策适用于用户通过网站表单、个人账户、联系处理者或办理租车时提供的信息。</p></> },
        { id: 'terms', title: '2. 基本定义', content: <ul><li><strong>个人信息</strong>：直接或间接关联到已识别或可识别自然人的信息。</li><li><strong>个人信息处理</strong>：对个人信息进行的任何操作，包括收集、记录、存储、使用、传输、匿名化和删除。</li><li><strong>用户</strong>：访问网站或使用其功能的人员。</li><li><strong>处理者</strong>：米哈伊尔·伊戈列维奇·沃洛金，负责组织个人信息处理或确定其目的和范围。</li></ul> },
        { id: 'purposes', title: '3. 个人信息处理目的', content: <><p>个人信息可能用于以下目的：</p><ul><li>处理用户咨询并与用户沟通。</li><li>办理和管理车辆预订。</li><li>在适用情况下创建和维护个人账户。</li><li>核实签订及履行租赁合同所需的信息和文件。</li><li>准备、签订和履行租赁合同，计算费用并记录付款。</li><li>处理索赔、履行法定义务并保护处理者的权利。</li><li>保障网站安全，防止技术故障和滥用行为。</li></ul><p>营销信息发送及其他非办理租车所必需的处理目的，仅在具有相应合法依据时进行。</p></> },
        { id: 'data', title: '4. 可能处理的个人信息类别', content: <><p>根据用户使用的网站功能，处理者可能处理以下信息：</p><ul><li><strong>联系方式：</strong>姓名、电话号码和电子邮箱。</li><li><strong>预订信息：</strong>所选车辆、租赁日期、取车和还车地点、附加服务及订单信息。</li><li><strong>证件信息：</strong>护照和驾驶证中的信息及其副本、组织信息，以及相关银行信息。</li><li><strong>技术信息：</strong>IP地址、浏览器和设备信息、访问时间及网站操作记录（如所使用的技术基础设施会自动收集这些信息）。</li></ul><p>处理者不应要求提供与上述目的无关的信息。</p></> },
        { id: 'grounds', title: '5. 处理的法律依据', content: <><p>根据具体情况，个人信息处理基于以下法律依据：</p><ul><li>在相关处理依法需要同意时，取得用户同意。</li><li>为签订或履行用户作为一方当事人的合同所必需，或根据用户请求在签订合同前采取相关措施。</li><li>俄罗斯联邦法律规定的处理者法定义务。</li><li>俄罗斯联邦法律明确规定的其他依据。</li></ul><p>在需要取得同意时，个人信息处理同意应与其他文件和确认事项分别办理。撤回同意不影响在存在独立合法依据时继续进行处理。</p></> },
        { id: 'procedure', title: '6. 信息处理和传输方式', content: <><p>个人信息可以通过自动化方式或非自动化方式处理。根据处理目的，处理者可以收集、整理、存储、更新、使用、在法律允许的情况下传输以及删除个人信息。</p><p>只有在具有合法依据且为实现相关目的所必需的范围内，才会向第三方提供信息。例如，可能向参与合同履行、网站运营或履行法律要求的相关方提供信息。</p><p>如委托第三方处理个人信息，处理者应确保具备必要的法律依据并落实相应处理条件。具体接收方取决于实际使用的服务和业务流程。</p><p>通过互联网收集俄罗斯公民个人信息时，处理者遵守俄罗斯法律关于使用位于俄罗斯境内数据库的要求。</p></> },
        { id: 'retention', title: '7. 信息保存期限及删除', content: <><p>个人信息的保存期限不超过实现处理目的所必需的期限，法律、合同或其他适用法律依据另有规定的除外。</p><p>处理目的实现后，信息将被删除或匿名化；但法律要求继续保存或为保护相关方权利及合法利益而需要继续保存的除外。</p><p>具体文件的保存期限取决于文件类型、实际用途及强制性保存要求。</p></> },
        { id: 'protection', title: '8. 个人信息保护', content: <><p>处理者采取必要且充分的组织和技术措施，防止个人信息遭到未经授权的访问、修改、披露、销毁及其他违法操作。</p><p>保护措施的范围取决于所处理信息的性质、处理风险以及俄罗斯联邦法律要求。</p></> },
        { id: 'rights', title: '9. 用户权利', content: <><p>在法律规定的情况下，用户有权：</p><ul><li>了解其个人信息的处理情况。</li><li>要求更正、限制处理或删除个人信息。</li><li>撤回此前作出的同意。</li><li>反对相关处理，或向主管机关提出投诉。</li><li>行使俄罗斯联邦法律规定的其他权利。</li></ul><p>为处理相关请求，处理者可能要求申请人在防止个人信息被非法披露所必需的范围内确认身份。</p></> },
        { id: 'cookies', title: '10. Cookie及技术信息', content: <><p>在网站运行、保存用户设置、保障安全或实现其他已说明功能所必需的情况下，网站及其技术基础设施可能使用Cookie和类似技术。</p><p>如果使用第三方分析系统、广告工具或其他服务，应另行披露其实际使用情况及相关信息处理目的。</p></> },
        { id: 'contact', title: '11. 处理者联系方式', content: <><p>如对个人信息处理、行使用户权利或撤回同意有任何疑问，请联系处理者：</p><p><strong>处理者：</strong>米哈伊尔·伊戈列维奇·沃洛金（Mikhail Igorevich Volodin）</p><p><strong>INN：</strong>272404167841</p><p><strong>电子邮箱：</strong>{' '}<a className="font-medium text-primary-700 underline underline-offset-4 transition-colors hover:text-primary-800 dark:text-primary-300 dark:hover:text-primary-200" href="mailto:volodin_rent@mail.ru">volodin_rent@mail.ru</a></p><p><strong>通信地址：</strong>俄罗斯克拉斯诺亚尔斯克市 Krasnodarskaya 街40/6号。</p><p>建议在请求中说明涉及哪些个人信息或具体处理行为。</p></> },
        { id: 'final', title: '12. 最终条款', content: <><p>如法律、网站功能或信息处理流程发生变化，处理者有权更新本政策。</p><p>最新版本将在本页面发布。除非另有说明，新版本自发布之日起生效。</p></> },
    ],
};

export default function PrivacyPolicy() {
    const { language } = useI18n();
    const currentSections = language === 'ru' ? sections : localizedSections[language];
    const pageText = {
        ru: { back: 'На главную', nav: 'Содержание политики', contents: 'Содержание', title: 'Политика обработки персональных данных', intro: 'Информация о том, какие персональные данные могут обрабатываться при использовании сайта и оформлении аренды автомобиля, для каких целей и как пользователь может реализовать свои права.', operator: 'Оператор', footer: 'Оператор персональных данных: Володин Михаил Игоревич', slogan: '«Волнами Драйв» — движение на твоей волне.', inn: 'ИНН' },
        en: { back: 'Home', nav: 'Privacy policy contents', contents: 'Contents', title: 'Personal Data Processing Policy', intro: 'Information about the personal data that may be processed when using the website and arranging a vehicle rental, the purposes of processing, and how users can exercise their rights.', operator: 'Operator', footer: 'Personal data operator: Mikhail Igorevich Volodin', slogan: 'Volnami Drive — move to your own rhythm.', inn: 'INN' },
        zh: { back: '返回首页', nav: '隐私政策目录', contents: '目录', title: '个人信息处理政策', intro: '本页面介绍使用网站及办理车辆租赁时可能处理的个人信息、处理目的，以及用户如何行使自身权利。', operator: '处理者', footer: '个人信息处理者：米哈伊尔·伊戈列维奇·沃洛金', slogan: 'Volnami Drive — 随心出发，畅享旅程。', inn: '税务识别号' },
    }[language];
    return (
        <main className="min-h-screen bg-gray-50 pb-16 pt-24 text-gray-900 dark:bg-gray-950 dark:text-gray-100">
            <div className="container mx-auto max-w-6xl px-4 sm:px-6">
                <Link
                    to="/"
                    className="mb-6 inline-flex items-center gap-2 text-sm font-medium text-gray-600 transition-colors hover:text-gray-950 dark:text-gray-400 dark:hover:text-white"
                >
                    <ArrowLeft className="h-4 w-4" aria-hidden="true" />
                    {pageText.back}
                </Link>

                <header className="relative mb-8 overflow-hidden rounded-3xl bg-white p-6 shadow-sm ring-1 ring-gray-200 dark:bg-gray-900 dark:ring-gray-800 sm:p-10 lg:p-14">
                    <div
                        aria-hidden="true"
                        className="pointer-events-none absolute -right-20 -top-20 h-64 w-64 rounded-full bg-primary-100/70 blur-3xl dark:bg-primary-900/20"
                    />

                    <div className="relative max-w-4xl">
                        <div className="mb-5 flex h-12 w-12 items-center justify-center rounded-2xl bg-primary-50 text-primary-700 dark:bg-primary-950/50 dark:text-primary-300">
                            <ShieldCheck className="h-6 w-6" aria-hidden="true" />
                        </div>

                        <p className="mb-3 text-sm font-semibold uppercase tracking-wider text-primary-700 dark:text-primary-300">
                            Волнами Драйв
                        </p>

                        <h1 className="text-3xl font-bold tracking-tight text-gray-950 dark:text-white sm:text-4xl lg:text-5xl">
                            {pageText.title}
                        </h1>

                        <p className="mt-5 max-w-3xl text-base leading-7 text-gray-600 dark:text-gray-300 sm:text-lg">
                            {pageText.intro}
                        </p>

                        <p className="mt-4 text-sm text-gray-500 dark:text-gray-400">
                            {pageText.operator}: {language === 'zh' ? '米哈伊尔·伊戈列维奇·沃洛金' : 'Володин Михаил Игоревич'} · INN 272404167841
                        </p>
                    </div>
                </header>

                <nav
                    aria-label={pageText.nav}
                    className="mb-8 rounded-2xl bg-white p-5 shadow-sm ring-1 ring-gray-200 dark:bg-gray-900 dark:ring-gray-800 sm:p-8"
                >
                    <h2 className="mb-5 text-lg font-bold tracking-tight text-gray-950 dark:text-white">
                        {pageText.contents}
                    </h2>

                    <ol className="grid gap-x-8 gap-y-3 sm:grid-cols-2">
                        {currentSections.map((section) => (
                            <li key={section.id}>
                                <a
                                    href={`#${section.id}`}
                                    className="text-sm leading-6 text-gray-600 transition-colors hover:text-primary-700 hover:underline dark:text-gray-300 dark:hover:text-primary-300"
                                >
                                    {section.title}
                                </a>
                            </li>
                        ))}
                    </ol>
                </nav>

                <div className="space-y-4">
                    {currentSections.map((section) => (
                        <section
                            key={section.id}
                            id={section.id}
                            className="scroll-mt-28 rounded-2xl bg-white p-6 shadow-sm ring-1 ring-gray-200 dark:bg-gray-900 dark:ring-gray-800 sm:p-8"
                        >
                            <h2 className="mb-5 border-b border-gray-100 pb-4 text-xl font-bold tracking-tight text-gray-950 dark:border-gray-800 dark:text-white sm:text-2xl">
                                {section.title}
                            </h2>

                            <div className="space-y-4 text-sm leading-7 text-gray-600 dark:text-gray-300 sm:text-base [&_li]:relative [&_li]:pl-5 [&_li]:before:absolute [&_li]:before:left-0 [&_li]:before:top-0 [&_li]:before:text-primary-600 [&_li]:before:content-['•'] dark:[&_li]:before:text-primary-300 [&_ol]:list-decimal [&_ol]:space-y-2 [&_ul]:space-y-2 [&_strong]:font-semibold [&_strong]:text-gray-900 dark:[&_strong]:text-gray-100">
                                {section.content}
                            </div>
                        </section>
                    ))}
                </div>

                <footer className="mt-8 rounded-2xl bg-gray-100 p-6 text-sm text-gray-600 dark:bg-gray-900 dark:text-gray-400 sm:p-8">
                    <p className="font-semibold text-gray-900 dark:text-white">
                        {pageText.footer}
                    </p>
                    <p className="mt-2">{pageText.inn} 272404167841.</p>
                    <p className="mt-2">
                        {pageText.slogan}
                    </p>
                </footer>
            </div>
        </main>
    );
}