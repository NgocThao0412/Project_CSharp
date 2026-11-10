import { useEffect, useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { dangKyTaiKhoan, dangNhapTaiKhoan } from '../API/GoiAPI.js';

const banDau = {
  tenDangNhap: '', hoTen: '', email: '', soDienThoai: '',
  diaChi: '', tinhThanhPho: '', phuongXa: '', matKhau: '', xacNhanMatKhau: '',
};

export default function TaiKhoan() {
  const { pathname, state } = useLocation();
  const navigate = useNavigate();
  const dangKy = pathname.endsWith('/dang-ky');
  const [form, setForm] = useState(banDau);
  const [loi, setLoi] = useState('');
  const [dangGui, setDangGui] = useState(false);

  useEffect(() => { setForm(banDau); setLoi(''); }, [pathname]);

  function thayDoi(e) {
    setForm(cu => ({ ...cu, [e.target.name]: e.target.value }));
  }

  async function gui(e) {
    e.preventDefault();
    if (dangGui) return;
    setLoi('');
    if (dangKy && form.matKhau !== form.xacNhanMatKhau) {
      setLoi('Xác nhận mật khẩu không khớp.');
      return;
    }
    setDangGui(true);
    try {
      if (dangKy) {
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
            <label>Tỉnh / Thành phố *
              <input name="tinhThanhPho" type="text" autoComplete="address-level1" value={form.tinhThanhPho} onChange={thayDoi}
                required maxLength={100} placeholder="Nhập tỉnh/thành phố" />
            </label>
            <label>Phường / Xã *
              <input name="phuongXa" type="text" autoComplete="address-level3" value={form.phuongXa} onChange={thayDoi}
                required maxLength={100} placeholder="Nhập phường/xã" />
            </label>
            <label>Mật khẩu *
              <input name="matKhau" type="password" autoComplete="new-password" value={form.matKhau} onChange={thayDoi}
                required minLength={8} maxLength={100} placeholder="Nhập mật khẩu" />
            </label>
            <label>Xác nhận mật khẩu *
              <input name="xacNhanMatKhau" type="password" autoComplete="new-password" value={form.xacNhanMatKhau} onChange={thayDoi}
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
        {loi && <div className="web-error" role="alert">{loi}</div>}
        <button className="web-primary" type="submit" disabled={dangGui}>
          {dangGui ? 'Đang xử lý...' : dangKy ? 'Đăng ký' : 'Đăng nhập'}
        </button>
      </form>
      <div className="web-account-switch">{dangKy ? 'Đã có tài khoản?' : 'Chưa có tài khoản?'}{' '}
        <Link to={dangKy ? '/tai-khoan/dang-nhap' : '/tai-khoan/dang-ky'}>{dangKy ? 'Đăng nhập' : 'Đăng ký'}</Link>
      </div>
    </section>
  </div>;
}
