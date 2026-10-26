import {
    Building2,
    Compass,
    MapIcon,
    SearchX,
} from "lucide-react";

import { DestinationType } from "./destination.type";

export const travelZones = [
    {
        type: DestinationType.Local,
        title: "Вокруг Красноярска (до 100 км)",
        description:
            "* г. Дивногорск, г. Сосновоборск, г. Железногорск",
        icon: Building2,
    },
    {
        type: DestinationType.Regional,
        title: "Малая Сибирь (до 250 км)",
        description: "Выберите город назначения из списка",
        icon: Compass,
    },
    {
        type: DestinationType.LongDistance,
        title: "Большая Сибирь (до 1'000 км)",
        description: "Выберите город назначения из списка",
        icon: MapIcon,
    },
    {
        type: DestinationType.Other,
        title: "Другое",
        description:
            "Опишите планируемый маршрут в поле ниже. Окончательные условия аренды определяются после проверки маршрута менеджером.",
        icon: SearchX,
    },
] as const;