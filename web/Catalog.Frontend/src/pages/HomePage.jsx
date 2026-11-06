import { Link } from 'react-router-dom';
import { useApi } from '../api.js';
import ProductCard from '../components/ProductCard.jsx';
import LoadState from '../components/LoadState.jsx';

// Preserves the exact classes, copy, and hero layout of Views/Home/Index.cshtml.
// The large hero search form was removed from the original before migration.
export default function HomePage() {
  const { data, loading, error } = useApi('/api/catalog/featured?count=6');

  return <>
    <section className="hero">
      <div className="wrap hero-grid">
        <div>
          <span className="eyebrow"></span>
          <h1>Chọn linh kiện phù hợp với nhu cầu của bạn.</h1>
          <p>Tìm kiếm, tham khảo sản phẩm và xem thông tin tồn khả dụng trước khi đặt hàng.</p>
        </div>
        <div className="hero-visual" aria-hidden="true">
          <div className="chip">CPU</div>
          <div className="chip secondary">RAM</div>
          <div className="chip third">SSD</div>
        </div>
      </div>
    </section>
    <section className="wrap section">
      <div className="section-heading">
        <div><span className="eyebrow blue"></span><h2>Sản phẩm nổi bật</h2></div>
        <Link to="/san-pham">Xem tất cả →</Link>
      </div>
      <LoadState loading={loading} error={error} />
      {!loading && !error &&
        <div className="product-grid">
          {data?.map(item => <ProductCard key={item.id} product={item} />)}
        </div>}
    </section>
  </>;
}
