import { useState } from 'react';
import { Link, useNavigate, useOutletContext, useParams } from 'react-router-dom';
import { money, useApi } from '../API/GoiAPI.js';
import LoadState from './ThanhPhan/TrangThaiTaiDuLieu.jsx';
import { getProductImage, onProductImageError } from '../config/HinhAnhSanPham.js';
import { themGioHang, muaNgaySanPham } from '../API/GioHangAPI.js';

export default function ProductDetailsPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { daDangNhap, yeuCauDangNhap } = useOutletContext();
  const [thongBao, setThongBao] = useState('');
  const [dangXuLy, setDangXuLy] = useState(false);
  async function them() {
    if (!daDangNhap) { yeuCauDangNhap('gio-hang'); return; }
    setDangXuLy(true);
    try { await themGioHang(Number(id)); setThongBao('Đã thêm sản phẩm vào giỏ hàng.'); }
    catch (err) { setThongBao(err.message); }
    finally { setDangXuLy(false); }
  }
  async function muaNgay() {
    if (!daDangNhap) { yeuCauDangNhap('gio-hang'); return; }
    setDangXuLy(true);
    setThongBao('');
    try {
      await muaNgaySanPham(Number(id));
      navigate('/dat-hang?mua-ngay=1');
    } catch (err) { setThongBao(err.message); }
    finally { setDangXuLy(false); }
  }
  const validId = /^\d+$/.test(id ?? '') && Number(id) > 0;
  const { data: product, loading, error } = useApi(validId ? `/api/catalog/products/${id}` : '/api/catalog/products/0');

  return <div className="wrap section">
    <LoadState loading={loading} error={error} />
    {!loading && !error && product && <>
      <div className="breadcrumb"><Link to="/">Trang chủ</Link> / <Link to="/san-pham">Sản phẩm</Link> / {product.name}</div>
      <div className="detail-grid">
        <div className="detail-image">
          <img src={getProductImage(product)} alt={product.name} onError={onProductImageError} />
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
          <div className="web-detail-actions">
            <button className="web-primary" type="button" disabled={dangXuLy || product.availableQuantity <= 0} onClick={them}>
              {dangXuLy ? 'Đang thêm...' : 'Thêm vào giỏ hàng'}
            </button>
            <button className="web-secondary" type="button" disabled={dangXuLy || product.availableQuantity <= 0} onClick={muaNgay}>Mua ngay</button>
          </div>
          {thongBao && <p className="web-info" role="status">{thongBao}</p>}
          <h2>Mô tả sản phẩm</h2><p>{product.description}</p>
          <Link className="primary-link" to="/san-pham">← Tiếp tục xem sản phẩm</Link>
          <p className="integration-note"></p>
        </div>
      </div>
    </>}
  </div>;
}
