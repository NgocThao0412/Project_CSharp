import { Link, useLocation } from 'react-router-dom';

export default function TaiKhoan() {
  const { pathname } = useLocation();
  const dangKy = pathname.endsWith('/dang-ky');
  return <div className="web-account-wrap">
    <section className="web-account-card">
      <div className="web-eyebrow">TÀI KHOẢN KHÁCH HÀNG</div>
      <h1>{dangKy ? 'Đăng ký' : 'Đăng nhập'}</h1>
      <p className="muted">{dangKy ? 'Tạo tài khoản để mua sắm thuận tiện hơn.' : 'Đăng nhập để theo dõi đơn hàng của bạn.'}</p>
      <form onSubmit={e => e.preventDefault()}>
        {dangKy && <label>Họ và tên<input type="text" autoComplete="name" placeholder="Nhập họ và tên" /></label>}
        <label>Email<input type="email" autoComplete="email" placeholder="Nhập email của bạn" /></label>
        {dangKy && <label>Số điện thoại<input type="tel" autoComplete="tel" placeholder="Nhập số điện thoại" /></label>}
        <label>Mật khẩu<input type="password" autoComplete={dangKy ? 'new-password' : 'current-password'} placeholder="Nhập mật khẩu" /></label>
        {dangKy && <label>Xác nhận mật khẩu<input type="password" autoComplete="new-password" placeholder="Nhập lại mật khẩu" /></label>}
        <button className="web-primary" type="submit" disabled>{dangKy ? 'Đăng ký' : 'Đăng nhập'} (chưa khả dụng)</button>
      </form>
      <div className="web-account-switch">{dangKy ? 'Đã có tài khoản?' : 'Chưa có tài khoản?'}{' '}
        <Link to={dangKy ? '/tai-khoan/dang-nhap' : '/tai-khoan/dang-ky'}>{dangKy ? 'Đăng nhập' : 'Đăng ký'}</Link>
      </div>
      <p className="web-info">Giao diện được giữ để kiểm thử. CSDL hiện chưa có cơ chế mật khẩu dành cho khách hàng nên không ghi nhận thông tin tài khoản.</p>
    </section>
  </div>;
}
