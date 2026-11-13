import { useEffect, useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { capNhatThongTinTaiKhoan, dangKyTaiKhoan, dangNhapTaiKhoan, dangXuatTaiKhoan, layThongTinTaiKhoan } from '../API/GoiAPI.js';

const banDau = {
  tenDangNhap: '', hoTen: '', email: '', soDienThoai: '',
  diaChi: '', tinhThanhPho: '', phuongXa: '', matKhau: '', xacNhanMatKhau: '',
};
const apiDiaGioi = 'https://provinces.open-api.vn/api/v2';
const tenDangNhapHopLe = /^(?=(?:[^A-Za-z]*[A-Za-z]){4})[A-Za-z0-9._-]{4,50}$/;
const hoTenHopLe = /^[\p{L}]+(?: +[\p{L}]+)*$/u;
const gmailHopLe = /^[a-z0-9._%+\-]+@gmail\.com$/i;

function kiemTraThongTin(form, dangKy, thongTinTaiKhoan) {
  const loi = {};
  const tenDangNhap = form.tenDangNhap.trim();
  if (!tenDangNhap) loi.tenDangNhap = 'Vui lòng nhập tên đăng nhập.';
  else if ((dangKy || thongTinTaiKhoan) && !tenDangNhapHopLe.test(tenDangNhap))
    loi.tenDangNhap = 'Tên đăng nhập phải có ít nhất 4 chữ cái, tối đa 50 ký tự, không có khoảng trắng hoặc ký tự đặc biệt khác.';

  if (!dangKy && !thongTinTaiKhoan) {
    if (!form.matKhau) loi.matKhau = 'Vui lòng nhập mật khẩu.';
    return loi;
  }

  const hoTen = form.hoTen.trim();
  const email = form.email.trim();
  const soDienThoai = form.soDienThoai.trim();
  const diaChi = form.diaChi.trim();
  if (!hoTen) loi.hoTen = 'Vui lòng nhập họ và tên.';
  else if (hoTen.length < 2 || hoTen.length > 255) loi.hoTen = 'Họ và tên phải từ 2 đến 255 ký tự.';
  else if (!hoTenHopLe.test(hoTen)) loi.hoTen = 'Họ tên chỉ được chứa chữ cái và khoảng trắng, không được nhập số hoặc ký tự đặc biệt.';
  if (!email) loi.email = 'Vui lòng nhập email.';
  else if (!gmailHopLe.test(email)) loi.email = 'Email phải hợp lệ và có đuôi @gmail.com.';
  else if (email.length > 50) loi.email = 'Email không được vượt quá 50 ký tự.';
  if (!soDienThoai) loi.soDienThoai = 'Vui lòng nhập số điện thoại.';
  else if (!/^0\d{9}$/.test(soDienThoai)) loi.soDienThoai = 'Số điện thoại phải đủ 10 chữ số và bắt đầu bằng 0.';
  if (!diaChi) loi.diaChi = 'Vui lòng nhập địa chỉ.';
  else if (diaChi.length < 2 || diaChi.length > 255) loi.diaChi = 'Địa chỉ phải từ 2 đến 255 ký tự.';
  if (!form.tinhThanhPho) loi.tinhThanhPho = 'Vui lòng chọn tỉnh/thành phố.';
  if (!form.phuongXa) loi.phuongXa = 'Vui lòng chọn phường/xã.';
  if (dangKy) {
    if (!form.matKhau) loi.matKhau = 'Vui lòng nhập mật khẩu.';
    else if (form.matKhau.length < 8 || form.matKhau.length > 100) loi.matKhau = 'Mật khẩu phải từ 8 đến 100 ký tự.';
    if (!form.xacNhanMatKhau) loi.xacNhanMatKhau = 'Vui lòng xác nhận mật khẩu.';
    else if (form.matKhau !== form.xacNhanMatKhau) loi.xacNhanMatKhau = 'Xác nhận mật khẩu không khớp.';
  }
  return loi;
}


export default function TaiKhoan() {
  const { pathname, state } = useLocation();
  const navigate = useNavigate();
  const dangKy = pathname.endsWith('/dang-ky');
  const thongTinTaiKhoan = pathname.endsWith('/thong-tin');
  const [form, setForm] = useState(banDau);
  const [loi, setLoi] = useState('');
  const [loiTruong, setLoiTruong] = useState({});
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
    setLoiTruong({});
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
    setLoiTruong(cu => ({ ...cu, [e.target.name]: '' }));
    setLoi('');
    setForm(cu => ({ ...cu, [e.target.name]: e.target.value,
      ...(e.target.name === 'tinhThanhPho' ? { phuongXa: '' } : {}) }));
  }

  async function gui(e) {
    e.preventDefault();
    if (dangGui) return;
    setLoi('');
    setThongBao('');
    const cacLoi = kiemTraThongTin(form, dangKy, thongTinTaiKhoan);
    setLoiTruong(cacLoi);
    if (Object.keys(cacLoi).length) return;
    setDangGui(true);
    const duLieu = {
      ...form,
      tenDangNhap: form.tenDangNhap.trim(), hoTen: form.hoTen.trim(),
      email: form.email.trim(), soDienThoai: form.soDienThoai.trim(),
      diaChi: form.diaChi.trim(), tinhThanhPho: form.tinhThanhPho.trim(),
      phuongXa: form.phuongXa.trim(),
    };
    try {
      if (thongTinTaiKhoan) {
        const { tenDangNhap, hoTen, email, soDienThoai, diaChi, tinhThanhPho, phuongXa } = duLieu;
        const data = await capNhatThongTinTaiKhoan({ tenDangNhap, hoTen, email, soDienThoai, diaChi, tinhThanhPho, phuongXa });
        setForm({ ...banDau, ...data });
        setDangSua(false);
        setThongBao('Đã cập nhật thông tin tài khoản. Tên đăng nhập mới đã có hiệu lực.');
        window.dispatchEvent(new Event('tai-khoan:changed'));
      } else if (dangKy) {
        await dangKyTaiKhoan(duLieu);
        navigate('/tai-khoan/dang-nhap', { replace: true, state: { dangKyThanhCong: true } });
      } else {
        await dangNhapTaiKhoan({ tenDangNhap: duLieu.tenDangNhap, matKhau: duLieu.matKhau });
        window.dispatchEvent(new Event('tai-khoan:changed'));
        navigate('/', { replace: true });
      }
    } catch (err) {
      const truong = err.message.startsWith('Tên đăng nhập') ? 'tenDangNhap'
        : err.message.startsWith('Email') ? 'email'
        : err.message.startsWith('Số điện thoại') ? 'soDienThoai' : '';
      if (truong) setLoiTruong(cu => ({ ...cu, [truong]: err.message }));
      else setLoi(err.message);
    } finally {
      setDangGui(false);
    }
  }

  const baoLoi = ten => loiTruong[ten] && <span className="web-field-error" role="alert">{loiTruong[ten]}</span>;

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
    {baoLoi('tinhThanhPho')}
  </label>;
  const chonPhuong = <label>Phường / Xã *
    <select name="phuongXa" value={form.phuongXa} onChange={thayDoi}
      required disabled={dangTaiPhuong || !form.tinhThanhPho || (thongTinTaiKhoan && !dangSua)}>
      <option value="">{dangTaiPhuong ? 'Đang tải phường/xã...' : 'Chọn Phường / Xã'}</option>
      {danhSachPhuong.map(item => <option key={item.code} value={item.name}>{item.name}</option>)}
    </select>
    {baoLoi('phuongXa')}
  </label>;

  if (thongTinTaiKhoan) return <div className="web-account-wrap">
    <div className="web-account-profile-layout">
      <aside className="web-account-profile-nav">
        <strong>Tài khoản của tôi</strong>
        <Link to="/tai-khoan/thong-tin" className="active">Thông tin tài khoản</Link>
        <Link to="/tai-khoan/don-hang">Đơn hàng của tôi</Link>
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
      {!dangTaiThongTin && form.tenDangNhap && <form onSubmit={gui} noValidate>
        <div className="web-account-columns">
          <div className="web-account-column">
            <label>Tên đăng nhập *<input name="tenDangNhap" value={form.tenDangNhap} onChange={thayDoi}
              autoComplete="username" maxLength={50} disabled={!dangSua} />{baoLoi('tenDangNhap')}</label>
            <label>Họ và tên *<input name="hoTen" value={form.hoTen} onChange={thayDoi}
              required minLength={2} maxLength={255} disabled={!dangSua} />{baoLoi('hoTen')}</label>
            <label>Email *<input name="email" type="email" value={form.email} onChange={thayDoi}
              required maxLength={50} disabled={!dangSua} />{baoLoi('email')}</label>
            <label>Số điện thoại *<input name="soDienThoai" type="tel" value={form.soDienThoai} onChange={thayDoi}
              required pattern="0[0-9]{9}" maxLength={20} disabled={!dangSua} />{baoLoi('soDienThoai')}</label>
          </div>
          <div className="web-account-column">
            <label>Địa chỉ (Số nhà, đường) *<input name="diaChi" value={form.diaChi} onChange={thayDoi}
              required minLength={2} maxLength={255} disabled={!dangSua} />{baoLoi('diaChi')}</label>
            {chonTinh}
            {chonPhuong}
          </div>
        </div>
        {dangSua ? <div className="web-profile-actions">
          <button type="submit" className="web-primary" disabled={dangGui || dangTaiTinh || dangTaiPhuong}>{dangGui ? 'Đang lưu...' : 'Lưu thay đổi'}</button>
          <button type="button" className="web-secondary" onClick={() => {
            setLoi('');
            setLoiTruong({});
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
      <form onSubmit={gui} noValidate>
        {dangKy ? <div className="web-account-columns">
          <div className="web-account-column">
            <label>Tên đăng nhập *
              <input name="tenDangNhap" type="text" autoComplete="username" value={form.tenDangNhap} onChange={thayDoi}
                required minLength={4} maxLength={50} placeholder="Nhập tên đăng nhập" />
              {baoLoi('tenDangNhap')}
            </label>
            <label>Họ và tên *
              <input name="hoTen" type="text" autoComplete="name" value={form.hoTen} onChange={thayDoi}
                required minLength={2} maxLength={255} placeholder="Nhập họ và tên" />
              {baoLoi('hoTen')}
            </label>
            <label>Email *
              <input name="email" type="email" autoComplete="email" value={form.email} onChange={thayDoi}
                required maxLength={50} placeholder="Nhập email của bạn" />
              {baoLoi('email')}
            </label>
            <label>Số điện thoại *
              <input name="soDienThoai" type="tel" autoComplete="tel" value={form.soDienThoai} onChange={thayDoi}
                required maxLength={20} pattern="0[0-9]{9}" placeholder="Nhập số điện thoại" />
              {baoLoi('soDienThoai')}
            </label>
          </div>
          <div className="web-account-column">
            <label>Địa chỉ (Số nhà, đường) *
              <input name="diaChi" type="text" autoComplete="street-address" value={form.diaChi} onChange={thayDoi}
                required minLength={2} maxLength={255} placeholder="Nhập địa chỉ" />
              {baoLoi('diaChi')}
            </label>
            {chonTinh}
            {chonPhuong}
            <label>Mật khẩu *
              <input name="matKhau" type="password" autoComplete="off" value={form.matKhau} onChange={thayDoi}
                required minLength={8} maxLength={100} placeholder="Nhập mật khẩu" />
              {baoLoi('matKhau')}
            </label>
            <label>Xác nhận mật khẩu *
              <input name="xacNhanMatKhau" type="password" autoComplete="off" value={form.xacNhanMatKhau} onChange={thayDoi}
                required placeholder="Nhập lại mật khẩu" />
              {baoLoi('xacNhanMatKhau')}
            </label>
          </div>
        </div> : <>
          <label>Tên đăng nhập
            <input name="tenDangNhap" type="text" autoComplete="username" value={form.tenDangNhap} onChange={thayDoi}
              required placeholder="Nhập tên đăng nhập" />
            {baoLoi('tenDangNhap')}
          </label>
          <label>Mật khẩu
            <input name="matKhau" type="password" autoComplete="current-password" value={form.matKhau} onChange={thayDoi}
              required placeholder="Nhập mật khẩu" />
            {baoLoi('matKhau')}
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
