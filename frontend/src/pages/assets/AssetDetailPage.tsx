import { useParams, useNavigate } from 'react-router-dom';
import { useState } from 'react';
import { useAsset } from '../../entities/asset/asset.hooks';
import { useBookingStore } from '../../features/booking/stores/bookingStore';
import { useAuthStore } from '../../features/auth/stores/authStore';
import { motion, AnimatePresence } from 'framer-motion';
import { Calendar, Fuel, Settings, Users, DoorOpen, Gauge, Sparkles, Shield, Star, MapPin, ChevronLeft, ChevronRight, CheckCircle, Zap, Car } from 'lucide-react';
import { DateRangePicker } from '../../features/booking/components/DateRangePicker';
import { formatPrice, formatDate } from '../../shared/utils/formatters';
import { calculateTotalPrice, calculateDays, isValidDateRange } from '../../features/booking/utils/priceCalculator';
import { getFuelTypeLabel, getTransmissionLabel, getDriveTypeLabel, getStatusLabel } from '../../entities/asset/asset.utils';

export const AssetDetailPage = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const { data: asset, isLoading, error } = useAsset(id!);
    const { setBookingDetails } = useBookingStore();
    const { isAuthenticated } = useAuthStore();
    
    const [selectedImage, setSelectedImage] = useState(0);
    const [dateRange, setDateRange] = useState({ start: null as Date | null, end: null as Date | null });
    const [showBookingModal, setShowBookingModal] = useState(false);
    
    // Используем реальные фото из API
    const images = asset?.photos && asset.photos.length > 0 
        ? asset.photos 
        : ['/car-placeholder.jpg', '/car-placeholder-2.jpg', '/car-placeholder-3.jpg'];
    
    const handleBookNow = () => {
        if (!isAuthenticated) {
            // Открыть модалку авторизации или перенаправить
            navigate('/login', { state: { from: `/assets/${id}` } });
            return;
        }
        
        if (dateRange.start && dateRange.end && isValidDateRange(dateRange.start, dateRange.end)) {
            const totalPrice = calculateTotalPrice(
                asset!.dailyPrice,
                dateRange.start,
                dateRange.end
            );
            
            setBookingDetails({
                asset,
                startDate: dateRange.start,
                endDate: dateRange.end,
                totalPrice: totalPrice,
            });
            navigate('/checkout');
        } else {
            setShowBookingModal(true);
        }
    };
    
    const calculateTotalPriceForDisplay = () => {
        if (!asset || !dateRange.start || !dateRange.end) return 0;
        return calculateTotalPrice(asset.dailyPrice, dateRange.start, dateRange.end);
    };
    
    const daysCount = dateRange.start && dateRange.end 
        ? calculateDays(dateRange.start, dateRange.end)
        : 0;
    
    if (isLoading) {
        return (
            <div className="min-h-screen flex items-center justify-center pt-20">
                <div className="text-center">
                    <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600 mx-auto"></div>
                    <p className="mt-4 text-gray-600 dark:text-gray-400">Loading vehicle details...</p>
                </div>
            </div>
        );
    }
    
    if (error || !asset) {
        return (
            <div className="min-h-screen flex items-center justify-center pt-20">
                <div className="text-center">
                    <p className="text-red-600 text-lg">Vehicle not found</p>
                    <button
                        onClick={() => navigate('/assets')}
                        className="mt-4 px-6 py-2 bg-primary-600 text-white rounded-lg 
                                   hover:bg-primary-700 transition-colors"
                    >
                        Back to Vehicles
                    </button>
                </div>
            </div>
        );
    }
    
    return (
        <div className="min-h-screen bg-gray-50 dark:bg-gray-900 pt-20">
            <div className="container mx-auto px-4 py-8">
                {/* Back Button */}
                <button
                    onClick={() => navigate('/assets')}
                    className="flex items-center space-x-2 text-gray-600 dark:text-gray-400 
                               hover:text-primary-600 mb-6 transition-colors"
                >
                    <ChevronLeft className="w-5 h-5" />
                    <span>Back to all vehicles</span>
                </button>
                
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
                    {/* Left Column - Gallery */}
                    <div>
                        <motion.div
                            initial={{ opacity: 0, x: -20 }}
                            animate={{ opacity: 1, x: 0 }}
                            className="relative rounded-2xl overflow-hidden bg-gray-100 dark:bg-gray-800"
                        >
                            <img
                                src={images[selectedImage]}
                                alt={`${asset.brandName} ${asset.model}`}
                                className="w-full h-[400px] object-cover"
                                onError={(e) => {
                                    e.currentTarget.src = '/car-placeholder.jpg';
                                }}
                            />
                            
                            {/* Navigation Buttons */}
                            {images.length > 1 && (
                                <>
                                    <button
                                        onClick={() => setSelectedImage(prev => 
                                            prev === 0 ? images.length - 1 : prev - 1
                                        )}
                                        className="absolute left-4 top-1/2 transform -translate-y-1/2
                                                   p-2 bg-white/90 dark:bg-gray-800/90 rounded-full
                                                   hover:bg-white dark:hover:bg-gray-800 transition-colors"
                                    >
                                        <ChevronLeft className="w-5 h-5" />
                                    </button>
                                    <button
                                        onClick={() => setSelectedImage(prev => 
                                            prev === images.length - 1 ? 0 : prev + 1
                                        )}
                                        className="absolute right-4 top-1/2 transform -translate-y-1/2
                                                   p-2 bg-white/90 dark:bg-gray-800/90 rounded-full
                                                   hover:bg-white dark:hover:bg-gray-800 transition-colors"
                                    >
                                        <ChevronRight className="w-5 h-5" />
                                    </button>
                                </>
                            )}
                        </motion.div>
                        
                        {/* Thumbnails */}
                        {images.length > 1 && (
                            <div className="flex gap-2 mt-4 overflow-x-auto pb-2">
                                {images.map((img, idx) => (
                                    <button
                                        key={idx}
                                        onClick={() => setSelectedImage(idx)}
                                        className={`w-20 h-20 rounded-lg overflow-hidden border-2 
                                                   transition-all duration-200 flex-shrink-0
                                                   ${selectedImage === idx 
                                                       ? 'border-primary-600' 
                                                       : 'border-transparent opacity-60 hover:opacity-100'}`}
                                    >
                                        <img 
                                            src={img} 
                                            alt="" 
                                            className="w-full h-full object-cover"
                                            onError={(e) => {
                                                e.currentTarget.src = '/car-placeholder.jpg';
                                            }}
                                        />
                                    </button>
                                ))}
                            </div>
                        )}
                    </div>
                    
                    {/* Right Column - Details */}
                    <motion.div
                        initial={{ opacity: 0, x: 20 }}
                        animate={{ opacity: 1, x: 0 }}
                        className="space-y-6"
                    >
                        {/* Title and Rating */}
                        <div>
                            <h1 className="text-3xl md:text-4xl font-bold text-gray-900 dark:text-white">
                                {asset.brandName} {asset.model}
                            </h1>
                            <div className="flex items-center justify-between mt-2">
                                <div className="flex items-center space-x-2">
                                    <div className="flex items-center">
                                        <Star className="w-5 h-5 text-yellow-400 fill-current" />
                                        <span className="ml-1 text-gray-600 dark:text-gray-300">4.9</span>
                                    </div>
                                    <span className="text-gray-400">•</span>
                                    <span className="text-gray-600 dark:text-gray-300">156 reviews</span>
                                </div>
                                <div className="flex items-center text-gray-600 dark:text-gray-300">
                                    <MapPin className="w-4 h-4 mr-1" />
                                    <span className="text-sm">{asset.locationName || 'Unknown Location'}</span>
                                </div>
                            </div>
                        </div>
                        
                        {/* Price */}
                        <div className="flex items-baseline space-x-2">
                            <span className="text-3xl font-bold text-primary-600">
                                {formatPrice(asset.dailyPrice)}
                            </span>
                            <span className="text-gray-600 dark:text-gray-400">/ day</span>
                        </div>
                        
                        {/* Key Specifications */}
                        <div className="grid grid-cols-2 gap-4 py-4 border-y border-gray-200 dark:border-gray-700">
                            <div className="flex items-center space-x-3">
                                <Gauge className="w-5 h-5 text-primary-600" />
                                <div>
                                    <p className="text-sm text-gray-500">Engine</p>
                                    <p className="font-semibold">{asset.engine}</p>
                                </div>
                            </div>
                            <div className="flex items-center space-x-3">
                                <Fuel className="w-5 h-5 text-primary-600" />
                                <div>
                                    <p className="text-sm text-gray-500">Fuel</p>
                                    <p className="font-semibold">{getFuelTypeLabel(asset.fuelType)}</p>
                                </div>
                            </div>
                            <div className="flex items-center space-x-3">
                                <Settings className="w-5 h-5 text-primary-600" />
                                <div>
                                    <p className="text-sm text-gray-500">Transmission</p>
                                    <p className="font-semibold">{getTransmissionLabel(asset.transmission)}</p>
                                </div>
                            </div>
                            <div className="flex items-center space-x-3">
                                <Car className="w-5 h-5 text-primary-600" />
                                <div>
                                    <p className="text-sm text-gray-500">Drive</p>
                                    <p className="font-semibold">{getDriveTypeLabel(asset.driveType)}</p>
                                </div>
                            </div>
                            <div className="flex items-center space-x-3">
                                <Users className="w-5 h-5 text-primary-600" />
                                <div>
                                    <p className="text-sm text-gray-500">Seats</p>
                                    <p className="font-semibold">{asset.seats} passengers</p>
                                </div>
                            </div>
                            <div className="flex items-center space-x-3">
                                <DoorOpen className="w-5 h-5 text-primary-600" />
                                <div>
                                    <p className="text-sm text-gray-500">Doors</p>
                                    <p className="font-semibold">{asset.doors}</p>
                                </div>
                            </div>
                            <div className="flex items-center space-x-3">
                                <Zap className="w-5 h-5 text-primary-600" />
                                <div>
                                    <p className="text-sm text-gray-500">Horsepower</p>
                                    <p className="font-semibold">{asset.horsepower} HP</p>
                                </div>
                            </div>
                            <div className="flex items-center space-x-3">
                                <Gauge className="w-5 h-5 text-primary-600" />
                                <div>
                                    <p className="text-sm text-gray-500">Top Speed</p>
                                    <p className="font-semibold">{asset.topSpeed} km/h</p>
                                </div>
                            </div>
                        </div>
                        
                        {/* Description */}
                        <div>
                            <h3 className="text-lg font-semibold mb-2">Description</h3>
                            <p className="text-gray-600 dark:text-gray-300 leading-relaxed">
                                {asset.fullDescription || asset.shortDescription}
                            </p>
                        </div>
                        
                        {/* Features */}
                        {asset.features && asset.features.length > 0 && (
                            <div>
                                <h3 className="text-lg font-semibold mb-2">Features</h3>
                                <div className="grid grid-cols-1 gap-2">
                                    {asset.features.map((feature, index) => (
                                        <div key={index} className="flex items-center space-x-2">
                                            <CheckCircle className="w-4 h-4 text-green-500" />
                                            <span className="text-sm text-gray-600 dark:text-gray-300">{feature}</span>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        )}
                        
                        {/* Booking Widget */}
                        <div className="bg-white dark:bg-gray-800 rounded-xl shadow-lg p-6 sticky top-24">
                            <h3 className="text-xl font-semibold mb-4">Book this vehicle</h3>
                            
                            <div className="space-y-4">
                                <DateRangePicker
                                    value={dateRange}
                                    onChange={setDateRange}
                                    minDate={new Date()}
                                />
                                
                                {dateRange.start && dateRange.end && isValidDateRange(dateRange.start, dateRange.end) && (
                                    <motion.div
                                        initial={{ opacity: 0, height: 0 }}
                                        animate={{ opacity: 1, height: 'auto' }}
                                        className="space-y-3 pt-4 border-t border-gray-200 dark:border-gray-700"
                                    >
                                        <div className="flex justify-between text-sm">
                                            <span className="text-gray-600 dark:text-gray-400">
                                                Daily rate:
                                            </span>
                                            <span>{formatPrice(asset.dailyPrice)}</span>
                                        </div>
                                        <div className="flex justify-between text-sm">
                                            <span className="text-gray-600 dark:text-gray-400">
                                                Number of days:
                                            </span>
                                            <span>{daysCount} days</span>
                                        </div>
                                        <div className="flex justify-between text-lg font-bold pt-2 border-t border-gray-200 dark:border-gray-700">
                                            <span>Total:</span>
                                            <span className="text-primary-600">
                                                {formatPrice(calculateTotalPriceForDisplay())}
                                            </span>
                                        </div>
                                    </motion.div>
                                )}
                                
                                <button
                                    onClick={handleBookNow}
                                    disabled={!asset.isAvailable}
                                    className={`w-full py-3 rounded-lg font-semibold 
                                               transition-all duration-300 transform hover:scale-[1.02]
                                               ${asset.isAvailable 
                                                   ? 'bg-primary-600 text-white hover:bg-primary-700' 
                                                   : 'bg-gray-300 text-gray-500 cursor-not-allowed'}`}
                                >
                                    {asset.isAvailable ? 'Book Now' : 'Not Available'}
                                </button>
                            </div>
                        </div>
                    </motion.div>
                </div>
                
                {/* Features & Benefits */}
                <div className="mt-12 grid grid-cols-1 md:grid-cols-3 gap-6">
                    <div className="flex items-center space-x-3 p-4 bg-white dark:bg-gray-800 rounded-xl">
                        <Shield className="w-8 h-8 text-primary-600" />
                        <div>
                            <h4 className="font-semibold">Full Insurance</h4>
                            <p className="text-sm text-gray-500">Comprehensive coverage included</p>
                        </div>
                    </div>
                    <div className="flex items-center space-x-3 p-4 bg-white dark:bg-gray-800 rounded-xl">
                        <Sparkles className="w-8 h-8 text-primary-600" />
                        <div>
                            <h4 className="font-semibold">Free Cancellation</h4>
                            <p className="text-sm text-gray-500">Up to 24 hours before pickup</p>
                        </div>
                    </div>
                    <div className="flex items-center space-x-3 p-4 bg-white dark:bg-gray-800 rounded-xl">
                        <CheckCircle className="w-8 h-8 text-primary-600" />
                        <div>
                            <h4 className="font-semibold">No Hidden Fees</h4>
                            <p className="text-sm text-gray-500">Transparent pricing</p>
                        </div>
                    </div>
                </div>
            </div>
            
            {/* Booking Modal for missing dates */}
            <AnimatePresence>
                {showBookingModal && (
                    <motion.div
                        initial={{ opacity: 0 }}
                        animate={{ opacity: 1 }}
                        exit={{ opacity: 0 }}
                        className="fixed inset-0 bg-black/50 flex items-center justify-center z-50"
                        onClick={() => setShowBookingModal(false)}
                    >
                        <motion.div
                            initial={{ scale: 0.9, opacity: 0 }}
                            animate={{ scale: 1, opacity: 1 }}
                            exit={{ scale: 0.9, opacity: 0 }}
                            className="bg-white dark:bg-gray-800 rounded-xl p-6 max-w-md mx-4"
                            onClick={(e) => e.stopPropagation()}
                        >
                            <h3 className="text-xl font-bold mb-4">Select Dates First</h3>
                            <p className="text-gray-600 dark:text-gray-300 mb-4">
                                Please select your rental dates before proceeding with the booking.
                            </p>
                            <button
                                onClick={() => setShowBookingModal(false)}
                                className="w-full py-2 bg-primary-600 text-white rounded-lg 
                                           hover:bg-primary-700 transition-colors"
                            >
                                OK
                            </button>
                        </motion.div>
                    </motion.div>
                )}
            </AnimatePresence>
        </div>
    );
};