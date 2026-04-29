import { AssetStatus, } from './asset.types';

export const getStatusColor = (status: AssetStatus): string => {
    switch (status) {
        case AssetStatus.Available: return 'green';
        case AssetStatus.Reserved: return 'yellow';
        case AssetStatus.InRent: return 'blue';
        case AssetStatus.Service: return 'orange';
        case AssetStatus.Disabled: return 'red';
        default: return 'gray';
    }
};