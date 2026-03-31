import { useEffect, useState } from "react";
import { getAssets } from "../../entities/asset/asset.api";
import type { Asset } from "../../entities/asset/asset.types";

export const AssetsPage = () => {
    const [assets, setAssets] = useState<Asset[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const load = async () => {
            try {
                const data = await getAssets();
                setAssets(data);
            }
            catch (e: any) {
                setError(e.message);
            }
            finally {
                setLoading(false);
            }
        };
        load();
    }, []);

    if (loading) return <div>Loading...</div>;
    if (error) return <div>Error: {error}</div>;

    return (
        <div>
            <h1>Assets</h1>
            <ul>
                {assets.map((a) => (
                    <li key={a.id}>
                        {a.brandName} {a.model} - {a.dailyPrice}
                    </li>
                ))}
            </ul>
        </div>
    );
};