import { useEffect, useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { capNhatThongTinTaiKhoan, dangKyTaiKhoan, dangNhapTaiKhoan, dangXuatTaiKhoan, layThongTinTaiKhoan } from '../API/GoiAPI.js';

const banDau = {
  tenDangNhap: '', hoTen: '', email: '', soDienThoai: '',
  diaChi: '', tinhThanhPho: '', phuongXa: '', matKhau: '', xacNhanMatKhau: '',
};
const apiDiaGioi = 'https://provinces.open-api.vn/api/v2';

export default function TaiKhoan() {
  const { pathname, state } = useLocation();
  const navigate = useNavigate();
  const dangKy = pathname.endsWith('/dang-ky');
  const thongTinTaiKhoan = pathname.endsWith('/thong-tin');
  const [form, setForm] = useState(banDau);
  const [loi, setLoi] = useState('');
  const [thongBao, setThongBao] = useState('');
  const [dangGui, setDangGui] = useState(false);
  const [dangSua, setDangSua] = useState(false);
  const [dangTaiThongTin, setDangTaiThongTin] = useState(false);
  const [tinhThanh, setTinhThanh] = useState([]);
  const [phuongXa, setPhuongXa] = useState([]);
  const [dangTaiTinh, setDangTaiTinh] = useState(false);
  const [dangTaiPhuong, setDangTaiPhuong] = useState(false);
  const [loiDiaGioi, setLoiDiaGioi] = useState('');

  useEffect(() => {
    setForm(banDau);
    setLoi('');
    setThongBao('');
    setDangSua(false);
    if (!thongTinTaiKhoan) return;
    let active = true;
    setDangTaiThongTin(true);
    layThongTinTaiKhoan().then(data => {
      if (active) setForm({ ...banDau, ...data });
    }).catch(err => {
      if (active) setLoi(err.message);
    }).finally(() => { if (active) setDangTaiThongTin(false); });
    return () => { active = false; };
  }, [pathname, thongTinTaiKhoan]);

  useEffect(() => {
    if (!dangKy && !thongTinTaiKhoan) return;
    const controller = new AbortController();
    setDangTaiTinh(true);
    setLoiDiaGioi('');
    fetch(`${apiDiaGioi}/`, { signal: controller.signal }).then(res => {
      if (!res.ok) throw new Error('Không tải được danh sách tỉnh/thành phố.');
      return res.json();
    }).then(data => setTinhThanh(data)).catch(err => {
      if (err.name !== 'AbortError') setLoiDiaGioi('Không thể tải danh mục địa chỉ. Vui lòng kiểm tra mạng và tải lại trang.');
    }).finally(() => { if (!controller.signal.aborted) setDangTaiTinh(false); });
    return () => controller.abort();
  }, [dangKy, thongTinTaiKhoan]);

  useEffect(() => {
    if (!dangKy && !thongTinTaiKhoan) return;
    const tinh = tinhThanh.find(item => item.name === form.tinhThanhPho);
    if (!tinh || tinh.code === 'cu') { setPhuongXa([]); return; }
    const controller = new AbortController();
    setDangTaiPhuong(true);
    setPhuongXa([]);
    setLoiDiaGioi('');
    fetch(`${apiDiaGioi}/p/${tinh.code}?depth=2`, { signal: controller.signal }).then(res => {
      if (!res.ok) throw new Error('Không tải được danh sách phường/xã.');
      return res.json();
    }).then(data => setPhuongXa(data.wards ?? [])).catch(err => {
      if (err.name !== 'AbortError') setLoiDiaGioi('Không thể tải phường/xã. Vui lòng kiểm tra mạng và chọn lại tỉnh/thành phố.');
    }).finally(() => { if (!controller.signal.aborted) setDangTaiPhuong(false); });
    return () => controller.abort();
  }, [dangKy, thongTinTaiKhoan, form.tinhThanhPho, tinhThanh]);

  function thayDoi(e) {
    setForm(cu => ({ ...cu, [e.target.name]: e.target.value,
      ...(e.target.name === 'tinhThanhPho' ? { phuongXa: '' } : {}) }));
  }

  async function gui(e) {
    e.preventDefault();
    if (dangGui) return;
    setLoi('');
    setThongBao('');
    if (dangKy && form.matKhau !== form.xacNhanMatKhau) {
      setLoi('Xác nhận mật khẩu không khớp.');
      return;
    }
    setDangGui(true);
    try {
      if (thongTinTaiKhoan) {
        const { hoTen, email, soDienThoai, diaChi, tinhThanhPho, phuongXa } = form;
        const data = await capNhatThongTinTaiKhoan({ hoTen, email, soDienThoai, diaChi, tinhThanhPho, phuongXa });
        setForm({ ...banDau, ...data });
        setDangSua(false);
        setThongBao('Đã cập nhật thông tin tài khoản.');
      } else if (dangKy) {
        await dangKyTaiKhoan(form);
        navigate('/tai-khoan/dang-nhap', { replace: true, state: { dangKyThanhCong: true } });
      } else {
        await dangNhapTaiKhoan({ tenDangNhap: form.tenDangNhap, matKhau: form.matKhau });
        window.dispatchEvent(new Event('tai-khoan:changed'));
        navigate('/', { replace: true });
      }
    } catch (err) {
      setLoi(err.message);
    } finally {
      setDangGui(false);
    }
  }

  const danhSachTinh = form.tinhThanhPho && !tinhThanh.some(t => t.name === form.tinhThanhPho)
    ? [{ name: form.tinhThanhPho, code: 'cu' }, ...tinhThanh] : tinhThanh;
  const danhSachPhuong = form.phuongXa && !phuongXa.some(x => x.name === form.phuongXa)
    ? [{ name: form.phuongXa, code: 'cu' }, ...phuongXa] : phuongXa;
  const chonTinh = <label>Tỉnh / Thành phố *
    <select name="tinhThanhPho" value={form.tinhThanhPho} onChange={thayDoi}
      required disabled={dangTaiTinh || (thongTinTaiKhoan && !dangSua)}>
      <option value="">{dangTaiTinh ? 'Đang tải tỉnh/thành phố...' : 'Chọn Tỉnh / Thành phố'}</option>
      {danhSachTinh.map(item => <option key={item.code} value={item.name}>{item.name}</option>)}
    </select>
  </label>;
  const chonPhuong = <label>Phường / Xã *
    <select name="phuongXa" value={form.phuongXa} onChange={thayDoi}
      required disabled={dangTaiPhuong || !form.tinhThanhPho || (thongTinTaiKhoan && !dangSua)}>
      <option value="">{dangTaiPhuong ? 'Đang tải phường/xã...' : 'Chọn Phường / Xã'}</option>
      {danhSachPhuong.map(item => <option key={item.code} value={item.name}>{item.name}</option>)}
    </select>
  </label>;

  if (thongTinTaiKhoan) return <div className="web-account-wrap">
    <div className="web-account-profile-layout">
      <aside className="web-account-profile-nav">
        <strong>Tài khoản của tôi</strong>
        <Link to="/tai-khoan/thong-tin" className="active">Thông tin tài khoản</Link>
        <Link to="/don-hang/tra-cuu">Đơn hàng của tôi</Link>
        <button type="button" onClick={async () => {
          try {
            await dangXuatTaiKhoan();
            window.dispatchEvent(new Event('tai-khoan:changed'));
            navigate('/', { replace: true });
          } catch (err) { setLoi(err.message); }
        }}>Đăng xuất</button>
      </aside>
    <section className="web-account-card web-account-register web-account-profile">
      <div className="web-eyebrow">TÀI KHOẢN KHÁCH HÀNG</div>
      <h1>Thông tin tài khoản</h1>
      {dangTaiThongTin && <p>Đang tải thông tin...</p>}
      {loi && <div className="web-error" role="alert">{loi}</div>}
      {thongBao && <p className="web-info" role="status">{thongBao}</p>}
      {!dangTaiThongTin && form.tenDangNhap && <form onSubmit={gui}>
        <div className="web-account-columns">
          <div className="web-account-column">
            <label>Tên đăng nhập<input value={form.tenDangNhap} readOnly /></label>
            <label>Họ và tên *<input name="hoTen" value={form.hoTen} onChange={thayDoi}
              required minLength={2} maxLength={255} disabled={!dangSua} /></label>
            <label>Email *<input name="email" type="email" value={form.email} onChange={thayDoi}
              required maxLength={50} disabled={!dangSua} /></label>
            <label>Số điện thoại *<input name="soDienThoai" type="tel" value={form.soDienThoai} onChange={thayDoi}
              required pattern="0[0-9]{9}" maxLength={10} disabled={!dangSua} /></label>
          </div>
          <div className="web-account-column">
            <label>Địa chỉ (Số nhà, đường) *<input name="diaChi" value={form.diaChi} onChange={thayDoi}
              required minLength={2} maxLength={255} disabled={!dangSua} /></label>
            {chonTinh}
            {chonPhuong}
          </div>
        </div>
        {dangSua ? <div className="web-profile-actions">
          <button type="submit" className="web-primary" disabled={dangGui || dangTaiTinh || dangTaiPhuong}>{dangGui ? 'Đang lưu...' : 'Lưu thay đổi'}</button>
          <button type="button" className="web-secondary" onClick={() => {
            setDangSua(false);
            layThongTinTaiKhoan().then(data => setForm({ ...banDau, ...data })).catch(err => setLoi(err.message));
          }}>Hủy</button>
        </div> : <button type="button" className="web-primary" onClick={() => setDangSua(true)}>Cập nhật thông tin</button>}
      </form>}
    </section>
    </div>
  </div>;

  return <div className="web-account-wrap">
    <section className={`web-account-card ${dangKy ? 'web-account-register' : ''}`}>
      <div className="web-eyebrow">TÀI KHOẢN KHÁCH HÀNG</div>
      <h1>{dangKy ? 'Đăng ký' : 'Đăng nhập'}</h1>
      <p className="muted">{dangKy ? 'Tạo tài khoản để mua sắm thuận tiện hơn.' : 'Đăng nhập để theo dõi đơn hàng của bạn.'}</p>
      {state?.dangKyThanhCong && !dangKy && <p className="web-info" role="status">Đăng ký thành công. Vui lòng đăng nhập.</p>}
      <form onSubmit={gui}>
        {dangKy ? <div className="web-account-columns">
          <div className="web-account-column">
            <label>Tên đăng nhập *
              <input name="tenDangNhap" type="text" autoComplete="username" value={form.tenDangNhap} onChange={thayDoi}
                required minLength={4} maxLength={50} pattern="[A-Za-z0-9._-]{4,50}" placeholder="Nhập tên đăng nhập" />
            </label>
            <label>Họ và tên *
              <input name="hoTen" type="text" autoComplete="name" value={form.hoTen} onChange={thayDoi}
                required minLength={2} maxLength={255} placeholder="Nhập họ và tên" />
            </label>
            <label>Email *
              <input name="email" type="email" autoComplete="email" value={form.email} onChange={thayDoi}
                required maxLength={50} placeholder="Nhập email của bạn" />
            </label>
            <label>Số điện thoại *
              <input name="soDienThoai" type="tel" autoComplete="tel" value={form.soDienThoai} onChange={thayDoi}
                required maxLength={10} pattern="0[0-9]{9}" placeholder="Nhập số điện thoại" />
            </label>
          </div>
          <div className="web-account-column">
            <label>Địa chỉ (Số nhà, đường) *
              <input name="diaChi" type="text" autoComplete="street-address" value={form.diaChi} onChange={thayDoi}
                required minLength={2} maxLength={255} placeholder="Nhập địa chỉ" />
            </label>
            {chonTinh}
            {chonPhuong}
            <label>Mật khẩu *
              <input name="matKhau" type="password" autoComplete="off" value={form.matKhau} onChange={thayDoi}
                required minLength={8} maxLength={100} placeholder="Nhập mật khẩu" />
            </label>
            <label>Xác nhận mật khẩu *
              <input name="xacNhanMatKhau" type="password" autoComplete="off" value={form.xacNhanMatKhau} onChange={thayDoi}
                required placeholder="Nhập lại mật khẩu" />
            </label>
          </div>
        </div> : <>
          <label>Tên đăng nhập
            <input name="tenDangNhap" type="text" autoComplete="username" value={form.tenDangNhap} onChange={thayDoi}
              required placeholder="Nhập tên đăng nhập" />
          </label>
          <label>Mật khẩu
            <input name="matKhau" type="password" autoComplete="current-password" value={form.matKhau} onChange={thayDoi}
              required placeholder="Nhập mật khẩu" />
          </label>
        </>}
        {loiDiaGioi && dangKy && <div className="web-error" role="alert">{loiDiaGioi}</div>}
        {loi && <div className="web-error" role="alert">{loi}</div>}
        <button className="web-primary" type="submit" disabled={dangGui || (dangKy && (dangTaiTinh || dangTaiPhuong || !tinhThanh.length || !phuongXa.length))}>
          {dangGui ? 'Đang xử lý...' : dangKy ? 'Đăng ký' : 'Đăng nhập'}
        </button>
      </form>
      <div className="web-account-switch">{dangKy ? 'Đã có tài khoản?' : 'Chưa có tài khoản?'}{' '}
        <Link to={dangKy ? '/tai-khoan/dang-nhap' : '/tai-khoan/dang-ky'}>{dangKy ? 'Đăng nhập' : 'Đăng ký'}</Link>
      </div>
    </section>
  </div>;
}
