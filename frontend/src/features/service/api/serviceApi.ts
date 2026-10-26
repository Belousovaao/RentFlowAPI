import { AdditionalService } from '@/entities/service/service.types';
import api from '@/shared/api/axios';

export const serviceApi = {
    getAll: () =>
        api.get<AdditionalService[]>(
            '/services'),
};