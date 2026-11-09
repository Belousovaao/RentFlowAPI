import { Fragment, useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { adminAssetsApi } from '@/features/admin/api/adminAsset.api';
import { Asset, AssetStatus, AssetStatusColors, AssetStatusLabels } from '@/entities/asset/asset.types';
import { Pencil, Plus, Image, ArrowLeft, } from 'lucide-react';
import { getPhotoUrl } from '@/shared/utils/photoUrl';
import { useI18n } from '@/app/providers/I18nProvider';

export const AdminAssetsListPage = () => {
  const { t, language } = useI18n();
  const [assets, setAssets] = useState<Asset[]>([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);
  const [showDisabled, setShowDisabled] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [maintenanceAssetId, setMaintenanceAssetId] = useState<string | null>(null);
  const [maintenancePeriods, setMaintenancePeriods] = useState<{ id: string; startDate: string; endDate: string; reason?: string | null }[]>([]);
  const [maintenanceStart, setMaintenanceStart] = useState('');
  const [maintenanceEnd, setMaintenanceEnd] = useState('');
  const [maintenanceReason, setMaintenanceReason] = useState('');
  const navigate = useNavigate();

  const fetchAssets = async () => {
    setLoading(true);
    try {
      const res = await adminAssetsApi.getList({ 
        page, 
        pageSize: 20,
        showDisabled,
        search: searchTerm || undefined
      });
      setAssets(res.data.data);
      setTotal(res.data.totalCount);
    } catch (err) {
      console.error(err);
      alert(t('admin.assets.loadError'));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchAssets();
  }, [page, showDisabled, searchTerm]);

  const getFirstExteriorPhoto = (asset: Asset): string => {
    // Ищем фото с типом Exterior
    const exteriorPhoto = asset.photos?.find(p => p.type === 'Exterior');
    if (exteriorPhoto) {
        return getPhotoUrl(exteriorPhoto.url);
    }
    return '/car-placeholder.jpg';
  };

  const handleStatusChange = async (assetId: string, newStatus: AssetStatus) => {
    try {
      await adminAssetsApi.updateStatus(assetId, newStatus);
      alert(`${t("admin.assets.statusChanged")} ${t(`asset.status.${newStatus}`)}`);
      fetchAssets(); // Обновляем список
    } catch (err) {
      alert(t('admin.assets.statusError'));
    }
  };

  const openMaintenance = async (assetId: string) => {
    if (maintenanceAssetId === assetId) {
      setMaintenanceAssetId(null);
      return;
    }
    setMaintenanceAssetId(assetId);
    setMaintenanceStart('');
    setMaintenanceEnd('');
    setMaintenanceReason('');
    try {
      const response = await adminAssetsApi.getMaintenancePeriods(assetId);
      setMaintenancePeriods(response.data);
    } catch {
      alert(t('admin.assets.maintenanceError'));
    }
  };

  const saveMaintenance = async (assetId: string) => {
    if (!maintenanceStart || !maintenanceEnd || maintenanceEnd < maintenanceStart) {
      alert(t('admin.assets.maintenanceError'));
      return;
    }
    try {
      const start = new Date(maintenanceStart + 'T00:00:00.000Z');
      const end = new Date(maintenanceEnd + 'T00:00:00.000Z');
      end.setUTCDate(end.getUTCDate() + 1);
      await adminAssetsApi.createMaintenancePeriod(assetId, {
        startDate: start.toISOString(),
        endDate: end.toISOString(),
        reason: maintenanceReason || undefined
      });
      const response = await adminAssetsApi.getMaintenancePeriods(assetId);
      setMaintenancePeriods(response.data);
      setMaintenanceStart('');
      setMaintenanceEnd('');
      setMaintenanceReason('');
    } catch {
      alert(t('admin.assets.maintenanceError'));
    }
  };

  const deleteMaintenance = async (assetId: string, periodId: string) => {
    try {
      await adminAssetsApi.deleteMaintenancePeriod(assetId, periodId);
      setMaintenancePeriods(current => current.filter(period => period.id !== periodId));
    } catch {
      alert(t('admin.assets.maintenanceError'));
    }
  };

  const totalPages = Math.ceil(total / 20);

  return (
    <div className="min-h-screen bg-gray-50 dark:bg-gray-900 pt-24 pb-16">
      <div className="container mx-auto px-4">
        {/* Заголовок и кнопка добавления */}
        <div className="flex justify-between items-center mb-6">
          <div className="flex items-center gap-4">
            <button
                onClick={() => navigate('/admin/')}
                className="p-2 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-lg transition-colors"
            >
                <ArrowLeft className="w-5 h-5" />
            </button>
            <h1 className="text-2xl font-bold text-gray-900 dark:text-white">
              {t("admin.assets.title")}
            </h1>
          </div>
          <Link 
            to="/admin/assets/new" 
            className="bg-primary-600 text-white px-4 py-2 rounded-lg flex items-center gap-2 hover:bg-primary-700 transition-colors"
          >
            <Plus className="w-4 h-4" /> {t("admin.assets.add")}
          </Link>
        </div>

        {/* Фильтры */}
        <div className="bg-white dark:bg-gray-800 rounded-lg shadow-sm p-4 mb-6">
          <div className="flex flex-col sm:flex-row gap-4">
            <div className="flex-1">
              <input
                type="text"
                placeholder={t("admin.assets.searchPlaceholder")}
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg 
                         bg-white dark:bg-gray-700 text-gray-900 dark:text-white
                         focus:ring-2 focus:ring-primary-500 focus:border-transparent"
              />
            </div>
            <label className="inline-flex items-center">
              <input
                type="checkbox"
                checked={showDisabled}
                onChange={(e) => setShowDisabled(e.target.checked)}
                className="rounded border-gray-300 text-primary-600 focus:ring-primary-500"
              />
              <span className="ml-2 text-sm text-gray-600 dark:text-gray-400">
                {t("admin.assets.showDisabled")}
              </span>
            </label>
          </div>
        </div>

        {/* Таблица с автомобилями */}
        {loading ? (
          <div className="flex justify-center py-12">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
          </div>
        ) : !assets || assets.length === 0 ? (
          <div className="text-center py-12 bg-white dark:bg-gray-800 rounded-xl">
            <p className="text-gray-500">{t("admin.assets.empty")}</p>
            <Link to="/admin/assets/new" className="text-primary-600 hover:underline mt-2 inline-block">
              {t("admin.assets.addFirst")}
            </Link>
          </div>
        ) : (
          <div className="overflow-hidden rounded-2xl border border-gray-200 bg-white dark:border-gray-700 dark:bg-gray-800">
            <table className="w-full table-auto divide-y divide-gray-200 dark:divide-gray-700">
              <thead className="bg-gray-50 dark:bg-gray-900">
                <tr>
                  <th className="px-4 py-2 text-sm font-medium text-gray-500 tracking-wider">
                    {t("common.photo")}
                  </th>
                  <th className="px-4 py-2 text-sm font-medium text-gray-500 tracking-wider">
                    {t("common.vehicle")}
                  </th>
                  <th className="px-4 py-2 text-sm font-medium text-gray-500 tracking-wider">
                    {t("common.year")}
                  </th>
                  <th className="px-4 py-2 whitespace-nowrap text-sm font-medium text-gray-500 tracking-wider">
                    {t("common.dailyPrice")}
                  </th>
                  <th className="px-4 py-2 text-sm font-medium text-gray-500 tracking-wider">
                    {t("common.status")}
                  </th>
                  <th className="px-4 py-2 text-sm font-medium text-gray-500 tracking-wider">
                    {t("common.actions")}
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200 dark:divide-gray-700">
                {assets.map((asset) => (
                  <Fragment key={asset.id}>
                  <tr 
                    className={`hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors
                      ${asset.status == AssetStatus.Disabled ? 'bg-gray-50 dark:bg-gray-900/50' : ''}`}
                  >
                    {/* Фото */}
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="relative group">
                      {asset.photos && asset.photos.length > 0 ? (
                        <img 
                          src={getFirstExteriorPhoto(asset)}
                          alt={`${asset.brandName} ${asset.model}`}
                          className="w-16 h-12 object-cover rounded"
                        />
                      ) : (
                          <div className="w-16 h-12 bg-gray-200 dark:bg-gray-700 rounded flex items-center justify-center">
                            <Image className="w-6 h-6 text-gray-400" />
                          </div>
                        )}
                      </div>
                    </td>
                    {/* Марка / Модель */}
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="text-sm font-medium text-gray-900 dark:text-white">
                        {asset.brandName} {asset.model}
                      </div>
                      {asset.status == AssetStatus.Disabled && (
                        <span className="inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400 mt-1">
                          {t("asset.status.Disabled")}
                        </span>
                      )}
                    </td>
                    {/* Год */}
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                      {asset.year}
                    </td>
                    {/* Цена */}
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-white">
                      {asset.dailyPrice.toLocaleString(language === "zh" ? "zh-CN" : language === "en" ? "en-GB" : "ru-RU")} ₽
                    </td>
                    {/* Статус */}
                    <td className="px-6 py-4 whitespace-nowrap">
                      <select
                        value={asset.status}
                        onChange={(e) => handleStatusChange(asset.id, e.target.value as AssetStatus)}
                        className={`px-2 py-1 rounded-full text-xs font-medium border-0 cursor-pointer ${AssetStatusColors[asset.status as AssetStatus]}`}
                      >
                        {Object.entries(AssetStatusLabels).filter(([status]) => status !== String(AssetStatus.Service) || asset.status === AssetStatus.Service).map(([status, label]) => (
                          <option key={status} value={status} className="text-gray-900">
                            {t(`asset.status.${status}`)}
                          </option>
                        ))}
                      </select>
                    </td>
                    {/* Действия */}
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="flex gap-2">
                        <Link 
                          to={`/admin/assets/${asset.id}/edit`} 
                          className="text-blue-600 hover:text-blue-800 dark:text-blue-400"
                          title={t("common.edit")}
                        >
                          <Pencil className="w-4 h-4" />
                        </Link>
                        <button type="button" onClick={() => openMaintenance(asset.id)} className="text-sm text-primary-600 hover:text-primary-800 dark:text-primary-400">
                          {t('admin.assets.maintenance')}
                        </button>
                      </div>
                    </td>
                  </tr>
                  {maintenanceAssetId === asset.id && (
                    <tr>
                      <td colSpan={6} className="px-6 py-4 bg-gray-50 dark:bg-gray-900/60">
                        <div className="space-y-3">
                          <div className="font-medium text-gray-900 dark:text-white">{t('admin.assets.maintenance')}</div>
                          {maintenancePeriods.length === 0 ? (
                            <p className="text-sm text-gray-500">{t('admin.assets.noMaintenance')}</p>
                          ) : (
                            <ul className="space-y-2">
                              {maintenancePeriods.map(period => (
                                <li key={period.id} className="flex flex-wrap items-center justify-between gap-2 text-sm">
                                  <span>{new Date(period.startDate).toLocaleDateString()} – {new Date(new Date(period.endDate).getTime() - 86400000).toLocaleDateString()}{period.reason ? ' · ' + period.reason : ''}</span>
                                  <button type="button" onClick={() => deleteMaintenance(asset.id, period.id)} className="text-red-600 hover:underline">
                                    {t('admin.assets.deleteMaintenance')}
                                  </button>
                                </li>
                              ))}
                            </ul>
                          )}
                          <div className="flex flex-wrap items-end gap-3">
                            <label className="text-sm">
                              <span className="block mb-1">{t('admin.assets.startDate')}</span>
                              <input type="date" value={maintenanceStart} onChange={e => setMaintenanceStart(e.target.value)} className="rounded border px-3 py-2 dark:bg-gray-800 dark:border-gray-600" />
                            </label>
                            <label className="text-sm">
                              <span className="block mb-1">{t('admin.assets.endDate')}</span>
                              <input type="date" value={maintenanceEnd} min={maintenanceStart || undefined} onChange={e => setMaintenanceEnd(e.target.value)} className="rounded border px-3 py-2 dark:bg-gray-800 dark:border-gray-600" />
                            </label>
                            <label className="text-sm flex-1 min-w-48">
                              <span className="block mb-1">{t('admin.assets.maintenanceReason')}</span>
                              <input type="text" value={maintenanceReason} onChange={e => setMaintenanceReason(e.target.value)} maxLength={500} className="w-full rounded border px-3 py-2 dark:bg-gray-800 dark:border-gray-600" />
                            </label>
                            <button type="button" onClick={() => saveMaintenance(asset.id)} className="rounded bg-primary-600 px-4 py-2 text-white hover:bg-primary-700">
                              {t('admin.assets.saveMaintenance')}
                            </button>
                          </div>
                        </div>
                      </td>
                    </tr>
                  )}
                  </Fragment>
                ))}
              
              </tbody>
            </table>
          </div>
        )}

        {/* Пагинация */}
        {totalPages > 1 && (
          <div className="flex justify-center gap-2 mt-8">
            <button
              onClick={() => setPage(p => Math.max(1, p - 1))}
              disabled={page === 1}
              className="px-4 py-2 border rounded-lg disabled:opacity-50 disabled:cursor-not-allowed
                       hover:bg-gray-100 dark:hover:bg-gray-700
                       text-gray-700 dark:text-gray-300"
            >
              {t("common.previous")}
            </button>
            <span className="px-4 py-2 text-gray-600 dark:text-gray-400">
              {t("common.pageOf").replace("{page}", String(page)).replace("{totalPages}", String(totalPages))}
            </span>
            <button
              onClick={() => setPage(p => Math.min(totalPages, p + 1))}
              disabled={page === totalPages}
              className="px-4 py-2 border rounded-lg disabled:opacity-50 disabled:cursor-not-allowed
                       hover:bg-gray-100 dark:hover:bg-gray-700
                       text-gray-700 dark:text-gray-300"
            >
              {t("common.next")}
            </button>
          </div>
        )}
      </div>
    </div>
  );
};