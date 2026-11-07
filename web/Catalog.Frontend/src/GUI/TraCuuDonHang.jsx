import { useState } from 'react';
import { Link } from 'react-router-dom';
import { money } from '../API/GoiAPI.js';
import { traCuuDonHang } from '../API/GioHangAPI.js';

export default function TraCuuDonHang() {
  const [maDon, setMaDon] = useState('');
  const [soDienThoai, setSoDienThoai] = useState('');
  const [ketQua, setKetQua] = useState(null);
  const [loi, setLoi] = useState('');
  const [dangTai, setDangTai] = useState(false);

  async function timKiem(e) {
    e.preventDefault();
    setLoi('');
    setKetQua(null);
    if (!Number.isInteger(Number(maDon)) || Number(maDon) <= 0 || !/^0\d{9}$/.test(soDienThoai.trim())) {
      setLoi('Vui lòng nhập mã đơn hợp lệ và số điện thoại gồm 10 chữ số, bắt đầu bằng 0.');
      return;
    }
    setDangTai(true);
    try { setKetQua(await traCuuDonHang(maDon, soDienThoai.trim())); }
    catch (err) { setLoi(err.message); }
    finally { setDangTai(false); }
  }

  return <div className="wrap section web-customer-page">
    <div className="breadcrumb"><Link to="/">Trang chủ</Link> / Tra cứu đơn hàng</div>
    <div className="web-order-lookup">
      <h1>Tra cứu đơn hàng</h1>
      <p className="muted">Nhập mã phiếu xuất và số điện thoại đã lưu trong hệ thống để kiểm tra đơn hàng.</p>
      <form className="web-inline-form" onSubmit={timKiem}>
        <label>Mã đơn hàng<input type="number" min="1" value={maDon} onChange={e => setMaDon(e.target.value)} required /></label>
        <label>Số điện thoại<input type="tel" value={soDienThoai} onChange={e => setSoDienThoai(e.target.value)} maxLength={10} placeholder="0xxxxxxxxx" required /></label>
        <button className="web-primary" type="submit" disabled={dangTai}>{dangTai ? 'Đang tra cứu...' : 'Tra cứu'}</button>
      </form>
      {loi && <div className="web-error" role="alert">{loi}</div>}
      {ketQua && <div className="web-order-result">
        <h2>Đơn hàng #{ketQua.ma}</h2>
        <p><strong>Khách hàng:</strong> {ketQua.hoTen}</p>
        <p><strong>Trạng thái:</strong> {ketQua.tenTrangThai}</p>
        <p><strong>Ngày lập:</strong> {ketQua.ngayDat ? new Date(ketQua.ngayDat).toLocaleString('vi-VN') : 'Chưa có'}</p>
        <ul>{(ketQua.chiTiet ?? []).map(item => <li key={item.maSanPham}>{item.ten} × {item.soLuong} — {money(item.thanhTien)}</li>)}</ul>
        <p className="web-summary-line"><span>Tổng tiền</span><strong>{money(ketQua.tongTien)}</strong></p>
      </div>}
    </div>
  </div>;
}
