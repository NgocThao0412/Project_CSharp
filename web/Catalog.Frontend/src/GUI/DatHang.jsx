import { useEffect, useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { layThongTinTaiKhoan, money } from '../API/GoiAPI.js';
import { datHang, layGioHang, layMuaNgay } from '../API/GioHangAPI.js';
import { getProductImage, onProductImageError } from '../config/HinhAnhSanPham.js';

const newRequestId = () => crypto.randomUUID();
const formTrong = { hoTen: '', soDienThoai: '', diaChi: '', email: '', ghiChu: '' };
const hopLeHoTen = /^[\p{L}]+(?: +[\p{L}]+)*$/u;

function layThongTinGiao(data) {
  if (!data) return formTrong;
  return {
    hoTen: data.hoTen ?? '',
    soDienThoai: data.soDienThoai ?? '',
    diaChi: [data.diaChi, data.phuongXa, data.tinhThanhPho].filter(Boolean).join(', '),
    email: data.email ?? '',
    ghiChu: '',
  };
}

function kiemTra(form) {
  const loi = {};
  const hoTen = form.hoTen.trim();
  const soDienThoai = form.soDienThoai.trim();
  const diaChi = form.diaChi.trim();
  const email = form.email.trim();
  if (!hoTen) loi.hoTen = 'Vui lòng nhập họ và tên người nhận.';
  else if (hoTen.length < 2 || hoTen.length > 100) loi.hoTen = 'Họ tên người nhận phải từ 2 đến 100 ký tự.';
  else if (!hopLeHoTen.test(hoTen)) loi.hoTen = 'Họ tên chỉ được chứa chữ cái và khoảng trắng, không được nhập số hoặc ký tự đặc biệt.';
  if (!soDienThoai) loi.soDienThoai = 'Vui lòng nhập số điện thoại người nhận.';
  else if (!/^0\d{9}$/.test(soDienThoai)) loi.soDienThoai = 'Số điện thoại phải đủ 10 chữ số và bắt đầu bằng 0.';
  if (!diaChi) loi.diaChi = 'Vui lòng nhập địa chỉ giao hàng.';
  else if (diaChi.length < 10 || diaChi.length > 255) loi.diaChi = 'Địa chỉ giao hàng phải từ 10 đến 255 ký tự.';
  if (email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) loi.email = 'Email người nhận không đúng định dạng.';
  else if (email.length > 50) loi.email = 'Email người nhận không được vượt quá 50 ký tự.';
  if (form.ghiChu.length > 500) loi.ghiChu = 'Ghi chú không được vượt quá 500 ký tự.';
  return loi;
}

export default function DatHang() {
  const navigate = useNavigate();
  const location = useLocation();
  const muaNgay = new URLSearchParams(location.search).get('mua-ngay') === '1';
  const [gio, setGio] = useState(null);
  const [loi, setLoi] = useState('');
  const [loiTruong, setLoiTruong] = useState({});
  const [loiThongTin, setLoiThongTin] = useState('');
  const [dangGui, setDangGui] = useState(false);
  const [maYeuCau] = useState(newRequestId);
  const [cheDoGiao, setCheDoGiao] = useState('toi');
  const [thongTinCaNhan, setThongTinCaNhan] = useState(null);
  const [form, setForm] = useState(formTrong);

  useEffect(() => {
    let active = true;
    setGio(null);
    (muaNgay ? layMuaNgay() : layGioHang()).then(data => { if (active) setGio(data); })
      .catch(err => { if (active) setLoi(err.message); });
    return () => { active = false; };
  }, [muaNgay]);

  useEffect(() => {
    let active = true;
    layThongTinTaiKhoan().then(data => { if (active) setThongTinCaNhan(data); })
      .catch(err => { if (active) setLoiThongTin(`Không tải được thông tin tài khoản: ${err.message}`); });
    return () => { active = false; };
  }, []);

  useEffect(() => {
    if (cheDoGiao === 'toi' && thongTinCaNhan)
      setForm(cu => ({ ...layThongTinGiao(thongTinCaNhan), ghiChu: cu.ghiChu }));
  }, [cheDoGiao, thongTinCaNhan]);

  function chonNguoiNhan(cheDo) {
    if (dangGui) return;
    setCheDoGiao(cheDo);
    setLoi('');
    setLoiTruong({});
    setForm(cu => ({ ...(cheDo === 'toi' ? layThongTinGiao(thongTinCaNhan) : formTrong), ghiChu: cu.ghiChu }));
  }

  function thayDoi(e) {
    const { name, value } = e.target;
    setForm(old => ({ ...old, [name]: value }));
    setLoiTruong(old => ({ ...old, [name]: '' }));
    setLoi('');
  }

  async function xacNhan(e) {
    e.preventDefault();
    if (dangGui || !gio?.items?.length || !gio.available) return;
    const cacLoi = kiemTra(form);
    setLoiTruong(cacLoi);
    if (Object.keys(cacLoi).length) return;
    setDangGui(true);
    setLoi('');
    try {
      const ketQua = await datHang({
        ...form, hoTen: form.hoTen.trim(), soDienThoai: form.soDienThoai.trim(),
        diaChi: form.diaChi.trim(), email: form.email.trim() || null,
        ghiChu: form.ghiChu.trim() || null,
        tongTienXacNhan: gio.total,
        maYeuCau,
        muaNgay,
      });
      navigate('/dat-hang/thanh-cong', { state: ketQua, replace: true });
    } catch (err) {
      setLoi(err.message);
    } finally {
      setDangGui(false);
    }
  }

  return <div className="wrap section web-customer-page web-checkout">
    <div className="breadcrumb"><Link to="/">Trang chủ</Link> / {muaNgay ? <Link to="/san-pham">Sản phẩm</Link> : <Link to="/gio-hang">Giỏ hàng</Link>} / Đặt hàng</div>
    <h1>Thông tin đặt hàng</h1>
    <p className="muted">Kiểm tra đơn hàng và nhập thông tin giao nhận.</p>
    {loi && <div className="web-error" role="alert">{loi}</div>}
    {!gio && !loi && <p>Đang tải giỏ hàng...</p>}
    {gio?.items?.length === 0 && <div className="web-empty"><h2>{muaNgay ? 'Chưa chọn sản phẩm mua ngay' : 'Giỏ hàng đang trống'}</h2><Link className="web-primary" to="/san-pham">Xem sản phẩm</Link></div>}
    {gio?.items?.length > 0 && <form className="web-checkout-grid" onSubmit={xacNhan} noValidate>
      <section className="web-checkout-card">
        <h2>Thông tin người nhận</h2>
        <div className="web-delivery-options" role="group" aria-label="Chọn người nhận hàng">
          <button type="button" className={`web-delivery-choice ${cheDoGiao === 'toi' ? 'active' : ''}`}
            aria-pressed={cheDoGiao === 'toi'} onClick={() => chonNguoiNhan('toi')} disabled={dangGui}>Giao cho tôi</button>
          <button type="button" className={`web-delivery-choice ${cheDoGiao === 'khac' ? 'active' : ''}`}
            aria-pressed={cheDoGiao === 'khac'} onClick={() => chonNguoiNhan('khac')} disabled={dangGui}>Giao cho người khác</button>
        </div>
        {cheDoGiao === 'toi' && loiThongTin && <div className="web-error" role="alert">{loiThongTin}. Bạn vẫn có thể tự nhập thông tin giao hàng.</div>}
        <div className="web-checkout-fields">
          <label className="web-checkout-full">Họ và tên *
            <input name="hoTen" autoComplete="name" value={form.hoTen} onChange={thayDoi}
              aria-invalid={!!loiTruong.hoTen} maxLength={100} placeholder="Nguyễn Văn A" disabled={dangGui} />
            {loiTruong.hoTen && <span className="web-field-error" role="alert">{loiTruong.hoTen}</span>}
          </label>
          <label>Số điện thoại *
            <input name="soDienThoai" type="tel" inputMode="numeric" autoComplete="tel" value={form.soDienThoai}
              onChange={thayDoi} aria-invalid={!!loiTruong.soDienThoai} maxLength={20} placeholder="09xxxxxxxx" disabled={dangGui} />
            {loiTruong.soDienThoai && <span className="web-field-error" role="alert">{loiTruong.soDienThoai}</span>}
          </label>
          <label>Email (không bắt buộc)
            <input name="email" type="email" autoComplete="email" value={form.email} onChange={thayDoi}
              aria-invalid={!!loiTruong.email} maxLength={50} placeholder="email@example.com" disabled={dangGui} />
            {loiTruong.email && <span className="web-field-error" role="alert">{loiTruong.email}</span>}
          </label>
          <label className="web-checkout-full">Địa chỉ giao hàng *
            <textarea name="diaChi" autoComplete="street-address" value={form.diaChi} onChange={thayDoi}
              aria-invalid={!!loiTruong.diaChi} maxLength={255} rows={3} placeholder="Số nhà, đường, phường/xã, tỉnh/thành phố" disabled={dangGui} />
            {loiTruong.diaChi && <span className="web-field-error" role="alert">{loiTruong.diaChi}</span>}
          </label>
          <label className="web-checkout-full">Ghi chú (không bắt buộc)
            <textarea name="ghiChu" value={form.ghiChu} onChange={thayDoi} maxLength={500}
              aria-invalid={!!loiTruong.ghiChu} rows={3} placeholder="Ví dụ: Giao giờ hành chính..." disabled={dangGui} />
            {loiTruong.ghiChu && <span className="web-field-error" role="alert">{loiTruong.ghiChu}</span>}
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
        <Link className="web-secondary" to={muaNgay ? '/san-pham' : '/gio-hang'}>{muaNgay ? 'Tiếp tục xem sản phẩm' : 'Quay lại giỏ hàng'}</Link>
        <p className="muted web-checkout-footnote">Giá và tồn kho được kiểm tra lại ở C# và SQL Server trước khi ghi đơn.</p>
      </aside>
    </form>}
  </div>;
}
