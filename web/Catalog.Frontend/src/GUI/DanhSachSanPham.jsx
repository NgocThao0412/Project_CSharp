import { useEffect, useState } from 'react';
import { Link, useLocation, useNavigate, useSearchParams, useOutletContext } from 'react-router-dom';
import { useApi } from '../API/GoiAPI.js';
import ProductCard from './ThanhPhan/TheSanPham.jsx';
import LoadState from './ThanhPhan/TrangThaiTaiDuLieu.jsx';
import ShopIcon from './ThanhPhan/BieuTuongCuaHang.jsx';
import { themGioHang } from '../API/GioHangAPI.js';


export default function ProductsPage() {
  const location = useLocation();
  const navigate = useNavigate();
  const { daDangNhap, yeuCauDangNhap } = useOutletContext();
  const [params] = useSearchParams();
  const q = params.get('q') ?? '';
  const category = params.get('category') ?? '';
  const rawPage = Number(params.get('page'));
  const page = Number.isInteger(rawPage) && rawPage > 0 ? Math.min(rawPage, 10000) : 1;
  const [keyword, setKeyword] = useState(q);
  const [selectedCategory, setSelectedCategory] = useState(category);
  const sort = ['gia-tang', 'gia-giam', 'ten'].includes(params.get('sort')) ? params.get('sort') : 'mac-dinh';
  const [favorites, setFavorites] = useState(() => new Set());
  const [notice, setNotice] = useState('');

  useEffect(() => { setKeyword(q); setSelectedCategory(category); }, [q, category]);
  useEffect(() => { setNotice(''); }, [location.pathname, location.search]);

  // C# API sap xep va phan trang tren TOAN BO ket qua SQL Server.
  const truyVan = new URLSearchParams({ q, category, page: String(page), sort });
  const { data, loading, error } = useApi(`/api/catalog/products?${truyVan.toString()}`);
  const pageCount = Math.max(1, Math.ceil((data?.total ?? 0) / (data?.pageSize || 8)));
  const sortedItems = data?.items ?? [];
  const categories = useApi('/api/catalog/categories');

  function applyFilter(event) {
    event.preventDefault();
    const p = new URLSearchParams();
    const cleanKeyword = keyword.trim().slice(0, 100);
    if (cleanKeyword) p.set('q', cleanKeyword);
    if (selectedCategory) p.set('category', selectedCategory);
    if (sort !== 'mac-dinh') p.set('sort', sort);
    navigate(`/san-pham${p.size ? '?' + p.toString() : ''}`);
  }

  function pageHref(targetPage) {
    const search = new URLSearchParams();
    if (q) search.set('q', q);
    if (category) search.set('category', category);
    if (sort !== 'mac-dinh') search.set('sort', sort);
    search.set('page', String(targetPage));
    return `${location.pathname}?${search.toString()}`;
  }

  function changeSort(value) {
    const next = new URLSearchParams(location.search);
    if (value === 'mac-dinh') next.delete('sort'); else next.set('sort', value);
    next.delete('page');
    navigate(`${location.pathname}${next.size ? `?${next.toString()}` : ''}`);
  }

  async function themVaoGio(item) {
    if (!daDangNhap) { yeuCauDangNhap('gio-hang'); return; }
    try {
      await themGioHang(item.id);
      setNotice(`Đã thêm ${item.name} vào giỏ hàng.`);
    } catch (err) { setNotice(err.message); }
  }

  function toggleFavorite(id) {
    setFavorites(old => {
      const next = new Set(old);
      if (next.has(id)) next.delete(id); else next.add(id);
      return next;
    });
  }

  return <div className="catalog-page-v2">
    <div className="catalog-container">
      <section className="catalog-banner" aria-label="Danh sách sản phẩm">
        <div className="catalog-banner-content">
          <h1>{q ? 'Kết quả tìm kiếm' : 'Sản phẩm'}</h1>
          <p>Khám phá đa dạng linh kiện máy tính chính hãng,<br /> chất lượng với giá tốt nhất.</p>
        </div>
        <div className="catalog-banner-waves" aria-hidden="true" />
        <img className="catalog-banner-computer" src="/images/catalog/hero-setup.webp" alt="" />
      </section>

      <form className="catalog-filters" onSubmit={applyFilter}>
        <label className="catalog-filter-field">
          <span><ShopIcon name="search" size={20} /> Tìm kiếm</span>
          <input name="q" value={keyword} onChange={e => setKeyword(e.target.value)} maxLength={100}
            placeholder="Nhập tên linh kiện, thương hiệu..." />
        </label>
        <label className="catalog-filter-field catalog-category-field">
          <span><ShopIcon name="list" size={20} /> Danh mục</span>
          <select name="category" value={selectedCategory} onChange={e => setSelectedCategory(e.target.value)}>
            <option value="">Tất cả danh mục</option>
            {(categories.data ?? []).map(c => <option key={c} value={c}>{c}</option>)}
          </select>
          <ShopIcon name="chevron" className="catalog-filter-select-chevron" size={17} />
        </label>
        <button className="catalog-apply-button" type="submit"><ShopIcon name="filter" size={18} /> Áp dụng</button>
        <Link className="catalog-reset-button" to="/san-pham" onClick={() => { setKeyword(''); setSelectedCategory(''); }}>
          <ShopIcon name="reset" size={18} /> Xóa lọc
        </Link>
      </form>

      <div className="catalog-results-toolbar">
        {!loading && !error
          ? <p>Tìm thấy <strong>{data?.total ?? 0}</strong> sản phẩm</p>
          : <p>Danh sách sản phẩm</p>}
        <label className="catalog-sort-control">Sắp xếp:
          <select value={sort} onChange={e => changeSort(e.target.value)} aria-label="Sắp xếp toàn bộ danh sách sản phẩm">
            <option value="mac-dinh">Mặc định</option>
            <option value="gia-tang">Giá thấp đến cao</option>
            <option value="gia-giam">Giá cao đến thấp</option>
            <option value="ten">Tên A – Z</option>
          </select>
          <ShopIcon name="chevron" size={17} />
        </label>
      </div>

      {notice && <div className="catalog-feedback" role="status">{notice}<button type="button" onClick={() => setNotice('')} aria-label="Đóng thông báo">×</button></div>}
      <LoadState loading={loading} error={error} />
      {!loading && !error && (sortedItems.length === 0
        ? <div className="empty-state"><h2>Không tìm thấy sản phẩm</h2>
            <p>Thử từ khóa khác hoặc bỏ lọc danh mục.</p><Link to="/san-pham">Xem tất cả sản phẩm</Link></div>
        : <div className="catalog-products-grid">
            {sortedItems.map(item => <ProductCard key={item.id} product={item} variant="catalog"
              isFavorite={favorites.has(item.id)} onToggleFavorite={toggleFavorite}
              onAddToCart={() => themVaoGio(item)} />)}
          </div>)}

      {!loading && !error && pageCount > 1 && <nav className="catalog-pagination" aria-label="Phân trang">
        <Link to={pageHref(Math.max(1, data.page - 1))} aria-label="Trang trước" className={data.page === 1 ? 'is-disabled' : ''}>
          <ShopIcon name="left" size={19} />
        </Link>
        {Array.from({ length: Math.min(pageCount, data.page + 2) - Math.max(1, data.page - 2) + 1 },
          (_, i) => Math.max(1, data.page - 2) + i).map(p =>
            <Link key={p} className={p === data.page ? 'active' : ''} to={pageHref(p)} aria-current={p === data.page ? 'page' : undefined}>{p}</Link>
          )}
        <Link to={pageHref(Math.min(pageCount, data.page + 1))} aria-label="Trang sau" className={data.page === pageCount ? 'is-disabled' : ''}>
          <ShopIcon name="right" size={19} />
        </Link>
      </nav>}

    </div>
  </div>;
}
