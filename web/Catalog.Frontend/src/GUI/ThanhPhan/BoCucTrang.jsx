import { useEffect, useState } from 'react';
import { Link, Outlet, useLocation, useNavigate } from 'react-router-dom';
import ShopIcon from './BieuTuongCuaHang.jsx';

const backendBase = import.meta.env.DEV ? 'http://localhost:5080' : '';

function ShopLogo() {
  return <svg className="shop-logo-icon" viewBox="0 0 56 56" fill="none" aria-hidden="true">
    <path d="M28 3 53 28 28 53 3 28Z" stroke="currentColor" strokeWidth="5" />
    <path d="M28 13 43 28 28 43 13 28Z" stroke="currentColor" strokeWidth="4" />
    <path d="M28 22 34 28 28 34 22 28Z" fill="currentColor" />
  </svg>;
}

export default function SiteLayout() {
  const navigate = useNavigate();
  const location = useLocation();
  const query = new URLSearchParams(location.search).get('q') ?? '';
  const [keyword, setKeyword] = useState(query);
  const isCatalog = location.pathname.startsWith('/san-pham') || location.pathname.startsWith('/tim-kiem');

  useEffect(() => setKeyword(query), [query]);
  useEffect(() => {
    const title = location.pathname.startsWith('/san-pham/') ? 'Chi tiết sản phẩm' : isCatalog ? 'Sản phẩm' : 'Trang chủ';
    document.title = `${title} - Cửa hàng linh kiện máy tính`;
  }, [location.pathname, isCatalog]);

  function search(e) {
    e.preventDefault();
    const value = keyword.trim().slice(0, 100);
    navigate(`/tim-kiem${value ? `?q=${encodeURIComponent(value)}` : ''}`);
  }

  return <>
    <header className="site-header shop-site-header">
      <div className="shop-header-inner">
        <Link to="/" className="shop-brand" aria-label="Cửa hàng linh kiện máy tính - Trang chủ">
          <ShopLogo />
          <span className="shop-brand-label">CỬA HÀNG<strong>LINH KIỆN MÁY TÍNH</strong></span>
        </Link>
        <nav className="shop-main-nav" aria-label="Điều hướng chính">
          <Link to="/" className={!isCatalog ? 'shop-nav-active' : ''}><ShopIcon name="home" /> Trang chủ</Link>
          <Link to="/san-pham" className={isCatalog ? 'shop-nav-active' : ''}><ShopIcon name="box" /> Sản phẩm</Link>
          <span className="shop-nav-placeholder" title="Mục Tin tức đang được phát triển"><ShopIcon name="newspaper" /> Tin tức</span>
          <span className="shop-nav-placeholder" title="Mục Liên hệ đang được phát triển"><ShopIcon name="phone" /> Liên hệ</span>
        </nav>
        <div className="shop-header-actions">
          <form className="shop-header-search" onSubmit={search} role="search">
            <input type="search" name="q" value={keyword} onChange={e => setKeyword(e.target.value)} maxLength={100}
              placeholder="Tìm sản phẩm, thương hiệu..." aria-label="Tìm sản phẩm" />
            <button type="submit" aria-label="Tìm kiếm"><ShopIcon name="search" size={22} /></button>
          </form>
          <span className="shop-header-separator" />
          <a href={`${backendBase}/gio-hang`} className="shop-action-icon" title="Mở giỏ hàng (Web C#)" aria-label="Giỏ hàng">
            <ShopIcon name="cart" size={26} />
          </a>
          <a href={`${backendBase}/tai-khoan/dang-nhap`} className="shop-action-icon" title="Đăng nhập (Web C#)" aria-label="Tài khoản">
            <ShopIcon name="user" size={26} />
          </a>
        </div>
      </div>
    </header>
    <main><Outlet /></main>
    <footer className="site-footer"><div className="wrap" /></footer>
  </>;
}
