import { useEffect, useMemo, useState } from 'react';
import { Link, useLocation, useNavigate, useSearchParams } from 'react-router-dom';
import { useApi } from '../API/GoiAPI.js';
import ProductCard from './ThanhPhan/TheSanPham.jsx';
import LoadState from './ThanhPhan/TrangThaiTaiDuLieu.jsx';
import ShopIcon from './ThanhPhan/BieuTuongCuaHang.jsx';


// Adapt the existing paginated C# API to the four cards per page in the design,
// without changing the SQL backend or the other team's code.
const CARDS_PER_PAGE = 4;
function useCatalogPage(q, category, requestedPage) {
  const [state, setState] = useState({ data: null, loading: true, error: null });
  useEffect(() => {
    const controller = new AbortController();
    setState({ data: null, loading: true, error: null });

    const fetchRemotePage = async (remotePage) => {
      const params = new URLSearchParams({ q, category, page: String(remotePage) });
      const response = await fetch(`/api/catalog/products?${params.toString()}`, {
        signal: controller.signal,
        headers: { Accept: 'application/json' },
      });
      if (!response.ok) throw new Error(`Không tải được danh sách sản phẩm (HTTP ${response.status}).`);
      return response.json();
    };

    (async () => {
      try {
        const first = await fetchRemotePage(1);
        const total = first.total ?? 0;
        const remoteSize = first.pageSize || 9;
        const pageCount = Math.max(1, Math.ceil(total / CARDS_PER_PAGE));
        const page = Math.min(requestedPage, pageCount);
        const start = (page - 1) * CARDS_PER_PAGE;
        const end = Math.min(start + CARDS_PER_PAGE, total);
        if (total === 0) {
          if (!controller.signal.aborted) {
            setState({ data: { items: [], total, page, pageCount }, loading: false, error: null });
          }
          return;
        }
        const startRemotePage = Math.floor(start / remoteSize) + 1;
        const endRemotePage = Math.floor((end - 1) / remoteSize) + 1;
        const firstBatch = startRemotePage === 1 ? first : await fetchRemotePage(startRemotePage);
        const secondBatch = endRemotePage > startRemotePage ? await fetchRemotePage(endRemotePage) : null;
        const offset = start - (startRemotePage - 1) * remoteSize;
        const items = [...(firstBatch.items ?? []), ...(secondBatch?.items ?? [])]
          .slice(offset, offset + end - start);
        if (!controller.signal.aborted) {
          setState({ data: { items, total, page, pageCount }, loading: false, error: null });
        }
      } catch (err) {
        if (err.name !== 'AbortError' && !controller.signal.aborted) {
          setState({ data: null, loading: false, error: err.message });
        }
      }
    })();
    return () => controller.abort();
  }, [q, category, requestedPage]);
  return state;
}

export default function ProductsPage() {
  const location = useLocation();
  const navigate = useNavigate();
  const [params] = useSearchParams();
  const q = params.get('q') ?? '';
  const category = params.get('category') ?? '';
  const rawPage = Number(params.get('page'));
  const page = Number.isInteger(rawPage) && rawPage > 0 ? Math.min(rawPage, 10000) : 1;
  const [keyword, setKeyword] = useState(q);
  const [selectedCategory, setSelectedCategory] = useState(category);
  const [sort, setSort] = useState('default');
  const [favorites, setFavorites] = useState(() => new Set());
  const [notice, setNotice] = useState('');

  useEffect(() => { setKeyword(q); setSelectedCategory(category); }, [q, category]);
  useEffect(() => { setNotice(''); }, [location.pathname, location.search]);

  const { data, loading, error } = useCatalogPage(q, category, page);
  const categories = useApi('/api/catalog/categories');

  const sortedItems = useMemo(() => {
    const items = [...(data?.items ?? [])];
    // Sort the visible server page only; server-side sorting can be added after SQL integration.
    if (sort === 'price-asc') items.sort((a, b) => a.price - b.price);
    if (sort === 'price-desc') items.sort((a, b) => b.price - a.price);
    if (sort === 'name') items.sort((a, b) => a.name.localeCompare(b.name, 'vi'));
    return items;
  }, [data?.items, sort]);

  function applyFilter(event) {
    event.preventDefault();
    const p = new URLSearchParams();
    const cleanKeyword = keyword.trim().slice(0, 100);
    if (cleanKeyword) p.set('q', cleanKeyword);
    if (selectedCategory) p.set('category', selectedCategory);
    navigate(`/san-pham${p.size ? '?' + p.toString() : ''}`);
  }

  function pageHref(targetPage) {
    const search = new URLSearchParams();
    if (q) search.set('q', q);
    if (category) search.set('category', category);
    search.set('page', String(targetPage));
    return `${location.pathname}?${search.toString()}`;
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
          <select value={sort} onChange={e => setSort(e.target.value)} aria-label="Sắp xếp sản phẩm trong trang">
            <option value="default">Mặc định</option>
            <option value="price-asc">Giá thấp đến cao</option>
            <option value="price-desc">Giá cao đến thấp</option>
            <option value="name">Tên A – Z</option>
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
              onAddToCart={() => setNotice('Giỏ hàng đang dùng dữ liệu riêng của phần đặt hàng. Chưa thể thêm sản phẩm này cho đến khi nhóm tích hợp chung danh mục.')} />)}
          </div>)}

      {!loading && !error && data?.pageCount > 1 && <nav className="catalog-pagination" aria-label="Phân trang">
        <Link to={pageHref(Math.max(1, data.page - 1))} aria-label="Trang trước" className={data.page === 1 ? 'is-disabled' : ''}>
          <ShopIcon name="left" size={19} />
        </Link>
        {Array.from({ length: Math.min(data.pageCount, data.page + 2) - Math.max(1, data.page - 2) + 1 },
          (_, i) => Math.max(1, data.page - 2) + i).map(p =>
            <Link key={p} className={p === data.page ? 'active' : ''} to={pageHref(p)} aria-current={p === data.page ? 'page' : undefined}>{p}</Link>
          )}
        <Link to={pageHref(Math.min(data.pageCount, data.page + 1))} aria-label="Trang sau" className={data.page === data.pageCount ? 'is-disabled' : ''}>
          <ShopIcon name="right" size={19} />
        </Link>
      </nav>}
      <p className="catalog-sort-note">Sắp xếp hiện áp dụng cho các sản phẩm trên trang đang xem.</p>
    </div>
  </div>;
}
