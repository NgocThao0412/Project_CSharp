import { Link } from 'react-router-dom';
import { money } from '../api.js';

export default function ProductCard({ product }) {
  return (
    <article className="product-card">
      <Link className="product-image" to={`/san-pham/${product.id}`} aria-label={`Xem ${product.name}`}>
        {product.imageUrl
          ? <img src={product.imageUrl} alt={product.name} loading="lazy" />
          : <span aria-hidden="true">PC</span>}
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
    </article>
  );
}
