import { useEffect, useState } from 'react';
import { Link, Outlet, useLocation, useNavigate } from 'react-router-dom';

// HTML structure and inline styling reproduce the original _Layout.cshtml.
const navStyle = { display: 'flex', alignItems: 'center', gap: '16px', flexWrap: 'wrap' };
const formStyle = {
  display: 'inline-flex', alignItems: 'center', width: '210px', height: '34px',
  border: '1px solid #cfd9eb', borderRadius: '8px', overflow: 'hidden',
  background: '#fff', margin: 0,
};
const inputStyle = {
  width: '100%', minWidth: 0, padding: '6px 9px', border: 0,
  outline: 'none', background: 'transparent', font: 'inherit',
  fontSize: '13px', color: '#1f2937',
};
const buttonStyle = {
  height: '100%', width: '36px', flexShrink: 0, display: 'flex',
  alignItems: 'center', justifyContent: 'center', border: 0,
  background: '#1765e7', color: 'white', cursor: 'pointer',
};

function SearchIcon() {
  return <svg width="17" height="17" viewBox="0 0 24 24" fill="none"
    stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
    <circle cx="11" cy="11" r="7" />
    <path d="m20 20-4-4" />
  </svg>;
}

export default function SiteLayout() {
  const navigate = useNavigate();
  const location = useLocation();
  const currentQuery = new URLSearchParams(location.search).get('q') ?? '';
  const [keyword, setKeyword] = useState(currentQuery);

  useEffect(() => setKeyword(currentQuery), [currentQuery]);

  useEffect(() => {
    const page = location.pathname.startsWith('/san-pham/')
      ? 'Chi tiết sản phẩm'
      : location.pathname.startsWith('/san-pham') || location.pathname.startsWith('/tim-kiem')
        ? 'Danh sách sản phẩm'
        : 'Trang chủ';
    document.title = `${page} - Cửa hàng linh kiện máy tính`;
  }, [location.pathname]);

  const onSearch = (event) => {
    event.preventDefault();
    const q = keyword.trim().slice(0, 100);
    navigate(`/tim-kiem${q ? `?q=${encodeURIComponent(q)}` : ''}`);
  };

  return <>
    <header className="site-header">
      <div className="wrap header-inner">
        <Link className="brand" to="/">
          <span className="brand-icon">◈</span>
          <span>CỬA HÀNG<br /><strong>LINH KIỆN MÁY TÍNH</strong></span>
        </Link>
        <nav aria-label="Điều hướng chính" style={navStyle}>
          <Link to="/">Trang chủ</Link>
          <form onSubmit={onSearch} role="search" style={formStyle}>
            <input type="search" name="q" value={keyword} onChange={e => setKeyword(e.target.value)}
              maxLength={100} placeholder="Tìm sản phẩm..." aria-label="Tìm sản phẩm" style={inputStyle} />
            <button type="submit" aria-label="Tìm kiếm" style={buttonStyle}><SearchIcon /></button>
          </form>
          <Link to="/san-pham">Sản phẩm</Link>
        </nav>
      </div>
    </header>
    <main><Outlet /></main>
    <footer className="site-footer"><div className="wrap" /></footer>
  </>;
}
