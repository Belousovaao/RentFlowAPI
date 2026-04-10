import { Asset } from '../../../entities/asset/asset.types';
import { AssetCard } from './AssetCard';

interface AssetGridProps {
    assets: Asset[];
}

export const AssetGrid = ({ assets }: AssetGridProps) => {
    console.log('🎨 AssetGrid rendering with', assets?.length, 'assets');
    
    if (!assets || assets.length === 0) {
        return (
            <div className="text-center py-12 bg-gray-50 dark:bg-gray-800 rounded-lg">
                <p className="text-gray-500">No vehicles available at the moment.</p>
                <p className="text-sm text-gray-400 mt-2">Check back later!</p>
            </div>
        );
    }

    return (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {assets.map((asset, index) => (
                <AssetCard key={asset.id} asset={asset} index={index} />
            ))}
        </div>
    );
};