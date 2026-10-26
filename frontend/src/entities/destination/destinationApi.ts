import api from '@/shared/api/axios';
import { DestinationDto, DestinationType } from '@/entities/destination/destination.type';

export const destinationApi = {
    getActive: (type: DestinationType) =>
        api.get<DestinationDto[]>('/destination', {
            params: { type },
        }),
};