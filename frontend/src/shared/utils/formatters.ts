export const formatPrice = (price: number, currency: string = '₽'): string => {
    return `${price.toLocaleString('ru-RU')} ${currency}`;
};

export const formatDate = (date: Date): string => {
    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const year = date.getFullYear();
    return `${day}.${month}.${year}`;
};