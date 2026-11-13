import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { dangXuatTaiKhoan, money } from '../API/GoiAPI.js';
import { layChiTietDonHang, layDanhSachDonHang } from '../API/GioHangAPI.js';

function ngayGio(value) {
  return value ? new Date(value).toLocaleString('vi-VN') : 'Chưa có thông tin';
}

function ThanhDieuHuong({ navigate }) {
  async function dangXuat() {
    try {
      await dangXuatTaiKhoan();
      window.dispatchEvent(new Event('tai-khoan:changed'));
      navigate('/', { replace: true });
    } catch (err) {
      window.alert(err.message);
    }
  }

  return <aside className="web-account-profile-nav">
    <strong>Tài khoản của tôi</strong>
    <Link to="/tai-khoan/thong-tin">Thông tin tài khoản</Link>
    <Link to="/tai-khoan/don-hang" className="active">Đơn hàng của tôi</Link>
    <button type="button" onClick={dangXuat}>Đăng xuất</button>
  </aside>;
}

export default function DonHangCuaToi() {
  const navigate = useNavigate();
  const { maDon } = useParams();
  const [danhSach, setDanhSach] = useState([]);
  const [chiTiet, setChiTiet] = useState(null);
  const [dangTai, setDangTai] = useState(true);
  const [loi, setLoi] = useState('');

  useEffect(() => {
    let active = true;
    setDangTai(true);
    setLoi('');
    setChiTiet(null);
    (maDon ? layChiTietDonHang(maDon) : layDanhSachDonHang()).then(data => {
      if (!active) return;
      if (maDon) setChiTiet(data);
      else setDanhSach(data);
    }).catch(err => { if (active) setLoi(err.message); })
      .finally(() => { if (active) setDangTai(false); });
    return () => { active = false; };
  }, [maDon]);

  return <div className="web-account-wrap">
    <div className="web-account-profile-layout">
      <ThanhDieuHuong navigate={navigate} />
      <section className="web-account-card web-account-register web-account-profile web-orders-section">
        <div className="web-eyebrow">TÀI KHOẢN KHÁCH HÀNG</div>
        <h1>{maDon ? `Chi tiết đơn hàng #${maDon}` : 'Đơn hàng của tôi'}</h1>
        {maDon && <Link className="web-order-back" to="/tai-khoan/don-hang">← Quay lại danh sách đơn hàng</Link>}
        {dangTai && <p className="muted">Đang tải thông tin đơn hàng...</p>}
        {loi && <div className="web-error" role="alert">{loi}</div>}
        {!dangTai && !loi && !maDon && (danhSach.length === 0
          ? <div className="web-empty"><p>Bạn chưa có đơn hàng nào.</p><Link className="web-primary" to="/san-pham">Xem sản phẩm</Link></div>
          : <div className="web-orders-list">
            {danhSach.map(don => <article className="web-order-card" key={don.ma}>
              <div className="web-order-card-info">
                <div><span>Mã đơn hàng</span><strong>#{don.ma}</strong></div>
                <div><span>Ngày đặt</span><strong>{ngayGio(don.ngayDat)}</strong></div>
                <div><span>Tổng tiền</span><strong className="web-order-price">{money(don.tongTien)}</strong></div>
                <div><span>Trạng thái</span><strong className={`web-order-status web-order-status-${don.trangThai}`}>{don.tenTrangThai}</strong></div>
              </div>
              <Link className="web-secondary" to={`/tai-khoan/don-hang/${don.ma}`}>Xem chi tiết</Link>
            </article>)}
          </div>)}
        {!dangTai && !loi && maDon && chiTiet && <>
          <div className="web-order-detail-overview">
            <div><span>Mã đơn hàng</span><strong>#{chiTiet.ma}</strong></div>
            <div><span>Ngày đặt</span><strong>{ngayGio(chiTiet.ngayDat)}</strong></div>
            <div><span>Trạng thái</span><strong className={`web-order-status web-order-status-${chiTiet.trangThai}`}>{chiTiet.tenTrangThai}</strong></div>
            <div><span>Thanh toán</span><strong>{chiTiet.phuongThucThanhToan}</strong></div>
          </div>
          <div className="web-order-parties">
            <section>
              <h2>Thông tin người đặt</h2>
              <p><strong>Họ tên:</strong> {chiTiet.hoTenNguoiDat}</p>
              <p><strong>Số điện thoại:</strong> {chiTiet.soDienThoaiNguoiDat}</p>
              <p><strong>Email:</strong> {chiTiet.emailNguoiDat || 'Chưa cung cấp'}</p>
            </section>
            <section>
              <h2>Thông tin người nhận</h2>
              <p><strong>Họ tên:</strong> {chiTiet.hoTenNguoiNhan}</p>
              <p><strong>Số điện thoại:</strong> {chiTiet.soDienThoaiNguoiNhan}</p>
              <p><strong>Địa chỉ giao hàng:</strong> {chiTiet.diaChiNguoiNhan || 'Chưa có thông tin'}</p>
              <p><strong>Ghi chú:</strong> {chiTiet.ghiChu || 'Không có'}</p>
            </section>
          </div>
          {!chiTiet.laDonWeb && <p className="web-info">Đơn này không có bản ghi giao hàng Web. Thông tin người nhận được tham chiếu từ hồ sơ khách hàng hiện tại.</p>}
          <h2 className="web-order-products-title">Sản phẩm trong đơn hàng</h2>
          <div className="web-order-table-scroll">
            <table className="web-order-table">
              <thead><tr><th>Sản phẩm</th><th>Số lượng</th><th>Đơn giá</th><th>Thành tiền</th></tr></thead>
              <tbody>{chiTiet.chiTiet.map(item => <tr key={item.maSanPham}>
                <td><Link to={`/san-pham/${item.maSanPham}`}>{item.ten}</Link></td>
                <td>{item.soLuong}</td>
                <td>{money(item.donGia)}</td>
                <td>{money(item.thanhTien)}</td>
              </tr>)}</tbody>
            </table>
          </div>
          <div className="web-summary-line web-checkout-total"><strong>Tổng tiền đơn hàng</strong><strong>{money(chiTiet.tongTien)}</strong></div>
        </>}
      </section>
    </div>
  </div>;
}
