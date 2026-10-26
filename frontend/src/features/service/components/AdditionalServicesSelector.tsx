import { AdditionalService, SelectedAdditionalService, ServiceType } from '@/entities/service/service.types';
import { formatPrice } from '@/shared/utils/formatters';
import { useI18n } from '@/app/providers/I18nProvider';
import { BookingServiceDto } from '@/entities/booking/booking.types';

interface Props {
    services: AdditionalService[];
    selectedServices: SelectedAdditionalService[];
    calculatedServices: BookingServiceDto[];
    onChange: (services: SelectedAdditionalService[]) => void;
}

export const AdditionalServicesSelector = ({
    services,
    selectedServices,
    calculatedServices,
    onChange
}: Props) => {

    const { t } = useI18n();

    const toggleService = (service: AdditionalService) => {
        const exists = selectedServices.some(
            x => x.id === service.id
        );

        if (exists) {
            onChange(
                selectedServices.filter(
                    x => x.id !== service.id
                )
            );
            return;
        }

        onChange([
            ...selectedServices,
            {
                id: service.id,
                name: service.name,
                type: service.type,
                price: service.price,
                selectedTime: getDefaultTime(service.type)
            }
        ]);
    };

    const updateTime = (
        serviceId: string,
        time: string
    ) => {
        onChange(
            selectedServices.map(service =>
                service.id === serviceId
                    ? {
                        ...service,
                        selectedTime: time
                    }
                    : service
            )
        );
    };

    return (
        <div className="space-y-3">
            <h3 className="text-xl font-bold text-gray-900 dark:text-white">
                {t("checkout.additionalServices")}
            </h3>
            <div className="space-y-3">
                {services.map(service => {
                    const selected = selectedServices.find(
                        x => x.id === service.id
                    );

                    const dynamicPrice = requiresTime(service.type);

                    const calculated = calculatedServices.find(
                        x => x.additionalServiceId === service.id
                    );

                    return (
                        <div
                            key={service.id}
                            className={`
                                rounded-xl border p-4 transition-all
                                ${
                                    selected
                                        ? 'border-primary-400 bg-primary-50/50 dark:bg-primary-900/10'
                                        : 'border-gray-200 dark:border-gray-700'
                                }
                            `}
                        >
                            <div className="flex items-start justify-between gap-4">

                                <label className="flex items-start gap-3 cursor-pointer min-w-0">

                                    <input
                                        type="checkbox"
                                        checked={!!selected}
                                        onChange={() => toggleService(service)}
                                        className="mt-1 h-4 w-4"
                                    />

                                    <div>
                                        <div className="font-medium text-gray-900 dark:text-white">
                                            {service.name}
                                        </div>

                                        {dynamicPrice && (
                                            <p className="text-sm text-gray-500 dark:text-gray-400 mt-1">
                                                {t("booking.priceDependsOnTime")}
                                            </p>
                                        )}
                                    </div>

                                </label>

                                <span className="shrink-0 font-medium text-primary-600">
                                    {dynamicPrice
                                        ? calculated
                                            ? `+${formatPrice(calculated.price)}`
                                            : ''
                                        : `+${formatPrice(service.price)}`
                                    }
                                </span>
                            </div>

                            {selected && dynamicPrice && (
                                <div className="mt-4 ml-7">
                                    <select
                                        value={
                                            selected.selectedTime ??
                                            getDefaultTime(service.type) ?? ''
                                        }
                                        onChange={(e) => updateTime(service.id, e.target.value)
                                        }
                                        className="w-full max-w-xs border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-2 bg-white dark:bg-gray-800 text-gray-900 dark:text-white focus:ring-2 focus:ring-primary-500"
                                    >
                                        {generateTimeOptions(service.type).map(time => (
                                            <option
                                                key={time}
                                                value={time}
                                            >
                                                {time}
                                            </option>
                                        ))}
                                    </select>
                                </div>
                            )}
                        </div>
                    );
                })}
            </div>
        </div>
    );
};

function requiresTime(type: ServiceType)
{
    return (
        type === ServiceType.EarlyPickup ||
        type === ServiceType.LateReturn
    );
}

function generateTimeOptions(type: ServiceType)
{
    const result: string[] = [];

    const startHour =
        type === ServiceType.LateReturn ? 12 : 0;

    const endHour =
        type === ServiceType.EarlyPickup ? 11 : 23;

    for (let hour = startHour; hour <= endHour; hour++)
    {
        // Для позднего возврата в 12:00 уже начинается бесплатное время,
        // поэтому начинаем с 12:30.
        if (type === ServiceType.LateReturn && hour === 12)
        {
            result.push("12:30");
            continue;
        }

        result.push(`${hour.toString().padStart(2, "0")}:00`);
        result.push(`${hour.toString().padStart(2, "0")}:30`);
    }

    return result;
}

function getDefaultTime(type: ServiceType): string | undefined {
    switch (type) {
        case ServiceType.EarlyPickup:
            return "11:30";

        case ServiceType.LateReturn:
            return "12:30";

        default:
            return undefined;
    }
}