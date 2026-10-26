const PHOTO_BASE_URL = import.meta.env.VITE_PHOTO_BASE_URL ?? 'http://localhost:9000/rentflow-photos';
 
/** Resolves a photo URL from DB (full or relative path) to a browser-loadable URL. */
export function getPhotoUrl(url: string | undefined | null): string {
    if (!url) return '';
    if (url.startsWith('http://') || url.startsWith('https://')) return url;
    const path = url.startsWith('/') ? url : `/${url}`;
    return `${PHOTO_BASE_URL.replace(/\/$/, '')}${path}`;
}
