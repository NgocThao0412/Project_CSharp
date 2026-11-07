import { Link, useLocation } from 'react-router-dom';
import { money } from '../API/GoiAPI.js';

export default function DatHangThanhCong() {
  const { state: don } = useLocation();
  return <div className="wrap section web-customer-page">
    <div className="breadcrumb"><Link to="/">Trang chủ</Link> / Đặt hàng / Kết quả</div>
    <section className="web-checkout-success">
      {don?.maDon ? <>
        <div className="web-success-mark" aria-hidden="true">✓</div>
        <h1>Đặt hàng thành công!</h1>
        <p>Đơn hàng đã được ghi vào SQL Server và đang chờ nhân viên duyệt.</p>
        <div className="web-success-order">
          <div><span>Mã đơn hàng</span><strong>#{don.maDon}</strong></div>
          <div><span>Tổng tiền</span><strong>{money(don.tongTien)}</strong></div>
          <div><span>Trạng thái</span><strong>{don.trangThai}</strong></div>
          <div><span>Thanh toán</span><strong>{don.hinhThucThanhToan}</strong></div>
        </div>
        <p className="muted">Hãy lưu mã đơn và sử dụng số điện thoại đã nhập để tra cứu trạng thái.</p>
        <div className="web-success-actions">
          <Link className="web-primary" to="/don-hang/tra-cuu">Tra cứu đơn hàng</Link>
          <Link className="web-secondary" to="/san-pham">Tiếp tục mua sắm</Link>
        </div>
      </> : <>
        <h1>Không có thông tin xác nhận</h1>
        <p>Nếu bạn vừa đặt hàng, hãy dùng mã đơn và số điện thoại để tra cứu.</p>
        <Link className="web-primary" to="/don-hang/tra-cuu">Tra cứu đơn hàng</Link>
      </>}
    </section>
  </div>;
}
