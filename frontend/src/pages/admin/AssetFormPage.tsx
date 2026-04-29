import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { adminAssetsApi } from '@/shared/api/adminAssetApi';
import { Asset, AssetCategory, AssetCategoryLabels, AssetStatus, AssetType, AssetTypeLabels, DriveType, DriveTypeLabels, FuelType, FuelTypeLabels, TransmissionType, TransmissionTypeLabels } from '@/entities/asset/asset.types';
import { ArrowLeft, Save, X, Plus, Upload, Trash2, GripVertical } from 'lucide-react';
import api from '@/shared/api/axios';
import { DndContext, closestCenter, KeyboardSensor, PointerSensor, useSensor, useSensors, DragEndEvent, } from '@dnd-kit/core';
import { arrayMove, SortableContext, sortableKeyboardCoordinates, verticalListSortingStrategy, useSortable, } from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';

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
            {...listeners}
        >
            <img 
                src={getFullImageUrl(photo.url)} 
                alt={`${type} ${index + 1}`} 
                className="w-full h-20 object-cover rounded-lg" 
            />
            <button
                type="button"
                onClick={() => onDelete(photo.id)}
                className="absolute top-1 right-1 p-1 bg-red-600 text-white rounded-full opacity-0 group-hover:opacity-100 transition-opacity z-10"
            >
                <Trash2 className="w-3 h-3" />
            </button>
            <div className="absolute bottom-1 left-1 bg-black/60 text-white text-xs px-1 rounded">
                {index + 1}
            </div>
            <div className="absolute top-1 left-1 bg-black/50 text-white p-1 rounded opacity-0 group-hover:opacity-100 transition-opacity">
                <GripVertical className="w-3 h-3" />
            </div>
        </div>
    );
};

export const AssetFormPage = () => {
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
        canDeliver: false,
        deliveryPrice: 0,
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
            console.log('Fetching asset with id:', id);
            const res = await adminAssetsApi.getById(id!);
            console.log('Fetched asset response:', res.data);
            setFormData({
                brandName: res.data.brandName,
                model: res.data.model,
                year: res.data.year,
                dailyPrice: res.data.dailyPrice,
                fuelType: res.data.fuelType as FuelType,
                transmission: res.data.transmission as TransmissionType,
                driveType: res.data.driveType as DriveType,
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
                canDeliver: res.data.canDeliver || false,
                deliveryPrice: res.data.deliveryPrice || 0
            });

            const photos = res.data.photos || [];
            const exterior = photos.filter(p => p.type == 'Exterior').sort((a, b) => a.order - b.order).map(p => ({ id: p.id, url: p.url }));
            const interior = photos.filter(p => p.type === 'Interior').sort((a, b) => a.order - b.order).map(p => ({ id: p.id, url: p.url }));
            
            setExteriorPhotos(exterior);
            setInteriorPhotos(interior);
            
            console.log('Exterior photos:', exterior);
            console.log('Interior photos:', interior);
        } catch (err) {
            console.error(err);
            alert('Ошибка загрузки данных автомобиля');
            navigate('/admin/assets');
        } finally {
            setLoading(false);
        }
    };

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
        const { name, value, type } = e.target;
        setFormData(prev => ({
        ...prev,
        [name]: type === 'number' ? parseFloat(value) : value
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
            alert(`Загружено ${urls.length} фото экстерьера`);
        } catch (err) {
            console.error(err);
            alert('Ошибка загрузки фото экстерьера');
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
            alert(`Загружено ${urls.length} фото интерьера`);
        } catch (err) {
            console.error(err);
            alert('Ошибка загрузки фото интерьера');
        } finally {
            setUploadingInterior(false);
            e.target.value = '';
        }
    };

    // Удаление фото экстерьера
    const deleteExteriorPhoto = async (photoId: string) => {
        if (!confirm('Удалить это фото?')) return;
        try {
            await api.delete(`/admin/assets/${id}/photos/exterior/${photoId}`);
            setExteriorPhotos(prev => prev.filter(p => p.id !== photoId));
            alert('Фото удалено');
        } catch (err) {
            console.error(err);
            alert('Ошибка удаления фото');
        }
    };

    const getFullImageUrl = (url: string) => {
        if (!url) return '';
        if (url.startsWith('http')) return url;
        return `http://localhost:9000/rentflow-photos${url}`;
    };

    // Удаление фото интерьера
    const deleteInteriorPhoto = async (photoId: string) => {
        if (!confirm('Удалить это фото?')) return;
        try {
            await api.delete(`/admin/assets/${id}/photos/interior/${photoId}`);
            setInteriorPhotos(prev => prev.filter(p => p.id !== photoId));
            alert('Фото удалено');
        } catch (err) {
            console.error(err);
            alert('Ошибка удаления фото');
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
                alert('Автомобиль обновлен');
            } 
            else {
                console.log('Creating new asset with data:', formData);
                const res = await adminAssetsApi.create(formData);
                console.log('Created asset ID:', res.data.id);
                console.log('Created asset:', res.data);
                alert('Автомобиль создан');
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
            alert(isEditMode ? 'Ошибка обновления' : 'Ошибка создания');
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
                        {isEditMode ? 'Редактирование автомобиля' : 'Новый автомобиль'}
                    </h1>
                </div>

                <form onSubmit={handleSubmit} className="space-y-6">
                {/* Основная информация */}
                <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm p-6">
                    <h2 className="text-lg font-semibold mb-4 text-gray-900 dark:text-white">
                    Основная информация
                    </h2>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                            Марка *
                            </label>
                            <input
                            type="text"
                            name="brandName"
                            value={formData.brandName || ''}
                            onChange={handleChange}
                            required
                            className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg
                                    bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                            Модель *
                            </label>
                            <input
                            type="text"
                            name="model"
                            value={formData.model || ''}
                            onChange={handleChange}
                            required
                            className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg
                                    bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                            Год выпуска *
                            </label>
                            <input
                            type="number"
                            name="year"
                            value={formData.year || ''}
                            onChange={handleChange}
                            required
                            min="1900"
                            max={new Date().getFullYear() + 1}
                            className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg
                                    bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                            Цена за день (₽) *
                            </label>
                            <input
                            type="number"
                            name="dailyPrice"
                            value={formData.dailyPrice || ''}
                            onChange={handleChange}
                            required
                            min="0"
                            step="100"
                            className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg
                                    bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Залог (₽)</label>
                            <input type="number" name="deposit" value={formData.deposit || ''} onChange={handleChange} className="w-full px-3 py-2 border rounded-lg"/>
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Цвет</label>
                            <input type="text" name="color" value={formData.color || ''} onChange={handleChange} className="w-full px-3 py-2 border rounded-lg"/>
                        </div>
                    </div>
                </div>
                {/* Тип и категория */}
                <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm p-6">
                    <h2 className="text-lg font-semibold mb-4 text-gray-900 dark:text-white">Тип и категория</h2>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                        <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Тип ТС</label>
                        <select name="type" value={formData.type || AssetType.Auto} onChange={handleChange} className="w-full px-3 py-2 border rounded-lg">
                        {Object.entries(AssetTypeLabels).map(([value, label]) => (
                            <option key={value} value={Number(value)}>{label}</option>
                        ))}
                        </select>
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Категория</label>
                        <select name="category" value={formData.category || AssetCategory.Crossover} onChange={handleChange} className="w-full px-3 py-2 border rounded-lg">
                        {Object.entries(AssetCategoryLabels).map(([value, label]) => (
                            <option key={value} value={Number(value)}>{label}</option>
                        ))}
                        </select>
                    </div>
                    </div>
                </div>
                {/* Технические характеристики */}
                <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm p-6">
                    <h2 className="text-lg font-semibold mb-4 text-gray-900 dark:text-white">Технические характеристики</h2>
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                    <div>
                        <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Топливо</label>
                        <select name="fuelType" value={formData.fuelType || FuelType.Petrol} onChange={handleChange} className="w-full px-3 py-2 border rounded-lg">
                        {Object.entries(FuelTypeLabels).map(([value, label]) => (
                            <option key={value} value={Number(value)}>{label}</option>
                        ))}
                        </select>
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">КПП</label>
                        <select name="transmission" value={formData.transmission || TransmissionType.Automatic} onChange={handleChange} className="w-full px-3 py-2 border rounded-lg">
                            {Object.entries(TransmissionTypeLabels).map(([value, label]) => (
                                <option key={value} value={Number(value)}>{label}</option>
                            ))}
                        </select>
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Привод</label>
                        <select name="driveType" value={formData.driveType || DriveType.FrontWheelDrive} onChange={handleChange} className="w-full px-3 py-2 border rounded-lg">
                        {Object.entries(DriveTypeLabels).map(([value, label]) => (
                            <option key={value} value={Number(value)}>{label}</option>
                        ))}
                        </select>
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Мощность (л.с.)</label>
                        <input type="number" name="horsepower" value={formData.horsepower || ''} onChange={handleChange} className="w-full px-3 py-2 border rounded-lg"/>
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Кол-во мест</label>
                        <input type="number" name="seats" value={formData.seats || ''} onChange={handleChange} className="w-full px-3 py-2 border rounded-lg"/>
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Кол-во дверей</label>
                        <input type="number" name="doors" value={formData.doors || ''} onChange={handleChange} className="w-full px-3 py-2 border rounded-lg"/>
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Двигатель</label>
                        <input type="text" name="engine" value={formData.engine || ''} onChange={handleChange} className="w-full px-3 py-2 border rounded-lg"/>
                    </div>
                    <div> 
                        <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Разгон (сек)</label>
                        <input type="text" name="acceleration" value={formData.acceleration || ''} onChange={handleChange} className="w-full px-3 py-2 border rounded-lg"/>
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Макс. скорость (км/ч)</label>
                        <input type="number" name="topSpeed" value={formData.topSpeed || ''} onChange={handleChange} className="w-full px-3 py-2 border rounded-lg"/>
                    </div>
                    </div>
                </div>
                {/* Описания */}
                <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm p-6">
                    <h2 className="text-lg font-semibold mb-4 text-gray-900 dark:text-white">Описания</h2>
                    <div className="space-y-4">
                    <div>
                        <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Краткое описание</label>
                        <textarea name="shortDescription" value={formData.shortDescription || ''} onChange={handleChange} rows={2} className="w-full px-3 py-2 border rounded-lg"/>
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Полное описание</label>
                        <textarea name="fullDescription" value={formData.fullDescription || ''} onChange={handleChange} rows={4} className="w-full px-3 py-2 border rounded-lg"/>
                    </div>
                    </div>
                </div>
                {/* Особенности */}
                <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm p-6">
                    <h2 className="text-lg font-semibold mb-4 text-gray-900 dark:text-white">Особенности</h2>
                    <div className="flex gap-2 mb-4">
                    <input type="text" value={newFeature} onChange={(e) => setNewFeature(e.target.value)} placeholder="Добавить особенность" className="flex-1 px-3 py-2 border rounded-lg"/>
                    <button type="button" onClick={addFeature} className="px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700">
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
                {/* Доставка */}
                <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm p-6">
                    <h2 className="text-lg font-semibold mb-4 text-gray-900 dark:text-white">Доставка</h2>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <label className="flex items-center gap-3">
                        <input type="checkbox" name="canDeliver" checked={formData.canDeliver || false} onChange={handleCheckboxChange} className="w-4 h-4"/>
                        <span>Возможна доставка</span>
                    </label>
                    {formData.canDeliver && (
                        <div>
                        <label className="block text-sm font-medium mb-1">Цена доставки (₽)</label>
                        <input type="number" name="deliveryPrice" value={formData.deliveryPrice || ''} onChange={handleChange} className="w-full px-3 py-2 border rounded-lg"/>
                        </div>
                    )}
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
                                            {uploadingExterior ? 'Загрузка...' : 'Загрузить фото экстерьера (до 36 шт)'}
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
                                    Рекомендуется: 36 фото, 1920x1080, JPG/PNG. Фото будут автоматически упорядочены по имени.
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
                                                type='Экстерьер'
                                                getFullImageUrl={getFullImageUrl}
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
                                            {uploadingInterior ? 'Загрузка...' : 'Загрузить фото интерьера (до 36 шт)'}
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
                            <div className="grid grid-cols-4 sm:grid-cols-6 md:grid-cols-8 gap-2">
                                {interiorPhotos.map((photo, idx) => (
                                    <div key={photo.id} className="relative group">
                                        <img src={photo.url} alt={`Интерьер ${idx + 1}`} className="w-full h-20 object-cover rounded-lg" />
                                        <button
                                            type="button"
                                            onClick={() => deleteInteriorPhoto(photo.id)}
                                            className="absolute top-1 right-1 p-1 bg-red-600 text-white rounded-full opacity-0 group-hover:opacity-100 transition-opacity"
                                        >
                                            <Trash2 className="w-3 h-3" />
                                        </button>
                                        <span className="absolute bottom-1 left-1 bg-black/60 text-white text-xs px-1 rounded">
                                            {idx + 1}
                                        </span>
                                    </div>
                                ))}
                            </div>
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
                    {saving ? 'Сохранение...' : (isEditMode ? 'Сохранить изменения' : 'Создать автомобиль')}
                    </button>
                    <button
                    type="button"
                    onClick={() => navigate('/admin/assets')}
                    className="px-6 py-3 border border-gray-300 dark:border-gray-600 rounded-lg
                            text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700
                            transition-colors"
                    >
                    Отмена
                    </button>
                </div>
                </form>
            </div>
        </div>
    );
};