import { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { money } from '../API/GoiAPI.js';
import { datHang, layGioHang } from '../API/GioHangAPI.js';
import { getProductImage, onProductImageError } from '../config/HinhAnhSanPham.js';

const newRequestId = () => crypto.randomUUID();

export default function DatHang() {
  const navigate = useNavigate();
  const [gio, setGio] = useState(null);
  const [loi, setLoi] = useState('');
  const [dangGui, setDangGui] = useState(false);
  const [maYeuCau] = useState(newRequestId);
  const [form, setForm] = useState({ hoTen: '', soDienThoai: '', diaChi: '', email: '', ghiChu: '' });

  useEffect(() => {
    let active = true;
    layGioHang().then(data => { if (active) setGio(data); })
      .catch(err => { if (active) setLoi(err.message); });
    return () => { active = false; };
  }, []);

  function thayDoi(e) {
    const { name, value } = e.target;
    setForm(old => ({ ...old, [name]: value }));
  }

  async function xacNhan(e) {
    e.preventDefault();
    if (dangGui || !gio?.items?.length || !gio.available) return;
    if (!/^0\d{9}$/.test(form.soDienThoai.trim())) {
      setLoi('Số điện thoại phải gồm 10 chữ số và bắt đầu bằng 0.');
      return;
    }
    setDangGui(true);
    setLoi('');
    try {
      const ketQua = await datHang({
        ...form, hoTen: form.hoTen.trim(), soDienThoai: form.soDienThoai.trim(),
        diaChi: form.diaChi.trim(), email: form.email.trim() || null,
        ghiChu: form.ghiChu.trim() || null,
        tongTienXacNhan: gio.total,
        maYeuCau,
      });
      navigate('/dat-hang/thanh-cong', { state: ketQua, replace: true });
    } catch (err) {
      setLoi(err.message);
    } finally {
      setDangGui(false);
    }
  }

  return <div className="wrap section web-customer-page web-checkout">
    <div className="breadcrumb"><Link to="/">Trang chủ</Link> / <Link to="/gio-hang">Giỏ hàng</Link> / Đặt hàng</div>
    <h1>Thông tin đặt hàng</h1>
    <p className="muted">Kiểm tra đơn hàng và nhập thông tin giao nhận. Bạn có thể đặt hàng không cần tài khoản.</p>
    {loi && <div className="web-error" role="alert">{loi} <Link to="/gio-hang">Kiểm tra giỏ hàng</Link></div>}
    {!gio && !loi && <p>Đang tải giỏ hàng...</p>}
    {gio?.items?.length === 0 && <div className="web-empty"><h2>Giỏ hàng đang trống</h2><Link className="web-primary" to="/san-pham">Xem sản phẩm</Link></div>}
    {gio?.items?.length > 0 && <form className="web-checkout-grid" onSubmit={xacNhan}>
      <section className="web-checkout-card">
        <h2>Thông tin người nhận</h2>
        <div className="web-checkout-fields">
          <label className="web-checkout-full">Họ và tên *
            <input name="hoTen" autoComplete="name" value={form.hoTen} onChange={thayDoi}
              required minLength={2} maxLength={100} placeholder="Nguyễn Văn A" disabled={dangGui} />
          </label>
          <label>Số điện thoại *
            <input name="soDienThoai" type="tel" inputMode="numeric" autoComplete="tel" value={form.soDienThoai}
              onChange={thayDoi} required pattern="0[0-9]{9}" maxLength={10} placeholder="09xxxxxxxx" disabled={dangGui} />
          </label>
          <label>Email (không bắt buộc)
            <input name="email" type="email" autoComplete="email" value={form.email} onChange={thayDoi}
              maxLength={50} placeholder="email@example.com" disabled={dangGui} />
          </label>
          <label className="web-checkout-full">Địa chỉ giao hàng *
            <textarea name="diaChi" autoComplete="street-address" value={form.diaChi} onChange={thayDoi}
              required minLength={10} maxLength={255} rows={3} placeholder="Số nhà, đường, phường/xã, tỉnh/thành phố" disabled={dangGui} />
          </label>
          <label className="web-checkout-full">Ghi chú (không bắt buộc)
            <textarea name="ghiChu" value={form.ghiChu} onChange={thayDoi} maxLength={500}
              rows={3} placeholder="Ví dụ: Giao giờ hành chính..." disabled={dangGui} />
          </label>
        </div>
        <h2>Phương thức thanh toán</h2>
        <div className="web-payment-method"><strong>Thanh toán khi nhận hàng (COD)</strong>
          <p>Đơn được chuyển sang trạng thái chờ duyệt. Chưa thu tiền trực tuyến.</p>
        </div>
      </section>
      <aside className="web-checkout-card web-checkout-summary">
        <h2>Đơn hàng của bạn</h2>
        {gio.items.map(item => <div className="web-checkout-line" key={item.id}>
          <img src={getProductImage({ id: item.id, name: item.name, imageUrl: item.imageUrl })}
            onError={onProductImageError} alt="" />
          <div><Link to={`/san-pham/${item.id}`}>{item.name}</Link><p className="muted">Số lượng: {item.quantity}</p></div>
          <strong>{money(item.lineTotal)}</strong>
        </div>)}
        <div className="web-summary-line web-checkout-total"><span>Thành tiền</span><strong>{money(gio.total)}</strong></div>
        {!gio.available && <div className="web-error" role="alert">Một số sản phẩm không đủ tồn kho, vui lòng cập nhật giỏ hàng.</div>}
        <button className="web-primary web-checkout-submit" type="submit"
          disabled={dangGui || !gio.available}>{dangGui ? 'Đang xác nhận...' : 'Xác nhận đặt hàng'}</button>
        <Link className="web-secondary" to="/gio-hang">Quay lại giỏ hàng</Link>
        <p className="muted web-checkout-footnote">Giá và tồn kho được kiểm tra lại ở C# và SQL Server trước khi ghi đơn.</p>
      </aside>
    </form>}
  </div>;
}
