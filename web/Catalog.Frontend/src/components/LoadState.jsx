export default function LoadState({ loading, error }) {
  if (loading) return <div className="loading-state" role="status">Đang tải sản phẩm...</div>;
  if (error) return <div className="empty-state" role="alert">
    <h2>Không thể tải dữ liệu</h2><p>{error}</p>
    <p>Hãy kiểm tra backend ASP.NET Core ở cổng 5080.</p>
  </div>;
  return null;
}
