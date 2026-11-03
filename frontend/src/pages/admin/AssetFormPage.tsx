import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { adminAssetsApi } from '@/features/admin/api/adminAsset.api';
import { Asset, AssetCategory, AssetCategoryLabels, AssetStatus, AssetType, AssetTypeLabels, DriveType, DriveTypeLabels, FuelType, FuelTypeLabels, TransmissionType, TransmissionTypeLabels } from '@/entities/asset/asset.types';
import { ArrowLeft, Save, X, Plus, Upload, Trash2, GripVertical } from 'lucide-react';
import api from '@/shared/api/axios';
import { DndContext, closestCenter, KeyboardSensor, PointerSensor, useSensor, useSensors, DragEndEvent, } from '@dnd-kit/core';
import { arrayMove, SortableContext, sortableKeyboardCoordinates, verticalListSortingStrategy, useSortable, } from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';
import { getPhotoUrl } from '@/shared/utils/photoUrl';
import { useI18n } from '@/app/providers/I18nProvider';

const SortablePhotoItem = ({ photo, index, onDelete, type, getFullImageUrl }: { 
    photo: { id: string; url: string }; 
    index: number; 
    onDelete: (id: string) => void;
    type: string;
    getFullImageUrl: (url: string) => string;
}) => {
    const {
        attributes,
        listeners,
        setNodeRef,
        transform,
        transition,
        isDragging,
    } = useSortable({ id: photo.id });

    const style = {
        transform: CSS.Transform.toString(transform),
        transition,
        opacity: isDragging ? 0.5 : 1,
        cursor: 'grab',
    };

    

    return (
        <div
            ref={setNodeRef}
            style={style}
            className="relative group"
            {...attributes}
            //{...listeners}
        >
            <img 
                src={getFullImageUrl(photo.url)} 
                alt={`${type} ${index + 1}`} 
                className="w-full h-20 object-cover rounded-lg" 
            />
            <button
                type="button"
                onClick={(e) => 
                    {
                        e.stopPropagation();
                        console.log('🖱️ Кнопка корзины НАЖАТА! photo.id:', photo.id);
                        onDelete(photo.id)
                    }}
                className="absolute top-1 right-1 p-1 bg-red-600 text-white rounded-full opacity-0 group-hover:opacity-100 transition-opacity z-10"
            >
                <Trash2 className="w-3 h-3" />
            </button>
            <div className="absolute bottom-1 left-1 bg-black/60 text-white text-xs px-1 rounded">
                {index + 1}
            </div>
            <div 
                {...listeners}
                className="absolute top-1 left-1 bg-black/50 text-white p-1 rounded opacity-0 group-hover:opacity-100 transition-opacity">
                <GripVertical className="w-3 h-3" />
            </div>
        </div>
    );
};

export const AssetFormPage = () => {
    const { t } = useI18n();
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const isEditMode = !!id && id !== 'new';
    
    const [loading, setLoading] = useState(false);
    const [saving, setSaving] = useState(false);
    const [newFeature, setNewFeature] = useState('');
    const [uploadingExterior, setUploadingExterior] = useState(false);
    const [uploadingInterior, setUploadingInterior] = useState(false);
    const [exteriorPhotos, setExteriorPhotos] = useState<{ id: string; url: string }[]>([]);
    const [interiorPhotos, setInteriorPhotos] = useState<{ id: string; url: string }[]>([]);
    const [formData, setFormData] = useState<Partial<Asset>>({
        brandName: '',
        model: '',
        year: new Date().getFullYear(),
        dailyPrice: 0,
        fuelType: FuelType.Petrol,
        transmission: TransmissionType.Automatic,
        driveType: DriveType.FrontWheelDrive,
        seats: 5,
        horsepower: 150,
        status: AssetStatus.Available,
        shortDescription: '',
        fullDescription: '',
        type: AssetType.Auto,
        category: AssetCategory.Crossover,
        doors: 5,
        engine: '',
        acceleration: '',
        topSpeed: 0,
        color: '',
        features: [],
        deposit: 5000,
        latitude: null,
        longitude: null
      });

    const sensors = useSensors(useSensor(PointerSensor), useSensor(KeyboardSensor, {coordinateGetter: sortableKeyboardCoordinates,}))
    // Загрузка данных при редактировании
    useEffect(() => {
        if (isEditMode && id) {
        fetchAsset();
        }
    }, [id]);

    const fetchAsset = async () => {
        setLoading(true);
        try {
            const res = await adminAssetsApi.getById(id!);
            setFormData({
                brandName: res.data.brandName,
                model: res.data.model,
                year: res.data.year,
                dailyPrice: res.data.dailyPrice,
                fuelType: res.data.fuelType as FuelType,
                transmission: res.data.transmission as TransmissionType,
                driveType: Number(res.data.driveType) as DriveType,
                seats: res.data.seats,
                horsepower: res.data.horsepower,
                status: res.data.status as AssetStatus,
                type: res.data.type as AssetType,
                category: res.data.category as AssetCategory,
                features: res.data.features || [],
                shortDescription: res.data.shortDescription || '',
                fullDescription: res.data.fullDescription || '',
                doors: res.data.doors || 5,
                engine: res.data.engine || '',
                acceleration: res.data.acceleration || '',
                topSpeed: res.data.topSpeed || 0,
                color: res.data.color || '',
                deposit: res.data.deposit || 5000,
            });
            console.log("API:", res.data.driveType, typeof res.data.driveType);
    
            const photos = res.data.photos || [];
            const exterior = photos.filter(p => p.type == 'Exterior').sort((a, b) => a.order - b.order).map(p => ({ id: p.id, url: p.url }));
            const interior = photos.filter(p => p.type === 'Interior').sort((a, b) => a.order - b.order).map(p => ({ id: p.id, url: p.url }));
            
            setExteriorPhotos(exterior);
            setInteriorPhotos(interior);
            
            console.log('Exterior photos:', exterior);
            console.log('Interior photos:', interior);
        } catch (err) {
            console.error(err);
            alert(t("admin.assetForm.loadError"));
            navigate('/admin/assets');
        } finally {
            setLoading(false);
        }
    };

    const numericFields = new Set([
        "year",
        "dailyPrice",
        "deposit",
        "horsepower",
        "doors",
        "seats",
        "topSpeed",
        "fuelType",
        "transmission",
        "driveType",
        "category",
        "type"
    ]);

    const handleChange = (
        e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>
    ) => {
        const { name, value } = e.target;

        setFormData(prev => ({
            ...prev,
            [name]: numericFields.has(name)
                ? Number(value)
                : value
        }));
    };

    const handleCheckboxChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, checked } = e.target;
        setFormData(prev => ({ ...prev, [name]: checked }));
    };

    const addFeature = () => {
        if (newFeature.trim()) {
          setFormData(prev => ({
            ...prev,
            features: [...(prev.features || []), newFeature.trim()]
          }));
          setNewFeature('');
        }
    };

    const removeFeature = (index: number) => {
        setFormData(prev => ({
          ...prev,
          features: prev.features?.filter((_, i) => i !== index)
        }));
    };

    // Загрузка фото
    const uploadPhotos = async (assetId: string, files: FileList, type: 'exterior' | 'interior') => {
        console.log('Asset ID:', assetId);
        console.log('Type:', type);
        console.log('Files count:', files.length);
        const formData = new FormData();
        for (let i = 0; i < files.length; i++) {
            formData.append('files', files[i]);
        }
        
        const url = `/admin/assets/${assetId}/photos/${type}/batch`;
        console.log('Request URL:', url);
        const response = await api.post(url, formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
        });
        console.log('Response:', response.data);

        return response.data.urls;
    };

    const handleExteriorUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
        if (!e.target.files?.length || !id) return;
        setUploadingExterior(true);
        try {
            const urls = await uploadPhotos(id, e.target.files, 'exterior');
            setExteriorPhotos(prev => [...prev, ...urls]);
            alert(`${t("admin.assetForm.exteriorUploaded")} ${urls.length}`);
        } catch (err) {
            console.error(err);
            alert(t("admin.assetForm.exteriorUploadError"));
        } finally {
            setUploadingExterior(false);
            e.target.value = '';
        }
    };

    const handleInteriorUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
        if (!e.target.files?.length || !id) return;
        setUploadingInterior(true);
        try {
            const urls = await uploadPhotos(id, e.target.files, 'interior');
            setInteriorPhotos(prev => [...prev, ...urls]);
            alert(`${t("admin.assetForm.interiorUploaded")} ${urls.length}`);
        } catch (err) {
            console.error(err);
            alert(t("admin.assetForm.interiorUploadError"));
        } finally {
            setUploadingInterior(false);
            e.target.value = '';
        }
    };

    // Удаление фото экстерьера
    const deleteExteriorPhoto = async (photoId: string) => {
        console.log('🔥 deleteExteriorPhoto ВЫЗВАНА! photoId:', photoId);
        if (!confirm(t("admin.assetForm.deletePhotoConfirm"))) return;
        try {
            await adminAssetsApi.deletePhoto(id!, photoId);
            setExteriorPhotos(prev => prev.filter(p => p.id !== photoId));
            alert(t("admin.assetForm.photoDeleted"));
        } catch (err) {
            console.error(err);
            alert(t("admin.assetForm.deletePhotoError"));
        }
    };

    // Удаление фото интерьера
    const deleteInteriorPhoto = async (photoId: string) => {
        if (!confirm(t("admin.assetForm.deletePhotoConfirm"))) return;
        try {
            await adminAssetsApi.deletePhoto(id!, photoId);
            setInteriorPhotos(prev => prev.filter(p => p.id !== photoId));
            alert(t("admin.assetForm.photoDeleted"));
        } 
        catch (err) {
            console.error(err);
            alert(t("admin.assetForm.deletePhotoError"));
    }
};

    const reorderPhotos = async (type: 'exterior' | 'interior', photoIds: string[]) => {
        try {
            await api.post(`/admin/assets/${id}/photos/${type}/reorder`, { photoIds });
            console.log(`Order saved for ${type} photos`);
        } catch (err) {
            console.error('Failed to reorder photos:', err);
        }
    };

    const handleDragEnd = (event: DragEndEvent, type: 'exterior' | 'interior') => {
        const { active, over } = event;
        console.log('DragEnd');
        console.log('active', active.id);
        console.log('over', over?.id);
        
        if (active.id !== over?.id) {
            const oldIndex = type === 'exterior' 
                ? exteriorPhotos.findIndex((p) => p.id === active.id)
                : interiorPhotos.findIndex((p) => p.id === active.id);
            const newIndex = type === 'exterior'
                ? exteriorPhotos.findIndex((p) => p.id === over?.id)
                : interiorPhotos.findIndex((p) => p.id === over?.id);
            
            let newOrder: { id: string; url: string }[];
                
            if (type === 'exterior') {
                newOrder = arrayMove(exteriorPhotos, oldIndex, newIndex);
                setExteriorPhotos(newOrder);
            } else {
                newOrder = arrayMove(interiorPhotos, oldIndex, newIndex);
                setInteriorPhotos(newOrder);
            }

            const photoIds = newOrder.map(p => p.id);
            reorderPhotos(type, photoIds);
        }
    };


    const handleSubmit = async (e: React.SubmitEvent) => {
        e.preventDefault();
        setSaving(true);
        
        try {
            console.log('=== Submitting form data ===');
            console.log('isEditMode:', isEditMode);
            console.log('formData:', formData);

            if (isEditMode && id) {
                console.log('Updating asset:', id);
                await adminAssetsApi.update(id, formData);
                alert(t("admin.assetForm.updated"));
            } 
            else {
                console.log('Creating new asset with data:', formData);
                const res = await adminAssetsApi.create(formData);
                console.log('Created asset ID:', res.data.id);
                console.log('Created asset:', res.data);
                alert(t("admin.assetForm.created"));
                // Перенаправляем на редактирование нового авто, чтобы можно было добавить фото
                navigate(`/admin/assets/${res.data.id}/edit`);
                return;
            }
            navigate('/admin/assets');
        } 
        catch (err: any) 
        {
            console.error('=== ERROR ===');
            console.error('Error response:', err.response);
            console.error('Error data:', err.response?.data);
            console.error('Error status:', err.response?.status);
            alert(isEditMode ? t("admin.assetForm.updateError") : t("admin.assetForm.createError"));
        } 
        finally 
        {
            setSaving(false);
        }
    };
    

    if (loading) {
        return (
            <div className="min-h-screen bg-gray-50 dark:bg-gray-900 pt-24 pb-16">
                <div className="container mx-auto px-4 flex justify-center">
                    <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
                </div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gray-50 dark:bg-gray-900 pt-24 pb-16">
            <div className="container mx-auto px-4 max-w-4xl">
                {/* Заголовок */}
                <div className="flex items-center gap-4 mb-6">
                    <button
                        onClick={() => navigate('/admin/assets')}
                        className="p-2 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-lg transition-colors"
                    >
                        <ArrowLeft className="w-5 h-5" />
                    </button>
                    <h1 className="text-2xl font-bold text-gray-900 dark:text-white">
                        {isEditMode ? t('admin.assetForm.editTitle') : t('admin.assetForm.newTitle')}
                    </h1>
                </div>

                <form onSubmit={handleSubmit} className="space-y-6">
                {/* Основная информация */}
                <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm p-6">
                    <h2 className="text-lg font-semibold mb-4 text-gray-900 dark:text-white">
                    {t("admin.assetForm.general")}
                    </h2>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                                {t("admin.assetForm.brandRequired")}
                            </label>
                            <input
                                type="text"
                                name="brandName"
                                value={formData.brandName || ''}
                                onChange={handleChange}
                                required
                                className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                                {t("admin.assetForm.modelRequired")}
                            </label>
                            <input
                                type="text"
                                name="model"
                                value={formData.model || ''}
                                onChange={handleChange}
                                required
                                className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                                {t("admin.assetForm.yearRequired")}
                            </label>
                            <input
                                type="number"
                                name="year"
                                value={formData.year || ''}
                                onChange={handleChange}
                                required
                                min="1900"
                                max={new Date().getFullYear() + 1}
                                className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                                {t("admin.assetForm.dailyPriceRequired")}
                            </label>
                            <input
                                type="number"
                                name="dailyPrice"
                                value={formData.dailyPrice || ''}
                                onChange={handleChange}
                                required
                                min="0"
                                step="100"
                                className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                                {t("admin.assetForm.deposit")}
                            </label>
                            <input 
                                type="number" 
                                name="deposit" 
                                value={formData.deposit || ''} 
                                onChange={handleChange} 
                                className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"/>
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                                {t("admin.assetForm.colorRequired")}
                            </label>
                            <input 
                                type="text" 
                                name="color" 
                                value={formData.color || ''} 
                                onChange={handleChange} 
                                className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"/>
                        </div>
                    </div>
                </div>
                {/* Тип и категория */}
                <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm p-6">
                    <h2 className="text-lg font-semibold mb-4 text-gray-900 dark:text-white">
                        {t("admin.assetForm.typeAndCategory")}
                    </h2>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                                {t("admin.assetForm.vehicleType")}
                            </label>
                            <select
                                value={formData.type || AssetType.Auto} 
                                onChange={handleChange} 
                                className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
                                {Object.entries(AssetTypeLabels).map(([value]) => (
                                    <option key={value} value={Number(value)}>{t(`asset.type.${value}`)}</option>
                                ))}
                            </select>
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                                {t("asset.category")}
                            </label>
                            <select
                                value={formData.category || AssetCategory.Crossover} 
                                onChange={handleChange} 
                                className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
                                {Object.entries(AssetCategoryLabels).map(([value]) => (
                                    <option key={value} value={value}>{t(`asset.category.${value}`)}</option>
                                ))}
                            </select>
                        </div>
                    </div>
                </div>
                {/* Технические характеристики */}
                <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm p-6">
                    <h2 className="text-lg font-semibold mb-4 text-gray-900 dark:text-white">
                        {t("asset.characteristics")}
                    </h2>
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                                {t("asset.fuel")}
                            </label>
                            <select 
                                name="fuelType" 
                                value={formData.fuelType || FuelType.Petrol} 
                                onChange={handleChange} 
                                className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
                                {Object.entries(FuelTypeLabels).map(([value]) => (
                                    <option key={value} value={Number(value)}>{t(`asset.fuelType.${value}`)}</option>
                                ))}
                            </select>
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                                {t("asset.transmission")}
                            </label>
                            <select 
                                name="transmission" 
                                value={formData.transmission || TransmissionType.Automatic} 
                                onChange={handleChange} 
                                className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
                                {Object.entries(TransmissionTypeLabels).map(([value]) => (
                                    <option key={value} value={Number(value)}>{t(`asset.transmissionType.${value}`)}</option>
                                ))}
                            </select>
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                                {t("asset.driveType")}
                            </label>
                            <select 
                                name="driveType" 
                                value={formData.driveType || DriveType.FrontWheelDrive} 
                                onChange={handleChange} 
                                className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
                                {Object.entries(DriveTypeLabels).map(([value]) => (
                                    <option key={value} value={Number(value)}>{t(`asset.driveTypeOption.${value}`)}</option>
                                ))}
                            </select>
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                                {t("admin.assetForm.horsepower")}
                            </label>
                            <input 
                                type="number" 
                                name="horsepower" 
                                value={formData.horsepower || ''} 
                                onChange={handleChange} 
                                className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"/>
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                                {t("asset.seats")}
                            </label>
                            <input 
                                type="number" 
                                name="seats" 
                                value={formData.seats || ''} 
                                onChange={handleChange} 
                                className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"/>
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                                {t("asset.doors")}
                            </label>
                            <input 
                                type="number" 
                                name="doors" 
                                value={formData.doors || ''} 
                                onChange={handleChange} 
                                className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"/>
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                                {t("asset.engine")}
                            </label>
                            <input 
                                type="text" 
                                name="engine" 
                                value={formData.engine || ''} 
                                onChange={handleChange} 
                                className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"/>
                        </div>
                        <div> 
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                                {t("asset.acceleration")}
                            </label>
                            <input 
                                type="text" 
                                name="acceleration" 
                                value={formData.acceleration || ''} 
                                onChange={handleChange} 
                                className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"/>
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                                {t("asset.topSpeed")}
                            </label>
                            <input 
                                type="number" 
                                name="topSpeed" 
                                value={formData.topSpeed || ''} 
                                onChange={handleChange} 
                                className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"/>
                        </div>
                    </div>
                </div>
                {/* Описания */}
                <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm p-6">
                    <h2 className="text-lg font-semibold mb-4 text-gray-900 dark:text-white">
                        {t("admin.assetForm.descriptions")}
                    </h2>
                    <div className="space-y-4">
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                                {t("admin.assetForm.shortDescription")}
                            </label>
                            <textarea 
                                name="shortDescription" 
                                value={formData.shortDescription || ''} 
                                onChange={handleChange} 
                                rows={2} 
                                className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"/>
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                                {t("admin.assetForm.fullDescription")}
                            </label>
                            <textarea 
                                name="fullDescription" 
                                value={formData.fullDescription || ''} 
                                onChange={handleChange} 
                                rows={4} 
                                className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"/>
                        </div>
                    </div>
                </div>
                {/* Особенности */}
                <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm p-6">
                    <h2 className="text-lg font-semibold mb-4 text-gray-900 dark:text-white">
                        {t("admin.assetForm.features")}
                    </h2>
                    <div className="flex gap-2 mb-4">
                        <input 
                            type="text" 
                            value={newFeature} 
                            onChange={(e) => setNewFeature(e.target.value)} 
                            placeholder={t("admin.assetForm.addFeature")} 
                            className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"/>
                        <button 
                            type="button" 
                            onClick={addFeature} 
                            className="px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700">
                            <Plus className="w-4 h-4" />
                        </button>
                    </div>
                    <div className="flex flex-wrap gap-2">
                    {formData.features?.map((feature, index) => (
                        <span key={index} className="px-3 py-1 bg-gray-100 dark:bg-gray-700 rounded-full text-sm flex items-center gap-2">
                        {feature}
                        <button type="button" onClick={() => removeFeature(index)} className="text-red-500 hover:text-red-700">
                            <X className="w-3 h-3" />
                        </button>
                        </span>
                    ))}
                    </div>
                </div>

                {/* Фото экстерьера для 360-обзора */}
                {isEditMode && (
                    <>
                        <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm p-6">
                            <h2 className="text-lg font-semibold mb-4 text-gray-900 dark:text-white">
                                📸 Экстерьер (360° обзор) - {exteriorPhotos.length}/36 фото
                            </h2>
                            <div className="mb-4">
                                <label className="flex items-center justify-center w-full h-32 border-2 border-dashed border-gray-300 dark:border-gray-600 rounded-lg cursor-pointer hover:border-primary-500 transition-colors">
                                    <div className="text-center">
                                        <Upload className="w-8 h-8 text-gray-400 mx-auto mb-2" />
                                        <span className="text-sm text-gray-500">
                                            {uploadingExterior ? t('common.loading') : t('admin.assetForm.uploadExterior')}
                                        </span>
                                    </div>
                                    <input
                                        type="file"
                                        accept="image/*"
                                        multiple
                                        onChange={handleExteriorUpload}
                                        disabled={uploadingExterior}
                                        className="hidden"
                                    />
                                </label>
                                <p className="text-xs text-gray-500 mt-2">
                                    {t("admin.assetForm.photoRecommendation")}
                                </p>
                            </div>
                            <DndContext
                                sensors={sensors}
                                collisionDetection={closestCenter}
                                onDragEnd={(event) => handleDragEnd(event, 'exterior')}
                            >
                                <SortableContext
                                    items={exteriorPhotos.map(p => p.id)}
                                    strategy={verticalListSortingStrategy}
                                >
                                    <div className="grid grid-cols-4 sm:grid-cols-6 md:grid-cols-8 gap-2">
                                        {exteriorPhotos.map((photo, idx) => (
                                            <SortablePhotoItem 
                                                key={photo.id}
                                                photo={photo}
                                                index={idx}
                                                onDelete={deleteExteriorPhoto}
                                                type={t('asset.exterior')}
                                                getFullImageUrl={getPhotoUrl}
                                            />
                                        ))}
                                    </div>
                                </SortableContext>
                            </DndContext>
                        </div>

                        {/* Фото интерьера для 360-обзора */}
                        <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm p-6">
                            <h2 className="text-lg font-semibold mb-4 text-gray-900 dark:text-white">
                                🪑 Интерьер (360° обзор) - {interiorPhotos.length}/36 фото
                            </h2>
                            <div className="mb-4">
                                <label className="flex items-center justify-center w-full h-32 border-2 border-dashed border-gray-300 dark:border-gray-600 rounded-lg cursor-pointer hover:border-primary-500 transition-colors">
                                    <div className="text-center">
                                        <Upload className="w-8 h-8 text-gray-400 mx-auto mb-2" />
                                        <span className="text-sm text-gray-500">
                                            {uploadingInterior ? t('common.loading') : t('admin.assetForm.uploadInterior')}
                                        </span>
                                    </div>
                                    <input
                                        type="file"
                                        accept="image/*"
                                        multiple
                                        onChange={handleInteriorUpload}
                                        disabled={uploadingInterior}
                                        className="hidden"
                                    />
                                </label>
                            </div>
                            <DndContext
                                sensors={sensors}
                                collisionDetection={closestCenter}
                                onDragEnd={(event) => handleDragEnd(event, 'interior')}
                            >
                                <SortableContext
                                    items={interiorPhotos.map(p => p.id)}
                                    strategy={verticalListSortingStrategy}
                                >
                                    <div className="grid grid-cols-4 sm:grid-cols-6 md:grid-cols-8 gap-2">
                                        {interiorPhotos.map((photo, idx) => (
                                            <SortablePhotoItem
                                                key={photo.id}
                                                photo={photo}
                                                index={idx}
                                                onDelete={deleteInteriorPhoto}
                                                type={t("asset.interior")}
                                                getFullImageUrl={getPhotoUrl}
                                            />
                                        ))}
                                    </div>
                                </SortableContext>
                            </DndContext>
                        </div>
                    </>
                )}

                {/* Кнопки */}
                <div className="flex gap-4">
                    <button
                    type="submit"
                    disabled={saving}
                    className="flex-1 bg-primary-600 text-white px-6 py-3 rounded-lg font-semibold
                            hover:bg-primary-700 transition-colors disabled:opacity-50
                            flex items-center justify-center gap-2"
                    >
                    <Save className="w-4 h-4" />
                    {saving ? t('common.saving') : (isEditMode ? t('common.saveChanges') : t('admin.assetForm.createAsset'))}
                    </button>
                    <button
                    type="button"
                    onClick={() => navigate('/admin/assets')}
                    className="px-6 py-3 border border-gray-300 dark:border-gray-600 rounded-lg
                            text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700
                            transition-colors"
                    >
                    {t("common.cancel")}
                    </button>
                </div>
                </form>
            </div>
        </div>
    );
};