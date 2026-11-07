import { useEffect, useState } from 'react';
import { Link, Outlet, useLocation, useNavigate } from 'react-router-dom';
import ShopIcon from './BieuTuongCuaHang.jsx';
import { layGioHang } from '../../API/GioHangAPI.js';


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
  const [cartCount, setCartCount] = useState(0);
  const isCatalog = location.pathname.startsWith('/san-pham') || location.pathname.startsWith('/tim-kiem');

  useEffect(() => setKeyword(query), [query]);
  useEffect(() => {
    let active = true;
    function refresh() {
      layGioHang().then(gio => {
        if (active) setCartCount((gio.items ?? []).reduce((sum, item) => sum + item.quantity, 0));
      }).catch(() => { /* Trang chu van su dung duoc neu API dang tam loi. */ });
    }
    refresh();
    window.addEventListener('gio-hang:changed', refresh);
    return () => { active = false; window.removeEventListener('gio-hang:changed', refresh); };
  }, []);
  useEffect(() => {
    const title = location.pathname.startsWith('/san-pham/') ? 'Chi tiết sản phẩm'
      : isCatalog ? 'Sản phẩm' : location.pathname === '/gio-hang' ? 'Giỏ hàng'
      : location.pathname.startsWith('/dat-hang') ? 'Đặt hàng'
      : location.pathname.startsWith('/tai-khoan/') ? 'Tài khoản'
      : location.pathname.startsWith('/don-hang/') ? 'Tra cứu đơn hàng' : 'Trang chủ';
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
          <Link to="/don-hang/tra-cuu" className="shop-order-link">Tra cứu đơn</Link>
          <Link to="/gio-hang" className="shop-action-icon" title="Giỏ hàng" aria-label="Giỏ hàng">
            <ShopIcon name="cart" size={26} />
            {cartCount > 0 && <span className="web-cart-count" aria-label={`${cartCount} sản phẩm trong giỏ`}>{cartCount > 99 ? '99+' : cartCount}</span>}
          </Link>
          <Link to="/tai-khoan/dang-nhap" className="shop-action-icon" title="Đăng nhập" aria-label="Tài khoản">
            <ShopIcon name="user" size={26} />
          </Link>
        </div>
      </div>
    </header>
    <main><Outlet /></main>
    <footer className="site-footer"><div className="wrap" /></footer>
  </>;
}
