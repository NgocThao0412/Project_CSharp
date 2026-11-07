import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { money } from '../API/GoiAPI.js';
import { capNhatGioHang, layGioHang, xoaGioHang } from '../API/GioHangAPI.js';
import { getProductImage, onProductImageError } from '../config/HinhAnhSanPham.js';

export default function GioHang() {
  const [gio, setGio] = useState(null);
  const [loi, setLoi] = useState('');
  const [dangXuLy, setDangXuLy] = useState(false);

  useEffect(() => {
    let active = true;
    layGioHang().then(data => { if (active) setGio(data); })
      .catch(err => { if (active) setLoi(err.message); });
    return () => { active = false; };
  }, []);

  async function capNhat(id, quantity) {
    setDangXuLy(true);
    setLoi('');
    try { setGio(await capNhatGioHang(id, quantity)); }
    catch (err) { setLoi(err.message); }
    finally { setDangXuLy(false); }
  }

  async function xoa(id) {
    setDangXuLy(true);
    setLoi('');
    try { setGio(await xoaGioHang(id)); }
    catch (err) { setLoi(err.message); }
    finally { setDangXuLy(false); }
  }

  return <div className="wrap section web-customer-page">
    <div className="breadcrumb"><Link to="/">Trang chủ</Link> / Giỏ hàng</div>
    <h1>Giỏ hàng của bạn</h1>
    {loi && <div className="web-error" role="alert">{loi}</div>}
    {!gio && !loi && <p>Đang tải giỏ hàng...</p>}
    {gio && gio.items.length === 0 && <div className="web-empty">
      <h2>Giỏ hàng đang trống</h2>
      <p>Hãy chọn linh kiện để bắt đầu mua sắm.</p>
      <Link className="web-primary" to="/san-pham">Xem sản phẩm</Link>
    </div>}
    {gio && gio.items.length > 0 && <div className="web-cart-layout">
      <div className="web-cart-items">{gio.items.map(item => <article className="web-cart-item" key={item.id}>
        <Link to={`/san-pham/${item.id}`}><img src={getProductImage({ id: item.id, name: item.name, imageUrl: item.imageUrl })}
          onError={onProductImageError} alt={item.name} /></Link>
        <div className="web-cart-copy">
          <h2><Link to={`/san-pham/${item.id}`}>{item.name}</Link></h2>
          <p className="muted">Đơn giá: {money(item.price)} · Tồn kho: {item.stock}</p>
          <div className="web-cart-controls">
            <label>Số lượng
              <input type="number" min="1" max={Math.min(99, item.stock)} value={item.quantity}
                disabled={dangXuLy} onChange={e => {
                  const n = Number(e.target.value);
                  if (Number.isInteger(n) && n > 0 && n <= item.stock && n <= 99) capNhat(item.id, n);
                }}/>
            </label>
            <button type="button" disabled={dangXuLy} className="web-text-button" onClick={() => xoa(item.id)}>Xóa</button>
          </div>
          {!item.available && <p className="web-error">Số lượng vượt quá tồn kho hiện tại.</p>}
        </div>
        <strong>{money(item.lineTotal)}</strong>
      </article>)}</div>
      <aside className="web-cart-summary">
        <h2>Tóm tắt giỏ hàng</h2>
        <div className="web-summary-line"><span>Tổng cộng</span><strong>{money(gio.total)}</strong></div>
        <p className="muted">Đặt hàng trực tuyến chưa được bật: nhóm cần thống nhất tài khoản khách hàng và quy trình ghi phiếu xuất trong SQL Server.</p>
        <button className="web-primary" disabled type="button">Đặt hàng (chưa khả dụng)</button>
        <Link to="/san-pham" className="web-secondary">Tiếp tục mua sắm</Link>
      </aside>
    </div>}
  </div>;
}
