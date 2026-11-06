import { useEffect, useMemo, useState } from 'react';
import { Link, useLocation, useNavigate, useSearchParams } from 'react-router-dom';
import { useApi } from '../api.js';
import ProductCard from '../components/ProductCard.jsx';
import LoadState from '../components/LoadState.jsx';

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

  useEffect(() => { setKeyword(q); setSelectedCategory(category); }, [q, category]);

  const query = useMemo(() => {
    const p = new URLSearchParams({ q, category, page: String(page) });
    return `/api/catalog/products?${p.toString()}`;
  }, [q, category, page]);
  const { data, loading, error } = useApi(query);
  const categories = useApi('/api/catalog/categories');

  function applyFilter(e) {
    e.preventDefault();
    const p = new URLSearchParams();
    const cleanKeyword = keyword.trim().slice(0, 100);
    if (cleanKeyword) p.set('q', cleanKeyword);
    if (selectedCategory) p.set('category', selectedCategory);
    navigate(`/san-pham${p.size ? '?' + p.toString() : ''}`);
  }

  function pageHref(p) {
    const qs = new URLSearchParams();
    if (q) qs.set('q', q);
    if (category) qs.set('category', category);
    qs.set('page', p);
    return `${location.pathname}?${qs.toString()}`;
  }

  return <>
    <section className="page-head"><div className="wrap">
      <div className="eyebrow blue"></div>
      <h1>{q ? `Kết quả cho “${q}”` : 'Tất cả sản phẩm'}</h1>
      <p>Tìm kiếm theo tên, thương hiệu hoặc lọc theo danh mục.</p>
    </div></section>
    <section className="wrap section">
      <form className="filters" onSubmit={applyFilter}>
        <label>Tìm kiếm <input name="q" value={keyword} onChange={e => setKeyword(e.target.value)}
          maxLength={100} placeholder="Nhập tên linh kiện..." /></label>
        <label>Danh mục <select name="category" value={selectedCategory} onChange={e => setSelectedCategory(e.target.value)}>
          <option value="">Tất cả danh mục</option>
          {(categories.data ?? []).map(c => <option key={c} value={c}>{c}</option>)}
        </select></label>
        <button type="submit">Áp dụng</button>
        <Link className="reset" to="/san-pham">Xóa lọc</Link>
      </form>
      {!loading && !error && <div className="results-summary">Tìm thấy <strong>{data?.total ?? 0}</strong> sản phẩm</div>}
      <LoadState loading={loading} error={error} />
      {!loading && !error && (data?.items?.length === 0 ?
        <div className="empty-state">
          <h2>Không tìm thấy sản phẩm</h2><p>Thử từ khóa khác hoặc bỏ lọc danh mục.</p>
          <Link to="/san-pham">Xem tất cả sản phẩm</Link>
        </div> :
        <div className="product-grid">
          {data?.items?.map(item => <ProductCard key={item.id} product={item} />)}
        </div>)}
      {!loading && !error && data?.pageCount > 1 &&
        <nav className="pagination" aria-label="Phân trang">
          {Array.from({ length: Math.min(data.pageCount, data.page + 2) - Math.max(1, data.page - 2) + 1 },
            (_, i) => Math.max(1, data.page - 2) + i).map(p =>
            <Link key={p} className={p === data.page ? 'active' : ''} to={pageHref(p)}
              aria-current={p === data.page ? 'page' : undefined}>{p}</Link>
          )}
        </nav>}
    </section>
  </>;
}
