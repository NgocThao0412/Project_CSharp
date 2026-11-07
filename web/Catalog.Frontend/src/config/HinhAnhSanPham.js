import '../product-images.css';
import imageIndex from '../data/product-image-index.json';

const PHOTO_ROOT = '/images/products/';
const normalize = text => String(text ?? '').normalize('NFD')
  .replace(/[\u0300-\u036f]/g, '').toLowerCase().replace(/\s+/g, ' ').trim();
const normalPath = file => `${PHOTO_ROOT}${file}`;
const sqlEntries = imageIndex.sqlProducts.map(item => ({...item, key: normalize(item.name)}));
const demoEntries = imageIndex.demoProducts.map(item => ({...item, key: normalize(item.name)}));

// Chỉ dùng ảnh theo tên và mã phù hợp; ID 3 của SQL khác ID 3 của dữ liệu demo.
export function getProductImage(product, fallbackUrl) {
  const originalUrl = typeof product?.imageUrl === 'string' ? product.imageUrl.trim() : '';
  if (/^[a-zA-Z0-9_.-]+\.(png|jpg|jpeg|webp|svg)$/i.test(originalUrl)
      && originalUrl.toLowerCase() !== 'tmp.png') {
    return `${PHOTO_ROOT}${originalUrl}`;
  }
  if (originalUrl && (originalUrl.startsWith('/') && !originalUrl.startsWith('//') || /^https?:\/\//i.test(originalUrl))) {
    return originalUrl;
  }

  const productId = Number(product?.id);
  const key = normalize(product?.name);
  const entry = sqlEntries.find(item => item.id === productId && item.key === key)
    ?? demoEntries.find(item => item.id === productId && item.key === key)
    ?? sqlEntries.find(item => item.key === key)
    ?? demoEntries.find(item => item.key === key);
  if (entry) return normalPath(entry.file);

  // Preserve Ny's category-based storefront images when no exact photo exists.
  if (typeof fallbackUrl === 'string' && fallbackUrl.startsWith('/') && !fallbackUrl.startsWith('//')) return fallbackUrl;

  const category = normalize(product?.category);
  if (category.includes('ram') || key.includes('ddr')) return normalPath('minh-hoa-ram.svg');
  if (category.includes('cpu') || key.includes('intel') || key.includes('ryzen')) return normalPath('minh-hoa-cpu.svg');
  if (category.includes('o cung') || key.includes('ssd') || key.includes('nvme')) return normalPath('minh-hoa-ssd.svg');
  if (category.includes('card do hoa') || key.includes('geforce') || key.includes('rtx')) return normalPath('minh-hoa-vga.svg');
  if (category.includes('nguon') || key.includes('psu')) return normalPath('minh-hoa-psu.svg');
  if (category.includes('vo may') || key.includes('case')) return normalPath('minh-hoa-case.svg');
  if (category.includes('man hinh') || key.includes('monitor')) return normalPath('minh-hoa-monitor.svg');
  return normalPath('minh-hoa-chung.svg');
}

export function onProductImageError(event) {
  const element = event.currentTarget;
  // If a DB image URL is unavailable, degrade to a local illustration once.
  if (!element.src.endsWith('/images/products/minh-hoa-chung.svg')) {
    element.onerror = null;
    element.src = normalPath('minh-hoa-chung.svg');
  }
}
