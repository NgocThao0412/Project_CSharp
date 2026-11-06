import { Link, useParams } from 'react-router-dom';
import { money, useApi } from '../api.js';
import LoadState from '../components/LoadState.jsx';

export default function ProductDetailsPage() {
  const { id } = useParams();
  const validId = /^\d+$/.test(id ?? '') && Number(id) > 0;
  const { data: product, loading, error } = useApi(validId ? `/api/catalog/products/${id}` : '/api/catalog/products/0');

  return <div className="wrap section">
    <LoadState loading={loading} error={error} />
    {!loading && !error && product && <>
      <div className="breadcrumb"><Link to="/">Trang chủ</Link> / <Link to="/san-pham">Sản phẩm</Link> / {product.name}</div>
      <div className="detail-grid">
        <div className="detail-image">
          {product.imageUrl ? <img src={product.imageUrl} alt={product.name} /> : <span aria-hidden="true">PC</span>}
        </div>
        <div className="detail-info">
          <div className="eyebrow blue">{product.category}</div>
          <h1>{product.name}</h1>
          <p className="muted">Thương hiệu: {product.brand} · Mã sản phẩm: {product.id}</p>
          <div className="detail-price">{money(product.price)}</div>
          <p className={`stock ${product.availableQuantity <= 0 ? 'sold-out' : ''}`}>
            {product.availableQuantity > 0
              ? `Đang có ${product.availableQuantity} sản phẩm khả dụng`
              : 'Sản phẩm tạm hết hàng'}
          </p>
          <h2>Mô tả sản phẩm</h2><p>{product.description}</p>
          <Link className="primary-link" to="/san-pham">← Tiếp tục xem sản phẩm</Link>
          <p className="integration-note"></p>
        </div>
      </div>
    </>}
  </div>;
}
