import { Link } from 'react-router-dom';

export default function NotFoundPage() {
  return <div className="wrap section"><div className="empty-state">
    <h2>Không tìm thấy trang</h2>
    <p>Địa chỉ bạn vừa truy cập không tồn tại.</p>
    <Link to="/">Quay về trang chủ</Link>
  </div></div>;
}
