import { Link } from 'react-router-dom';
import { money } from '../../API/GoiAPI.js';
import { getProductImage, onProductImageError } from '../../config/HinhAnhSanPham.js';
import ShopIcon from './BieuTuongCuaHang.jsx';

const IMAGES = '/images/catalog/';

function productImage(product) {
  if (product.imageUrl) return product.imageUrl;
  const text = `${product.category ?? ''} ${product.name ?? ''} ${product.brand ?? ''}`.toLocaleLowerCase('vi-VN');
  if (text.includes('mainboard') || text.includes('bo mạch')) return `${IMAGES}mainboard.webp`;
  if (text.includes('rtx') || text.includes('đồ họa') || text.includes('vga')) return `${IMAGES}rtx-graphics.webp`;
  if (text.includes('ram') || text.includes('ddr')) return `${IMAGES}rgb-ram.webp`;
  if (text.includes('intel')) return `${IMAGES}intel-core.webp`;
  if (text.includes('amd') || text.includes('ryzen')) return `${IMAGES}amd-chip.svg`;
  if (text.includes('ssd') || text.includes('ổ cứng')) return `${IMAGES}ssd.svg`;
  if (text.includes('nguồn') || text.includes('650w')) return `${IMAGES}psu.svg`;
  if (text.includes('vỏ') || text.includes('case')) return `${IMAGES}case.svg`;
  return `${IMAGES}part.svg`;
}

export default function ProductCard({ product, variant = 'standard', onAddToCart, isFavorite = false, onToggleFavorite }) {
  if (variant === 'catalog') {
    // Optional metadata is used only when supplied by the real API. Never invent ratings or discounts.
    const discount = Number(product.discountPercent ?? 0);
    const hasDiscount = discount > 0 && Number(product.originalPrice) > Number(product.price);
    const rating = Number(product.rating);
    const hasRating = Number.isFinite(rating) && rating >= 0 && rating <= 5 && product.rating != null;
    const isAvailable = Number(product.availableQuantity) > 0;
    return <article className="catalog-card">
      <div className="catalog-card-media">
        <span className={`catalog-card-badge ${isAvailable ? 'catalog-badge-blue' : 'catalog-badge-gray'}`}>
          {isAvailable ? 'Có sẵn' : 'Hết hàng'}
        </span>
        <button type="button" className={`catalog-favorite ${isFavorite ? 'is-favorite' : ''}`}
          onClick={() => onToggleFavorite?.(product.id)} aria-label={isFavorite ? 'Bỏ yêu thích' : 'Yêu thích sản phẩm'} aria-pressed={isFavorite}>
          <ShopIcon name="heart" size={21} />
        </button>
        <Link to={`/san-pham/${product.id}`} className="catalog-card-image-link" aria-label={`Xem ${product.name}`}>
          <img src={getProductImage(product, productImage(product))} alt={product.name} loading="lazy" onError={onProductImageError} />
        </Link>
      </div>
      <div className="catalog-card-body">
        <h3><Link to={`/san-pham/${product.id}`}>{product.name}</Link></h3>
        <div className="catalog-card-subtitle">{hasRating ? <>
          <span className="catalog-rating"><ShopIcon name="star" size={15} /> {rating.toFixed(1)}</span>
          {product.reviewCount != null && <span>({product.reviewCount} đánh giá)</span>}
        </> : <span>{product.category} · {product.brand}</span>}</div>
        <div className="catalog-card-pricing">
          <span className="catalog-current-price">{money(product.price)}</span>
          {hasDiscount && <><s className="catalog-old-price">{money(product.originalPrice)}</s><span className="catalog-price-discount">-{discount}%</span></>}
        </div>
        <div className="catalog-card-actions">
          <button type="button" className="catalog-cart-button" disabled={!isAvailable}
            title={isAvailable ? 'Tính năng giỏ hàng đang được kết nối với Web C#' : 'Sản phẩm tạm hết hàng'}
            onClick={() => onAddToCart?.(product)}>
            <ShopIcon name="cart" size={17} /> Thêm vào giỏ
          </button>
          <Link className="catalog-detail-button" to={`/san-pham/${product.id}`}>Xem chi tiết</Link>
        </div>
      </div>
    </article>;
  }

  return <article className="product-card">
    <Link className="product-image" to={`/san-pham/${product.id}`} aria-label={`Xem ${product.name}`}>
      <img src={getProductImage(product, productImage(product))} alt={product.name} loading="lazy" onError={onProductImageError} />
    </Link>
    <div className="product-body">
      <div className="muted">{product.category} · {product.brand}</div>
      <h3><Link to={`/san-pham/${product.id}`}>{product.name}</Link></h3>
      <div className="price">{money(product.price)}</div>
      <div className={`stock ${product.availableQuantity <= 0 ? 'sold-out' : ''}`}>
        {product.availableQuantity > 0 ? `Còn ${product.availableQuantity} sản phẩm` : 'Tạm hết hàng'}
      </div>
      <Link className="details-link" to={`/san-pham/${product.id}`}>Xem chi tiết →</Link>
    </div>
  </article>;
}
